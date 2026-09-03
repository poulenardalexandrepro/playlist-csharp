# 🎵 PlaylistApp

<div align="center">

[![Ouvrir dans GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/ggaillard/playlist-csharp?quickstart=1)

**[📊 Tableau de bord](https://ggaillard.github.io/playlist-csharp)** · **[📘 Parcours](PARCOURS_TP.md)** · **[❓ Poser une question](../../issues/new?template=question.yml)**

</div>

---

## 🚀 Par où commencer ?

### 👉 Un seul point de départ : **[Ouvrir le Parcours pédagogique →](PARCOURS_TP.md)**

Tout est là, dans l'ordre : la **méthode**, la **mise en route**, les **4 TP** et les **productions à rendre**. Suivez-le de haut en bas, sans vous disperser.

## 🎯 À quoi sert ce dépôt ?

Ce dépôt est le **support de cours complet** pour apprendre C# et .NET 10 à travers le développement d'une application de gestion de playlists musicales. Il contient 4 TP progressifs, tous exécutables **directement dans votre navigateur** grâce à GitHub Codespaces — **aucune installation locale requise**.

### 🗺️ Votre parcours en un coup d'œil

```mermaid
flowchart LR
    TP0["🚀 TP0<br/>Mise en place<br/>(ça démarre)"] --> TP1["📘 TP1<br/>Console & POO<br/>(ça marche)"]
    TP1 --> TP2["📗 TP2<br/>+ base de données<br/>(ça se souvient)"]
    TP2 --> TP3["📕 TP3<br/>+ API REST / SOA<br/>(c'est exposé)"]
    TP3 --> TP4["🎏 TP4<br/>+ événements / EOA<br/>(ça évolue)"]
```

### ⏱️ Estimation du temps

| TP | Sujet | Durée estimée |
|---|---|---|
| 🚀 **TP0** | Mise en place de l'environnement | ~30 min |
| 📘 **TP1** | Console & POO | ~4 h |
| 📗 **TP2** | Entity Framework Core | ~6 h |
| 📕 **TP3** | API REST & SOA | ~4 h |
| 🎏 **TP4** | Architecture événementielle (EOA) | ~4 h |
| | **Total** | **~18 h 30** |

> ⏳ Estimations indicatives, à votre rythme : la lecture des fiches concepts + auto-évaluations est incluse. Le TP2 est le plus long (modélisation des données + migrations).

---

## 🚦 Mise en route

Commencez par le **[🚀 TP0 — Mettre en place l'environnement](TP0_GUIDE.md)** : faire tourner l'application en **Codespaces** ou en **local (VS Code)**. Procédure détaillée aussi dans le [Guide étudiant](GUIDE_ETUDIANT.md), parcours complet dans le [Parcours](PARCOURS_TP.md).

---

## 📁 Architecture du dépôt

```
playlist-csharp/
│
├── 🎯 docs/index.html         TABLEAU DE BORD de progression (publié sur GitHub Pages)
├── 📋 PROGRESSION.md          Checklist versionnée (à committer)
├── 🗺️  PARCOURS_TP.md          La méthode pédagogique + index des missions
├── 🎓 cours/                  Concepts de cours + auto-évaluations
├── 🚀 TP0_GUIDE.md            Mise en place de l'environnement (à faire en premier)
├── 📖 GUIDE_ETUDIANT.md       Mise en route détaillée + dépannage
│
├── 📘 PlaylistApp/            TP1 — Console & POO
│   ├── TP1_GUIDE.md               ← fiche du TP (objectifs, UML, missions)
│   ├── Models/  Services/  Program.cs  Dockerfile
│
├── 📗 PlaylistAppEF/          TP2 — Entity Framework Core + SQLite
│   ├── TP2_GUIDE.md
│   ├── Data/  Models/  Repositories/  Migrations/  Program.cs
├── 📗 PlaylistAppEF.Tests/        31 tests (xUnit)
│
├── 📕 PlaylistAppAPI/         TP3 & TP4 — API REST (SOA) puis Événements (EOA)
│   ├── TP3_GUIDE.md               ← TP3 : API REST & architecture SOA
│   ├── TP4_GUIDE.md               ← TP4 : architecture événementielle EOA
│   ├── Controllers/  Events/  Program.cs
├── 📕 PlaylistAppAPI.Tests/       13 tests (8 intégration + 5 EOA)
│
├── .devcontainer/            Environnement (Codespaces / VS Code local) — expliqué au TP0
├── .github/workflows/        CI/CD : build + tests + déploiement Pages
└── .gitpod.yml               Alternative Gitpod
```

> **Tout est dans le dépôt** : pas de document à télécharger. Les fiches de TP, la méthode, le suivi de progression et le code vivent ensemble et restent versionnés.


## 📦 Productions à rendre

Pour chaque TP, le rendu tient dans votre dépôt : **du code qui compile, des tests verts, des commits réguliers**.

| TP | À rendre | Preuve auto (badge CI) |
|---|---|---|
| 📘 TP1 | 3 missions committées · app console qui se lance | TP1 – Build ✅ |
| 📗 TP2 | migration appliquée · entité Artiste (1-N) | TP2 – Tests ✅ (31 tests) |
| 📕 TP3 | API + Swagger · endpoints testés | TP3 – API ✅ (8 tests) |
| 🎏 TP4 | 3 modifications EOA · événements dans les logs | 5 tests EOA ✅ |

Détail complet et critères : **[Parcours → Productions à rendre](PARCOURS_TP.md#-productions-à-rendre-par-tp)**.

---

## 📊 Suivre ma progression

Deux outils complémentaires, **entièrement dans le dépôt** :

| Outil | Usage | Où |
|---|---|---|
| 🎯 **Tableau de bord interactif** | Cocher mes missions, voir mes barres de progression, exporter | [Page GitHub Pages](https://ggaillard.github.io/playlist-csharp) |
| 📋 **PROGRESSION.md** | Checklist versionnée, visible par l'enseignant | [PROGRESSION.md](PROGRESSION.md) |

Le tableau de bord sauvegarde votre avancement dans le navigateur. Quand vous voulez le partager (rendu, point d'étape), cliquez « Exporter », collez le résultat dans `PROGRESSION.md`, et committez.


## 🔗 Ressources

| Ressource | Lien |
|---|---|
| Documentation complète | [GitHub Pages du cours](https://ggaillard.github.io/playlist-csharp) |
| Référence GitHub (sqlite-dotnet-core) | [jasonsturges/sqlite-dotnet-core](https://github.com/jasonsturges/sqlite-dotnet-core) |
| Documentation EF Core | [learn.microsoft.com/ef/core](https://learn.microsoft.com/fr-fr/ef/core/) |
| Documentation ASP.NET Core | [learn.microsoft.com/aspnet/core](https://learn.microsoft.com/fr-fr/aspnet/core/) |
| GitHub Codespaces | [github.com/codespaces](https://github.com/codespaces) |

---

## ❓ Besoin d'aide ?

Ouvrez une [issue avec le template "Question"](../../issues/new?template=question.yml) en précisant :
- Le TP concerné (TP1 / TP2 / TP3 / TP4)
- Le message d'erreur complet
- La commande que vous avez tapée

