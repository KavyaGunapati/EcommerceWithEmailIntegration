using Models.DTOs;

namespace Interfaces.IManagers
{
    public interface IPaymentManager
    {
        Task<(string PaymentIntentId, string ClientSecret)> CreatePaymentIntentAsync(int orderId, decimal amount);
        Task<Result> ConfirmPayment(string paymentIntentId, string PaymentMethodId);
        Task HandleWebhookAsync(string json, string signatureHeader);
    }
}
