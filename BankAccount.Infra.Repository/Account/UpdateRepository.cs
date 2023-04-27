namespace BankAccount.Infra.Repository.Account;

public class UpdateRepository<TBaseEntity> : IUpdateRepository<TBaseEntity> where TBaseEntity : BaseEntity
{
    private readonly IMongoContext _mongoContext;

    public UpdateRepository(IMongoContext mongoContext)
    {
        _mongoContext = mongoContext;
        Collection = mongoContext.GetCollection<TBaseEntity>(typeof(TBaseEntity).Name);
    }

    public IMongoCollection<TBaseEntity> Collection { get; }

    public Task Execute(TBaseEntity entity)
    {
        if (Guid.Empty.Equals(entity.Id))
            throw new ArgumentException($"Can't update because {nameof(BaseEntity.Id)} is not valid!");

        _mongoContext.AddCommand(() =>
            Collection.ReplaceOneAsync(Builders<TBaseEntity>.Filter.Eq("_id", entity.Id), entity));
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _mongoContext?.Dispose();
    }
}