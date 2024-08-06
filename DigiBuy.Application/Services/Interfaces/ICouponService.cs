using DigiBuy.Application.Dtos.CouponDTOs;

namespace DigiBuy.Application.Services.Interfaces;

public interface ICouponService
{
    Task<CreateCouponDTO> CreateCouponAsync(CreateCouponDTO couponDto);
    Task<ReadCouponDTO> GetCouponByIdAsync(Guid id);
    Task<IEnumerable<ReadCouponDTO>> GetAllCouponsAsync();
    Task UpdateCouponAsync(Guid id, UpdateCouponDTO couponDto); 
    Task DeleteCouponAsync(Guid id);
}