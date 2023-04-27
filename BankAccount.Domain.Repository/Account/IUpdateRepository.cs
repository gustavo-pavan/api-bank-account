namespace BankAccount.Domain.Repository.Account;

public interface IUpdateRepository<in TBaseEntity> : IDisposable where TBaseEntity : BaseEntity
{
    Task Execute(TBaseEntity entity);
}