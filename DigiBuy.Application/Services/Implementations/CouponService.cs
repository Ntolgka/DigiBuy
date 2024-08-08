using System.Text.Json;
using AutoMapper;
using DigiBuy.Application.Dtos.CouponDTOs;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace DigiBuy.Application.Services.Implementations;

public class CouponService : ICouponService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly IDistributedCache cache;

    public CouponService(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.cache = cache;
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
        
        await cache.RemoveAsync("AllCoupons");
        await cache.RemoveAsync($"Coupon_Code_{couponDto.Code}");
        
        return couponDto;
    }

    public async Task<ReadCouponDTO> GetCouponByIdAsync(Guid id)
    {
        var cacheKey = $"Coupon_{id}";
        var cachedCoupon = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedCoupon))
        {
            return JsonSerializer.Deserialize<ReadCouponDTO>(cachedCoupon);
        }
        
        var coupon = await unitOfWork.GetRepository<Coupon>().GetByIdAsync(id);
        var couponDto = mapper.Map<ReadCouponDTO>(coupon);

        if (couponDto != null)
        {
            var serializedCoupon = JsonSerializer.Serialize(couponDto);
            await cache.SetStringAsync(cacheKey, serializedCoupon);
        }

        return couponDto;
    }
    
    public async Task<ReadCouponDTO> GetCouponByCodeAsync(string code)
    {
        var cacheKey = $"Coupon_Code_{code}";
        var cachedCoupon = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedCoupon))
        {
            return JsonSerializer.Deserialize<ReadCouponDTO>(cachedCoupon);
        }
        
        var coupon = await unitOfWork.GetRepository<Coupon>().FirstOrDefaultAsync(c => c.Code == code);
        var couponDto = mapper.Map<ReadCouponDTO>(coupon);

        if (couponDto != null)
        {
            var serializedCoupon = JsonSerializer.Serialize(couponDto);
            await cache.SetStringAsync(cacheKey, serializedCoupon);
        }

        return couponDto;
    }
    

    public async Task<IEnumerable<ReadCouponDTO>> GetAllCouponsAsync()
    {
        var cacheKey = "AllCoupons";
        var cachedCoupons = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedCoupons))
        {
            return JsonSerializer.Deserialize<IEnumerable<ReadCouponDTO>>(cachedCoupons);
        }
        
        var coupons = await unitOfWork.GetRepository<Coupon>().GetAllAsync();
        var couponDtos = mapper.Map<IEnumerable<ReadCouponDTO>>(coupons);

        if (couponDtos != null && couponDtos.Any())
        {
            var serializedCoupons = JsonSerializer.Serialize(couponDtos);
            await cache.SetStringAsync(cacheKey, serializedCoupons);
        }

        return couponDtos;
    }
    

    public async Task UpdateCouponAsync(Guid id, UpdateCouponDTO couponDto)
    {
        var coupon = await unitOfWork.GetRepository<Coupon>().GetByIdAsync(id);
        if (coupon == null)
        {
            throw new KeyNotFoundException("Coupon not found");
        }

        mapper.Map(couponDto, coupon);
        coupon.UpdateDate = DateTime.UtcNow;
        unitOfWork.GetRepository<Coupon>().Update(coupon);
        await unitOfWork.CompleteAsync();
        
        await cache.RemoveAsync($"Coupon_{id}");
        await cache.RemoveAsync($"Coupon_Code_{coupon.Code}");
        await cache.RemoveAsync("AllCoupons");
    }

    public async Task DeleteCouponAsync(Guid id)
    {
        var coupon = await unitOfWork.GetRepository<Coupon>().GetByIdAsync(id);
        if (coupon == null)
        {
            throw new KeyNotFoundException("Coupon not found");
        }

        coupon.IsUsed = true;
        unitOfWork.GetRepository<Coupon>().Update(coupon);
        await unitOfWork.CompleteAsync();
        
        await cache.RemoveAsync($"Coupon_{id}");
        await cache.RemoveAsync($"Coupon_Code_{coupon.Code}");
        await cache.RemoveAsync("AllCoupons");
    }
}
