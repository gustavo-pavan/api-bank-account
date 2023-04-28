#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BankAccount.Presentation.Api/BankAccount.Presentation.Api.csproj", "BankAccount.Presentation.Api/"]
COPY ["BankAccount.Application.Request.Command/BankAccount.Application.Request.Command.csproj", "BankAccount.Application.Request.Command/"]
COPY ["BankAccount.Domain.Entity/BankAccount.Domain.Entity.csproj", "BankAccount.Domain.Entity/"]
COPY ["BankAccount.IoC.Injection/BankAccount.IoC.Injection.csproj", "BankAccount.IoC.Injection/"]
COPY ["BankAccount.Application.Request.Handler/BankAccount.Application.Request.Handler.csproj", "BankAccount.Application.Request.Handler/"]
COPY ["BankAccount.Domain.Repository/BankAccount.Domain.Repository.csproj", "BankAccount.Domain.Repository/"]
COPY ["BankAccount.Application.Request.Validation/BankAccount.Application.Request.Validation.csproj", "BankAccount.Application.Request.Validation/"]
COPY ["BankAccount.Helper.Behavior/BankAccount.Helper.Behavior.csproj", "BankAccount.Helper.Behavior/"]
COPY ["BankAccount.Helper.Extensions/BankAccount.Helper.Extensions.csproj", "BankAccount.Helper.Extensions/"]
COPY ["BankAccount.Infra.Context/BankAccount.Infra.Context.csproj", "BankAccount.Infra.Context/"]
COPY ["BankAccount.Infra.Repository/BankAccount.Infra.Repository.csproj", "BankAccount.Infra.Repository/"]
RUN dotnet restore "BankAccount.Presentation.Api/BankAccount.Presentation.Api.csproj"
COPY . .
WORKDIR "/src/BankAccount.Presentation.Api"
RUN dotnet build "BankAccount.Presentation.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BankAccount.Presentation.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankAccount.Presentation.Api.dll"]