using OrderSolution.API.Enums;

namespace OrderSolution.API.Data
{
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string CustomerId { get; set; }
        public required OrderStatus OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public required string ShippingAddress { get; set; }
        public required PaymentMethod PaymentMethod { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
    }
}
