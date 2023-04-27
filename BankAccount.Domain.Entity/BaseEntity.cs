namespace BankAccount.Domain.Entity;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
}