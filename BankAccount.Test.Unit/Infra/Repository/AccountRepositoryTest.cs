using BankAccount.Domain.Entity;
using BankAccount.Infra.Repository;
using BankAccount.Test.Unit.Mocks;

namespace BankAccount.Test.Unit.Infra.Repository;

public class AccountRepositoryTest
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task Should_Create_Account_And_Generate_Id()
    {
        Account account = new(_faker.Name.FullName(), _faker.Random.Decimal(0, 100), _faker.Random.AlphaNumeric(400));

        var mongoContextMock = MongoContextMock.Mock(new List<Account> { account });

        AccountRepository repository = new(mongoContextMock.Object);

        await repository.Create(account);
        account.Id.Should().Be(account.Id);
    }

    [Fact]
    public void Should_Throw_Exception_When_Id_Is_Invalid_In_Create_Account()
    {
        Account account = new(_faker.Random.Guid(), _faker.Name.FullName(), _faker.Random.Decimal(0, 100),
            _faker.Random.AlphaNumeric(400));

        var mongoContextMock = MongoContextMock.Mock(new List<Account> { account });

        AccountRepository repository = new(mongoContextMock.Object);

        var func = async () => await repository.Create(account);

        func.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Can't create because {nameof(BaseEntity.Id)} is not valid!");
    }

    [Fact]
    public void Should_Throw_Exception_When_Id_Is_Invalid_In_Update_Account()
    {
        Account account = new(_faker.Name.FullName(), _faker.Random.Decimal(0, 100), _faker.Random.AlphaNumeric(400));

        var mongoContextMock = MongoContextMock.Mock(new List<Account> { account });

        AccountRepository repository = new(mongoContextMock.Object);

        var func = async () => await repository.Update(account);

        func.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Can't update because {nameof(BaseEntity.Id)} is not valid!");
    }

    [Fact]
    public void Should_Throw_Exception_When_Id_Is_Invalid_In_Delete_Account()
    {
        var mongoContextMock = MongoContextMock.Mock(new List<Account>());

        AccountRepository repository = new(mongoContextMock.Object);

        var func = async () => await repository.Delete(Guid.Empty);

        func.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Can't delete because {nameof(BaseEntity.Id)} is not valid!");
    }
}