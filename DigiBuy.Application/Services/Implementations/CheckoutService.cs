using System.Security.Claims;
using AutoMapper;
using DigiBuy.Application.Dtos.CheckoutDTOs;
using DigiBuy.Application.Dtos.UserDTOs;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;
using DigiBuy.Application.Helpers;

namespace DigiBuy.Application.Services.Implementations;

public class CheckoutService : ICheckoutService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly IUserService userService;
    private readonly IEmailService emailService;

    public CheckoutService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService, IEmailService emailService)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.userService = userService;
        this.emailService = emailService;
    }

    public async Task<CheckoutResultDTO> CheckoutAsync(string orderId, string? couponCode, ClaimsPrincipal userClaims, bool usePoints, CardDetails cardDetails)
    {
        var userId = userClaims.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException("User not authenticated.");
        }

        var order = await GetOrderWithDetailsAsync(orderId, userId);
        var user = await GetUserAsync(userId);
        var coupon = await ValidateCouponAsync(couponCode);

        // Calculate order amount, points to use, and new points earned
        var (totalAmount, discountAmount, pointsToUse, newPointsEarned, pointsUsed) = await CalculateOrderAmount(order, user, coupon, usePoints);
        
        var (amountToChargeByCard, amountToDeductFromWallet) = DeterminePaymentAmounts(user, totalAmount);
        
        if (amountToChargeByCard > 0)
        {
            ValidateCardAndCharge(amountToChargeByCard, cardDetails, user.Email);
        }

        await FinalizeOrderAsync(user, pointsUsed, order, coupon, totalAmount, amountToDeductFromWallet, newPointsEarned);

        return new CheckoutResultDTO
        {
            TotalAmount = order.TotalAmount,
            DiscountAmount = discountAmount,
            CouponCode = coupon.Code,
            PointsUsed = pointsUsed,
            PointsEarned = newPointsEarned,
            AmountChargedToCard = amountToChargeByCard,
            AmountDeductedFromWallet = amountToDeductFromWallet
        };
    }

    private async Task<Order> GetOrderWithDetailsAsync(string orderId, string userId)
    {
        var orders = await unitOfWork.GetRepository<Order>()
            .QueryAsync(o => o.Id.ToString() == orderId && o.UserId == userId, nameof(Order.OrderDetails));

        var order = orders.FirstOrDefault();
        if (order == null)
        {
            throw new InvalidOperationException("Order not found or does not belong to the user.");
        }

        return order;
    }

    private async Task<User> GetUserAsync(string userId)
    {
        var userDto = await userService.GetUserByIdAsync(userId);
        if (userDto == null)
        {
            throw new InvalidOperationException("User not found.");
        }
        return mapper.Map<User>(userDto);
    }

    private async Task<Coupon> ValidateCouponAsync(string couponCode)
    {
        if (string.IsNullOrEmpty(couponCode))
        {
            return null;
        }

        var coupon = await unitOfWork.GetRepository<Coupon>().FirstOrDefaultAsync(c => c.Code == couponCode);
        if (coupon == null || coupon.IsUsed || coupon.ExpiryDate < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Invalid or expired coupon.");
        }

        return coupon;
    }

    private async Task<(decimal totalAmount, decimal discountAmount, decimal pointsToUse, decimal newPointsEarned, decimal pointsUsed)> CalculateOrderAmount(Order order, User user, Coupon coupon, bool usePoints)
    {
        decimal totalAmount = order.TotalAmount;
        decimal discountAmount = coupon?.Amount ?? 0;
        totalAmount -= discountAmount;

        decimal pointsToUse = 0;
        if (usePoints && totalAmount > 0)
        {
            pointsToUse = Math.Min(user.PointsBalance, totalAmount);
            totalAmount -= pointsToUse;
        }

        decimal pointsUsed = pointsToUse;

        decimal newPointsEarned = 0;
        
        if (totalAmount == 0)
            return (totalAmount, discountAmount, pointsToUse, newPointsEarned, pointsUsed);
        
        foreach (var detail in order.OrderDetails)
        {
            var product = await unitOfWork.GetRepository<Product>().FirstOrDefaultAsync(p => p.Id == detail.ProductId);
            if (product != null)
            {
                decimal productPrice = detail.Price * detail.Quantity;
                decimal applicablePrice = productPrice - Math.Min(pointsToUse, productPrice);
                pointsToUse -= Math.Min(pointsToUse, productPrice);

                // Calculate reward points based on the remaining amount after points deduction
                decimal rewardPoints = applicablePrice * product.RewardPercentage / 100;
                rewardPoints = Math.Min(rewardPoints, product.MaxRewardPoints);

                newPointsEarned += rewardPoints;
            }
        }
        return (totalAmount, discountAmount, pointsToUse, newPointsEarned, pointsUsed);
    }

    private (decimal amountToChargeByCard, decimal amountToDeductFromWallet) DeterminePaymentAmounts(User user, decimal totalAmount)
    {
        decimal amountToDeductFromWallet = Math.Min(totalAmount, user.WalletBalance);
        decimal amountToChargeByCard = totalAmount - amountToDeductFromWallet;

        return (amountToChargeByCard, amountToDeductFromWallet);
    }

    private void ValidateCardAndCharge(decimal amountToChargeByCard, CardDetails cardDetails, string userEmail)
    {
        if (!PaymentHelper.ValidateCard(cardDetails))
        {   
            throw new InvalidOperationException("Invalid card details provided.");
        }
        
        // Send a confirmation email
        var subject = "Payment Confirmation";
        var message = $"Dear customer, your payment of ₺{amountToChargeByCard} has been successfully processed.";

        emailService.EnqueueEmail(userEmail, subject, message);
    }

    private async Task FinalizeOrderAsync(User user, decimal pointsUsed, Order order, Coupon coupon, decimal totalAmount, decimal amountDeductedFromWallet, decimal newPointsEarned)
    {
        // Deduct points and update user balances
        user.PointsBalance -= pointsUsed;
        user.WalletBalance -= amountDeductedFromWallet;
        user.PointsBalance += newPointsEarned;
        
        if (coupon != null)
        {
            coupon.IsUsed = true;
            unitOfWork.GetRepository<Coupon>().Update(coupon);
        }
        
        order.CouponAmount = coupon?.Amount ?? 0;
        order.PointsUsed = pointsUsed;
        order.UpdateDate = DateTime.UtcNow;
        order.IsActive = false;
        order.CouponCode = coupon?.Code ?? string.Empty;

        foreach (var products in order.OrderDetails)
        {
            products.IsActive = false;
        }
        
        unitOfWork.GetRepository<Order>().Update(order);
        
        var userDto = mapper.Map<UpdateUserDTO>(user);
        await userService.UpdateUserAsync(userDto);

        await unitOfWork.CompleteAsync();
    }
}




