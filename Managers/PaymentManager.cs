using DataAccess.Entities;
using Interfaces.IManagers;
using Interfaces.IRepository;
using Models.DTOs;
using Stripe;
namespace Managers
{
    public class PaymentManager : IPaymentManager
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<Payment> _paymentRepository;
        public PaymentManager(IGenericRepository<Order> orderRepository, IGenericRepository<Payment> paymentRepository)
        {
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
        }
        public async Task<Result> ConfirmPayment(string paymentIntentId, string PaymentMethodId)
        {
            var confirmOptions = new PaymentIntentConfirmOptions
            {
                PaymentMethod = PaymentMethodId
            };
            var service = new PaymentIntentService();
            var paymentIntent = service.Confirm(paymentIntentId, confirmOptions);
            var payment=(await _paymentRepository.GetAllAsync()).FirstOrDefault(p => p.IntentPaymentMethodId == paymentIntentId);
            if (payment == null)
            {
                return new Result
                {
                    Success = false,
                    Message = "Payment not found"
                };
            }
            payment.Status = paymentIntent.Status;
            await _paymentRepository.UpdateAsync(payment);
            var order = await _orderRepository.GetByIdAsync(payment.OrderId);
            if (paymentIntent.Status == "succeeded" && order != null)
            {
                order.Status = "Paid";
                await _orderRepository.UpdateAsync(order);
            }
            return new Result
            {
                Success = true,
                Message = "Payment confirmed",
                Data = paymentIntent
            };
        }

        public async Task<(string PaymentIntentId, string ClientSecret)> CreatePaymentIntentAsync(int orderId, decimal amount)
        {
            var options = new PaymentIntentCreateOptions
            {
                Currency = "usd",
                Amount = (long)(amount * 100),
                PaymentMethodTypes = new List<string> { "card" },
                Metadata = new Dictionary<string, string> { { "orderId", orderId.ToString() } }
            };
            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options);
            return (paymentIntent.Id, paymentIntent.ClientSecret);
        }

        public Task HandleWebhookAsync(string json, string signatureHeader)
        {
            throw new NotImplementedException();
        }
    }
     
}
