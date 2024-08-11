using DigiBuy.Application.Dtos.ProductDTOs;
using FluentValidation;

namespace DigiBuy.Application.Validators.Create;

public class CreateProductDTOValidator : AbstractValidator<CreateProductDTO>
{
    public CreateProductDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product description is required.")
            .MaximumLength(500).WithMessage("Product description cannot exceed 500 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(x => x.Stock)
            .NotEmpty().WithMessage("Product stock is required.");

        RuleFor(x => x.RewardPercentage)
            .InclusiveBetween(0, 100).WithMessage("Reward percentage must be between 0 and 100.");

        RuleFor(x => x.MaxRewardPoints)
            .GreaterThan(0).WithMessage("Max reward points must be greater than zero.");
    }
}