using DigiBuy.Application.Dtos.UserDTOs;
using FluentValidation;

namespace DigiBuy.Application.Validators.Create;

public class LoginUserDTOValidator : AbstractValidator<LoginUserDTO>
{
    public LoginUserDTOValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}