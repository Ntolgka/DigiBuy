using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Enumerations;

namespace DigiBuy.Application.Dtos.UserDTOs;

public class ReadUserDTO
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public decimal WalletBalance { get; set; }
    public decimal PointsBalance { get; set; }
    public UserStatus Status { get; set; }
    public UserRole Role { get; set; }
}