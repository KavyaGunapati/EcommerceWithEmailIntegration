using Models.DTOs;

namespace Interfaces.IManagers
{
    public interface IOrderManager
    {
        Task<Result> AddOrderAsync(OrderDto orderDto);
        Task<Result> DeleteOrderAsync(int id);
        Task<Result<List<OrderDto>>> GetAllOrdersAsync();
        Task<Result<OrderDto>> GetOrderById(int id);
        Task<Result> ConfirmPayment(string paymentIntentId, string paymentMethodId);
    }
}
