using System.Reflection;

namespace BankAccount.Infra.Repository.Account;

public class CreateRepository<TBaseEntity> : ICreateRepository<TBaseEntity> where TBaseEntity : BaseEntity
{
    private readonly IMongoContext _mongoContext;

    public CreateRepository(IMongoContext mongoContext)
    {
        _mongoContext = mongoContext;
        Collection = mongoContext.GetCollection<TBaseEntity>(typeof(TBaseEntity).Name);
    }

    public IMongoCollection<TBaseEntity> Collection { get; }

    public Task Execute(TBaseEntity entity)
    {
        if (!Guid.Empty.Equals(entity.Id))
            throw new ArgumentException($"Can't create because {nameof(BaseEntity.Id)} is not valid!");

        var type = entity.GetType();
        var props = type.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        props?.SetValue(entity, Guid.NewGuid(), null);

        _mongoContext.AddCommand(() => Collection.InsertOneAsync(entity));
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _mongoContext?.Dispose();
    }
}