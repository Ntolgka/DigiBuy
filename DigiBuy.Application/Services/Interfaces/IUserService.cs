using DigiBuy.Application.Dtos.UserDTOs;

namespace DigiBuy.Application.Services.Interfaces;

public interface IUserService
{
    Task<ReadUserDTO> RegisterAsync(CreateUserDTO userDto);
    Task<ReadUserDTO> RegisterAdminAsync(CreateUserDTO userDto);
    Task<string> LoginAsync(LoginUserDTO loginDto);
    Task LogoutAsync();
    Task ChangePasswordAsync(ChangePasswordDTO changePasswordDto);
    Task<ReadUserDTO> GetUserByIdAsync(string userId);
    Task UpdateUserAsync(UpdateUserDTO userDto);    
    Task DeleteUserAsync(string userId);
}