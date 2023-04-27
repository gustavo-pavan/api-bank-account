namespace BankAccount.Test.Unit.Infra.UoW;

public class UnitOfWorkTest
{
    [Fact]
    public async Task Should_Return_Status_True()
    {
        var contextMock = new Mock<IMongoContext>();
        contextMock.Setup(x => x.SaveChanges())
            .ReturnsAsync(2);

        var sessionMock = new Mock<IClientSessionHandle>();

        contextMock.Setup(x => x.SessionHandle)
            .ReturnsAsync(sessionMock.Object);

        UnitOfWork uow = new(contextMock.Object);

        var result = await uow.Commit();
        result.Should().BeTrue();
    }
}