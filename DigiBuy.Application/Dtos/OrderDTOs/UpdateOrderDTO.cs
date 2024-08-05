namespace DigiBuy.Application.Dtos.OrderDTOs;

public class UpdateOrderDTO
{
    public decimal TotalAmount { get; set; }
    public decimal CouponAmount { get; set; }
    public string CouponCode { get; set; }
    public decimal PointsUsed { get; set; }
}