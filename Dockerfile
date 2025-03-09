
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BlinKayTest/BlinKayTest.API.csproj", "BlinKayTest/"]
COPY ["BlinkayTest.Application/BlinkayTest.Application.csproj", "BlinkayTest.Application/"]
COPY ["BlinKayTest.Contracts/BlinKayTest.Shared.csproj", "BlinKayTest.Contracts/"]
COPY ["BlinKayTest.Infrastructure.SQL/BlinKayTest.Infrastructure.SQL.csproj", "BlinKayTest.Infrastructure.SQL/"]
RUN dotnet restore "./BlinKayTest/BlinKayTest.API.csproj"
COPY . .
WORKDIR "/src/BlinKayTest"
RUN dotnet build "./BlinKayTest.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlinKayTest.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlinKayTest.API.dll"]