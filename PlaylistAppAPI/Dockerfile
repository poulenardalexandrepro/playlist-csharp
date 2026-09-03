# ══════════════════════════════════════════════════════════════════════════════
#  Dockerfile – PlaylistApp API (TP3 : SOA + EOA)
#  ASP.NET Core Web API  |  Cas pratique PlaylistApp
# ══════════════════════════════════════════════════════════════════════════════

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copier les projets référencés (API dépend de EF)
COPY PlaylistAppAPI/PlaylistAppAPI.csproj ./PlaylistAppAPI/
COPY PlaylistAppEF/PlaylistAppEF.csproj   ./PlaylistAppEF/

# Restaurer les dépendances
RUN dotnet restore PlaylistAppAPI/PlaylistAppAPI.csproj

# Copier tout le code source
COPY PlaylistAppAPI/ ./PlaylistAppAPI/
COPY PlaylistAppEF/  ./PlaylistAppEF/

# Publier en Release
RUN dotnet publish PlaylistAppAPI/PlaylistAppAPI.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime ───────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

LABEL description="PlaylistApp API REST – SOA + EOA"

COPY --from=build /app/publish .

RUN mkdir -p /data && chmod 777 /data

ENV DB_PATH=/data
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5000

EXPOSE 5000

ENTRYPOINT ["dotnet", "PlaylistAppAPI.dll"]
