namespace BankAccount.Test.Unit.Domain;

public class AccountTest :IDisposable
{
    private dynamic? _dynamic;
    private readonly Faker _faker = new();

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

    [Fact]
    public void Should_Create_New_Instance_Equal_Object()
    {
        Account account = new(_dynamic?.Id, _dynamic?.Name, _dynamic?.Balance, _dynamic?.Description);
        (_dynamic as object).Should().BeEquivalentTo(account);
    }

    public void Dispose()
    {
        _dynamic = null;
    }
}

public class Account
{
    public Account(string name, decimal balance, string description)
    {
        Name = name;
        Balance = balance;
        Description = description;
    }

    public Account(Guid id, string name, decimal balance, string description) : this(name, balance, description)
        => (Id) = (id);

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public decimal Balance { get; private set; }
    public string Description { get; private set; }
}