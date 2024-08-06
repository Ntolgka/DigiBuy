using DigiBuy.Application.Dtos.ProductDTOs;

namespace DigiBuy.Application.Services.Interfaces;

public interface IProductService
{
    Task<CreateProductDTO> CreateProductAsync(CreateProductDTO productDto);
    Task<ReadProductDTO> GetProductByIdAsync(Guid id);
    Task<IEnumerable<ReadProductDTO>> GetAllProductsAsync();
    Task<IEnumerable<ReadProductDTO>> GetProductsByCategoryAsync(Guid categoryId);
    Task UpdateProductAsync(UpdateProductDTO productDto);
    Task DeleteProductAsync(Guid id);
}