namespace BankAccount.Application.Request.Command.Account;

public class DeleteRequestCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}