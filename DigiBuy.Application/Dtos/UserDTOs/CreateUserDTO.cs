using DigiBuy.Domain.Entities;

namespace DigiBuy.Application.Dtos.UserDTOs;

public class CreateUserDTO
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
}