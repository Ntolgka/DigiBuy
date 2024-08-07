using System.Security.Claims;
using DigiBuy.Application.Dtos.CheckoutDTOs;

namespace DigiBuy.Application.Services.Interfaces;

public interface ICheckoutService
{
    Task<CheckoutResultDTO> CheckoutAsync(string orderId, string couponCode, ClaimsPrincipal userClaims, bool usePoints);
}