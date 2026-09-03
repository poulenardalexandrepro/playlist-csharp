# ══════════════════════════════════════════════════════════════
#  Dockerfile – PlaylistApp (C# / .NET 10)
#  Cas pratique : Gestion de Playlist Musicale
#
#  Build multi-stage :
#   - Stage 1 "build"  : compilation avec le SDK .NET complet
#   - Stage 2 "runtime": image finale légère (ASP.NET runtime)
# ══════════════════════════════════════════════════════════════

# ── Stage 1 : Build ──────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Répertoire de travail dans le conteneur
WORKDIR /src

# Copier uniquement le .csproj pour bénéficier du cache Docker
# (les dépendances NuGet ne sont retéléchargées que si le .csproj change)
COPY PlaylistApp.csproj ./

# Restaurer les dépendances NuGet
RUN dotnet restore

# Copier le reste du code source
COPY . ./

# Compiler en mode Release et publier dans /app/publish
RUN dotnet publish -c Release -o /app/publish --no-restore

# ── Stage 2 : Runtime ─────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime

# Métadonnées de l'image
LABEL description="Application console de gestion de playlists musicales"
LABEL version="1.0"

# Répertoire de travail
WORKDIR /app

# Copier uniquement les fichiers publiés depuis le stage build
COPY --from=build /app/publish .

# Variable d'environnement (optionnel)
ENV DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION=1

# Point d'entrée : lancement de l'application
ENTRYPOINT ["dotnet", "PlaylistApp.dll"]
