using MongoDB.Bson;
using MongoDB.Driver;
using ProductService.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ProductService.Services;

public class ProductManagementService
{
    private readonly IMongoCollection<Product> _products;
    private readonly IMongoCollection<BsonDocument> _productsRaw;
    private readonly IMongoDatabase _database;

    public ProductManagementService(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDbSettings:ConnectionString"];
        var databaseName = configuration["MongoDbSettings:DatabaseName"];
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
        _products = _database.GetCollection<Product>("products");
        _productsRaw = _database.GetCollection<BsonDocument>("products");
    }

    public async Task<List<Product>> GetProductsAsync(string? category)
    {
        if (string.IsNullOrEmpty(category))
            return await _products.Find(_ => true).ToListAsync();

        var filter = Builders<Product>.Filter.Eq(p => p.Category, category);
        return await _products.Find(filter).ToListAsync();
    }

    public async Task<string> SearchProductsAsync(string query)
    {
        var jsQuery = $"this.Name.match('{query}') || this.Description.match('{query}')";

        var filter = new BsonDocument
        {
            { "$where", jsQuery }
        };

        var documents = await _productsRaw.Find(filter).ToListAsync();
        var settings = new MongoDB.Bson.IO.JsonWriterSettings { OutputMode = MongoDB.Bson.IO.JsonOutputMode.CanonicalExtendedJson };
        return documents.ToJson(settings);
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        await _products.InsertOneAsync(product);
        return product;
    }

    public async Task<bool> UpdateProductAsync(string id, Product product)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        var update = Builders<Product>.Update
            .Set(p => p.Name, product.Name)
            .Set(p => p.Description, product.Description)
            .Set(p => p.Price, product.Price)
            .Set(p => p.Category, product.Category)
            .Set(p => p.Stock, product.Stock)
            .Set(p => p.ImageUrl, product.ImageUrl);

        var result = await _products.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteProductAsync(string id)
    {
        var result = await _products.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<string> ExportProductsAsync(string filename)
    {
        var connectionString = "mongodb://mongodb:27017";
        var command = $"mongoexport --uri={connectionString} --db=vuln_products --collection=products --out=/tmp/{filename}.json";

        var processInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(processInfo);
        if (process == null)
            return "Failed to start export process";

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return string.IsNullOrEmpty(error) ? $"Export completed: {output}" : $"Export output: {output}\nErrors: {error}";
    }

    public string ImportCsvProducts(string uploadedFilePath)
    {
        var command = $"cat {uploadedFilePath} | head -n 100";

        var processInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(processInfo);
        if (process == null)
            return "Failed to process CSV";

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output;
    }

    public async Task<string> GetProductStatsAsync(string groupByField)
    {
        var pipelineJson = $"[{{\"$group\": {{\"_id\": \"${groupByField}\", \"count\": {{\"$sum\": 1}}, \"avgPrice\": {{\"$avg\": \"$Price\"}}}}}}]";

        var pipeline = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(pipelineJson);
        var pipelineStages = pipeline.Select(stage => (BsonDocument)stage).ToArray();

        var pipelineDefinition = PipelineDefinition<BsonDocument, BsonDocument>.Create(pipelineStages);

        var results = await _productsRaw.Aggregate(pipelineDefinition).ToListAsync();
        return results.ToJson();
    }

    public async Task<object> GetDebugInfoAsync()
    {
        var products = await _products.Find(_ => true).ToListAsync();
        var stats = await _database.RunCommandAsync<BsonDocument>(new BsonDocument("dbStats", 1));
        
        // Convert MongoDB BsonDocument to standard JSON string to avoid System.Text.Json casting exceptions
        var statsJson = stats.ToJson();

        return new
        {
            TotalProducts = products.Count,
            Products = products,
            DatabaseStats = statsJson,
            Environment = new
            {
                MachineName = System.Environment.MachineName,
                OSVersion = System.Environment.OSVersion.ToString(),
                ProcessorCount = System.Environment.ProcessorCount,
                WorkingSet = System.Environment.WorkingSet,
                CurrentDirectory = System.Environment.CurrentDirectory
            }
        };
    }
}
