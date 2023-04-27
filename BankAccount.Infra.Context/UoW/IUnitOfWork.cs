namespace BankAccount.Infra.Context.UoW;

public interface IUnitOfWork
{
    Task<bool> Commit();
}