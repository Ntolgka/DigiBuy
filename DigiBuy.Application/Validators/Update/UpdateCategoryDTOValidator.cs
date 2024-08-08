using DigiBuy.Application.Dtos.CategoryDTOs;
using FluentValidation;

namespace DigiBuy.Application.Validators.Update;

public class UpdateCategoryDTOValidator : AbstractValidator<UpdateCategoryDTO>
{
    public UpdateCategoryDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("URL is required.");

        RuleFor(x => x.Tags)
            .MaximumLength(200).WithMessage("Tags cannot exceed 200 characters.");
    }
}