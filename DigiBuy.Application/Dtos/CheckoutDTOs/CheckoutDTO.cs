using DigiBuy.Application.Dtos.OrderDetailDTOs;

namespace DigiBuy.Application.Dtos.CheckoutDTOs;

public class CheckoutDTO
{
    public string UserId { get; set; }
    public decimal CartTotal { get; set; }
    public string CouponCode { get; set; }
    public List<CreateOrderDetailDTO> OrderDetails { get; set; }
}