namespace Models.DTOs
{
    public class PaymentDto
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string IntentPaymentMethodId { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
