using AutoMapper;
using DataAccess.Entities;
using Interfaces.IManagers;
using Interfaces.IRepository;
using Models.DTOs;

namespace Managers
{
    public class UserManager : IUserManager
    {
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IMapper _mapper;
        public UserManager(IGenericRepository<Customer> customerRepository,IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }
        public async Task<Result> AddCustomerAsync(CustomerDto user)
        {
            var customer=_mapper.Map<Customer>(user);
           await _customerRepository.AddAsync(customer);
            return new Result { Success = true, Message = "Customer added successfully" };
        }

        public async Task<Result> DeleteUserAsync(int id)
        {
            var product=await _customerRepository.GetByIdAsync(id);
            if(product==null)
            {
                return new Result { Success = false, Message = "Customer not found" };
            }
            await _customerRepository.DeleteAsync(product);
            return new Result { Success = true, Message = "Customer deleted successfully" };
        }

        public async Task<Result<List<CustomerDto>>> GetAllCustomersAsync()
        {
            var customers=await _customerRepository.GetAllAsync();
            var customerDtos=_mapper.Map<List<CustomerDto>>(customers);
            return new Result<List<CustomerDto>> { Success = true, Data = customerDtos };
        }

        public async Task<Result<CustomerDto>> GetCustomerByIdAsync(int id)
        {
            var customer=await _customerRepository.GetByIdAsync(id);
            if(customer==null)
            {
                return new Result<CustomerDto> { Success = false, Message = "Customer not found" };
            }
            var customerDto=_mapper.Map<CustomerDto>(customer);
            return new Result<CustomerDto> { Success = true, Data = customerDto };
        }

        public async Task<Result> UpdateCustomerAsync( CustomerDto user)
        {
            var customer=await _customerRepository.GetByIdAsync(user.Id);
            if(customer==null)
            {
                return new Result { Success = false, Message = "Customer not found" };
            }
            _mapper.Map(user, customer);
            await _customerRepository.UpdateAsync(customer);
            return new Result { Success = true, Message = "Customer updated successfully" };
        }
    }
}
