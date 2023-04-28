namespace BankAccount.Application.Request.Handler.Account;

public class UpdateRequestCommandHandler : IRequestHandler<UpdateRequestCommand, AccountEntity>
{
    private readonly IUpdateRepository<AccountEntity> _updateRepository;
    private readonly ILogger<CreateRequestCommandHandler> _logger;

    public UpdateRequestCommandHandler(IUpdateRepository<AccountEntity> updateRepository,
        ILogger<CreateRequestCommandHandler> logger)
    {
        _updateRepository = updateRepository;
        _logger = logger;
    }

    public async Task<AccountEntity> Handle(UpdateRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Start handler to update account bank");
            var account = new AccountEntity(request.Id, request.Name, request.Balance, request.Description);

            _logger.LogInformation("Execute transaction with database");
            await _updateRepository.Execute(account);

            _logger.LogInformation("Update account with success");
            return account;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error Update: {e.Message}");
            throw;
        }
    }
}