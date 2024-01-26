FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY BudgetWebApi/* BudgetWebApi/
COPY BudgetServices/* BudgetServices/
COPY BudgetModel/* BudgetModel/

RUN dotnet restore ./BudgetWebApi/BudgetWebApi.csproj
RUN dotnet build ./BudgetWebApi/BudgetWebApi.csproj

FROM build AS publish
RUN dotnet publish ./BudgetWebApi/BudgetWebApi.csproj -c Release -o ./Bin

FROM base AS final

WORKDIR /app
COPY --from=publish /src/Bin .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "BudgetWebApi.dll"]
