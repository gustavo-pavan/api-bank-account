namespace BankAccount.Test.Unit.Infra.Repository;

public class DeleteRepositoryTes
{
    [Fact]
    public void Should_Throw_Exception_When_Id_Is_Invalid_In_Delete_Account()
    {
        var mongoContextMock = MongoContextMock.Mock(new List<Account>());

        DeleteRepository<Account> repository = new(mongoContextMock.Object);

        var func = async () => await repository.Execute(Guid.Empty);

        func.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Can't delete because {nameof(BaseEntity.Id)} is not valid!");
    }
}