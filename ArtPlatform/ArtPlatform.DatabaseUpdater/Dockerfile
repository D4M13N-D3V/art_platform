FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ArtPlatform.DatabaseUpdater/ArtPlatform.DatabaseUpdater.csproj", "ArtPlatform.DatabaseUpdater/"]
RUN dotnet restore "ArtPlatform.DatabaseUpdater/ArtPlatform.DatabaseUpdater.csproj"
COPY . .
WORKDIR "/src/ArtPlatform.DatabaseUpdater"
RUN dotnet build "ArtPlatform.DatabaseUpdater.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ArtPlatform.DatabaseUpdater.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArtPlatform.DatabaseUpdater.dll"]
