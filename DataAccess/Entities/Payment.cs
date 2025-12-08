namespace DataAccess.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string IntentPaymentMethodId { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
