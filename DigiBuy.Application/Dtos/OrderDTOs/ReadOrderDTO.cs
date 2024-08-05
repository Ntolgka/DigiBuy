using DigiBuy.Application.Dtos.OrderDetailDTOs;

namespace DigiBuy.Application.Dtos.OrderDTOs;

public class ReadOrderDTO
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CouponAmount { get; set; }
    public string CouponCode { get; set; }
    public decimal PointsUsed { get; set; }
    public ICollection<ReadOrderDetailDTO> OrderDetails { get; set; }
}