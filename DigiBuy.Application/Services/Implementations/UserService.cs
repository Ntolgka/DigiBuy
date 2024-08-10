using AutoMapper;
using DigiBuy.Application.Dtos.UserDTOs;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DigiBuy.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;
    private readonly IMapper mapper;
    private readonly JwtTokenService jwtTokenService;
    private readonly ILogger<UserService> logger;

    public UserService(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, JwtTokenService jwtTokenService, ILogger<UserService> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager; 
        this.mapper = mapper;
        this.jwtTokenService = jwtTokenService;
        this.logger = logger;
    }

    public async Task<ReadUserDTO> RegisterAsync(CreateUserDTO userDto)
    {
        try
        {
            var user = mapper.Map<User>(userDto);
            user.Status = UserStatus.Active;
            user.Role = UserRole.User;
            var result = await userManager.CreateAsync(user, userDto.Password);

            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("Failed to register user with errors: {Errors}", errorMessages);
                throw new Exception(errorMessages);
            }

            string userRole = UserRole.User.ToString();
            await userManager.AddToRoleAsync(user, userRole);

            return mapper.Map<ReadUserDTO>(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred during registration for user: {UserName}", userDto.UserName);
            throw;
        }
    }
    
    public async Task<ReadUserDTO> RegisterAdminAsync(CreateUserDTO userDto)
    {
        try
        {
            var user = mapper.Map<User>(userDto);
            user.Status = UserStatus.Active;
            user.Role = UserRole.Admin;
            
            var result = await userManager.CreateAsync(user, userDto.Password);

            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("Failed to register admin with errors: {Errors}", errorMessages);
                throw new Exception(errorMessages);
            }

            var adminRole = UserRole.Admin;
            
            var addRoleResult = await userManager.AddToRoleAsync(user, adminRole.ToString());
            if (!addRoleResult.Succeeded)
            {
                var errorMessages = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                logger.LogError("Failed to add admin role to user with errors: {Errors}", errorMessages);
                throw new Exception(errorMessages);
            }

            return mapper.Map<ReadUserDTO>(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred during admin registration for user: {UserName}", userDto.UserName);
            throw;
        }
    }

    public async Task<string> LoginAsync(LoginUserDTO loginDto)
    {
        try
        {
            var user = await userManager.FindByNameAsync(loginDto.UserName);

            if (user == null || !await userManager.CheckPasswordAsync(user, loginDto.Password) || user.Status == UserStatus.Inactive)
            {
                logger.LogWarning("Invalid login attempt for user: {UserName}", loginDto.UserName);
                throw new Exception("Invalid username or password.");
            }

            await signInManager.SignInAsync(user, isPersistent: false);

            return jwtTokenService.GenerateToken(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred during login for user: {UserName}", loginDto.UserName);
            throw;
        }
    }
    
    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }
    
    public async Task ChangePasswordAsync(ChangePasswordDTO changePasswordDto)
    {
        try
        {
            // Find the user by ID
            var user = await userManager.FindByIdAsync(changePasswordDto.UserId);
            if (user == null)
            {
                logger.LogWarning("User not found for ID: {UserId}", changePasswordDto.UserId);
                throw new Exception("User not found.");
            }

            // Change the password
            var result = await userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("Failed to change password with errors: {Errors}", errorMessages);
                throw new Exception(errorMessages);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while changing password for user: {UserId}", changePasswordDto.UserId);
            throw;
        }
    }

    public async Task<ReadUserDTO> GetUserByIdAsync(string userId)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User not found for ID: {UserId}", userId);
                throw new Exception("User not found.");
            }

            return mapper.Map<ReadUserDTO>(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while retrieving user by ID: {UserId}", userId);
            throw;
        }
    }
    
    public async Task<decimal> GetUserPointsAsync(string userId)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User not found for ID: {UserId}", userId);
                throw new Exception("User not found.");
            }

            return user.PointsBalance;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while retrieving user by ID: {UserId}", userId);
            throw;
        }
    }

    public async Task UpdateUserAsync(UpdateUserDTO userDto)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userDto.Id);
            if (user == null)
            {
                logger.LogWarning("User not found for ID: {UserId}", userDto.Id);
                throw new Exception("User not found.");
            }

            mapper.Map(userDto, user);
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("Failed to update user with errors: {Errors}", errorMessages);
                throw new Exception(errorMessages);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while updating user with ID: {UserId}", userDto.Id);
            throw;
        }
    }

    public async Task DeleteUserAsync(string userId)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User not found for ID: {UserId}", userId);
                throw new Exception("User not found.");
            }

            user.Status = UserStatus.Inactive;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("Failed to delete user with errors: {Errors}", errorMessages);
                throw new Exception(errorMessages);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while deleting user with ID: {UserId}", userId);
            throw;
        }
    }
}