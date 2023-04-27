using BankAccount.Domain.Entity;
using BankAccount.Test.Unit.Infra.Repository;
using MongoDB.Driver;

namespace BankAccount.Test.Unit.Mocks;

public class MongoContextMock
{
    public static Mock<IMongoContext> Mock(IEnumerable<Account> accounts)
    {
        var cursorMock = new Mock<IAsyncCursor<Account>>();
        var mongoContextMock = new Mock<IMongoContext>();
        var mongoCollectionMock = new Mock<IMongoCollection<Account>>();

        cursorMock.Setup(_ => _.Current).Returns(accounts);

        cursorMock
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        cursorMock
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true))
            .Returns(Task.FromResult(false));

        mongoCollectionMock.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Account>>(),
                It.IsAny<FindOptions<Account, Account>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        mongoContextMock.Setup(x => x.AddCommand(It.IsAny<Func<Task>>()));

        mongoContextMock.Setup(x => x.GetCollection<Account>(It.IsAny<string>()))
            .Returns(mongoCollectionMock.Object);
        return mongoContextMock;
    }
}