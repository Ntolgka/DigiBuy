using DigiBuy.Application.Dtos.OrderDTOs;

namespace DigiBuy.Application.Services.Interfaces;

public interface IOrderService
{
    Task<CreateOrderDTO> CreateOrderAsync(CreateOrderDTO orderDto);
    Task<ReadOrderDTO> GetOrderByIdAsync(Guid id);
    Task<IEnumerable<ReadOrderDTO>> GetAllOrdersAsync();
    Task<IEnumerable<ReadOrderDTO>> GetOrdersByUserIdAsync(string userId);
    Task UpdateOrderAsync(UpdateOrderDTO orderDto);
    Task DeleteOrderAsync(Guid id);
}