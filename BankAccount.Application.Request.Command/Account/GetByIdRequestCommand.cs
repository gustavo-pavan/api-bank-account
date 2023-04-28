namespace BankAccount.Application.Request.Command.Account;

public class GetByIdRequestCommand : IRequest<AccountEntity?>
{
    public Guid Id { get; set; }
}