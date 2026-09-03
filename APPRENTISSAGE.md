# 🧭 Guide d'apprentissage — Comment réussir ce parcours

> Ce guide explique **comment apprendre** avec ce dépôt : la logique pédagogique, ce qu'il faut savoir avant, comment s'auto-évaluer, et les pièges classiques.
> Stack à jour : **.NET 10 LTS · C# 14 · EF Core 10 · ASP.NET Core 10**.

---

## 1. La philosophie : apprendre en modifiant, pas en recopiant

Vous n'écrivez jamais à partir d'une page blanche. À chaque étape :

```
   LANCER ──▶ COMPRENDRE ──▶ MODIFIER ──▶ VALIDER ──▶ (TP suivant)
  l'exemple    le code par     par paliers   par les
  qui marche   la lecture      🟢🟡🔴         tests
```

C'est la façon dont on apprend en entreprise : on rejoint un projet **existant**, on le lit, on le fait évoluer. Recopier un corrigé n'apprend rien ; **modifier en comprenant** ancre durablement.

---

## 2. Ce que vous devez savoir avant de commencer

| Niveau | Attendu | Si ce n'est pas acquis |
|---|---|---|
| Indispensable | Lire un peu d'anglais technique | Utilisez un traducteur sur les messages d'erreur |
| Indispensable | Notion de variable, boucle, condition | Révisez les bases d'un langage (Python, JS…) |
| Utile | Avoir déjà vu une base de données | Le TP2 vous l'expliquera depuis le début |
| Non requis | Connaître C# | Le TP1 part de zéro |
| Non requis | Installer quoi que ce soit | Tout tourne dans GitHub Codespaces |

---

## 3. Objectifs d'apprentissage par niveau cognitif

On ne vous demande pas seulement de « savoir », mais de **monter en autonomie**. Voici la progression visée (taxonomie de Bloom) :

| Niveau | Verbe | Exemple concret dans le parcours |
|---|---|---|
| 1. Connaître | Identifier | Reconnaître une classe, une propriété, une méthode |
| 2. Comprendre | Expliquer | Dire ce que fait un ORM, à quoi sert une migration |
| 3. Appliquer | Utiliser | Lancer une migration, écrire une requête LINQ |
| 4. Analyser | Distinguer | Comparer SOA et EOA, repérer une erreur dans un test |
| 5. Évaluer | Justifier | Choisir le bon code de statut HTTP, valider par les tests |
| 6. Créer | Concevoir | Ajouter une entité, un endpoint, un événement (paliers 🔴) |

Les **paliers de modification** suivent cette montée : 🟢 guidé (appliquer) → 🟡 semi-guidé (analyser) → 🔴 autonome (créer).

---

## 4. Comment s'auto-évaluer

À la fin de chaque TP, posez-vous ces questions. Si vous répondez « oui » sans hésiter, c'est acquis.

### Après le TP1
- [ ] Je peux expliquer la différence entre `List` et `Dictionary`
- [ ] Je sais écrire une requête LINQ simple (`Where`, `OrderBy`)
- [ ] Je comprends ce qu'est l'encapsulation (champ privé / propriété publique)

### Après le TP2
- [ ] Je peux expliquer ce que fait Entity Framework Core (le mot « ORM »)
- [ ] Je sais créer et appliquer une migration
- [ ] Je comprends pourquoi on ne modifie jamais la base à la main
- [ ] Je sais ce qu'est une relation N-N (table de liaison)

### Après le TP3 (API REST & SOA)
- [ ] Je peux décrire le rôle de chaque verbe HTTP (GET, POST, PUT, DELETE)
- [ ] Je sais lire un code de statut (2xx / 4xx / 5xx)
- [ ] Je comprends l'architecture en couches (Controller → Repository → BD)
- [ ] Je comprends pourquoi un test d'intégration complète les tests unitaires

### Après le TP4 (architecture événementielle)
- [ ] Je peux expliquer le problème du couplage fort
- [ ] Je sais décrire le patron publish/subscribe (émetteur, bus, abonnés)
- [ ] Je sais pourquoi l'émetteur ne connaît pas ses abonnés
- [ ] Je sais quand préférer SOA et quand préférer EOA

> Le **[tableau de bord de progression](https://ggaillard.github.io/playlist-csharp)** suit vos missions ; cette liste-ci suit votre **compréhension**. Les deux comptent.

---

## 5. Erreurs et idées fausses fréquentes

| Idée fausse | Réalité |
|---|---|
| « Si ça compile, ça marche » | Faux. Un test d'intégration peut révéler une erreur invisible à la compilation (vu en TP3). |
| « Je modifie la base directement » | Non : on modifie le modèle C#, puis on génère une migration. |
| « `DateTime.UtcNow` comme valeur par défaut de seed » | Piège : crée des migrations « fantômes ». On utilise une date fixe. |
| « async/await rend le code plus rapide » | Non : ça libère le thread pendant l'attente (réseau, disque), ça ne calcule pas plus vite. |
| « Un seul gros commit à la fin » | Mauvaise pratique : committez petit et souvent, avec des messages clairs. |

---

## 6. Méthode face à une erreur

1. **Lire le message en entier** — il indique souvent le fichier et la ligne.
2. **Repérer le type** : erreur de compilation (`CS....`) ? d'exécution ? de test (`Assert`) ?
3. **Reproduire** : la plus petite action qui déclenche l'erreur.
4. **Chercher** : copier le message d'erreur dans un moteur de recherche.
5. **Demander** : ouvrir une *issue* (modèle « Question ») avec le message complet.

> Une erreur n'est pas un échec : c'est l'occasion d'apprendre. Les développeurs passent une grande partie de leur temps à diagnostiquer.

---

## 7. Ce qui est « à jour » dans ce parcours

Le code utilise les technologies **actuelles**, pas des versions dépassées :

| Techno | Version | Nouveauté exploitée |
|---|---|---|
| .NET | **10 LTS** (nov. 2025) | Support long terme jusqu'en 2028 |
| C# | **14** | Constructeurs primaires, expressions de collection |
| EF Core | **10** | ORM, migrations, requêtes LINQ |
| ASP.NET Core | **10** | API minimale, Swagger/OpenAPI |
| Conteneurs | Docker multi-stage | Images officielles `dotnet:10.0` |

> Exemple de modernité : le contrôleur de l'API utilise un **constructeur primaire** (C# 12+), plus concis que l'ancienne syntaxe :
> ```csharp
> // Avant (verbeux)
> public ChansonsController(PlaylistContext ctx) { _ctx = ctx; }
> // Maintenant (C# 14)
> public class ChansonsController(PlaylistContext ctx) : ControllerBase
> ```

---

## 8. Glossaire express

| Terme | Définition simple |
|---|---|
| **POO** | Programmation Orientée Objet : organiser le code en classes (objets) |
| **ORM** | Object-Relational Mapping : relie les classes C# aux tables SQL |
| **DbContext** | La « porte d'entrée » vers la base de données dans EF Core |
| **Migration** | Script versionné décrivant un changement de structure de la base |
| **Seed** | Données initiales insérées dans la base au départ |
| **API REST** | Interface web exposant des ressources via des URLs et des verbes HTTP |
| **SOA** | Architecture en couches qui se répondent (synchrone) |
| **EOA / EDA** | Architecture par événements : on publie, des handlers réagissent (découplé) |
| **CI/CD** | Intégration / déploiement continus : tests et build automatiques à chaque push |
| **DevContainer** | Environnement de dev reproductible défini dans le dépôt |

---

## 9. Par où commencer

1. Lisez le **[PARCOURS_TP.md](PARCOURS_TP.md)** (la méthode + l'index des missions).
2. Ouvrez votre **[tableau de bord de progression](https://ggaillard.github.io/playlist-csharp)**.
3. Attaquez le **[TP1](PlaylistApp/TP1_GUIDE.md)**.

Bon parcours — et rappelez-vous : on apprend en **modifiant**, pas en recopiant.
