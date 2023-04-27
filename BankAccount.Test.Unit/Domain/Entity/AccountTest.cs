using BankAccount.Domain.Entity;

namespace BankAccount.Test.Unit.Domain.Entity;

public class AccountTest : IDisposable
{
    private readonly Faker _faker = new();
    private dynamic? _dynamic;

    public AccountTest()
    {
        _dynamic = new
        {
            Id = _faker.Random.Guid(),
            Name = _faker.Name.FullName(),
            Description = _faker.Random.AlphaNumeric(400),
            Balance = _faker.Random.Decimal(0, 1000)
        };
    }

    public void Dispose()
    {
        _dynamic = null;
    }

    [Fact]
    public void Should_Create_New_Instance_Equal_Object()
    {
        Account account = new(_dynamic?.Id, _dynamic?.Name, _dynamic?.Balance, _dynamic?.Description);
        (_dynamic as object).Should().BeEquivalentTo(account);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Should_Throw_Exception_When_Name_Is_Invalid(string name)
    {
        var action = () => new Account(_dynamic?.Id, name, _dynamic?.Balance, _dynamic?.Description);
        action.Should().Throw<ArgumentException>().WithMessage($"{nameof(Account.Name)} can't be null or empty");
    }
}