# ══════════════════════════════════════════════════════════════════════════════
#  Dockerfile – PlaylistApp EF  (TP2 : Entity Framework Core + SQLite)
#
#  Build multi-étapes :
#   Stage 1 "build"     → SDK complet : compilation + génération migrations
#   Stage 2 "runtime"   → Image légère : uniquement le runtime .NET 10
#
#  ⚡ Inspiration : github.com/jasonsturges/sqlite-dotnet-core
# ══════════════════════════════════════════════════════════════════════════════

# ── Stage 1 : Build & Migrations ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Installer l'outil EF Core dans l'image de build
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copier le .csproj d'abord → optimisation du cache Docker
# (NuGet n'est retéléchargé que si le .csproj change)
COPY PlaylistAppEF.csproj ./
RUN dotnet restore

# Copier tout le code source
COPY . ./

# Générer les migrations si elles n'existent pas encore
# (en production on les commite dans Git → on supprime cette ligne)
RUN dotnet ef migrations add MigrationInitiale --no-build 2>/dev/null || true

# Compiler en mode Release
RUN dotnet publish -c Release -o /app/publish

# ── Stage 2 : Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime

LABEL description="TP2 – Playlist EF Core + SQLite dans Docker"
LABEL version="2.0"

WORKDIR /app

# Copier les artefacts publiés depuis le stage build
COPY --from=build /app/publish .

# ── Dossier pour la base SQLite ───────────────────────────────────────────────
# Ce dossier sera monté comme volume pour persister les données
RUN mkdir -p /data && chmod 777 /data
ENV DB_PATH=/data

# ── Autoriser les couleurs ANSI dans la console ───────────────────────────────
ENV DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION=1

# Point d'entrée
ENTRYPOINT ["dotnet", "PlaylistAppEF.dll"]
