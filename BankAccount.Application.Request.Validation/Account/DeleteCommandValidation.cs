namespace BankAccount.Application.Request.Validation.Account;

public class DeleteCommandValidation : AbstractValidator<DeleteRequestCommand>
{
    public DeleteCommandValidation()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("{property} can't be null")
            .NotEmpty().WithMessage("{property} can't be empty");
    }
}