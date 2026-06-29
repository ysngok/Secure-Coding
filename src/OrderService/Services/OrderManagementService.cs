using MongoDB.Bson;
using MongoDB.Driver;
using OrderService.Models;

namespace OrderService.Services;

public class OrderManagementService
{
    private readonly IMongoCollection<Order> _orders;
    private readonly IMongoCollection<BsonDocument> _ordersRaw;

    public OrderManagementService(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDbSettings:ConnectionString"];
        var databaseName = configuration["MongoDbSettings:DatabaseName"];
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _orders = database.GetCollection<Order>("orders");
        _ordersRaw = database.GetCollection<BsonDocument>("orders");
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        if (order.Discount > 0)
        {
            order.TotalPrice = order.TotalPrice * (1 - order.Discount / 100);
        }

        await _orders.InsertOneAsync(order);
        return order;
    }

    public async Task<string> GetOrdersAsync(string? filterJson)
    {
        BsonDocument filter;

        if (string.IsNullOrEmpty(filterJson))
        {
            filter = new BsonDocument();
        }
        else
        {
            filter = BsonDocument.Parse(filterJson);
        }

        var results = await _ordersRaw.Find(filter).ToListAsync();
        var settings = new MongoDB.Bson.IO.JsonWriterSettings { OutputMode = MongoDB.Bson.IO.JsonOutputMode.CanonicalExtendedJson };
        return results.ToJson(settings);
    }

    public async Task<Order?> GetOrderByIdAsync(string id)
    {
        return await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();
    }
}
