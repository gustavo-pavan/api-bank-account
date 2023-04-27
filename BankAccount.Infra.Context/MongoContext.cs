using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace BankAccount.Infra.Context;

public class MongoContext : IMongoContext
{
    private readonly List<Func<Task>> _commands;

    private readonly IConfiguration _configuration;

    private IMongoDatabase? _database;

    public MongoContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _commands = new List<Func<Task>>();
    }

    public IClientSessionHandle? SessionHandle { get; set; }
    public MongoClient? MongoClient { get; set; }

    public void Dispose()
    {
        SessionHandle?.Dispose();
        GC.SuppressFinalize(this);
    }

    public void AddCommand(Func<Task> func)
    {
        _commands.Add(func);
    }

    public async Task<int> SaveChanges()
    {
        ConfigureMongo();

        using (SessionHandle = await MongoClient?.StartSessionAsync()!)
        {
            SessionHandle.StartTransaction();

            var commandTask = _commands.Select(x => x());
            await Task.WhenAll(commandTask);

            await SessionHandle.CommitTransactionAsync();

            return _commands.Count();
        }

        return default;
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        ConfigureMongo();
        return _database?.GetCollection<T>(collectionName) ??
               throw new InvalidOperationException("Database is not configured");
    }

    private void ConfigureMongo()
    {
        if (MongoClient != null) return;

        MongoClient = new MongoClient(_configuration["MongoSettings:Connection"]);
        _database = MongoClient.GetDatabase(_configuration["MongoSettings:DatabaseName"]);
    }
}