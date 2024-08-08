using DigiBuy.Application.Dtos.ProductCategoryDTOs;
using FluentValidation;

namespace DigiBuy.Application.Validators.Update;

public class UpdateProductCategoryDTOValidator : AbstractValidator<UpdateProductCategoryDTO>
{
    public UpdateProductCategoryDTOValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");
    }
}