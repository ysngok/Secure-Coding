using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderService.Models;

public class OrderItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public List<OrderItem> Items { get; set; } = new();

    public decimal TotalPrice { get; set; }

    public decimal Discount { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? ShippingAddress { get; set; }
}
