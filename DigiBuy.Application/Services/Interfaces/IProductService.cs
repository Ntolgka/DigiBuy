using DigiBuy.Application.Dtos.ProductCategoryDTOs;
using DigiBuy.Application.Dtos.ProductDTOs;

namespace DigiBuy.Application.Services.Interfaces;

public interface IProductService
{
    Task<CreateProductDTO> CreateProductAsync(CreateProductDTO productDto);
    Task<ReadProductDTO> GetProductByIdAsync(Guid id);
    Task<IEnumerable<ReadProductDTO>> GetAllProductsAsync();
    Task<IEnumerable<ReadProductDTO>> GetProductsByCategoryAsync(Guid categoryId);
    Task<IEnumerable<ReadProductDTO>> GetProductsByStatusAsync(bool status);
    Task UpdateProductAsync(Guid id, UpdateProductDTO productDto);
    Task UpdateProductStockAsync(Guid productId, int newStock);
    Task DeleteProductAsync(Guid id);
    Task<IEnumerable<ReadProductCategoryDTO>> GetProductCategoriesAsync(Guid productId);
    Task AddCategoryToProductAsync(Guid productId, Guid categoryId);
    Task RemoveCategoryFromProductAsync(Guid productId, Guid categoryId);
}