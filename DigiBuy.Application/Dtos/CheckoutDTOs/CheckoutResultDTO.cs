namespace DigiBuy.Application.Dtos.CheckoutDTOs;

public class CheckoutResultDTO
{
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? CouponCode { get; set; }
    public decimal PointsUsed { get; set; }
    public decimal PointsEarned { get; set; }
    public decimal AmountChargedToCard { get; set; }
    public decimal AmountDeductedFromWallet { get; set; }
}