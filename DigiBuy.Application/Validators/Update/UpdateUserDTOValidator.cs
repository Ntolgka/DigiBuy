using DigiBuy.Application.Dtos.UserDTOs;
using FluentValidation;

namespace DigiBuy.Application.Validators.Update;

public class UpdateUserDTOValidator : AbstractValidator<UpdateUserDTO>
{
    public UpdateUserDTOValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.WalletBalance)
            .GreaterThanOrEqualTo(0).WithMessage("Wallet balance cannot be negative.");

        RuleFor(x => x.PointsBalance)
            .GreaterThanOrEqualTo(0).WithMessage("Points balance cannot be negative.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid user status.");
    }
}