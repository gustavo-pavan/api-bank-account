using System.Reflection;
using BankAccount.Domain.Entity;
using BankAccount.Domain.Repository;
using BankAccount.Infra.Context;
using MongoDB.Driver;

namespace BankAccount.Infra.Repository;

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
            throw new ArgumentException($"Can't create because {nameof(BaseEntity.Id)} is not valid!");

        var type = entity.GetType();
        var props = type.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        props?.SetValue(entity, Guid.NewGuid(), null);

        MongoContext.AddCommand(() => Collection.InsertOneAsync(entity));
        return Task.CompletedTask;
    }

    public Task Update(TBaseEntity entity)
    {
        if (Guid.Empty.Equals(entity.Id))
            throw new ArgumentException($"Can't update because {nameof(BaseEntity.Id)} is not valid!");

        MongoContext.AddCommand(() =>
            Collection.ReplaceOneAsync(Builders<TBaseEntity>.Filter.Eq("_id", entity.Id), entity));
        return Task.CompletedTask;
    }

    public Task Delete(Guid id)
    {
        if (Guid.Empty.Equals(id))
            throw new OperationCanceledException($"Can't delete because {nameof(BaseEntity.Id)} is not valid!");

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