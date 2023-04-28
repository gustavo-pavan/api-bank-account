namespace BankAccount.Application.Request.Validation.Account;

public class GetByIdCommandValidation : AbstractValidator<GetByIdRequestCommand>
{
    public GetByIdCommandValidation()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("{property} can't be null")
            .NotEmpty().WithMessage("{property} can't be empty");
    }
}