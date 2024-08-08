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

    public CheckoutService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.userService = userService;
    }
    
    // TODO - Make this to add points to user regarding to RewardPercentage and make payment then soft delete the order and delete orderdetail(?)
    public async Task<CheckoutResultDTO> CheckoutAsync(string orderId, string couponCode, ClaimsPrincipal userClaims, bool usePoints, CardDetails cardDetails)
    {
        var userId = userClaims.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from claims

        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException("User not authenticated.");
        }

        // Retrieve the existing order
        var order = await GetOrderWithDetailsAsync(orderId, userId);

        // Retrieve user information
        var user = await GetUserAsync(userId);

        // Validate the coupon
        var coupon = await ValidateCouponAsync(couponCode);

        // Calculate the total amount and apply coupon/points
        var (totalAmount, discountAmount, pointsToUse) = CalculateOrderAmount(order, user, coupon, usePoints);

        // Simulate payment
        if (!PaymentHelper.ValidateCard(cardDetails))
        {
            throw new InvalidOperationException("Invalid card details provided.");
        }
        
        SimulateCardPayment(totalAmount);

        // Finalize the order
        await FinalizeOrderAsync(user, pointsToUse, order, coupon, totalAmount);

        return new CheckoutResultDTO
        {
            TotalAmount = totalAmount,
            DiscountAmount = discountAmount,
            PointsUsed = pointsToUse
        };
    }

    private async Task<Order> GetOrderWithDetailsAsync(string orderId, string userId)
    {
        // Eagerly load OrderDetails
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

    private (decimal totalAmount, decimal discountAmount, decimal pointsToUse) CalculateOrderAmount(Order order, User user, Coupon coupon, bool usePoints)
    {
        decimal totalAmount = order.OrderDetails.Sum(od => od.Price * od.Quantity);
        decimal discountAmount = coupon?.Amount ?? 0;
        totalAmount -= discountAmount;

        decimal pointsToUse = 0;
        if (usePoints)
        {
            pointsToUse = PointsHelper.CalculatePointsToUse(user, totalAmount);
            if (pointsToUse > user.PointsBalance)
            {
                pointsToUse = user.PointsBalance;
            }
            totalAmount -= pointsToUse;
        }

        return (totalAmount, discountAmount, pointsToUse);
    }

    private void SimulateCardPayment(decimal totalAmount)
    {
        // Send email that the payment was successfull
        Console.WriteLine($"Simulated card payment of ${totalAmount}");
    }

    private async Task FinalizeOrderAsync(User user, decimal pointsToUse, Order order, Coupon coupon, decimal totalAmount)
    {
        PointsHelper.DeductPoints(user, pointsToUse);
        user.WalletBalance -= Math.Max(totalAmount, 0);

        if (coupon != null)
        {
            coupon.IsUsed = true;
            unitOfWork.GetRepository<Coupon>().Update(coupon);
        }

        var updatedUserDto = mapper.Map<UpdateUserDTO>(user);

        order.TotalAmount = totalAmount;
        order.CouponAmount = coupon?.Amount ?? 0;
        order.PointsUsed = pointsToUse;
        order.UpdateDate = DateTime.UtcNow;
        order.IsActive = false;

        unitOfWork.GetRepository<Order>().Update(order);
        await userService.UpdateUserAsync(updatedUserDto);
        await unitOfWork.CompleteAsync();
    }
}



