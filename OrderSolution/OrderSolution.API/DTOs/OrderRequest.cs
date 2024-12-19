using OrderSolution.API.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderSolution.API.DTOs
{
    public class CreateOrderRequest
    {
        public required string UserId { get; set; }
        public required string ShippingAddress { get; set; }
        [EnumDataType(typeof(PaymentMethod))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required PaymentMethod PaymentMethod { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; } = [];
    }

    public class CreateOrderItemRequest
    {
        public required string ProductId { get; set; }
        public required int Quantity { get; set; }
        public required decimal UnitPrice { get; set; }
    }
}
