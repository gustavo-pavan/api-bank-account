namespace BankAccount.Application.Request.Handler.Account;

public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, AccountEntity>
{
    private readonly ICreateRepository<AccountEntity> _createRepository;
    private readonly ILogger<CreateRequestCommandHandler> _logger;

    public CreateRequestCommandHandler(ICreateRepository<AccountEntity> createRepository,
        ILogger<CreateRequestCommandHandler> logger)
    {
        _createRepository = createRepository;
        _logger = logger;
    }

    public async Task<AccountEntity> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Start handler to create new account bank");
            var account = new AccountEntity(request.Name, request.Balance, request.Description);

            _logger.LogInformation("Execute transaction with database");
            await _createRepository.Execute(account);

            _logger.LogInformation("Create account with success");
            return account;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error: {e.Message}");
            throw;
        }
    }
}