namespace OrderSolution.API.Data
{
    public class OrderItem
    {
        public int Id { get; set; }
        public required string OrderId { get; set; }
        public required string ProductId { get; set; }
        public int Quanity { get; set; }
        public decimal UnitPrice { get; set; }
        public Order Order { get; set; } = null!;
    }
}
