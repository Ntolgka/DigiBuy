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
        product.InsertDate = DateTime.UtcNow;
        await unitOfWork.GetRepository<Product>().AddAsync(product);
        await unitOfWork.CompleteAsync();
        return productDto;
    }

    public async Task<ReadProductDTO> GetProductByIdAsync(Guid id)
    {
        var product = await unitOfWork.GetRepository<Product>().GetByIdAsync(id, nameof(Product.ProductCategories), nameof(Product.ProductCategories) + "." + nameof(ProductCategory.Category));
        return mapper.Map<ReadProductDTO>(product);
    }

    public async Task<IEnumerable<ReadProductDTO>> GetAllProductsAsync()
    {
        var products = await unitOfWork.GetRepository<Product>().GetAllAsync(nameof(Product.ProductCategories), nameof(Product.ProductCategories) + "." + nameof(ProductCategory.Category));
        return mapper.Map<IEnumerable<ReadProductDTO>>(products);
    }

    public async Task<IEnumerable<ReadProductDTO>> GetProductsByCategoryAsync(Guid categoryId)
    {
        var products = await unitOfWork.GetRepository<Product>()
            .QueryAsync(p => p.ProductCategories.Any(c => c.CategoryId == categoryId), nameof(Product.ProductCategories), nameof(Product.ProductCategories) + "." + nameof(ProductCategory.Category));
        return mapper.Map<IEnumerable<ReadProductDTO>>(products);
    }

    public async Task UpdateProductAsync(Guid id, UpdateProductDTO productDto)
    {
        var product = await unitOfWork.GetRepository<Product>().GetByIdAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        mapper.Map(productDto, product);
        product.UpdateDate = DateTime.UtcNow;
        unitOfWork.GetRepository<Product>().Update(product);
        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteProductAsync(Guid id)
    {
        await unitOfWork.GetRepository<Product>().DeleteAsync(id);
        await unitOfWork.CompleteAsync();
    }

    public async Task AddCategoryToProductAsync(Guid productId, Guid categoryId)
    {
        var product = await unitOfWork.GetRepository<Product>().GetByIdAsync(productId);
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(categoryId);
        if (category == null)
        {
            throw new KeyNotFoundException("Category not found");
        }

        var productCategory = new ProductCategory
        {
            ProductId = productId,
            CategoryId = categoryId
        };

        product.UpdateDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);;
        
        await unitOfWork.GetRepository<ProductCategory>().AddAsync(productCategory);
        await unitOfWork.CompleteAsync();
    }

    public async Task RemoveCategoryFromProductAsync(Guid productId, Guid categoryId)
    {
        var productCategory = await unitOfWork.GetRepository<ProductCategory>().FirstOrDefaultAsync(pc => pc.ProductId == productId && pc.CategoryId == categoryId);
        if (productCategory == null)
        {
            throw new KeyNotFoundException("ProductCategory relationship not found");
        }

        productCategory.UpdateDate = DateTime.UtcNow;
        
        await unitOfWork.GetRepository<ProductCategory>().DeleteAsync(productCategory.Id);
        await unitOfWork.CompleteAsync();
    }
}