using DigiBuy.Application.Dtos.OrderDetailDTOs;
using FluentValidation;

namespace DigiBuy.Application.Validators.Update;

public class UpdateOrderDetailDTOValidator : AbstractValidator<UpdateOrderDetailDTO>
{
    public UpdateOrderDetailDTOValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}