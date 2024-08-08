using System.Text.Json;
using AutoMapper;
using DigiBuy.Application.Dtos.CategoryDTOs;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace DigiBuy.Application.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly IDistributedCache cache;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.cache = cache;
    }

    public async Task<CreateCategoryDTO> CreateCategoryAsync(CreateCategoryDTO categoryDto)
    {
        var category = mapper.Map<Category>(categoryDto);
        category.InsertDate = DateTime.UtcNow;
        await unitOfWork.GetRepository<Category>().AddAsync(category);
        await unitOfWork.CompleteAsync();
        
        await cache.RemoveAsync("AllCategories");
        
        return categoryDto;
    }

    public async Task<ReadCategoryDTO> GetCategoryByIdAsync(Guid id)
    {
        var cacheKey = $"Category_{id}";
        var cachedCategory = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedCategory))
        {
            return JsonSerializer.Deserialize<ReadCategoryDTO>(cachedCategory);
        }
        
        var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(id);
        var categoryDto = mapper.Map<ReadCategoryDTO>(category);

        if (categoryDto != null)
        {
            var serializedCategory = JsonSerializer.Serialize(categoryDto);
            await cache.SetStringAsync(cacheKey, serializedCategory);
        }

        return categoryDto;
    }

    public async Task<IEnumerable<ReadCategoryDTO>> GetAllCategoriesAsync()
    {
        var cacheKey = "AllCategories";
        var cachedCategories = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedCategories))
        {
            return JsonSerializer.Deserialize<IEnumerable<ReadCategoryDTO>>(cachedCategories);
        }
        
        var categories = await unitOfWork.GetRepository<Category>().GetAllAsync();
        var categoriesDto = mapper.Map<IEnumerable<ReadCategoryDTO>>(categories);

        if (categoriesDto != null && categoriesDto.Any())
        {
            var serializedCategories = JsonSerializer.Serialize(categoriesDto);
            await cache.SetStringAsync(cacheKey, serializedCategories);
        }

        return categoriesDto;
    }

    public async Task UpdateCategoryAsync(Guid id, UpdateCategoryDTO categoryDto)
    {
        var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(id);
        if (category == null)
        {
            throw new KeyNotFoundException("Category not found");
        }

        mapper.Map(categoryDto, category);
        category.UpdateDate = DateTime.UtcNow;
        unitOfWork.GetRepository<Category>().Update(category);
        await unitOfWork.CompleteAsync();
        
        await cache.RemoveAsync($"Category_{id}");
        await cache.RemoveAsync("AllCategories");
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var productsInCategory = await unitOfWork.GetRepository<ProductCategory>()
            .QueryAsync(pc => pc.CategoryId == id);
        if (productsInCategory.Any())
        {
            throw new InvalidOperationException("Cannot delete category with associated products");
        }

        await unitOfWork.GetRepository<Category>().DeleteAsync(id);
        await unitOfWork.CompleteAsync();
        
        await cache.RemoveAsync($"Category_{id}");
        await cache.RemoveAsync("AllCategories");
    }
}