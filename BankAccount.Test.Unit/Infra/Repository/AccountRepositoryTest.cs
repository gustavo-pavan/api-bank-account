using System.Reflection;
using BankAccount.Domain.Entity;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace BankAccount.Test.Unit.Infra.Repository;

public class AccountRepositoryTest
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task Should_Create_Data()
    {
        Account account = new(_faker.Name.FullName(), _faker.Random.Decimal(0, 100), _faker.Random.AlphaNumeric(400));

        var cursorMock = new Mock<IAsyncCursor<Account>>();
        var mongoContextMock = new Mock<IMongoContext>();
        var mongoCollectionMock = new Mock<IMongoCollection<Account>>();

        cursorMock.Setup(_ => _.Current).Returns(new List<Account> { account });

        cursorMock
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        cursorMock
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true))
            .Returns(Task.FromResult(false));

        mongoCollectionMock.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Account>>(),
                It.IsAny<FindOptions<Account, Account>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        mongoContextMock.Setup(x => x.AddCommand(It.IsAny<Func<Task>>()));

        mongoContextMock.Setup(x => x.GetCollection<Account>(It.IsAny<string>()))
            .Returns(mongoCollectionMock.Object);

        AccountRepository repository = new(mongoContextMock.Object);

        await repository.Create(account);
        account.Id.Should().Be(account.Id);
    }
}

public interface IAccountRepository : IRepository<Account>
{
}

public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository(IMongoContext mongoContext) : base(mongoContext)
    {
    }
}

public interface IRepository<T> : IDisposable where T : BaseEntity
{
    Task Create(T entity);
    Task Update(T entity);
    Task Delete(Guid id);
    Task<IEnumerable<T>> Get();
    Task<T?> GetById(Guid id);
}

public abstract class Repository<TBaseEntity> : IRepository<TBaseEntity> where TBaseEntity : BaseEntity
{
    protected readonly IMongoContext MongoContext;

    protected Repository(IMongoContext mongoContext)
    {
        MongoContext = mongoContext;
        Collection = mongoContext.GetCollection<TBaseEntity>(typeof(TBaseEntity).Name);
    }

    protected IMongoCollection<TBaseEntity> Collection { get; }

    public Task Create(TBaseEntity entity)
    {
        if (!Guid.Empty.Equals(entity.Id))
            throw new OperationCanceledException($"Can't create because {nameof(entity.Id)} is not valid!");

        var type = entity.GetType();
        var props = type.GetProperty("Id",BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        props?.SetValue(entity, Guid.NewGuid(), null);

        MongoContext.AddCommand(() => Collection.InsertOneAsync(entity));
        return Task.CompletedTask;
    }

    public Task Update(TBaseEntity entity)
    {
        if (Guid.Empty.Equals(entity.Id))
            throw new OperationCanceledException($"Can't update because {nameof(entity.Id)} is not valid!");

        MongoContext.AddCommand(() =>
            Collection.ReplaceOneAsync(Builders<TBaseEntity>.Filter.Eq("_id", entity.Id), entity));
        return Task.CompletedTask;
    }

    public Task Delete(Guid id)
    {
        if (Guid.Empty.Equals(id))
            throw new OperationCanceledException($"Can't delete because {nameof(id)} is not valid!");

        MongoContext.AddCommand(() => Collection.DeleteOneAsync(Builders<TBaseEntity>.Filter.Eq("_id", id)));
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<TBaseEntity>> Get()
    {
        var result = await Collection.FindAsync(Builders<TBaseEntity>.Filter.Empty);
        return await result.ToListAsync();
    }

    public async Task<TBaseEntity?> GetById(Guid id)
    {
        var data = await Collection.FindAsync(Builders<TBaseEntity>.Filter.Eq("_id", id));
        if (data == null)
            return null;

        return await data.SingleOrDefaultAsync();
    }

    public void Dispose()
    {
        MongoContext?.Dispose();
    }
}

public interface IMongoContext : IDisposable
{
    void AddCommand(Func<Task> func);
    Task<int> SaveChanges();
    IMongoCollection<T> GetCollection<T>(string collectionName);
}

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