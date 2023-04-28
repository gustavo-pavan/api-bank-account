namespace BankAccount.Test.Unit.Application.Handler;

public class UpdateCommandHandlerTest
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task Should_Update_Account()
    {
        var mockLogger = new Mock<ILogger<CreateRequestCommandHandler>>();

        var mongoContextMock = MongoContextMock.Mock(new List<Account>());

        UpdateRepository<Account> repository = new(mongoContextMock.Object);

        UpdateRequestCommandHandler requestCommand = new(repository, mockLogger.Object);

        UpdateRequestCommand command = new()
        {
            Id = _faker.Random.Guid(),
            Balance = _faker.Random.Decimal(0, 1000),
            Description = _faker.Random.AlphaNumeric(400),
            Name = _faker.Name.FullName()
        };

        var result = await requestCommand.Handle(command, CancellationToken.None);

        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Should_Throw_Exception_Update_Account()
    {
        var mockLogger = new Mock<ILogger<CreateRequestCommandHandler>>();

        var mongoContextMock = MongoContextMock.Mock(new List<Account>());

        UpdateRepository<Account> repository = new(mongoContextMock.Object);

        UpdateRequestCommandHandler requestCommand = new(repository, mockLogger.Object);

        UpdateRequestCommand command = new()
        {
            Balance = _faker.Random.Decimal(0, 1000),
            Description = _faker.Random.AlphaNumeric(400),
            Name = _faker.Name.FullName()
        };

        Func<Task> func  = () => requestCommand.Handle(command, CancellationToken.None);
        func.Should().ThrowAsync<ArgumentException>();
    }
}

public class UpdateRequestCommand : IRequest<Account>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Balance { get; set; }
}

public class UpdateRequestCommandHandler : IRequestHandler<UpdateRequestCommand, Account>
{
    private readonly IUpdateRepository<Account> _updateRepository;
    private readonly ILogger<CreateRequestCommandHandler> _logger;

    public UpdateRequestCommandHandler(IUpdateRepository<Account> updateRepository,
        ILogger<CreateRequestCommandHandler> logger)
    {
        _updateRepository = updateRepository;
        _logger = logger;
    }

    public async Task<Account> Handle(UpdateRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Start handler to update account bank");
            var account = new Account(request.Id, request.Name, request.Balance, request.Description);

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