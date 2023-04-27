namespace BankAccount.Domain.Repository.Account;

public interface IGetByIdRepository<TBaseEntity> : IDisposable where TBaseEntity : BaseEntity
{
    Task<TBaseEntity?> Execute(Guid id);
}