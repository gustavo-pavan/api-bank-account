namespace BankAccount.Test.Unit.Application.Handler;

public class CreateCommandHandlerTest
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task Should_Create_Account()
    {
        var mockLogger = new Mock<ILogger<CreateRequestCommandHandler>>();

        var mongoContextMock = MongoContextMock.Mock(new List<Account>());

        CreateRepository<Account> repository = new(mongoContextMock.Object);

        CreateRequestCommandHandler requestCommand = new(repository, mockLogger.Object);

        CreateRequestCommand command = new()
        {
            Balance = _faker.Random.Decimal(0, 1000),
            Description = _faker.Random.AlphaNumeric(400),
            Name = _faker.Name.FullName()
        };

        var result = await requestCommand.Handle(command, CancellationToken.None);

        result.Id.Should().NotBe(Guid.Empty);
    }
}

public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, Account>
{
    private readonly ICreateRepository<Account> _createRepository;
    private readonly ILogger<CreateRequestCommandHandler> _logger;

    public CreateRequestCommandHandler(ICreateRepository<Account> createRepository,
        ILogger<CreateRequestCommandHandler> logger)
    {
        _createRepository = createRepository;
        _logger = logger;
    }

    public async Task<Account> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Start handler to create new account bank");
            var account = new Account(request.Name, request.Balance, request.Description);

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

public class CreateRequestCommand : IRequest<Account>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Balance { get; set; }
}