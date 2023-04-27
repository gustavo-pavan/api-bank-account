namespace BankAccount.Domain.Repository;

public interface IRepository<T> : IDisposable where T : BaseEntity
{
    Task Create(T entity);
    Task Update(T entity);
    Task Delete(Guid id);
    Task<IEnumerable<T>> Get();
    Task<T?> GetById(Guid id);
}