#!/bin/bash
# ══════════════════════════════════════════════════════════════════════════════
#  .devcontainer/post-create.sh
#  Exécuté après la création du Dev Container.
#  Fonctionne identiquement en LOCAL et sur GitHub Codespaces.
#
#  Le script est IDEMPOTENT : il peut être relancé (Rebuild Container) sans
#  échouer ni dupliquer de lignes dans ~/.bashrc.
# ══════════════════════════════════════════════════════════════════════════════

# Pas de `set -e` : une étape non critique qui échoue ne doit jamais empêcher
# les suivantes. Chaque étape signale elle-même son succès ou son échec.

WORKSPACE_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
DOTNET_TOOLS="$HOME/.dotnet/tools"
export PATH="$PATH:$DOTNET_TOOLS"

echo ""
echo "🎵 ════════════════════════════════════════════════════════════════"
echo "🎵  PlaylistApp – Configuration du Dev Container"
echo "🎵 ════════════════════════════════════════════════════════════════"

# ── 1. Dossier de données SQLite (avant tout le reste) ───────────────────────
echo ""
echo "📂 Dossier data/ pour SQLite..."
mkdir -p "$WORKSPACE_ROOT/data"
echo "   ✅ $WORKSPACE_ROOT/data"

# ── 2. Outil EF Core ────────────────────────────────────────────────────────
echo ""
echo "📦 dotnet-ef (Entity Framework Core CLI)..."
if dotnet tool list --global 2>/dev/null | grep -q dotnet-ef; then
  dotnet tool update --global dotnet-ef --nologo >/dev/null 2>&1
  echo "   ✅ déjà présent, mis à jour si besoin"
else
  if dotnet tool install --global dotnet-ef --nologo >/dev/null 2>&1; then
    echo "   ✅ installé"
  else
    echo "   ⚠️  installation impossible (réseau ?). Relancez plus tard :"
    echo "      dotnet tool install --global dotnet-ef"
  fi
fi
echo "   → version : $(dotnet ef --version 2>/dev/null | tail -1 || echo 'indisponible')"

# ── 3. Packages NuGet ───────────────────────────────────────────────────────
echo ""
echo "📦 Restauration des packages NuGet..."
for csproj in PlaylistApp/PlaylistApp.csproj \
              PlaylistAppEF/PlaylistAppEF.csproj \
              PlaylistAppAPI/PlaylistAppAPI.csproj; do
  if [ -f "$WORKSPACE_ROOT/$csproj" ]; then
    if dotnet restore "$WORKSPACE_ROOT/$csproj" --nologo >/dev/null 2>&1; then
      echo "   ✅ $csproj"
    else
      echo "   ⚠️  $csproj — échec, réessayez : dotnet restore $csproj"
    fi
  fi
done

# ── 4. Git ──────────────────────────────────────────────────────────────────
echo ""
echo "⚙️  Configuration Git..."
git config --global init.defaultBranch main 2>/dev/null || true
git config --global core.editor "code --wait" 2>/dev/null || true
echo "   ✅ fait"

# ── 5. PATH persistant (sans doublon si le script est relancé) ──────────────
echo ""
echo "🔧 Outils dotnet dans le PATH..."
LIGNE_PATH='export PATH="$PATH:$HOME/.dotnet/tools"'
for profil in "$HOME/.bashrc" "$HOME/.zshrc"; do
  [ -f "$profil" ] || continue
  grep -qF "$LIGNE_PATH" "$profil" || echo "$LIGNE_PATH" >> "$profil"
done
echo "   ✅ fait"

# ── 6. Résumé ───────────────────────────────────────────────────────────────
echo ""
echo "════════════════════════════════════════════════════════════════════"
echo "✅  Dev Container prêt."
echo ""
echo "  ▶️  DÉMARRER ICI — sans Docker, c'est le plus simple :"
echo ""
echo "      cd PlaylistApp     && dotnet run     # TP1 — console"
echo "      cd PlaylistAppEF   && dotnet run     # TP2 — EF Core"
echo "      cd PlaylistAppAPI  && dotnet run     # TP3 et TP4 — API"
echo ""
echo "  🧪  Tests :"
echo "      dotnet test PlaylistAppEF.Tests/"
echo "      dotnet test PlaylistAppAPI.Tests/"
echo ""
echo "  🗄️  Migrations EF Core (TP2) :"
echo "      cd PlaylistAppEF"
echo "      dotnet ef migrations add NomDeLaMigration"
echo "      dotnet ef database update"
echo ""
echo "  🌐  Swagger, après 'dotnet run' dans PlaylistAppAPI :"
if [ -n "$CODESPACES" ]; then
  echo "      Onglet PORTS en bas de VS Code → port 5000 → icône 🌐"
  echo "      (dans un Codespace, http://localhost:5000 ne s'ouvre PAS)"
else
  echo "      http://localhost:5000/swagger"
fi
echo ""
echo "  🐳  Docker compose : utile au TP2/TP3 pour voir la conteneurisation,"
echo "      mais jamais obligatoire pour faire tourner l'application."
echo ""
echo "  ❓  Un blocage ? → DEPANNAGE.md"
echo "════════════════════════════════════════════════════════════════════"
echo ""
