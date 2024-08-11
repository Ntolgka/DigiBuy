using System.Text.Json;
using AutoMapper;
using DigiBuy.Application.Dtos.CategoryDTOs;
using DigiBuy.Application.Dtos.ProductCategoryDTOs;
using DigiBuy.Application.Dtos.ProductDTOs;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace DigiBuy.Application.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly IDistributedCache cache;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.cache = cache;
    }

    public async Task<CreateProductDTO> CreateProductAsync(CreateProductDTO productDto)
    {
        var product = mapper.Map<Product>(productDto);
        product.InsertDate = DateTime.UtcNow;
        product.IsActive = true;
        await unitOfWork.GetRepository<Product>().AddAsync(product);
        await unitOfWork.CompleteAsync();
        
        await cache.RemoveAsync($"ProductsByStatus_{product.IsActive}");
        await cache.RemoveAsync("AllProducts");
        await cache.RemoveAsync($"ProductsByCategory_{product.ProductCategories.FirstOrDefault()?.CategoryId}");
        
        return productDto;
    }

    public async Task<ReadProductDTO> GetProductByIdAsync(Guid id)
    {
        var cacheKey = $"Product_{id}";
        var cachedProduct = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedProduct))
        {
            return JsonSerializer.Deserialize<ReadProductDTO>(cachedProduct);
        }
        
        var product = await unitOfWork.GetRepository<Product>().GetByIdAsync(id, nameof(Product.ProductCategories), nameof(Product.ProductCategories) + "." + nameof(ProductCategory.Category));
        
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }
        
        var productDto = mapper.Map<ReadProductDTO>(product);
        var serializedProduct = JsonSerializer.Serialize(productDto);

        await cache.SetStringAsync(cacheKey, serializedProduct, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return productDto;
    }

    public async Task<IEnumerable<ReadProductDTO>> GetAllProductsAsync()
    {
        var cacheKey = "AllProducts";
        var cachedProducts = await cache.GetStringAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cachedProducts))
        {
            return JsonSerializer.Deserialize<IEnumerable<ReadProductDTO>>(cachedProducts);
        }
        
        var products = await unitOfWork.GetRepository<Product>().GetAllAsync(nameof(Product.ProductCategories), nameof(Product.ProductCategories) + "." + nameof(ProductCategory.Category));
        
        var productDtos = mapper.Map<IEnumerable<ReadProductDTO>>(products);

        var serializedProducts = JsonSerializer.Serialize(productDtos);
        await cache.SetStringAsync(cacheKey, serializedProducts, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return productDtos;
    }

    public async Task<IEnumerable<ReadProductDTO>> GetProductsByCategoryAsync(Guid categoryId)
    {
        var cacheKey = $"ProductsByCategory_{categoryId}";
        var cachedProducts = await cache.GetStringAsync(cacheKey);
    
        if (!string.IsNullOrEmpty(cachedProducts))
        {
            return JsonSerializer.Deserialize<IEnumerable<ReadProductDTO>>(cachedProducts);
        }
        
        var products = await unitOfWork.GetRepository<Product>()
            .QueryAsync(p => p.ProductCategories.Any(c => c.CategoryId == categoryId), nameof(Product.ProductCategories), nameof(Product.ProductCategories) + "." + nameof(ProductCategory.Category));
        
        var productDtos = mapper.Map<IEnumerable<ReadProductDTO>>(products);

        var serializedProducts = JsonSerializer.Serialize(productDtos);
        await cache.SetStringAsync(cacheKey, serializedProducts, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return productDtos;
    }
    
    public async Task<IEnumerable<ReadProductDTO>> GetProductsByStatusAsync(bool status)
    {
        var cacheKey = $"ProductsByStatus_{status}";
        var cachedProducts = await cache.GetStringAsync(cacheKey);
    
        if (!string.IsNullOrEmpty(cachedProducts))
        {
            return JsonSerializer.Deserialize<IEnumerable<ReadProductDTO>>(cachedProducts);
        }
        
        var products = await unitOfWork.GetRepository<Product>()
            .QueryAsync((c => c.IsActive == status), nameof(Product.ProductCategories), nameof(Product.ProductCategories) + "." + nameof(ProductCategory.Category));
        
        var productDtos = mapper.Map<IEnumerable<ReadProductDTO>>(products);

        var serializedProducts = JsonSerializer.Serialize(productDtos);
        await cache.SetStringAsync(cacheKey, serializedProducts, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return productDtos;
    }

    public async Task UpdateProductAsync(Guid id, UpdateProductDTO productDto)
    {
        var product = await unitOfWork.GetRepository<Product>().GetByIdAsync(id);
        var originalStatus = product.IsActive;
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        mapper.Map(productDto, product);
        product.UpdateDate = DateTime.UtcNow;
        unitOfWork.GetRepository<Product>().Update(product);
        await unitOfWork.CompleteAsync();
        
        await cache.RemoveAsync($"ProductsByStatus_{product.IsActive}");
        await cache.RemoveAsync($"ProductsByStatus_{originalStatus}");
        await cache.RemoveAsync($"Product_{id}");
        await cache.RemoveAsync("AllProducts");
        await cache.RemoveAsync($"ProductsByCategory_{product.ProductCategories.FirstOrDefault()?.CategoryId}");
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var product = await unitOfWork.GetRepository<Product>().GetByIdAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }
        
        await unitOfWork.GetRepository<Product>().DeleteAsync(id);
        await unitOfWork.CompleteAsync();
        
        await cache.RemoveAsync($"Product_{id}");
        await cache.RemoveAsync("AllProducts");
        await cache.RemoveAsync($"ProductsByCategory_{product.ProductCategories.FirstOrDefault()?.CategoryId}");
    }
    
    public async Task UpdateProductStockAsync(Guid productId, int newStock)
    {
        var productRepository = unitOfWork.GetRepository<Product>();
        var product = await productRepository.GetByIdAsync(productId);

        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        product.Stock = newStock;
        product.UpdateDate = DateTime.UtcNow;
    
        productRepository.Update(product);
        await unitOfWork.CompleteAsync();
        
        await cache.RemoveAsync($"Product_{productId}");
        await cache.RemoveAsync("AllProducts");
        await cache.RemoveAsync($"ProductsByCategory_{product.ProductCategories.FirstOrDefault()?.CategoryId}");
    }
    
    public async Task<IEnumerable<ReadProductCategoryDTO>> GetProductCategoriesAsync(Guid productId)
    {
        var cacheKey = $"ProductCategories_{productId}";
        var cachedCategories = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedCategories))
        {
            return JsonSerializer.Deserialize<IEnumerable<ReadProductCategoryDTO>>(cachedCategories);
        }

        var product = await unitOfWork.GetRepository<Product>()
            .GetByIdAsync(productId, nameof(Product.ProductCategories), nameof(Product.ProductCategories) + "." + nameof(ProductCategory.Category));
    
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        var categories = product.ProductCategories.Select(pc => pc.Category);
        var categoryDtos = mapper.Map<IEnumerable<ReadProductCategoryDTO>>(categories);

        var serializedCategories = JsonSerializer.Serialize(categoryDtos);
        await cache.SetStringAsync(cacheKey, serializedCategories, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return categoryDtos;
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
        
        await cache.RemoveAsync($"Product_{productId}");
        await cache.RemoveAsync("AllProducts");
        await cache.RemoveAsync($"ProductsByCategory_{categoryId}");
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
        
        await cache.RemoveAsync($"Product_{productId}");
        await cache.RemoveAsync("AllProducts");
        await cache.RemoveAsync($"ProductsByCategory_{categoryId}");
    }
}