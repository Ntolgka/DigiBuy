using DigiBuy.Application.Dtos.CouponDTOs;
using FluentValidation;

namespace DigiBuy.Application.Validators.Update;

public class UpdateCouponDTOValidator : AbstractValidator<UpdateCouponDTO>
{
    public UpdateCouponDTOValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Coupon code is required.")
            .MaximumLength(50).WithMessage("Coupon code cannot exceed 50 characters.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.");

        RuleFor(x => x.IsUsed)
            .NotNull().WithMessage("IsUsed flag must be provided.");
    }
}