using DigiBuy.Application.Dtos.OrderDetailDTOs;
using FluentValidation;

namespace DigiBuy.Application.Validators.Create;

public class CreateOrderDetailDTOValidator : AbstractValidator<CreateOrderDetailDTO>
{
    public CreateOrderDetailDTOValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required.");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}