using DigiBuy.Application.Dtos.OrderDetailDtos;

namespace DigiBuy.Application.Dtos.OrderDtos;

public class OrderDTO
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CouponAmount { get; set; }
    public string CouponCode { get; set; }
    public decimal PointsUsed { get; set; }
    public List<OrderDetailDTO> OrderDetails { get; set; }
}