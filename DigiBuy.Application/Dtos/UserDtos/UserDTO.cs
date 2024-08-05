﻿using DigiBuy.Domain.Entities;

namespace DigiBuy.Application.Dtos.UserDtos;

public class UserDTO
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public decimal WalletBalance { get; set; }
    public decimal PointsBalance { get; set; }
    public UserStatus Status { get; set; }
}