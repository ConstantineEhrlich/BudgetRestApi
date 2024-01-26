FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY AuthService/* AuthService/

RUN dotnet restore ./AuthService/AuthService.csproj
RUN dotnet build ./AuthService/AuthService.csproj

FROM build AS publish
RUN dotnet publish ./AuthService/AuthService.csproj -c Release -o ./Bin

FROM base AS final

WORKDIR /app
COPY --from=publish /src/Bin .
ENV ASPNETCORE_URLS=http://+:5232
ENTRYPOINT ["dotnet", "AuthService.dll"]
