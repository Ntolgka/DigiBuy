using System.Security.Claims;
using DigiBuy.Application.Dtos.OrderDTOs;

namespace DigiBuy.Application.Services.Interfaces;

public interface IOrderService
{
    Task<CreateOrderDTO> CreateOrderAsync(CreateOrderDTO orderDto, string userId);
    Task<ReadOrderDTO> GetOrderByIdAsync(Guid id);
    Task<ReadOrderDTO> GetOrderByOrderNumberAsync(string orderId);
    Task<IEnumerable<ReadOrderDTO>> GetAllOrdersAsync();
    Task<IEnumerable<ReadOrderDTO>> GetOrdersByUserIdAsync(string userId);
    Task<IEnumerable<ReadOrderDTO>> GetActiveOrdersByUserIdAsync();
    Task<IEnumerable<ReadOrderDTO>> GetInactiveOrdersByUserIdAsync();
    Task UpdateOrderAsync(Guid id, UpdateOrderDTO orderDto);
    Task DeleteOrderAsync(Guid id); 
}