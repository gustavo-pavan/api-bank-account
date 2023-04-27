namespace BankAccount.Infra.Context;

public interface IMongoContext : IDisposable
{
    public Task<IClientSessionHandle> SessionHandle { get; }
    void AddCommand(Func<Task> func);
    Task<int> SaveChanges();
    IMongoCollection<T> GetCollection<T>(string collectionName);
}