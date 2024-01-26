FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /src
COPY BudgetWebApi/* BudgetWebApi/
COPY BudgetServices/* BudgetServices/
COPY BudgetModel/* BudgetModel/

# Install ef tools
RUN dotnet tool install --global dotnet-ef --version 6.0.21
ENV PATH="${PATH}:/root/.dotnet/tools"
# Prepare the environment to run the migration command
WORKDIR /src/BudgetModel/
