using AutoMapper;
using DigiBuy.Application.Dtos.OrderDTOs;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;

namespace DigiBuy.Application.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<CreateOrderDTO> CreateOrderAsync(CreateOrderDTO orderDto)
    {
        var order = mapper.Map<Order>(orderDto);
        await unitOfWork.GetRepository<Order>().AddAsync(order);
        await unitOfWork.CompleteAsync();
        return orderDto;
    }

    public async Task<ReadOrderDTO> GetOrderByIdAsync(Guid id)
    {
        var order = await unitOfWork.GetRepository<Order>().GetByIdAsync(id);
        return mapper.Map<ReadOrderDTO>(order);
    }

    public async Task<IEnumerable<ReadOrderDTO>> GetAllOrdersAsync()
    {
        var orders = await unitOfWork.GetRepository<Order>().GetAllAsync();
        return mapper.Map<IEnumerable<ReadOrderDTO>>(orders);
    }
    
    public async Task<IEnumerable<ReadOrderDTO>> GetOrdersByUserIdAsync(string userId)
    {
        var orders = await unitOfWork.GetRepository<Order>().QueryAsync(o => o.UserId == userId);
        return mapper.Map<IEnumerable<ReadOrderDTO>>(orders);
    }

    public async Task UpdateOrderAsync(UpdateOrderDTO orderDto)
    {
        var order = mapper.Map<Order>(orderDto);
        unitOfWork.GetRepository<Order>().Update(order);
        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteOrderAsync(Guid id)
    {
        await unitOfWork.GetRepository<Order>().DeleteAsync(id);
        await unitOfWork.CompleteAsync();
    }
}