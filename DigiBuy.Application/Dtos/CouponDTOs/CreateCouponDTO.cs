namespace DigiBuy.Application.Dtos.CouponDTOs;

public class CreateCouponDTO
{
    public string Code { get; set; }
    public decimal Amount { get; set; }
    public DateTime ExpiryDate { get; set; }
}