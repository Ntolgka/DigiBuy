FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DigiBuy.Api/DigiBuy.Api.csproj", "DigiBuy.Api/"]
RUN dotnet restore "DigiBuy.Api/DigiBuy.Api.csproj"
COPY . .
WORKDIR "/src/DigiBuy.Api"
RUN dotnet build "DigiBuy.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DigiBuy.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DigiBuy.Api.dll"]
