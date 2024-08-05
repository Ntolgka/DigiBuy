using Microsoft.AspNetCore.Identity;

namespace DigiBuy.Domain.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public decimal WalletBalance { get; set; }
    public decimal PointsBalance { get; set; }
    public UserStatus Status { get; set; }
}

public enum UserStatus
{
    Active,
    Inactive
}