using DigiBuy.Application.Dtos.ProductCategoryDTOs;
using FluentValidation;

namespace DigiBuy.Application.Validators.Create;

public class CreateProductCategoryDTOValidator : AbstractValidator<CreateProductCategoryDTO>
{
    public CreateProductCategoryDTOValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");
    }
}