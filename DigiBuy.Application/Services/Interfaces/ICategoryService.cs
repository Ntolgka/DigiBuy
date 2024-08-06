using DigiBuy.Application.Dtos.CategoryDTOs;

namespace DigiBuy.Application.Services.Interfaces;

public interface ICategoryService
{
    Task<CreateCategoryDTO> CreateCategoryAsync(CreateCategoryDTO categoryDto);
    Task<ReadCategoryDTO> GetCategoryByIdAsync(Guid id);
    Task<IEnumerable<ReadCategoryDTO>> GetAllCategoriesAsync(); 
    Task UpdateCategoryAsync(Guid id, UpdateCategoryDTO categoryDto);   
    Task DeleteCategoryAsync(Guid id);  
}