namespace SharedSolution.Contracts.Orders
{
    public record OrderCreated
    {
        public required string OrderId { get; set; }
        public required string CustomerId { get; set; }
        public IEnumerable<OrderDetail> Details { get; set; } = [];
    }

    public record OrderDetail
    {
        public required string ProductId { get; set; }
        public required int Quantity { get; set; }
    }
}
