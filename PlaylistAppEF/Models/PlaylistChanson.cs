using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaylistAppEF.Models;

/// <summary>
/// Table de jonction (entité d'association) entre Playlist et Chanson.
/// Implémente la relation Plusieurs-à-Plusieurs avec des données supplémentaires :
/// - Position : ordre de la chanson dans la playlist
/// - AjouteLe : date d'ajout à la playlist
/// </summary>
[Table("PlaylistChansons")]
public class PlaylistChanson
{
    // ── Clés étrangères (forment la clé primaire composite) ───────────────────
    [Required]
    public int PlaylistId { get; set; }      // FK → Playlists.Id

    [Required]
    public int ChansonId { get; set; }       // FK → Chansons.Id

    // ── Données propres à la relation ─────────────────────────────────────────
    public int      Position  { get; set; } = 0;         // Ordre dans la playlist
    public DateTime AjouteLe  { get; set; } = DateTime.UtcNow;

    // ── Propriétés de navigation ──────────────────────────────────────────────
    [ForeignKey(nameof(PlaylistId))]
    public Playlist? Playlist { get; set; }

    [ForeignKey(nameof(ChansonId))]
    public Chanson? Chanson   { get; set; }
}
