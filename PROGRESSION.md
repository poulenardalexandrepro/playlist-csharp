# 📊 Ma progression — PlaylistApp

> Cochez les cases au fur et à mesure (`[ ]` → `[x]`), puis **committez** ce fichier.
> Vous pouvez aussi utiliser le **[tableau de bord interactif](https://ggaillard.github.io/playlist-csharp)** et son bouton « Exporter ».

**Progression globale : 0 / 29**

---

## 🚀 TP0 — Mise en place de l'environnement

- [ ] 🎓 Lire la fiche « Environnement de développement » + auto-évaluation
- [ ] Préparer l'environnement (Codespaces ou VS Code local)
- [ ] Lancer l'application (`cd PlaylistApp && dotnet run`)

## 📘 TP1 — Console & POO

- [ ] 🎓 Lire les fiches POO · Collections · LINQ + auto-évaluation
- [ ] 🟢 Ajouter une note (1-5) aux chansons
- [ ] 🟡 Trier une playlist par durée
- [ ] 🔴 Recherche par genre musical
- [ ] 🐳 Construire l'image Docker

## 📗 TP2 — Entity Framework Core

- [ ] 🎓 Lire les fiches ORM · Migrations · Relations + auto-évaluation
- [ ] Appliquer la migration (créer la base)
- [ ] Vérifier la persistance après redémarrage
- [ ] 🟢 Ajouter un champ « Label » (+ migration)
- [ ] 🟡 Méthode « chansons par genre »
- [ ] 🔴 Ajouter une entité Artiste (relation 1-N)
- [ ] ✅ Tests TP2 au vert (31 tests)
- [ ] 🐳 Lancer avec docker compose

## 📕 TP3 — API REST & architecture SOA

- [ ] 🎓 Lire les fiches REST/HTTP · SOA + auto-évaluation
- [ ] Lancer l'API et ouvrir Swagger
- [ ] Tester GET et POST dans Swagger
- [ ] 🟢 Endpoint GET /api/chansons/top/{n}
- [ ] 🟡 Validation à la création (400 si note invalide)
- [ ] 🔴 Contrôleur PlaylistsController complet
- [ ] ✅ Tests d'intégration au vert (8 tests)

## 🎏 TP4 — Architecture événementielle (EOA)

- [ ] 🎓 Lire la fiche EOA + auto-évaluation
- [ ] Observer un événement dans les logs (POST → [AUDIT])
- [ ] 🟢 Publier un événement à la suppression
- [ ] 🟡 Créer un HistoriqueHandler
- [ ] 🔴 Événement métier complet (NoteModifieeEvent)
- [ ] ✅ Tests EOA au vert (5 tests)

---

### Légende
🚀 mise en place · 🎓 concept + auto-évaluation · 🟢 guidé · 🟡 semi-guidé · 🔴 autonome · ✅ tests · 🐳 Docker
