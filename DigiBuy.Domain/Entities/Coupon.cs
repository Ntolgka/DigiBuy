namespace DigiBuy.Domain.Entities;

public class Coupon : BaseEntity
{
    public string Code { get; set; }
    public decimal Amount { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsUsed { get; set; }
}