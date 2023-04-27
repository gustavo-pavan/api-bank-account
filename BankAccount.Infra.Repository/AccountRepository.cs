using BankAccount.Domain.Entity;
using BankAccount.Domain.Repository;
using BankAccount.Infra.Context;

namespace BankAccount.Infra.Repository;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository(IMongoContext mongoContext) : base(mongoContext)
    {
    }
}