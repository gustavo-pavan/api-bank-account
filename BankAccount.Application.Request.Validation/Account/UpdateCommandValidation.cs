namespace BankAccount.Application.Request.Validation.Account;

public class UpdateCommandValidation : AbstractValidator<UpdateRequestCommand>
{
    public UpdateCommandValidation()
    {
        RuleFor(x => x.Name)
            .NotNull().WithMessage("{property} can't be null")
            .NotEmpty().WithMessage("{property} can't be empty");

        RuleFor(x => x.Id)
            .NotNull().WithMessage("{property} can't be null")
            .NotEmpty().WithMessage("{property} can't be empty");
    }
}