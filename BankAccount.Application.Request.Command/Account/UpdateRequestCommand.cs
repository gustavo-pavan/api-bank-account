namespace BankAccount.Application.Request.Command.Account;

public class UpdateRequestCommand : IRequest<AccountEntity>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Balance { get; set; }
}