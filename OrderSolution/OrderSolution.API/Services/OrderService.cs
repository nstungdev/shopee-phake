using Microsoft.EntityFrameworkCore;
using OrderSolution.API.Data;
using OrderSolution.API.DTOs;
using OrderSolution.API.Enums;

namespace OrderSolution.API.Services
{
    public interface IOrderService
    {
        Task CreateOrderAsync(CreateOrderRequest request);
        Task<Order?> GetOrderAsync(string id);
    }
    public class OrderService(OrderDbContext dbContext) : IOrderService
    {
        public async Task CreateOrderAsync(CreateOrderRequest request)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var trans = await dbContext.Database.BeginTransactionAsync();
                Order order = new()
                {
                    CustomerId = request.UserId,
                    OrderStatus = OrderStatus.Pending,
                    PaymentMethod = request.PaymentMethod,
                    ShippingAddress = request.ShippingAddress,
                    OrderDate = DateTime.UtcNow
                };
                await dbContext.Orders.AddAsync(order);
                await dbContext.SaveChangesAsync();

                IEnumerable<OrderItem> items = request.Items.Select(i => new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = i.ProductId,
                    Quanity = i.Quantity,
                    UnitPrice = i.UnitPrice
                });
                await dbContext.OrderItems.AddRangeAsync(items);
                await dbContext.SaveChangesAsync();
                await trans.CommitAsync();
            });
        }

        public async Task<Order?> GetOrderAsync(string id)
        {
            return await dbContext.Orders.Include(e => e.OrderItems).FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
