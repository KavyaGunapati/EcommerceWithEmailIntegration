using Models.DTOs;

namespace Interfaces.IManagers
{
    public interface ICategoryManager
    {
        Task<Result> AddCategoryAsync(CategoryDto categoryDto);
        Task<Result> UpdateCategoryAsync(CategoryDto categoryDto);
        Task<Result> DeleteCategoryAsync(int id);
        Task<Result<List<CategoryDto>>> GetAllCategoriesAsync();
        Task<Result<CategoryDto>> GetCategoryByIdAsync(int id);
    }
}
