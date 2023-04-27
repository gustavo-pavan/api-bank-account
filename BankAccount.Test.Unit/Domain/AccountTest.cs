namespace BankAccount.Test.Unit.Domain;

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

public abstract class Entity
{
    public Guid Id { get; protected set; }
}

public class Account : Entity
{
    public Account(string name, decimal balance, string description)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{nameof(Name)} can't be null or empty");

        Name = name;
        Balance = balance;
        Description = description;
    }

    public Account(Guid id, string name, decimal balance, string description) : this(name, balance, description)
    {
        Id = id;
    }

    public string Name { get; private set; }
    public decimal Balance { get; private set; }
    public string Description { get; private set; }
}