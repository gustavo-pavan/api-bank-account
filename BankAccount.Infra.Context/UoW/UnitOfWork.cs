using Microsoft.Extensions.Logging;

namespace BankAccount.Infra.Context.UoW;

public class UnitOfWork : IDisposable, IUnitOfWork
{
    private readonly IMongoContext _mongoContext;
    private IClientSessionHandle? _session;
    private readonly ILogger<UnitOfWork> _logger;
    private bool Disposed { get; set; }
    public bool HasActiveTransaction => _session != null;

    public UnitOfWork(IMongoContext mongoContext, ILogger<UnitOfWork> logger)
    {
        _mongoContext = mongoContext;
        _logger = logger;
    }

    public async Task BeginTransactionAsync()
    {
        try
        {
            _logger.LogInformation("Start transaction");
            if (_session is not null) return;

            _session = await _mongoContext.SessionHandle;
            _session.StartTransaction();
            _logger.LogInformation("Finish transaction");
        }
        catch (Exception e)
        {
            _logger.LogError($"Error open transaction: {e.Message}");
            throw;
        }
    }


    public async Task RollbackAsync()
    {
        try
        {
            if (_session is not null)
            {
                _logger.LogInformation("Rollback transaction");
                await _session.AbortTransactionAsync();
            }
        }
        finally
        {
            if (_session is not null)
            {
                _session.Dispose();
                _session = null;
            }
            _logger.LogInformation("Finish rollback");
        }
    }

    public async Task CommitAsync()
    {
        try
        {
            _logger.LogInformation("Start commit");
            await _mongoContext.SaveChanges();
            _logger.LogInformation("Finish commit");

        }
        catch
        {
            _logger.LogInformation("Rollback transaction");
            await RollbackAsync();
        }
        finally
        {
            _logger.LogInformation("Dispose transaction");
            _session?.Dispose();
            _mongoContext?.Dispose();
        }
    }

    public virtual void Dispose(bool disposing)
    {
        _logger.LogInformation("Dispose Unit of Work virtual");
        if (!Disposed)
            if (disposing)
                _mongoContext.Dispose();

        Disposed = true;
    }

    public void Dispose()
    {
        _logger.LogInformation("Dispose unit of work");
        Dispose(true);
        GC.SuppressFinalize(this);
    }

}