using DigiBuy.Application.Dtos.CheckoutDTOs;
using DigiBuy.Application.Dtos.OrderDTOs;

namespace DigiBuy.Application.Services.Interfaces;

public interface ICheckoutService
{
    Task<CheckoutResultDTO> CheckoutAsync(CreateOrderDTO orderDto, string userId);
}