using AutoMapper;
using DataAccess.Entities;
using Interfaces.IManagers;
using Interfaces.IRepository;
using Models.DTOs;

namespace Managers
{
    public class ProductManager : IProductManager
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        public ProductManager(IGenericRepository<Product> productRepository,IMapper mapper)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }
        public async Task<Result> AddProductAsync(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _productRepository.AddAsync(product);
            return new Result { Success = true, Message = "Product added successfully." };
        }

        public async Task<Result> DeleteProductAsync(int productId)
        {
            var product =await  _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return new Result { Success = false, Message = "Product not found." };
            }
            await _productRepository.DeleteAsync(product);
            return new Result { Success = true, Message = "Product deleted successfully." };
        }

        public async Task<Result<List<ProductDto>>> GetAllProducts()
        {
            var products = await _productRepository.GetAllAsync();
            var productDtos = _mapper.Map<List<ProductDto>>(products);
            return new Result<List<ProductDto>> { Success = true, Data = productDtos };
        }

        public async Task<Result<ProductDto>> GetProductByIdAsync(int productId)
        {
           var product = await  _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return new Result<ProductDto> { Success = false, Message = "Product not found." };
            }
            var productDto = _mapper.Map<ProductDto>(product);
            return new Result<ProductDto> { Success = true, Data = productDto };
        }

        public async Task<Result> UpdateProductAsync(ProductDto productDto)
        {
                var product=await _productRepository.GetByIdAsync(productDto.Id);
                if (product == null)
                {
                    return new Result { Success = false, Message = "Product not found." };
            }
                _mapper.Map(productDto, product);
                await _productRepository.UpdateAsync(product);
                return new Result { Success = true, Message = "Product updated successfully." };
        }
    }
}
