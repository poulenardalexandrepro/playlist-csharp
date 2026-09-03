namespace PlaylistApp.Models;

/// <summary>
/// Représente une playlist contenant une collection ordonnée de chansons.
/// </summary>
public class Playlist
{
    // ── Propriétés ──────────────────────────────────────────────────────────
    public int          Id          { get; set; }
    public string       Nom        { get; set; } = string.Empty;
    public string       Description { get; set; } = string.Empty;
    public DateTime     CreeLe   { get; private set; }
    private List<Chanson>  _chansons      = new();

    // Accès en lecture seule à la liste (encapsulation)
    public IReadOnlyList<Chanson> Chansons => _chansons.AsReadOnly();

    // ── Propriété calculée ──────────────────────────────────────────────────
    /// <summary>Durée totale de la playlist en secondes</summary>
    public int DureeTotale => _chansons.Sum(s => s.DureeSecondes);

    public string DureeTotaleFormatee()
    {
        int h = DureeTotale / 3600;
        int m = (DureeTotale % 3600) / 60;
        int s = DureeTotale % 60;
        return h > 0
            ? $"{h}h {m:D2}min {s:D2}s"
            : $"{m:D2}min {s:D2}s";
    }

    // ── Constructeur ────────────────────────────────────────────────────────
    public Playlist() => CreeLe = DateTime.Now;

    public Playlist(int id, string name, string description = "") : this()
    {
        Id          = id;
        Nom        = name;
        Description = description;
    }

    // ── Méthodes CRUD ───────────────────────────────────────────────────────
    public void AjouterChanson(Chanson chanson)
    {
        if (_chansons.Any(s => s.Id == chanson.Id))
            throw new InvalidOperationException($"La chanson '{chanson.Titre}' est déjà dans la playlist.");
        _chansons.Add(chanson);
    }

    public bool RetirerChanson(int chansonId)
    {
        var chanson = _chansons.FirstOrDefault(s => s.Id == chansonId);
        if (chanson is null) return false;
        _chansons.Remove(chanson);
        return true;
    }

    public void MonterChanson(int chansonId)
    {
        int idx = _chansons.FindIndex(s => s.Id == chansonId);
        if (idx > 0)
            (_chansons[idx], _chansons[idx - 1]) = (_chansons[idx - 1], _chansons[idx]);
    }

    public void DescendreChanson(int chansonId)
    {
        int idx = _chansons.FindIndex(s => s.Id == chansonId);
        if (idx >= 0 && idx < _chansons.Count - 1)
            (_chansons[idx], _chansons[idx + 1]) = (_chansons[idx + 1], _chansons[idx]);
    }

    public void Melanger()
    {
        var rng = new Random();
        for (int i = _chansons.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (_chansons[i], _chansons[j]) = (_chansons[j], _chansons[i]);
        }
    }

    // ── Recherche ───────────────────────────────────────────────────────────
    public IEnumerable<Chanson> RechercherParArtiste(string artist)
        => _chansons.Where(s => s.Artiste.Contains(artist, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Chanson> RechercherParGenre(string genre)
        => _chansons.Where(s => s.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));

    // ── Affichage ───────────────────────────────────────────────────────────
    public void Display()
    {
        Console.WriteLine($"\n🎵 Playlist : {Nom}");
        Console.WriteLine($"   {Description}");
        Console.WriteLine($"   Créée le : {CreeLe:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"   {_chansons.Count} titre(s) – Durée totale : {DureeTotaleFormatee()}");
        Console.WriteLine(new string('─', 70));

        if (_chansons.Count == 0)
        {
            Console.WriteLine("   (playlist vide)");
            return;
        }

        for (int i = 0; i < _chansons.Count; i++)
            Console.WriteLine($"  {i + 1,2}. {_chansons[i]}");
    }
}
