using System.Text.Json;
using AutoMapper;
using DigiBuy.Application.Dtos.OrderDetailDTOs;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace DigiBuy.Application.Services.Implementations;

public class OrderDetailService : IOrderDetailService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly IDistributedCache cache;

    public OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.cache = cache;
        
    }

    public async Task<CreateOrderDetailDTO> CreateOrderDetailAsync(CreateOrderDetailDTO orderDetailDto, string userId)
    {
        var usersOrders = await unitOfWork.GetRepository<Order>()
            .QueryAsync(o => o.UserId == userId, nameof(Order.OrderDetails));
        
        var usersOrder = usersOrders.FirstOrDefault();
        
        var product = await unitOfWork.GetRepository<Product>()
            .FirstOrDefaultAsync(p => p.Id == orderDetailDto.ProductId);

        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {orderDetailDto.ProductId} not found.");
        }
        
        var existingOrderDetail = usersOrder.OrderDetails
            .FirstOrDefault(od => od.ProductId == orderDetailDto.ProductId);

        if (existingOrderDetail != null)
        {
            existingOrderDetail.Quantity += orderDetailDto.Quantity;
            usersOrder.TotalAmount += product.Price * orderDetailDto.Quantity;
            existingOrderDetail.UpdateDate = DateTime.UtcNow;
            unitOfWork.GetRepository<OrderDetail>().Update(existingOrderDetail);
        }
        else
        {
            usersOrder.TotalAmount += product.Price * orderDetailDto.Quantity;
        
            var orderDetail = mapper.Map<OrderDetail>(orderDetailDto);
            orderDetail.OrderId = usersOrder.Id;
            orderDetail.Price = product.Price;
            orderDetail.ProductId = product.Id;
        
        
            await unitOfWork.GetRepository<OrderDetail>().AddAsync(orderDetail);
        }
        
        await unitOfWork.CompleteAsync();
        
        await cache.RemoveAsync($"OrderDetail_{orderDetailDto.ProductId}");
        await cache.RemoveAsync($"OrderDetails_Order_{usersOrder.Id}");
        await cache.RemoveAsync("AllOrderDetails");

        return orderDetailDto;
    }

    // Retrieve a single OrderDetail
    public async Task<ReadOrderDetailDTO> GetOrderDetailByIdAsync(Guid id)
    {
        var cacheKey = $"OrderDetail_{id}";
        var cachedOrderDetail = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedOrderDetail))
        {
            return JsonSerializer.Deserialize<ReadOrderDetailDTO>(cachedOrderDetail);
        }
        
        var orderDetail = await unitOfWork.GetRepository<OrderDetail>().GetByIdAsync(id);
        if (orderDetail == null)
        {
            throw new KeyNotFoundException("Order detail not found.");
        }

        var orderDetailDto = mapper.Map<ReadOrderDetailDTO>(orderDetail);
        
        var serializedOrderDetail = JsonSerializer.Serialize(orderDetailDto);
        await cache.SetStringAsync(cacheKey, serializedOrderDetail);

        return orderDetailDto;
    }

    public async Task<IEnumerable<ReadOrderDetailDTO>> GetAllOrderDetailsAsync()
    {
        var cacheKey = "AllOrderDetails";
        var cachedOrderDetails = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedOrderDetails))
        {
            return JsonSerializer.Deserialize<IEnumerable<ReadOrderDetailDTO>>(cachedOrderDetails);
        }
        
        var orderDetails = await unitOfWork.GetRepository<OrderDetail>().GetAllAsync();
        var orderDetailDtos = mapper.Map<IEnumerable<ReadOrderDetailDTO>>(orderDetails);

        if (orderDetailDtos != null && orderDetailDtos.Any())
        {
            var serializedOrderDetails = JsonSerializer.Serialize(orderDetailDtos);
            await cache.SetStringAsync(cacheKey, serializedOrderDetails);
        }

        return orderDetailDtos;
    }

    public async Task UpdateOrderDetailAsync(Guid id, UpdateOrderDetailDTO orderDetailDto)
    {
        var orderDetail = await unitOfWork.GetRepository<OrderDetail>().GetByIdAsync(id);
        if (orderDetail == null)
        {
            throw new KeyNotFoundException("Order detail not found.");
        }

        mapper.Map(orderDetailDto, orderDetail);
        unitOfWork.GetRepository<OrderDetail>().Update(orderDetail);
        await unitOfWork.CompleteAsync();
        
        await cache.RemoveAsync($"OrderDetail_{id}");
        await cache.RemoveAsync($"OrderDetails_Order_{orderDetail.OrderId}");
        await cache.RemoveAsync("AllOrderDetails");
    }

    public async Task DeleteOrderDetailAsync(Guid id)
    {
        var orderDetail = await unitOfWork.GetRepository<OrderDetail>().GetByIdAsync(id);
        if (orderDetail == null)
        {
            throw new KeyNotFoundException("Order detail not found.");
        }

        unitOfWork.GetRepository<OrderDetail>().Delete(orderDetail);
        await unitOfWork.CompleteAsync();
        
        await cache.RemoveAsync($"OrderDetail_{id}");
        await cache.RemoveAsync($"OrderDetails_Order_{orderDetail.OrderId}");
        await cache.RemoveAsync("AllOrderDetails");
    }

    // Retrieve a collection of OrderDetail
    public async Task<IEnumerable<ReadOrderDetailDTO>> GetOrderDetailsByOrderIdAsync(Guid orderId)
    {
        var cacheKey = $"OrderDetails_Order_{orderId}";
        var cachedOrderDetails = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedOrderDetails))
        {
            return JsonSerializer.Deserialize<IEnumerable<ReadOrderDetailDTO>>(cachedOrderDetails);
        }
        
        var orderDetails = await unitOfWork.GetRepository<OrderDetail>()
            .QueryAsync(od => od.OrderId == orderId);
        
        var orderDetailDtos = mapper.Map<IEnumerable<ReadOrderDetailDTO>>(orderDetails);
        
        if (orderDetailDtos != null && orderDetailDtos.Any())
        {
            var serializedOrderDetails = JsonSerializer.Serialize(orderDetailDtos);
            await cache.SetStringAsync(cacheKey, serializedOrderDetails);
        }
        return orderDetailDtos;
    }
}