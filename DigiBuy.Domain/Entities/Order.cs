namespace DigiBuy.Domain.Entities;

public class Order : BaseEntity
{
    // Guid id for operations and OrderNumber for users. guid is more safe and OrderNumber is more readable
    public string OrderNumber { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CouponAmount { get; set; }
    public string CouponCode { get; set; } = string.Empty;
    public decimal PointsUsed { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}