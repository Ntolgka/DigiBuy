namespace DigiBuy.Application.Dtos.CouponDTOs;

public class UpdateCouponDTO
{
    public string Code { get; set; }
    public decimal Amount { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsUsed { get; set; }
}