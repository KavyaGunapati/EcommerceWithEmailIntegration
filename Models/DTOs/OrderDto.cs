namespace Models.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; } 
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}
