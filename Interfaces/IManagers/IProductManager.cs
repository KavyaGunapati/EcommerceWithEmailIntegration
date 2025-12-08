using Models.DTOs;

namespace Interfaces.IManagers
{
    public interface IProductManager
    {
        Task<Result> AddProductAsync(ProductDto productDto);
        Task<Result> DeleteProductAsync(int productId);
        Task<Result> UpdateProductAsync(ProductDto productDto);
        Task<Result<List<ProductDto>>> GetAllProducts();
        Task<Result<ProductDto>> GetProductByIdAsync(int productId);
    }
}
