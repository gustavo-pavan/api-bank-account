namespace BankAccount.Test.Unit.Infra.Repository;

public class UpdateRepositoryTest
{
    private readonly Faker _faker = new();

    [Fact]
    public void Should_Throw_Exception_When_Id_Is_Invalid_In_Update_Account()
    {
        Account account = new(_faker.Name.FullName(), _faker.Random.Decimal(0, 100), _faker.Random.AlphaNumeric(400));

        var mongoContextMock = MongoContextMock.Mock(new List<Account> { account });

        UpdateRepository<Account> repository = new(mongoContextMock.Object);

        var func = async () => await repository.Execute(account);

        func.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Can't update because {nameof(BaseEntity.Id)} is not valid!");
    }
}