using DigiBuy.Application.Dtos.OrderDetailDTOs;

namespace DigiBuy.Application.Dtos.OrderDTOs;

public class UpdateOrderDTO
{
    public decimal TotalAmount { get; set; }
    public decimal CouponAmount { get; set; }
    public string CouponCode { get; set; }
    public decimal PointsUsed { get; set; }
    public List<OrderDetailDTO> OrderDetails { get; set; }
}