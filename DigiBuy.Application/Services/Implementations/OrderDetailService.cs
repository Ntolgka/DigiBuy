using AutoMapper;
using DigiBuy.Application.Dtos.OrderDetailDTOs;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;

namespace DigiBuy.Application.Services.Implementations;

public class OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper) : IOrderDetailService
{
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly IMapper mapper = mapper;

        public async Task<CreateOrderDetailDTO> CreateOrderDetailAsync(CreateOrderDetailDTO orderDetailDto)
        {
            var orderDetail = mapper.Map<OrderDetail>(orderDetailDto);  
            await unitOfWork.GetRepository<OrderDetail>().AddAsync(orderDetail);
            await unitOfWork.CompleteAsync();
            return orderDetailDto;
        }

        public async Task<ReadOrderDetailDTO> GetOrderDetailByIdAsync(Guid id)
        {
            var orderDetail = await unitOfWork.GetRepository<OrderDetail>().GetByIdAsync(id);
            return mapper.Map<ReadOrderDetailDTO>(orderDetail);
        }

        public async Task<IEnumerable<ReadOrderDetailDTO>> GetAllOrderDetailsAsync()
        {
            var orderDetails = await unitOfWork.GetRepository<OrderDetail>().GetAllAsync();
            return mapper.Map<IEnumerable<ReadOrderDetailDTO>>(orderDetails);
        }

        public async Task<IEnumerable<ReadOrderDetailDTO>> GetOrderDetailsByOrderIdAsync(Guid orderId)
        {
            var orderDetails = await unitOfWork.GetRepository<OrderDetail>().QueryAsync(od => od.OrderId == orderId);
            return mapper.Map<IEnumerable<ReadOrderDetailDTO>>(orderDetails);
        }

        public async Task UpdateOrderDetailAsync(UpdateOrderDetailDTO orderDetailDto)
        {
            var orderDetail = mapper.Map<OrderDetail>(orderDetailDto);
            unitOfWork.GetRepository<OrderDetail>().Update(orderDetail);
            await unitOfWork.CompleteAsync();
        }

        public async Task DeleteOrderDetailAsync(Guid id)
        {
            await unitOfWork.GetRepository<OrderDetail>().DeleteAsync(id);
            await unitOfWork.CompleteAsync();
        }
    }