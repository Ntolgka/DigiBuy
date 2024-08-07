using AutoMapper;
using DigiBuy.Application.Dtos.CategoryDTOs;
using DigiBuy.Application.Dtos.CouponDTOs;
using DigiBuy.Application.Dtos.OrderDetailDTOs;
using DigiBuy.Application.Dtos.OrderDTOs;
using DigiBuy.Application.Dtos.ProductCategoryDTOs;
using DigiBuy.Application.Dtos.ProductDTOs;
using DigiBuy.Application.Dtos.UserDTOs;
using DigiBuy.Domain.Entities;

namespace DigiBuy.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
        {
            // Category
            CreateMap<Category, ReadCategoryDTO>().ReverseMap();
            
            CreateMap<CreateCategoryDTO, Category>().ReverseMap();
            
            CreateMap<UpdateCategoryDTO, Category>()
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ReverseMap();

            // Coupon
            CreateMap<Coupon, ReadCouponDTO>().ReverseMap();
            
            CreateMap<CreateCouponDTO, Coupon>().ReverseMap();
            
            CreateMap<UpdateCouponDTO, Coupon>()
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ReverseMap();

            // Order
            CreateMap<Order, ReadOrderDTO>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                .ReverseMap(); 

            CreateMap<CreateOrderDTO, Order>()
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore());

            // OrderDetail
            CreateMap<OrderDetail, ReadOrderDetailDTO>().ReverseMap();

            CreateMap<CreateOrderDetailDTO, OrderDetail>().ReverseMap();
            
            CreateMap<OrderDetailDTO, OrderDetail>();

            // Product
            CreateMap<Product, ReadProductDTO>().ReverseMap();

            CreateMap<CreateProductDTO, Product>().ReverseMap();
            
            CreateMap<UpdateProductDTO, Product>()
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ReverseMap();

            // ProductCategory
            CreateMap<ProductCategory, ReadProductCategoryDTO>().ReverseMap();

            CreateMap<CreateProductCategoryDTO, ProductCategory>().ReverseMap();

            // User
            CreateMap<User, ReadUserDTO>().ReverseMap();

            CreateMap<CreateUserDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.WalletBalance, opt => opt.Ignore())
                .ForMember(dest => dest.PointsBalance, opt => opt.Ignore()) 
                .ForMember(dest => dest.Status, opt => opt.Ignore()) 
                .ReverseMap();

            CreateMap<UpdateUserDTO, User>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.WalletBalance, opt => opt.Ignore()) 
                .ForMember(dest => dest.PointsBalance, opt => opt.Ignore()) 
                .ForMember(dest => dest.Status, opt => opt.Ignore()) 
                .ReverseMap();
        }
}