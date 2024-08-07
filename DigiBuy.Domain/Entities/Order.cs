namespace DigiBuy.Domain.Entities;

public class Order : BaseEntity
{
    public string UserId { get; set; }
    public User User { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CouponAmount { get; set; }
    public string CouponCode { get; set; } = string.Empty;
    public decimal PointsUsed { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}