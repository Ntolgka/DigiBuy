using AutoMapper;
using DigiBuy.Application.Dtos.ProductDTOs;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;

namespace DigiBuy.Application.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<CreateProductDTO> CreateProductAsync(CreateProductDTO productDto)
    {
        var product = mapper.Map<Product>(productDto);
        await unitOfWork.GetRepository<Product>().AddAsync(product);
        await unitOfWork.CompleteAsync();
        return productDto;
    }

    public async Task<ReadProductDTO> GetProductByIdAsync(Guid id)
    {
        var product = await unitOfWork.GetRepository<Product>().GetById(id);
        return mapper.Map<ReadProductDTO>(product);
    }

    public async Task<IEnumerable<ReadProductDTO>> GetAllProductsAsync()
    {
        var products = await unitOfWork.GetRepository<Product>().GetAllAsync();
        return mapper.Map<IEnumerable<ReadProductDTO>>(products);
    }

    public async Task<IEnumerable<ReadProductDTO>> GetProductsByCategoryAsync(Guid categoryId)
    {
        var products = await unitOfWork.GetRepository<Product>()
            .QueryAsync(p => p.ProductCategories.Any(c => c.CategoryId == categoryId));
        return mapper.Map<IEnumerable<ReadProductDTO>>(products);
    }

    public async Task UpdateProductAsync(UpdateProductDTO productDto)
    {
        var product = mapper.Map<Product>(productDto);
        unitOfWork.GetRepository<Product>().Update(product);
        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteProductAsync(Guid id)
    {
        await unitOfWork.GetRepository<Product>().DeleteAsync(id);
        await unitOfWork.CompleteAsync();
    }
}