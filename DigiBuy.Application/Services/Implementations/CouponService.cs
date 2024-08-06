using AutoMapper;
using DigiBuy.Application.Dtos.CouponDTOs;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;

namespace DigiBuy.Application.Services.Implementations;

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
        var coupon = mapper.Map<Coupon>(couponDto);
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

    public async Task UpdateCouponAsync(UpdateCouponDTO couponDto)
    {
        var coupon = mapper.Map<Coupon>(couponDto);
        unitOfWork.GetRepository<Coupon>().Update(coupon);
        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteCouponAsync(Guid id)
    {
        await unitOfWork.GetRepository<Coupon>().DeleteAsync(id);
        await unitOfWork.CompleteAsync();
    }
}