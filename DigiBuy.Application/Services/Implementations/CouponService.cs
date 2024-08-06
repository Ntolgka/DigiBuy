using AutoMapper;
using DigiBuy.Application.Dtos.CouponDTOs;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;

namespace DigiBuy.Application.Services.Implementations;

// TODO - Every time coupon is used, the amount will decrease and when its 0, the IsUsed will be true.
public class CouponService : ICouponService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public CouponService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<CreateCouponDTO> CreateCouponAsync(CreateCouponDTO couponDto)
    {
        var existingCoupon = await unitOfWork.GetRepository<Coupon>().FirstOrDefaultAsync(c => c.Code == couponDto.Code);
        if (existingCoupon != null)
        {
            throw new Exception("Coupon code already exists.");
        }
        
        if (couponDto.Code.Length > 10)
        {
            throw new Exception("Coupon code must be less than 10 characters.");
        }

        var coupon = mapper.Map<Coupon>(couponDto);
        coupon.InsertDate = DateTime.UtcNow;
        coupon.IsUsed = false;
        await unitOfWork.GetRepository<Coupon>().AddAsync(coupon);
        await unitOfWork.CompleteAsync();
        return couponDto;
    }

    public async Task<ReadCouponDTO> GetCouponByIdAsync(Guid id)
    {
        var coupon = await unitOfWork.GetRepository<Coupon>().GetById(id);
        return mapper.Map<ReadCouponDTO>(coupon);
    }

    public async Task<IEnumerable<ReadCouponDTO>> GetAllCouponsAsync()
    {
        var coupons = await unitOfWork.GetRepository<Coupon>().GetAllAsync();
        return mapper.Map<IEnumerable<ReadCouponDTO>>(coupons);
    }

    public async Task UpdateCouponAsync(Guid id, UpdateCouponDTO couponDto)
    {
        var coupon = await unitOfWork.GetRepository<Coupon>().GetById(id);
        if (coupon == null)
        {
            throw new KeyNotFoundException("Coupon not found");
        }

        mapper.Map(couponDto, coupon);
        coupon.UpdateDate = DateTime.UtcNow;
        unitOfWork.GetRepository<Coupon>().Update(coupon);
        await unitOfWork.CompleteAsync();
    }

    // Soft Delete
    public async Task DeleteCouponAsync(Guid id)
    {
        var coupon = await unitOfWork.GetRepository<Coupon>().GetById(id);
        coupon.IsUsed = true;
        
        await unitOfWork.CompleteAsync();
    }
}