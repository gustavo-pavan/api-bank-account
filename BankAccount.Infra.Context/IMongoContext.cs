using MongoDB.Driver;

namespace BankAccount.Infra.Context;

public interface IMongoContext : IDisposable
{
    void AddCommand(Func<Task> func);
    Task<int> SaveChanges();
    IMongoCollection<T> GetCollection<T>(string collectionName);
}