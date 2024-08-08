using DigiBuy.Application.Dtos.OrderDTOs;
using FluentValidation;

namespace DigiBuy.Application.Validators.Update;

public class UpdateOrderDTOValidator : AbstractValidator<UpdateOrderDTO>
{
    public UpdateOrderDTOValidator()
    {
        RuleFor(x => x.TotalAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Total amount cannot be negative.");

        RuleFor(x => x.CouponAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Coupon amount cannot be negative.");

        RuleFor(x => x.CouponCode)
            .MaximumLength(50).WithMessage("Coupon code cannot exceed 50 characters.");

        RuleFor(x => x.PointsUsed)
            .GreaterThanOrEqualTo(0).WithMessage("Points used cannot be negative.");

        RuleFor(x => x.OrderDetails)
            .NotNull().WithMessage("Order details cannot be null.")
            .Must(x => x.Count > 0).WithMessage("Order must contain at least one detail.");
    }
}