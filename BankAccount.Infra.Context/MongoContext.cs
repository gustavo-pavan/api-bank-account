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

    private Task<IClientSessionHandle>? Session { get; set; }

    public MongoClient? MongoClient { get; set; }

    public Task<IClientSessionHandle> SessionHandle
    {
        get
        {
            if (Session != null) return Session;

            ConfigureMongo();

            if (MongoClient != null)
                Session = MongoClient.StartSessionAsync();

            return Session ?? throw new InvalidOperationException("Session can't be create");
        }
    }

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
        var commandTask = _commands.Select(x => x());
        await Task.WhenAll(commandTask);

        return _commands.Count();
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