using AutoMapper;
using DigiBuy.Application.Dtos.CheckoutDTOs;
using DigiBuy.Application.Dtos.OrderDTOs;
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
    private readonly ICouponService couponService;

    public CheckoutService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService, ICouponService couponService)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.userService = userService;
        this.couponService = couponService;
    }

    public async Task<CheckoutResultDTO> CheckoutAsync(CreateOrderDTO orderDto, string userId)
    {
        // Fetch user entity
        var userDto = await userService.GetUserByIdAsync(userId);
        if (userDto == null)
        {
            throw new InvalidOperationException("User not found.");
        }
        
        var user = mapper.Map<User>(userDto);

        // Fetch and validate coupon
        Coupon coupon = null;
        if (!string.IsNullOrEmpty(orderDto.CouponCode))
        {
            coupon = await unitOfWork.GetRepository<Coupon>().FirstOrDefaultAsync(c => c.Code == orderDto.CouponCode);
            if (coupon == null || coupon.IsUsed || coupon.ExpiryDate < DateTime.Now)
            {
                throw new InvalidOperationException("Invalid or expired coupon.");
            }
        }

        // Calculate total amount, discount, and points used
        var totalAmount = orderDto.OrderDetails.Sum(od => od.Price * od.Quantity);
        var discountAmount = coupon?.Amount ?? 0;
        var pointsUsed = PointsHelper.CalculatePointsToUse(user, totalAmount - discountAmount);

        // Create order entity
        var order = mapper.Map<Order>(orderDto);
        order.UserId = userId;
        order.TotalAmount = totalAmount;
        order.CouponAmount = discountAmount;
        order.CouponCode = coupon?.Code;
        order.PointsUsed = pointsUsed;

        // Map and assign order details
        order.OrderDetails = orderDto.OrderDetails.Select(od => mapper.Map<OrderDetail>(od)).ToList();

        // Update user's points and wallet balance
        PointsHelper.DeductPoints(user, pointsUsed);
        user.WalletBalance -= Math.Max(totalAmount - discountAmount - pointsUsed, 0);

        // Mark coupon as used if applicable
        if (coupon != null)
        {
            await couponService.UseCouponAsync(coupon.Code, discountAmount);
        }

        // Add order to the database
        await unitOfWork.GetRepository<Order>().AddAsync(order);
        await unitOfWork.CompleteAsync();

        // Return the result DTO
        return new CheckoutResultDTO
        {
            TotalAmount = totalAmount,
            DiscountAmount = discountAmount,
            PointsUsed = pointsUsed
        };
    }
}

