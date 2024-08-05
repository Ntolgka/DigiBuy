using DigiBuy.Application.Dtos.OrderDetailDTOs;

namespace DigiBuy.Application.Dtos.OrderDTOs;

public class CreateOrderDTO
{
    public string UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CouponAmount { get; set; }
    public string CouponCode { get; set; }
    public decimal PointsUsed { get; set; }
    public ICollection<CreateOrderDetailDTO> OrderDetails { get; set; }
}