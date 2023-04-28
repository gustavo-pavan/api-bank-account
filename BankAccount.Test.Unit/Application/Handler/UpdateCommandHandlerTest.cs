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