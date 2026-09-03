# 📖 Guide Étudiant – Démarrage pas-à-pas

> **Objectif** : Vous aurez votre propre dépôt GitHub opérationnel en moins de 5 minutes, sans rien installer sur votre poste.

---

## Étape 1 – Créer votre dépôt depuis le template

### Pourquoi "template" et pas "fork" ?

| | Template (✅ à faire) | Fork (❌ à éviter) |
|---|---|---|
| Historique Git | Repart de zéro (propre) | Copie tout l'historique |
| Indépendance | Dépôt totalement indépendant | Lié au dépôt d'origine |
| Pull Requests | Vers votre propre dépôt | Vers le dépôt du prof |
| GitHub Classroom | Compatible | Non compatible |

### Comment faire

1. Aller sur le dépôt du cours : `https://github.com/PROF_USER/playlist-csharp`
2. Cliquer sur le bouton vert **`Use this template`**
3. Cliquer sur **`Create a new repository`**
4. Renseigner :
   - **Repository name** : `playlist-csharp-VOTRE_NOM` *(ex: playlist-csharp-dupont)*
   - **Visibility** : **Public** *(obligatoire pour GitHub Pages gratuit)*
5. Cliquer **`Create repository`**

✅ Votre dépôt personnel est créé à `https://github.com/VOTRE_USER/playlist-csharp-VOTRE_NOM`

---

## Étape 2 – Ouvrir dans GitHub Codespaces

> Aucune installation locale requise. Tout tourne dans votre navigateur.

1. Sur votre dépôt → cliquer **`Code`** (bouton vert)
2. Onglet **`Codespaces`**
3. Cliquer **`Create codespace on main`**
4. Attendre **~2 minutes** (construction de l'environnement Docker)
5. VS Code s'ouvre dans votre navigateur !

### Ce qui est automatiquement installé

Le fichier `.devcontainer/devcontainer.json` configure tout :

| Outil | Version | Utilité |
|---|---|---|
| .NET SDK | 10.0 LTS | Compiler et exécuter C# |
| dotnet-ef | 8.x | Gérer les migrations EF Core |
| Docker CLI | Dernière | Construire et lancer des conteneurs |
| Docker Compose | V2 | Orchestrer multi-conteneurs |
| Git | Dernière | Versionner votre code |

### Extensions VS Code pré-installées

| Extension | Raccourci utile |
|---|---|
| C# Dev Kit | `Ctrl+.` → suggestions, `F12` → définition |
| Docker | Panneau latéral → voir conteneurs |
| SQLite Viewer | Cliquer sur un fichier `.db` → voir les tables |
| REST Client | Fichiers `.http` → tester l'API |
| GitLens | `Ctrl+Shift+G` → historique Git |

---

## Étape 3 – Premier commit (vérifier que tout fonctionne)

Dans le terminal Codespaces :

```bash
# Vérifier les outils
dotnet --version          # → 10.0.xxx
dotnet ef --version       # → Entity Framework Core .NET Command-line Tools 10.x
docker --version          # → Docker version 26.x.x
git --version             # → git version 2.x.x

# Configurer votre identité Git
git config --global user.name  "Prénom NOM"
git config --global user.email "votre@email.com"

# Premier push pour déclencher les GitHub Actions
git add .
git commit -m "chore: initialisation de mon dépôt de cours"
git push
```

✅ Allez dans l'onglet **Actions** de votre dépôt → vous devez voir les workflows s'exécuter.

---

## Étape 4 – Travailler sur les TP

### Convention de nommage des commits

Utilisez la convention **Conventional Commits** pour que votre progression soit lisible :

```
feat: ajout de la méthode DureeFormatee() dans Chanson
fix: correction du calcul de durée totale
test: ajout du test TestAjouterChansonPlaylist
docs: mise à jour du README avec les étapes TP2
refactor: extraction de la couche Repository
```

### Workflow recommandé pour chaque TP

```bash
# 1. Créer une branche pour chaque TP
git checkout -b tp1-collections
git checkout -b tp2-ef-core
git checkout -b tp3-api-rest

# 2. Coder, tester localement
dotnet run          # tester à la main
dotnet test         # tests automatiques

# 3. Committer régulièrement (pas tout à la fin !)
git add .
git commit -m "feat: ajout de la méthode AddSong dans Playlist"

# 4. Pousser → GitHub Actions valident automatiquement
git push origin tp2-ef-core

# 5. Créer une Pull Request vers main quand le TP est terminé
# GitHub → Pull requests → New pull request
```

---

## Étape 5 – Lire les résultats des tests automatiques

Après chaque `git push`, GitHub Actions exécute les tests. Pour voir les résultats :

1. Aller dans l'onglet **`Actions`** de votre dépôt
2. Cliquer sur le workflow qui vient de tourner
3. Cliquer sur le job → voir chaque étape

### Interpréter les badges

| Badge | Signification |
|---|---|
| ![passing](https://img.shields.io/badge/build-passing-brightgreen) | ✅ Tous les tests passent |
| ![failing](https://img.shields.io/badge/build-failing-red) | ❌ Au moins un test échoue |
| ![in progress](https://img.shields.io/badge/build-running-yellow) | ⏳ Pipeline en cours |

---

## Étape 6 – GitHub Pages (votre progression en ligne)

Une fois le workflow `pages.yml` activé, votre documentation est accessible sur :
```
https://VOTRE_USER.github.io/playlist-csharp-VOTRE_NOM
```

Pour activer GitHub Pages :
1. Votre dépôt → **Settings** → **Pages**
2. Source : **GitHub Actions**
3. Sauvegarder

---

## ⚠️ Problèmes fréquents

### Le Codespace ne s'ouvre pas / plante

```bash
# Reconstruire le devcontainer
Ctrl+Shift+P → "Dev Containers: Rebuild Container"
```

### `dotnet ef` : commande introuvable

```bash
dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"
```

### Les tests échouent avec une erreur de BD

```bash
# Supprimer la BD SQLite et recommencer
rm -f data/playlist.db
dotnet ef database update
```

### Docker : permission denied

```bash
# Dans Codespaces, Docker tourne en mode DinD (Docker in Docker)
# Attendre quelques secondes après l'ouverture du Codespace
docker info   # Si erreur → attendre 30s et réessayer
```

### Mon Codespace est lent / freezé

- Codespaces est limité à **60h/mois gratuit** (compte gratuit GitHub)
- Pensez à **arrêter le Codespace** quand vous ne travaillez pas :
  [github.com/codespaces](https://github.com/codespaces) → votre codespace → **Stop codespace**
- Alternative : Gitpod (50h/mois) → `https://gitpod.io/#https://github.com/ggaillard/playlist-csharp`

---

## 📋 Checklist finale avant de rendre le TP

- [ ] Mon dépôt est **public** sur GitHub
- [ ] Les badges CI sont **verts** dans mon README
- [ ] Mon historique Git montre **plusieurs commits** (pas un seul gros commit)
- [ ] Les commits ont des **messages explicites** (Conventional Commits)
- [ ] Mon README contient **les cases cochées** de ma progression
- [ ] L'URL de mon dépôt est partagée avec le professeur
