﻿namespace BankAccount.Helper.Behavior;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling commnad {name} ({@Command})", request.GetGenericTypeName());
        var response = await next();
        _logger.LogInformation("Command {name} handled - response: {reponse}", request.GetGenericTypeName(), response);

        return response;
    }
}