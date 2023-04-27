namespace BankAccount.Domain.Repository.Account;

public interface ICreateRepository<in TBaseEntity> : IDisposable where TBaseEntity : BaseEntity
{
    Task Execute(TBaseEntity entity);
}