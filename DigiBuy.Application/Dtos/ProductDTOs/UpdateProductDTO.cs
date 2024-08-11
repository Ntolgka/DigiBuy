namespace DigiBuy.Application.Dtos.ProductDTOs;

public class UpdateProductDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public int Stock { get; set; }
    public decimal Price { get; set; }
    public decimal RewardPercentage { get; set; }
    public decimal MaxRewardPoints { get; set; }
}