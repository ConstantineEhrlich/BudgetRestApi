FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY BudgetWebApi/* BudgetWebApi/
COPY BudgetServices/* BudgetServices/
COPY BudgetModel/* BudgetModel/

# Restore nuget packages before the build
RUN dotnet restore ./BudgetWebApi/BudgetWebApi.csproj

# Build and publish executable
RUN dotnet publish ./BudgetWebApi/BudgetWebApi.csproj -c Release -o ./Bin

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /app
COPY --from=build-env /app/Bin .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["/app/BudgetWebApi"]
