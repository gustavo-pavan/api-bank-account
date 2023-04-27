namespace BankAccount.Infra.Repository;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository(IMongoContext mongoContext) : base(mongoContext)
    {
    }
}