namespace BankAccount.Infra.Repository.Account;

public class GetByIdRepository<TBaseEntity> : IGetByIdRepository<TBaseEntity> where TBaseEntity : BaseEntity
{
    private readonly IMongoContext _mongoContext;

    public GetByIdRepository(IMongoContext mongoContext)
    {
        _mongoContext = mongoContext;
        Collection = mongoContext.GetCollection<TBaseEntity>(typeof(TBaseEntity).Name);
    }

    public IMongoCollection<TBaseEntity> Collection { get; }

    public async Task<TBaseEntity?> Execute(Guid id)
    {
        var data = await Collection.FindAsync(Builders<TBaseEntity>.Filter.Eq("_id", id));
        if (data == null)
            return null;

        return await data.SingleOrDefaultAsync();
    }

    public void Dispose()
    {
        _mongoContext?.Dispose();
    }
}