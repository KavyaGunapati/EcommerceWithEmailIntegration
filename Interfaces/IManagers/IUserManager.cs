using Models.DTOs;

namespace Interfaces.IManagers
{
    public interface IUserManager
    {
        Task<Result> AddCustomerAsync(CustomerDto user);
        Task<Result<List<CustomerDto>>> GetAllCustomersAsync();
        Task<Result<CustomerDto>> GetCustomerByIdAsync(int id);
        Task<Result> UpdateCustomerAsync( CustomerDto user);
        Task<Result> DeleteUserAsync(int id);
    }
}
