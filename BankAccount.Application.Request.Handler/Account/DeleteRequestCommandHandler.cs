namespace BankAccount.Application.Request.Handler.Account;

public class DeleteRequestCommandHandler : IRequestHandler<DeleteRequestCommand, bool>
{
    private readonly IDeleteRepository<AccountEntity> _deleteRepository;
    private readonly ILogger<DeleteRequestCommandHandler> _logger;

    public DeleteRequestCommandHandler(IDeleteRepository<AccountEntity> deleteRepository,
        ILogger<DeleteRequestCommandHandler> logger)
    {
        _deleteRepository = deleteRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Start handler to delete account bank");
            _logger.LogInformation("Execute transaction with database");
            await _deleteRepository.Execute(request.Id);

            _logger.LogInformation("Delete account with success");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error: {e.Message}");
            throw;
        }
    }
}