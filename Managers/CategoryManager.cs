using AutoMapper;
using DataAccess.Entities;
using Interfaces.IManagers;
using Interfaces.IRepository;
using Models.DTOs;
using SecureTaskAPI.Models.Constants;

namespace Managers
{
    public class CategoryManager : ICategoryManager
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryManager(IGenericRepository<Category> categoryRepository,IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<Result> AddCategoryAsync(CategoryDto categoryDto)
        {
           var category=_mapper.Map<Category>(categoryDto);
            await _categoryRepository.AddAsync(category);
            return new Result { Success = true, Message = ErrorConstants.Added };
        }

        public async Task<Result> DeleteCategoryAsync(int id)
        {
            var category=await _categoryRepository.GetByIdAsync(id);
            if(category==null)
            {
                return new Result { Success = false, Message ="Category Not found" };
            }
            await _categoryRepository.DeleteAsync(category);
            return new Result { Success = true, Message = ErrorConstants.Deleted };
        }

        public async Task<Result<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            var categories =await _categoryRepository.GetAllAsync();
            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return new Result<List<CategoryDto>> { Success = true, Data = categoryDtos };
        }

        public async Task<Result<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            var categoryTask =await _categoryRepository.GetByIdAsync(id);
            if(categoryTask==null)
            {
                return new Result<CategoryDto> { Success = false, Message = "Category Not found" };
            }
            var categoryDto = _mapper.Map<CategoryDto>(categoryTask);
            return new Result<CategoryDto> { Success = true, Data = categoryDto };
        }

        public async Task<Result> UpdateCategoryAsync(CategoryDto categoryDto)
        {
            var category=await _categoryRepository.GetByIdAsync(categoryDto.Id);
            if(category==null)
            {
                return new Result { Success = false, Message = "Category Not found" };
            }
            category.Name= !string.IsNullOrEmpty(categoryDto.Name)?categoryDto.Name:category.Name;
            await _categoryRepository.UpdateAsync(category);
            return new Result { Success = true, Message = ErrorConstants.Updated };
        }
    }
}
