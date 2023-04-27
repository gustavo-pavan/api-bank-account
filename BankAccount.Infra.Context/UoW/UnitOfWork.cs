namespace BankAccount.Infra.Context.UoW;

public class UnitOfWork : IDisposable, IUnitOfWork
{
    private readonly IMongoContext _mongoContext;

    public UnitOfWork(IMongoContext mongoContext)
    {
        _mongoContext = mongoContext;
    }

    public async Task<bool> Commit()
    {
        var sessionHandler = await _mongoContext.SessionHandle;

        try
        {
            sessionHandler.StartTransaction();

            var result = await _mongoContext.SaveChanges();

            await sessionHandler.CommitTransactionAsync();

            return result > default(int);
        }
        catch (Exception)
        {
            await sessionHandler.AbortTransactionAsync();

            throw;
        }
        finally
        {
            sessionHandler.Dispose();
        }
    }

    public void Dispose()
    {
        _mongoContext.Dispose();
    }
}