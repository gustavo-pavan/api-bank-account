using BankAccount.Infra.Context;
using MongoDB.Driver;

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

public interface IUnitOfWork
{
    Task<bool> Commit();
}

public class UnitOfWork : IDisposable, IUnitOfWork
{
    private readonly IMongoContext _mongoContext;

    public UnitOfWork(IMongoContext mongoContext)
    {
        _mongoContext = mongoContext;
    }

    public async Task<bool> Commit()
    {
        var sessionHandler = await _mongoContext.SessionHandle;

        try
        {
            sessionHandler.StartTransaction();

            var result = await _mongoContext.SaveChanges();

            await sessionHandler.CommitTransactionAsync();

            return result > default(int);
        }
        catch (Exception)
        {
            await sessionHandler.AbortTransactionAsync();
            throw;
        }
        finally
        {
            sessionHandler.Dispose();
        }
    }

    public void Dispose()
    {
        _mongoContext.Dispose();
    }
}