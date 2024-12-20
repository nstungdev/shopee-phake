using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderSolution.API.Data;
using OrderSolution.API.DTOs;
using OrderSolution.API.Enums;
using SharedSolution.Contracts.Orders;

namespace OrderSolution.API.Services
{
    public interface IOrderService
    {
        Task CreateOrderAsync(CreateOrderRequest request);
        Task<OrderResponse?> GetOrderAsync(string id);
    }
    public class OrderService(OrderDbContext dbContext, IPublishEndpoint publishEndpoint) : IOrderService
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
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                });
                await dbContext.OrderItems.AddRangeAsync(items);

                await publishEndpoint.Publish(new OrderCreated
                {
                    CustomerId = order.CustomerId,
                    OrderId = order.Id,
                    Details = items.Select(i => new OrderDetail
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    })
                });
                await dbContext.SaveChangesAsync();
                await trans.CommitAsync();
            });
        }

        public async Task<OrderResponse?> GetOrderAsync(string id)
        {
            var order = await dbContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return null;

            return new OrderResponse
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                OrderStatus = order.OrderStatus.ToString(),
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                PaymentMethod = order.PaymentMethod.ToString(),
                CreateAt = order.CreateAt,
                UpdateAt = order.UpdateAt,
                OrderTotalPrice = order.OrderItems.Sum(i => i.Quantity * i.UnitPrice),
                OrderTotalAmount = order.OrderItems.Sum(i => i.Quantity),
                Items = order.OrderItems.Select(i => new OrderItemResponse
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.Quantity * i.UnitPrice
                })
            };
        }
    }
}
