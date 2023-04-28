namespace BankAccount.Helper.Behavior;

public class PipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : new()
{
    private readonly ILogger<PipelineBehavior<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public PipelineBehavior(IEnumerable<IValidator<TRequest>> validators,
        ILogger<PipelineBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Pipeline behavior starts");

        var failures = _validators
            .Select(x => x.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(x => x != null)
            .ToList();

        if (failures.Any()) _logger.LogError("Object is not valid!");

        return failures.Any()
            ? throw new ValidationException(failures)
            : next();
    }
}