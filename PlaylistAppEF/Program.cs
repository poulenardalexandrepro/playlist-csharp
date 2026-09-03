using Microsoft.EntityFrameworkCore;
using PlaylistAppEF.Data;
using PlaylistAppEF.Models;
using PlaylistAppEF.Repositories;

// ══════════════════════════════════════════════════════════════════════════════
//  🎵  PlaylistApp EF – TP2 : Entity Framework Core + SQLite
//      C# / .NET 10  |  Cas pratique PlaylistApp
//      Référence GitHub : https://github.com/jasonsturges/sqlite-dotnet-core
// ══════════════════════════════════════════════════════════════════════════════

// ── Initialisation du contexte et des migrations ──────────────────────────────
using var context = new PlaylistContext();
var repo = new MusiqueRepository(context);

Console.WriteLine("⏳ Initialisation de la base de données SQLite...");
await context.Database.MigrateAsync();      // Applique les migrations (crée la BD si besoin)
Console.WriteLine("✅ Base de données prête.\n");

AfficherBienvenue();
bool running = true;

while (running)
{
    AfficherMenu();
    string choix = Console.ReadLine()?.Trim() ?? "";

    switch (choix)
    {
        // ── Chansons ─────────────────────────────────────────────────────────
        case "1": await ListerChansonsAsync();       break;
        case "2": await RechercherChansonAsync();    break;
        case "3": await AjouterChansonAsync();       break;
        case "4": await ModifierNoteAsync();         break;
        case "5": await SupprimerChansonAsync();     break;
        case "6": await TopChansonsAsync();          break;

        // ── Playlists ─────────────────────────────────────────────────────────
        case "7":  await ListerPlaylistsAsync();            break;
        case "8":  await VoirPlaylistAsync();               break;
        case "9":  await CreerPlaylistAsync();              break;
        case "10": await AjouterChansonPlaylistAsync();     break;
        case "11": await RetirerChansonPlaylistAsync();     break;
        case "12": await SupprimerPlaylistAsync();          break;

        // ── Statistiques / EF ─────────────────────────────────────────────────
        case "13": await repo.AfficherStatistiquesAsync();  break;
        case "14": AfficherInfoEF();                        break;

        case "0":
            Console.WriteLine("\n👋  À bientôt !\n");
            running = false;
            break;

        default:
            Console.WriteLine("❌  Option invalide.");
            break;
    }

    if (running)
    {
        Console.WriteLine("\nAppuyez sur [Entrée] pour continuer...");
        Console.ReadLine();
        Console.Clear();
    }
}

// ══════════════════════════════════════════════════════════════════════════════
//  Fonctions locales
// ══════════════════════════════════════════════════════════════════════════════

void AfficherBienvenue()
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("╔════════════════════════════════════════════════════╗");
    Console.WriteLine("║   🎵  PlaylistApp EF  –  TP2  🎵                 ║");
    Console.WriteLine("║   Entity Framework Core + SQLite dans Docker       ║");
    Console.WriteLine("╚════════════════════════════════════════════════════╝");
    Console.ResetColor();
}

void AfficherMenu()
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\n══ MENU PRINCIPAL ═══════════════════════════════════");
    Console.ResetColor();
    Console.WriteLine("  🎵 Gestion des chansons");
    Console.WriteLine("   1. Lister toutes les chansons");
    Console.WriteLine("   2. Rechercher une chanson");
    Console.WriteLine("   3. Ajouter une chanson");
    Console.WriteLine("   4. Modifier la note d'une chanson");
    Console.WriteLine("   5. Supprimer une chanson");
    Console.WriteLine("   6. Top 5 chansons les mieux notées");
    Console.WriteLine("\n  📋 Gestion des playlists");
    Console.WriteLine("   7. Lister toutes les playlists");
    Console.WriteLine("   8. Afficher une playlist");
    Console.WriteLine("   9. Créer une playlist");
    Console.WriteLine("  10. Ajouter une chanson à une playlist");
    Console.WriteLine("  11. Retirer une chanson d'une playlist");
    Console.WriteLine("  12. Supprimer une playlist");
    Console.WriteLine("\n  📊 Analyse");
    Console.WriteLine("  13. Statistiques");
    Console.WriteLine("  14. Infos Entity Framework");
    Console.WriteLine("   0. Quitter");
    Console.Write("\n▶  Votre choix : ");
}

async Task ListerChansonsAsync()
{
    var chansons = await repo.ObtenirToutesChansonsAsync();
    Console.WriteLine($"\n📚 Bibliothèque – {chansons.Count} chanson(s) :");
    Console.WriteLine(new string('─', 90));
    if (chansons.Count == 0) { Console.WriteLine("  (vide)"); return; }
    foreach (var c in chansons) Console.WriteLine($"  {c}");
}

async Task RechercherChansonAsync()
{
    Console.Write("\n🔍 Terme (titre / artiste / album) : ");
    string terme = Console.ReadLine() ?? "";
    var res = await repo.RechercherChansonsAsync(terme);
    Console.WriteLine($"\n  {res.Count} résultat(s) pour « {terme} » :");
    Console.WriteLine(new string('─', 90));
    if (res.Count == 0) { Console.WriteLine("  Aucun résultat."); return; }
    foreach (var c in res) Console.WriteLine($"  {c}");
}

async Task AjouterChansonAsync()
{
    Console.WriteLine("\n➕ Nouvelle chanson");
    Console.Write("  Titre   : "); string titre   = Console.ReadLine() ?? "";
    Console.Write("  Artiste : "); string artiste = Console.ReadLine() ?? "";
    Console.Write("  Album   : "); string album   = Console.ReadLine() ?? "";
    Console.Write("  Genre   : "); string genre   = Console.ReadLine() ?? "";
    Console.Write("  Année   : "); int.TryParse(Console.ReadLine(), out int annee);
    Console.Write("  Durée (s): "); int.TryParse(Console.ReadLine(), out int dur);
    Console.Write("  Note (1-5): "); int.TryParse(Console.ReadLine(), out int note);

    var c = await repo.AjouterChansonAsync(new Chanson
    {
        Titre = titre, Artiste = artiste, Album = album,
        Genre = genre, Annee = annee, DureeSecondes = dur,
        Note = Math.Clamp(note, 1, 5)
    });
    Console.WriteLine($"\n  ✅  Chanson ajoutée (ID #{c.Id})");
}

async Task ModifierNoteAsync()
{
    Console.Write("\n⭐ ID de la chanson : "); int.TryParse(Console.ReadLine(), out int id);
    Console.Write("   Nouvelle note (1-5) : "); int.TryParse(Console.ReadLine(), out int note);
    bool ok = await repo.ModifierNoteAsync(id, Math.Clamp(note, 1, 5));
    Console.WriteLine(ok ? "  ✅  Note mise à jour." : "  ❌  Chanson introuvable.");
}

async Task SupprimerChansonAsync()
{
    Console.Write("\n🗑️  ID de la chanson à supprimer : "); int.TryParse(Console.ReadLine(), out int id);
    try
    {
        bool ok = await repo.SupprimerChansonAsync(id);
        Console.WriteLine(ok ? "  ✅  Supprimée." : "  ❌  Introuvable.");
    }
    catch (DbUpdateException) { Console.WriteLine("  ❌  Cette chanson est dans une playlist. Retirez-la d'abord."); }
}

async Task TopChansonsAsync()
{
    var top = await repo.TopChansonsAsync(5);
    Console.WriteLine("\n🏆 Top 5 chansons les mieux notées :");
    Console.WriteLine(new string('─', 90));
    for (int i = 0; i < top.Count; i++)
        Console.WriteLine($"  {i+1}. {top[i]}");
}

async Task ListerPlaylistsAsync()
{
    var pls = await repo.ObtenirToutesPlaylistsAsync();
    Console.WriteLine($"\n🎵 {pls.Count} playlist(s) :");
    Console.WriteLine(new string('─', 60));
    foreach (var p in pls)
        Console.WriteLine($"  #{p.Id,-3} {p.Nom,-25} {p.NombreChansons,3} chanson(s) – {p.DureeTotaleFormatee()}");
}

async Task VoirPlaylistAsync()
{
    Console.Write("\n📋 ID de la playlist : "); int.TryParse(Console.ReadLine(), out int id);
    var pl = await repo.ObtenirPlaylistAsync(id);
    if (pl is null) { Console.WriteLine("  ❌  Introuvable."); return; }

    Console.WriteLine($"\n🎵 {pl.Nom}");
    Console.WriteLine($"   {pl.Description}");
    Console.WriteLine($"   {pl.NombreChansons} chanson(s) – {pl.DureeTotaleFormatee()}");
    Console.WriteLine(new string('─', 90));
    foreach (var pc in pl.PlaylistChansons.OrderBy(x => x.Position))
        Console.WriteLine($"  {pc.Position,2}. {pc.Chanson}");
}

async Task CreerPlaylistAsync()
{
    Console.Write("\n✨ Nom : "); string nom = Console.ReadLine() ?? "";
    Console.Write("   Description : "); string desc = Console.ReadLine() ?? "";
    var pl = await repo.CreerPlaylistAsync(nom, desc);
    Console.WriteLine($"\n  ✅  Playlist « {pl.Nom} » créée (ID #{pl.Id})");
}

async Task AjouterChansonPlaylistAsync()
{
    Console.Write("\n🎵 ID playlist : "); int.TryParse(Console.ReadLine(), out int plId);
    Console.Write("   ID chanson  : "); int.TryParse(Console.ReadLine(), out int cId);
    try { await repo.AjouterChansonPlaylistAsync(plId, cId); Console.WriteLine("  ✅  Ajoutée."); }
    catch (Exception ex) { Console.WriteLine($"  ❌  {ex.Message}"); }
}

async Task RetirerChansonPlaylistAsync()
{
    Console.Write("\n🗑️  ID playlist : "); int.TryParse(Console.ReadLine(), out int plId);
    Console.Write("    ID chanson  : "); int.TryParse(Console.ReadLine(), out int cId);
    bool ok = await repo.RetirerChansonPlaylistAsync(plId, cId);
    Console.WriteLine(ok ? "  ✅  Retirée." : "  ❌  Introuvable.");
}

async Task SupprimerPlaylistAsync()
{
    Console.Write("\n🗑️  ID playlist à supprimer : "); int.TryParse(Console.ReadLine(), out int id);
    bool ok = await repo.SupprimerPlaylistAsync(id);
    Console.WriteLine(ok ? "  ✅  Playlist supprimée (liaisons supprimées par CASCADE)." : "  ❌  Introuvable.");
}

void AfficherInfoEF()
{
    Console.WriteLine("\n📚 Entity Framework Core – Concepts clés");
    Console.WriteLine(new string('─', 55));
    Console.WriteLine("  DbContext     → Point d'entrée vers la BD (PlaylistContext)");
    Console.WriteLine("  DbSet<T>      → Table en BD (Chansons, Playlists...)");
    Console.WriteLine("  Migration     → Script SQL versionné (dotnet ef migrations add)");
    Console.WriteLine("  Eager Loading → Chargement des relations (.Include().ThenInclude())");
    Console.WriteLine("  LINQ to SQL   → Requêtes C# traduites en SQL par EF Core");
    Console.WriteLine("  Seed Data     → Données initiales dans OnModelCreating()");
    Console.WriteLine("  Cascade       → Suppression automatique des liaisons");
    Console.WriteLine("\n  Commandes utiles :");
    Console.WriteLine("  dotnet ef migrations add NomMigration");
    Console.WriteLine("  dotnet ef database update");
    Console.WriteLine("  dotnet ef migrations list");
}
