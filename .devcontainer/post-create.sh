#!/bin/bash
# ══════════════════════════════════════════════════════════════════════════════
#  .devcontainer/post-create.sh
#  Script exécuté UNE SEULE FOIS après la création du DevContainer
#  Fonctionne identiquement en LOCAL et sur GitHub Codespaces
# ══════════════════════════════════════════════════════════════════════════════

set -e  # Arrêter en cas d'erreur

echo ""
echo "🎵 ════════════════════════════════════════════════════════════════"
echo "🎵  PlaylistApp – Configuration du DevContainer"
echo "🎵 ════════════════════════════════════════════════════════════════"
echo ""

# ── 1. Installer l'outil EF Core ─────────────────────────────────────────────
echo "📦 Installation de dotnet-ef (Entity Framework Core CLI)..."
dotnet tool install --global dotnet-ef
echo "   ✅ dotnet-ef $(dotnet ef --version 2>/dev/null | head -1)"

# ── 2. Restaurer les packages NuGet de tous les projets ──────────────────────
echo ""
echo "📦 Restauration des packages NuGet..."

for csproj in PlaylistApp/PlaylistApp.csproj PlaylistAppEF/PlaylistAppEF.csproj PlaylistAppAPI/PlaylistAppAPI.csproj; do
  if [ -f "$csproj" ]; then
    echo "   → dotnet restore $csproj"
    dotnet restore "$csproj" --nologo 2>&1 | tail -1
  fi
done
echo "   ✅ Packages NuGet restaurés"

# ── 3. Créer le dossier de données SQLite ────────────────────────────────────
echo ""
echo "📂 Création du dossier /data pour SQLite..."
# Racine du workspace déduite de l'emplacement du script (portable quel que soit le nom du dépôt)
WORKSPACE_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
mkdir -p "$WORKSPACE_ROOT/data"
chmod 777 "$WORKSPACE_ROOT/data"
echo "   ✅ Dossier /data créé"

# ── 4. Configurer Git (identité par défaut si non configurée) ─────────────────
echo ""
echo "⚙️  Configuration Git..."
git config --global core.editor "code --wait" 2>/dev/null || true
git config --global init.defaultBranch main 2>/dev/null || true
echo "   ✅ Git configuré"

# ── 5. Ajouter dotnet tools au PATH ──────────────────────────────────────────
echo ""
echo "🔧 Ajout des outils dotnet au PATH..."
echo 'export PATH="$PATH:/home/vscode/.dotnet/tools"' >> ~/.bashrc
echo 'export PATH="$PATH:/home/vscode/.dotnet/tools"' >> ~/.zshrc 2>/dev/null || true
echo "   ✅ PATH mis à jour"

# ── 6. Afficher le résumé ─────────────────────────────────────────────────────
echo ""
echo "════════════════════════════════════════════════════════════════════"
echo "✅  DevContainer prêt !"
echo ""
echo "  📚 TP1 (Console)  : cd PlaylistApp    && docker compose up"
echo "  🗄️  TP2 (EF Core)  : cd PlaylistAppEF  && docker compose up --build"
echo "  🌐 TP3 (API REST) : cd PlaylistAppAPI  && docker compose up --build"
echo "  🧪 Tests          : dotnet test PlaylistAppEF.Tests/"
echo "  📊 Swagger        : http://localhost:5000"
echo ""
echo "  Commandes EF Core :"
echo "  dotnet ef migrations add NomMigration"
echo "  dotnet ef database update"
echo "════════════════════════════════════════════════════════════════════"
echo ""
