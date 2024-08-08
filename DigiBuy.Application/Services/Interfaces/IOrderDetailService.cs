using DigiBuy.Application.Dtos.OrderDetailDTOs;

namespace DigiBuy.Application.Services.Interfaces;

public interface IOrderDetailService
{
    Task<CreateOrderDetailDTO> CreateOrderDetailAsync(CreateOrderDetailDTO orderDetailDto, string userId);
    Task<ReadOrderDetailDTO> GetOrderDetailByIdAsync(Guid id);
    Task<IEnumerable<ReadOrderDetailDTO>> GetAllOrderDetailsAsync();
    Task<IEnumerable<ReadOrderDetailDTO>> GetOrderDetailsByOrderIdAsync(Guid orderId);
    Task UpdateOrderDetailAsync(Guid id, UpdateOrderDetailDTO orderDetailDto);  
    Task DeleteOrderDetailAsync(Guid id);
}   