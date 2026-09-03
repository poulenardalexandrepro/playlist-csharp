namespace PlaylistApp.Models;

/// <summary>
/// Représente une chanson dans la bibliothèque musicale.
/// </summary>
public class Chanson
{
    // ── Propriétés ──────────────────────────────────────────────────────────
    public int    Id       { get; set; }
    public string Titre    { get; set; } = string.Empty;
    public string Artiste   { get; set; } = string.Empty;
    public string Album    { get; set; } = string.Empty;
    public int    DureeSecondes { get; set; }          // durée en secondes
    public string Genre    { get; set; } = string.Empty;
    public int    Annee     { get; set; }

    // ── Constructeur ────────────────────────────────────────────────────────
    public Chanson() { }

    public Chanson(int id, string title, string artist, string album,
                int duration, string genre, int year)
    {
        Id       = id;
        Titre    = title;
        Artiste   = artist;
        Album    = album;
        DureeSecondes = duration;
        Genre    = genre;
        Annee     = year;
    }

    // ── Méthodes ────────────────────────────────────────────────────────────
    /// <summary>Durée formatée mm:ss</summary>
    public string DureeFormatee()
    {
        int minutes = DureeSecondes / 60;
        int seconds = DureeSecondes % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }

    public override string ToString()
        => $"[{Id:D3}] {Titre} – {Artiste} | {Album} ({Annee}) | {DureeFormatee()} | {Genre}";
}
