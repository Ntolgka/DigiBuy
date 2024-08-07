using AutoMapper;
using DigiBuy.Application.Dtos.OrderDetailDTOs;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;

namespace DigiBuy.Application.Services.Implementations;

public class OrderDetailService : IOrderDetailService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<CreateOrderDetailDTO> CreateOrderDetailAsync(CreateOrderDetailDTO orderDetailDto)
    {
        var orderDetail = mapper.Map<OrderDetail>(orderDetailDto);
        
        await unitOfWork.GetRepository<OrderDetail>().AddAsync(orderDetail);
        await unitOfWork.CompleteAsync();

        return orderDetailDto;
    }

    // Retrieve a single OrderDetail
    public async Task<ReadOrderDetailDTO> GetOrderDetailByIdAsync(Guid id)
    {
        var orderDetail = await unitOfWork.GetRepository<OrderDetail>().GetByIdAsync(id);
        if (orderDetail == null)
        {
            throw new KeyNotFoundException("Order detail not found.");
        }

        return mapper.Map<ReadOrderDetailDTO>(orderDetail);
    }

    public async Task<IEnumerable<ReadOrderDetailDTO>> GetAllOrderDetailsAsync()
    {
        var orderDetails = await unitOfWork.GetRepository<OrderDetail>().GetAllAsync();
        return mapper.Map<IEnumerable<ReadOrderDetailDTO>>(orderDetails);
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
    }

    // Retrieve a collection of OrderDetail
    public async Task<IEnumerable<ReadOrderDetailDTO>> GetOrderDetailsByOrderIdAsync(Guid orderId)
    {
        var orderDetails = await unitOfWork.GetRepository<OrderDetail>()
            .QueryAsync(od => od.OrderId == orderId);
        return mapper.Map<IEnumerable<ReadOrderDetailDTO>>(orderDetails);
    }
}