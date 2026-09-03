using PlaylistApp.Models;

namespace PlaylistApp.Services;

/// <summary>
/// Service central : gère la bibliothèque de chansons et toutes les playlists.
/// Applique le pattern Repository simplifié.
/// </summary>
public class Bibliotheque
{
    // ── Stockage en mémoire ──────────────────────────────────────────────────
    private readonly Dictionary<int, Chanson>     _chansons     = new();
    private readonly Dictionary<int, Playlist> _playlists = new();
    private int _nextSongId     = 1;
    private int _nextPlaylistId = 1;

    // ── Constructeur : données de démonstration ──────────────────────────────
    public Bibliotheque()
    {
        ChargerDonnees();
    }

    // ════════════════════════ GESTION DES CHANSONS ═══════════════════════════

    public Chanson AjouterChanson(string title, string artist, string album,
                        int duration, string genre, int year)
    {
        var chanson = new Chanson(_nextSongId++, title, artist, album, duration, genre, year);
        _chansons[chanson.Id] = chanson;
        return chanson;
    }

    public Chanson? ObtenirChanson(int id)
        => _chansons.TryGetValue(id, out var s) ? s : null;

    public IEnumerable<Chanson> ObtenirToutesChansons()
        => _chansons.Values.OrderBy(s => s.Artiste).ThenBy(s => s.Titre);

    public IEnumerable<Chanson> SearchSongs(string query)
        => _chansons.Values.Where(s =>
            s.Titre.Contains(query, StringComparison.OrdinalIgnoreCase)  ||
            s.Artiste.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            s.Album.Contains(query, StringComparison.OrdinalIgnoreCase));

    public bool DeleteSong(int id) => _chansons.Remove(id);

    // ════════════════════════ GESTION DES PLAYLISTS ══════════════════════════

    public Playlist CreerPlaylist(string name, string description = "")
    {
        var pl = new Playlist(_nextPlaylistId++, name, description);
        _playlists[pl.Id] = pl;
        return pl;
    }

    public Playlist? GetPlaylist(int id)
        => _playlists.TryGetValue(id, out var pl) ? pl : null;

    public IEnumerable<Playlist> GetAllPlaylists()
        => _playlists.Values.OrderBy(p => p.Nom);

    public bool DeletePlaylist(int id) => _playlists.Remove(id);

    public void AddSongToPlaylist(int playlistId, int chansonId)
    {
        var playlist = GetPlaylist(playlistId)
            ?? throw new KeyNotFoundException($"Playlist #{playlistId} introuvable.");
        var chanson = ObtenirChanson(chansonId)
            ?? throw new KeyNotFoundException($"Chanson #{chansonId} introuvable.");
        playlist.AjouterChanson(chanson);
    }

    // ════════════════════════ STATISTIQUES ═══════════════════════════════════

    public void DisplayStats()
    {
        Console.WriteLine("\n📊 Statistiques de la bibliothèque");
        Console.WriteLine(new string('═', 40));
        Console.WriteLine($"  Chansons    : {_chansons.Count}");
        Console.WriteLine($"  Playlists   : {_playlists.Count}");

        if (_chansons.Count > 0)
        {
            var genres = _chansons.Values
                .GroupBy(s => s.Genre)
                .OrderByDescending(g => g.Count())
                .Take(5);

            Console.WriteLine("\n  Top genres :");
            foreach (var g in genres)
                Console.WriteLine($"    • {g.Key,-15} {g.Count()} titre(s)");

            var topArtist = _chansons.Values
                .GroupBy(s => s.Artiste)
                .OrderByDescending(g => g.Count())
                .First();
            Console.WriteLine($"\n  Artiste le + représenté : {topArtist.Key} ({topArtist.Count()} titres)");
        }
    }

    // ════════════════════════ DONNÉES DE DÉMO ════════════════════════════════

    private void ChargerDonnees()
    {
        // Chansons
        var s1  = AjouterChanson("Bohemian Rhapsody",       "Queen",         "A Night at the Opera",  354, "Rock",       1975);
        var s2  = AjouterChanson("Hotel California",         "Eagles",        "Hotel California",       391, "Rock",       1977);
        var s3  = AjouterChanson("Blinding Lights",          "The Weeknd",    "After Hours",            200, "Pop",        2019);
        var s4  = AjouterChanson("Shape of You",             "Ed Sheeran",    "÷ (Divide)",             234, "Pop",        2017);
        var s5  = AjouterChanson("Lose Yourself",            "Eminem",        "8 Mile Soundtrack",      326, "Rap",        2002);
        var s6  = AjouterChanson("God's Plan",               "Drake",         "Scorpion",               198, "Rap",        2018);
        var s7  = AjouterChanson("Smells Like Teen Spirit",  "Nirvana",       "Nevermind",              301, "Rock",       1991);
        var s8  = AjouterChanson("Rolling in the Deep",      "Adele",         "21",                     228, "Soul",       2010);
        var s9  = AjouterChanson("Billie Jean",              "Michael Jackson","Thriller",              294, "Pop",        1982);
        var s10 = AjouterChanson("Stayin' Alive",            "Bee Gees",      "Saturday Night Fever",  245, "Disco",      1977);
        var s11 = AjouterChanson("One More Time",            "Daft Punk",     "Discovery",              321, "Électro",    2000);
        var s12 = AjouterChanson("Get Lucky",                "Daft Punk",     "Random Access Memories", 369, "Électro",    2013);

        // Playlists de démo
        var rock = CreerPlaylist("Rock Classics", "Les incontournables du rock");
        rock.AjouterChanson(s1); rock.AjouterChanson(s2); rock.AjouterChanson(s7);

        var pop = CreerPlaylist("Pop Hits 2010-2020", "Les meilleures chansons pop");
        pop.AjouterChanson(s3); pop.AjouterChanson(s4); pop.AjouterChanson(s8); pop.AjouterChanson(s9);

        var electro = CreerPlaylist("Électro Vibes", "Pour danser toute la nuit");
        electro.AjouterChanson(s10); electro.AjouterChanson(s11); electro.AjouterChanson(s12);
    }
}
