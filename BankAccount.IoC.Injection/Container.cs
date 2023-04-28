namespace BankAccount.IoC.Injection;

public static class Container
{
    public static void AddDependency(this IServiceCollection service)
    {
        service.AddScoped<IMongoContext, MongoContext>();

        service.AddTransient(typeof(ICreateRepository<>), typeof(CreateRepository<>));
        service.AddTransient(typeof(IUpdateRepository<>), typeof(UpdateRepository<>));
        service.AddTransient(typeof(IDeleteRepository<>), typeof(DeleteRepository<>));
        service.AddTransient(typeof(IGetByIdRepository<>), typeof(GetByIdRepository<>));
        service.AddTransient(typeof(IGetRepository<>), typeof(GetRepository<>));

        service.AddMediatR(x =>
            {
                x.RegisterServicesFromAssemblies(Assembly.Load("BankAccount.Application.Request.Command"),
                    Assembly.Load("BankAccount.Application.Request.Handler"));
            }
        );

        service.AddTransient(typeof(IPipelineBehavior<,>), typeof(PipelineBehavior<,>));
        service.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        service.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        service.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));

        service.AddValidatorsFromAssembly(Assembly.Load("BankAccount.Application.Request.Validation"));
    }
}