namespace BankAccount.Helper.Behavior;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(ILogger<TransactionBehavior<TRequest, TResponse>> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var typeName = request.GetGenericTypeName();

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            _logger.LogInformation("Begin command ({@Command})", typeName);

            var response = await next();

            _logger.LogInformation("Commit command ({@Command})", typeName);

            await _unitOfWork.CommitAsync();

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError("Exception command for ({@Command}), Exception : {exception}", typeName, e.Message);
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}