namespace BankAccount.Domain.Entity;

public class Account : BaseEntity
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