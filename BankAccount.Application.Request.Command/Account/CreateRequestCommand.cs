namespace BankAccount.Application.Request.Command.Account;

public class CreateRequestCommand : IRequest<AccountEntity>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Balance { get; set; }
}