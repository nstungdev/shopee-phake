using MassTransit;
using SharedSolution.Contracts.Orders;

namespace NotificationSolution.API.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        public Task Consume(ConsumeContext<OrderCreated> context)
        {
            Console.WriteLine($"Order created: {context.Message.ToString()}");
            return Task.CompletedTask;
        }
    }
}
