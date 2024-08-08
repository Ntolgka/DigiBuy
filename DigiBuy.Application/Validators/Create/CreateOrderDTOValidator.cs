using DigiBuy.Application.Dtos.OrderDTOs;
using FluentValidation;

namespace DigiBuy.Application.Validators.Create;

public class CreateOrderDTOValidator : AbstractValidator<CreateOrderDTO>
{
    public CreateOrderDTOValidator()
    {
        RuleFor(x => x.OrderDetails)
            .NotEmpty().WithMessage("Order details are required.")
            .Must(x => x != null && x.Count > 0).WithMessage("Order must contain at least one detail.");
        
    }
}