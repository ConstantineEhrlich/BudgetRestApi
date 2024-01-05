FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

## Copy the projects
COPY BudgetWebApi/*.csproj BudgetWebApi/
COPY BudgetServices/*.csproj BudgetServices/
COPY BudgetModel/*.csproj BudgetModel/

# Restore depensdencies
RUN dotnet restore ./BudgetWebApi/BudgetWebApi.csproj

# Copy everything else
COPY BudgetWebApi/* BudgetWebApi/
COPY BudgetServices/* BudgetServices/
COPY BudgetModel/* BudgetModel/

# Build and publish executable
RUN dotnet publish ./BudgetWebApi/BudgetWebApi.csproj -c Release -o ./Bin

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app
COPY --from=build-env /app/Bin .
ENTRYPOINT ["dotnet", "BudgetWebApi.dll"]
