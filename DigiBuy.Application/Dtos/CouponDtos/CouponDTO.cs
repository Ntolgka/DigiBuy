namespace DigiBuy.Application.Dtos.CouponDtos;

public class CouponDTO
{
    public int Id { get; set; }
    public string Code { get; set; }
    public decimal Amount { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsUsed { get; set; }
}