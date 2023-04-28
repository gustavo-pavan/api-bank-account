using BankAccount.Domain.Repository.Account;
using MediatR;

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

public class DeleteRequestCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DeleteRequestCommandHandler : IRequestHandler<DeleteRequestCommand, bool>
{
    private readonly IDeleteRepository<Account> _deleteRepository;
    private readonly ILogger<DeleteRequestCommandHandler> _logger;

    public DeleteRequestCommandHandler(IDeleteRepository<Account> deleteRepository,
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