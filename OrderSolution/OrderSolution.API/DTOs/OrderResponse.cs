namespace OrderSolution.API.DTOs
{
    public class OrderResponse
    {
        public required string OrderId { get; set; }
        public required string CustomerId { get; set; }
        public required string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public required string ShippingAddress { get; set; }
        public required string PaymentMethod { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public decimal OrderTotalPrice { get; set; }
        public int OrderTotalAmount { get; set; }
        public IEnumerable<OrderItemResponse> Items { get; set; } = [];

    }

    public class OrderItemResponse
    {
        public required string ProductId { get; set; }
        public int Quantity {  get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
