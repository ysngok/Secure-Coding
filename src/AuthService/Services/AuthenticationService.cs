using AuthService.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace AuthService.Services;

public class AuthenticationService
{
    private readonly IMongoCollection<User> _users;
    private readonly IMongoCollection<BsonDocument> _usersRaw;

    public AuthenticationService(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDbSettings:ConnectionString"];
        var databaseName = configuration["MongoDbSettings:DatabaseName"];
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _users = database.GetCollection<User>("users");
        _usersRaw = database.GetCollection<BsonDocument>("users");
    }

    public async Task<User> RegisterAsync(User user)
    {
        await _users.InsertOneAsync(user);
        return user;
    }

    public async Task<(User? user, string? error)> LoginAsync(BsonDocument loginDoc)
    {
        var filter = new BsonDocument
        {
            { "Username", loginDoc.GetValue("username", BsonNull.Value) },
            { "Password", loginDoc.GetValue("password", BsonNull.Value) }
        };

        var user = await _usersRaw.Find(filter).FirstOrDefaultAsync();

        if (user == null)
        {
            var usernameFilter = new BsonDocument { { "Username", loginDoc.GetValue("username", BsonNull.Value) } };
            var existingUser = await _usersRaw.Find(usernameFilter).FirstOrDefaultAsync();

            if (existingUser == null)
            {
                return (null, "User not found");
            }
            return (null, "Invalid password");
        }

        var result = new User
        {
            Id = user["_id"].AsObjectId.ToString(),
            Username = user.GetValue("Username", "").AsString,
            Email = user.GetValue("Email", "").AsString,
            IsAdmin = user.GetValue("IsAdmin", false).AsBoolean
        };

        return (result, null);
    }

    public async Task<List<User>> SearchUsersAsync(string query)
    {
        var filter = Builders<User>.Filter.Regex(u => u.Username, new BsonRegularExpression(new Regex(query, RegexOptions.IgnoreCase)));
        return await _users.Find(filter).ToListAsync();
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _users.Find(_ => true).ToListAsync();
    }
}
