namespace BankAccount.Domain.Repository.Account;

public interface IDeleteRepository<in TBaseEntity> : IDisposable where TBaseEntity : BaseEntity
{
    Task Execute(Guid id);
}