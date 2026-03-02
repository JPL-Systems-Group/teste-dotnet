FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY OrdersApi.sln ./
COPY src/Orders.Api/Orders.Api.csproj src/Orders.Api/
COPY src/Orders.Application/Orders.Application.csproj src/Orders.Application/
COPY src/Orders.Domain/Orders.Domain.csproj src/Orders.Domain/
COPY src/Orders.Infrastructure/Orders.Infrastructure.csproj src/Orders.Infrastructure/
RUN dotnet restore src/Orders.Api/Orders.Api.csproj

COPY src ./src
RUN dotnet publish src/Orders.Api/Orders.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Orders.Api.dll"]
