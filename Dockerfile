FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RPSLS.API/RPSLS.API.csproj", "RPSLS.API/"]
COPY ["RPSLS.Application/RPSLS.Application.csproj", "RPSLS.Application/"]
COPY ["RPSLS.Domain/RPSLS.Domain.csproj", "RPSLS.Domain/"]
COPY ["RPSLS.Infrastructure/RPSLS.Infrastructure.csproj", "RPSLS.Infrastructure/"]
RUN dotnet restore "RPSLS.API/RPSLS.API.csproj"
COPY . .
WORKDIR "/src/RPSLS.API"
RUN dotnet build "RPSLS.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RPSLS.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RPSLS.API.dll"]