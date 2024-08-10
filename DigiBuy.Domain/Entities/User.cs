using DigiBuy.Domain.Enumerations;
using Microsoft.AspNetCore.Identity;

namespace DigiBuy.Domain.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    // Users can buy products with their bank card but there's WalletBalance property because for instance
    // When user refund something admins can add the amount to the WalletBalance. 
    public decimal WalletBalance { get; set; }
    public decimal PointsBalance { get; set; }
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
    public List<Order> Orders { get; set; } = new List<Order>();
}

public enum UserStatus
{
    Active,
    Inactive
}