namespace DigiBuy.Application.Dtos.CheckoutDTOs;

public class CheckoutResultDTO
{
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal PointsUsed { get; set; }
}