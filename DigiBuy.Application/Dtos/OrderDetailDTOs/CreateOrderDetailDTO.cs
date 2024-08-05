namespace DigiBuy.Application.Dtos.OrderDetailDTOs;

public class CreateOrderDetailDTO
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}