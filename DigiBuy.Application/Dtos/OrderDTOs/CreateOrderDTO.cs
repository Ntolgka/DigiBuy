using DigiBuy.Application.Dtos.OrderDetailDTOs;

namespace DigiBuy.Application.Dtos.OrderDTOs;

public class CreateOrderDTO
{
    public ICollection<OrderDetailDTO> OrderDetails { get; set; }
}