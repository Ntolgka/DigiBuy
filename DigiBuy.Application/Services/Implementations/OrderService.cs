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

    public async Task<CreateOrderDTO> CreateOrderAsync(CreateOrderDTO orderDto, string userId)
    {
        var existingOrder = await unitOfWork.GetRepository<Order>()
            .FirstOrDefaultAsync(o => o.UserId == userId && o.IsActive);

        if (existingOrder != null)
        {
            throw new InvalidOperationException("You already have an active order.");
        }   
        
        var order = mapper.Map<Order>(orderDto);
        order.UserId = userId;
        order.IsActive = true;
        order.InsertDate = DateTime.UtcNow;
        
        if (order.OrderDetails == null)
        {
            order.OrderDetails = new List<OrderDetail>();
        }

        foreach (var item in orderDto.OrderDetails)
        {
            var product = await unitOfWork.GetRepository<Product>()
                .FirstOrDefaultAsync(p => p.Id == item.ProductId);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {item.ProductId} not found.");
            }

            var productPrice = product.Price;
            order.TotalAmount += productPrice * item.Quantity;

            var orderDetail = new OrderDetail
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = productPrice, 
                InsertDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                IsActive = true
            };
            order.OrderDetails.Add(orderDetail);
        }

        await unitOfWork.GetRepository<Order>().AddAsync(order);
        await unitOfWork.CompleteAsync();

        return orderDto;
    }

    public async Task<ReadOrderDTO> GetOrderByIdAsync(Guid id)
    {
        var order = await unitOfWork.GetRepository<Order>().GetByIdAsync(id, nameof(Order.OrderDetails));
        if (order == null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        return mapper.Map<ReadOrderDTO>(order);
    }
    
    public async Task<IEnumerable<ReadOrderDTO>> GetOrdersByUserIdAsync(string userId)
    {
        var orders = await unitOfWork.GetRepository<Order>()
            .QueryAsync(o => o.UserId == userId, nameof(Order.OrderDetails));
        return mapper.Map<IEnumerable<ReadOrderDTO>>(orders);
    }

    public async Task<IEnumerable<ReadOrderDTO>> GetAllOrdersAsync()
    {
        var orders = await unitOfWork.GetRepository<Order>().GetAllAsync(nameof(Order.OrderDetails));
        return mapper.Map<IEnumerable<ReadOrderDTO>>(orders);
    }

    public async Task UpdateOrderAsync(Guid id, UpdateOrderDTO orderDto)
    {
        var order = await unitOfWork.GetRepository<Order>().GetByIdAsync(id);
        if (order == null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        mapper.Map(orderDto, order);
        unitOfWork.GetRepository<Order>().Update(order);
        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteOrderAsync(Guid id)
    {
        var order = await unitOfWork.GetRepository<Order>().GetByIdAsync(id);
        if (order == null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        unitOfWork.GetRepository<Order>().Delete(order);
        await unitOfWork.CompleteAsync();
    }
}