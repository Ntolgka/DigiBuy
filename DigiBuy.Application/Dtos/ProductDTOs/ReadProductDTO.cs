﻿namespace DigiBuy.Application.Dtos.ProductDTOs;

public class ReadProductDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public decimal RewardPercentage { get; set; }
    public decimal MaxRewardPoints { get; set; }
}