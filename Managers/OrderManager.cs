using DataAccess.Entities;
using Interfaces.IManagers;
using Interfaces.IRepository;
using Models.DTOs;

namespace Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<OrderItem> _orderItemRepository;
        private readonly IPaymentManager _paymentManager;
        private readonly IEmailService _emailManager;
        public OrderManager(IGenericRepository<Order> orderRepository,IGenericRepository<Product> productRepository,
            IGenericRepository<Customer> customerRepository,IGenericRepository<OrderItem> orderItemRepository,
            IPaymentManager paymentManager,IEmailService emailService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _orderItemRepository = orderItemRepository;
            _paymentManager = paymentManager;
            _emailManager = emailService;
        }
        public async Task<Result> AddOrderAsync(OrderDto orderDto)
        {
            var customer=await _customerRepository.GetByIdAsync(orderDto.CustomerId);
            if(customer==null)
            {
                return new Result { Success = false, Message = "Customer not found" };
            }
            var order = new Order
            {
                CustomerId = orderDto.CustomerId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = orderDto.TotalAmount,
                Status =string.IsNullOrEmpty(orderDto.Status)? "Pending":orderDto.Status,
                OrderItems = new List<OrderItem>()
            };
            decimal total = 0m;
            foreach (var itemDto in orderDto.OrderItems)
            {
                var product=await _productRepository.GetByIdAsync(itemDto.ProductId);
                if(product==null)
                {
                    return new Result { Success = false, Message = $"Product with ID {itemDto.ProductId} not found" };
                }
                if(itemDto.Quantity<=0)
                {
                    return new Result { Success = false, Message = $"Invalid quantity for product {product.Name}" };
                }
                if (product.Stock<itemDto.Quantity)
                {
                    return new Result { Success = false, Message = $"Insufficient stock for product {product.Name}" };
                }
               
                total += itemDto.Quantity * itemDto.UnitPrice;
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice
                };
                order.OrderItems.Add(orderItem);
                product.Stock -= itemDto.Quantity;
                await _productRepository.UpdateAsync(product);
            }
            order.TotalAmount=total;
            await _orderRepository.AddAsync(order);
            await _emailManager.SendEmailAsync(customer.Email, "Order Confirmation", $"Your order with ID {order.Id} has been placed successfully.");
            return new Result { Success = true, Message = "Order placed successfully", Data = order.Id };
        }

        public Task<Result> ConfirmPayment(string paymentIntentId, string paymentMethodId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteOrderAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<OrderDto>>> GetAllOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<OrderDto>> GetOrderById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
