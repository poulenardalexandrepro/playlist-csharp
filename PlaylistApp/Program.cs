using PlaylistApp.Models;
using PlaylistApp.Services;

// ══════════════════════════════════════════════════════════════════════════════
//  🎵  PlaylistApp – Application console de gestion de playlists musicales
//      C# / .NET 10  |  Cas pratique C# / .NET 10
// ══════════════════════════════════════════════════════════════════════════════

var library = new Bibliotheque();

ShowWelcome();
bool running = true;

while (running)
{
    ShowMainMenu();
    string choice = Console.ReadLine()?.Trim() ?? "";

    switch (choice)
    {
        // ── Bibliothèque ────────────────────────────────────────────────────
        case "1": ListAllSongs();       break;
        case "2": SearchSongs();        break;
        case "3": AddNewSong();         break;
        case "4": DeleteSong();         break;

        // ── Playlists ───────────────────────────────────────────────────────
        case "5": ListAllPlaylists();   break;
        case "6": ViewPlaylist();       break;
        case "7": CreerPlaylist();     break;
        case "8": AddSongToPlaylist();  break;
        case "9": RemoveSongFromPlaylist(); break;
        case "10": ShufflePlaylist();   break;

        // ── Statistiques / Quitter ──────────────────────────────────────────
        case "11": library.DisplayStats(); break;
        case "0":
            Console.WriteLine("\n👋  À bientôt !\n");
            running = false;
            break;

        default:
            Console.WriteLine("❌  Option invalide. Réessayez.");
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
//  Méthodes locales (Local Functions)
// ══════════════════════════════════════════════════════════════════════════════

void ShowWelcome()
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔══════════════════════════════════════════════╗");
    Console.WriteLine("║        🎵  PlaylistApp  🎵                  ║");
    Console.WriteLine("║   Gestion de playlists musicales en C#       ║");
    Console.WriteLine("╚══════════════════════════════════════════════╝");
    Console.ResetColor();
}

void ShowMainMenu()
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\n══ MENU PRINCIPAL ══════════════════════════════");
    Console.ResetColor();
    Console.WriteLine("  📚 Bibliothèque");
    Console.WriteLine("   1. Lister toutes les chansons");
    Console.WriteLine("   2. Rechercher une chanson");
    Console.WriteLine("   3. Ajouter une chanson");
    Console.WriteLine("   4. Supprimer une chanson");
    Console.WriteLine("\n  🎵 Playlists");
    Console.WriteLine("   5. Lister toutes les playlists");
    Console.WriteLine("   6. Afficher une playlist");
    Console.WriteLine("   7. Créer une playlist");
    Console.WriteLine("   8. Ajouter une chanson à une playlist");
    Console.WriteLine("   9. Retirer une chanson d'une playlist");
    Console.WriteLine("  10. Mélanger une playlist (shuffle)");
    Console.WriteLine("\n  📊 Autre");
    Console.WriteLine("  11. Statistiques");
    Console.WriteLine("   0. Quitter");
    Console.Write("\n▶  Votre choix : ");
}

void ListAllSongs()
{
    Console.WriteLine("\n📚 Bibliothèque – Toutes les chansons :");
    Console.WriteLine(new string('─', 70));
    var chansons = library.ObtenirToutesChansons().ToList();
    if (chansons.Count == 0) { Console.WriteLine("  (aucune chanson)"); return; }
    foreach (var s in chansons) Console.WriteLine($"  {s}");
    Console.WriteLine($"\n  Total : {chansons.Count} chanson(s)");
}

void SearchSongs()
{
    Console.Write("\n🔍 Terme de recherche (titre / artiste / album) : ");
    string query = Console.ReadLine() ?? "";
    var results = library.SearchSongs(query).ToList();
    Console.WriteLine($"\n  {results.Count} résultat(s) pour « {query} » :");
    Console.WriteLine(new string('─', 70));
    if (results.Count == 0) { Console.WriteLine("  Aucun résultat."); return; }
    foreach (var s in results) Console.WriteLine($"  {s}");
}

void AddNewSong()
{
    Console.WriteLine("\n➕ Ajouter une nouvelle chanson");
    Console.WriteLine(new string('─', 40));
    Console.Write("  Titre   : "); string title   = Console.ReadLine() ?? "Inconnu";
    Console.Write("  Artiste : "); string artist  = Console.ReadLine() ?? "Inconnu";
    Console.Write("  Album   : "); string album   = Console.ReadLine() ?? "Inconnu";
    Console.Write("  Genre   : "); string genre   = Console.ReadLine() ?? "Autre";
    Console.Write("  Année   : "); int.TryParse(Console.ReadLine(), out int year);
    Console.Write("  Durée (secondes) : "); int.TryParse(Console.ReadLine(), out int dur);

    var chanson = library.AjouterChanson(title, artist, album, dur, genre, year);
    Console.WriteLine($"\n  ✅  Chanson ajoutée avec l'ID #{chanson.Id}");
}

void DeleteSong()
{
    Console.Write("\n🗑️  ID de la chanson à supprimer : ");
    if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("ID invalide."); return; }
    bool ok = library.DeleteSong(id);
    Console.WriteLine(ok ? "  ✅  Supprimée." : "  ❌  Chanson introuvable.");
}

void ListAllPlaylists()
{
    Console.WriteLine("\n🎵 Toutes les playlists :");
    Console.WriteLine(new string('─', 50));
    var pls = library.GetAllPlaylists().ToList();
    if (pls.Count == 0) { Console.WriteLine("  (aucune playlist)"); return; }
    foreach (var p in pls)
        Console.WriteLine($"  #{p.Id} – {p.Nom,-25} {p.Chansons.Count,3} titre(s) – {p.DureeTotaleFormatee()}");
}

void ViewPlaylist()
{
    Console.Write("\n📋 ID de la playlist à afficher : ");
    if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("ID invalide."); return; }
    var pl = library.GetPlaylist(id);
    if (pl is null) { Console.WriteLine("  ❌  Playlist introuvable."); return; }
    pl.Display();
}

void CreerPlaylist()
{
    Console.Write("\n✨ Nom de la nouvelle playlist : ");
    string name = Console.ReadLine() ?? "Ma Playlist";
    Console.Write("   Description (optionnel) : ");
    string desc = Console.ReadLine() ?? "";
    var pl = library.CreerPlaylist(name, desc);
    Console.WriteLine($"\n  ✅  Playlist « {pl.Nom} » créée (ID #{pl.Id})");
}

void AddSongToPlaylist()
{
    Console.Write("\n🎵 ID de la playlist : ");
    if (!int.TryParse(Console.ReadLine(), out int plId)) { Console.WriteLine("ID invalide."); return; }
    Console.Write("   ID de la chanson  : ");
    if (!int.TryParse(Console.ReadLine(), out int sId))  { Console.WriteLine("ID invalide."); return; }
    try
    {
        library.AddSongToPlaylist(plId, sId);
        Console.WriteLine("  ✅  Chanson ajoutée à la playlist.");
    }
    catch (Exception ex) { Console.WriteLine($"  ❌  {ex.Message}"); }
}

void RemoveSongFromPlaylist()
{
    Console.Write("\n🗑️  ID de la playlist : ");
    if (!int.TryParse(Console.ReadLine(), out int plId)) { Console.WriteLine("ID invalide."); return; }
    var pl = library.GetPlaylist(plId);
    if (pl is null) { Console.WriteLine("  ❌  Playlist introuvable."); return; }
    Console.Write("   ID de la chanson à retirer : ");
    if (!int.TryParse(Console.ReadLine(), out int sId))  { Console.WriteLine("ID invalide."); return; }
    bool ok = pl.RetirerChanson(sId);
    Console.WriteLine(ok ? "  ✅  Chanson retirée." : "  ❌  Chanson introuvable dans cette playlist.");
}

void ShufflePlaylist()
{
    Console.Write("\n🔀 ID de la playlist à mélanger : ");
    if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("ID invalide."); return; }
    var pl = library.GetPlaylist(id);
    if (pl is null) { Console.WriteLine("  ❌  Playlist introuvable."); return; }
    pl.Melanger();
    Console.WriteLine($"  ✅  Playlist « {pl.Nom} » mélangée !");
    pl.Display();
}
