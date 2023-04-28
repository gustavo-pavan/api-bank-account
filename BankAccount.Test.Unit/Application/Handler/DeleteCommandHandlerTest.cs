namespace BankAccount.Test.Unit.Application.Handler;

public class DeleteCommandHandlerTest
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task Should_Delete_Account()
    {
        var mockLogger = new Mock<ILogger<DeleteRequestCommandHandler>>();

        var mongoContextMock = MongoContextMock.Mock(new List<Account>());

        DeleteRepository<Account> repository = new(mongoContextMock.Object);

        DeleteRequestCommandHandler requestCommand = new(repository, mockLogger.Object);

        DeleteRequestCommand command = new()
        {
            Id = _faker.Random.Guid()
        };

        var result = await requestCommand.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_Throw_Exception_Update_Account()
    {
        var mockLogger = new Mock<ILogger<DeleteRequestCommandHandler>>();
        
        var mongoContextMock = MongoContextMock.Mock(new List<Account>());

        DeleteRepository<Account> repository = new(mongoContextMock.Object);

        DeleteRequestCommandHandler requestCommand = new(repository, mockLogger.Object);

        DeleteRequestCommand command = new();

        Func<Task> func = () => requestCommand.Handle(command, CancellationToken.None);
        func.Should().ThrowAsync<ArgumentException>();
    }
}