using AutoMapper;
using DigiBuy.Application.Dtos.CategoryDTOs;
using DigiBuy.Application.Services.Interfaces;
using DigiBuy.Domain.Entities;
using DigiBuy.Domain.Repositories;

namespace DigiBuy.Application.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<CreateCategoryDTO> CreateCategoryAsync(CreateCategoryDTO categoryDto)
    {
        var category = mapper.Map<Category>(categoryDto);
        await unitOfWork.GetRepository<Category>().AddAsync(category);
        await unitOfWork.CompleteAsync();
        return categoryDto;
    }

    public async Task<ReadCategoryDTO> GetCategoryByIdAsync(Guid id)
    {
        var category = await unitOfWork.GetRepository<Category>().GetById(id);
        return mapper.Map<ReadCategoryDTO>(category);
    }

    public async Task<IEnumerable<ReadCategoryDTO>> GetAllCategoriesAsync()
    {
        var categories = await unitOfWork.GetRepository<Category>().GetAllAsync();
        return mapper.Map<IEnumerable<ReadCategoryDTO>>(categories);
    }

    public async Task UpdateCategoryAsync(UpdateCategoryDTO categoryDto)
    {
        var category = mapper.Map<Category>(categoryDto);
        unitOfWork.GetRepository<Category>().Update(category);
        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        await unitOfWork.GetRepository<Category>().DeleteAsync(id);
        await unitOfWork.CompleteAsync();
    }
}