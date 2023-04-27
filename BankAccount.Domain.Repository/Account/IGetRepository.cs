namespace BankAccount.Domain.Repository.Account;

public interface IGetRepository<TBaseEntity> : IDisposable where TBaseEntity : BaseEntity
{
    Task<IEnumerable<TBaseEntity>> Execute();
}