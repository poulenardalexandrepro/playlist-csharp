using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaylistAppEF.Models;

/// <summary>
/// Entité Playlist mappée sur la table "Playlists".
/// Relation N-N avec Chanson via la table de jonction PlaylistChanson.
/// </summary>
[Table("Playlists")]
public class Playlist
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreeLe { get; set; } = DateTime.UtcNow;
    public DateTime ModifieLe { get; set; } = DateTime.UtcNow;

    // ── Navigation EF Core ────────────────────────────────────────────────────
    // Une playlist contient plusieurs chansons via la table de jonction
    public ICollection<PlaylistChanson> PlaylistChansons { get; set; } = new List<PlaylistChanson>();

    // ── Propriétés calculées (non mappées en BD) ──────────────────────────────
    [NotMapped]
    public int DureeTotal => PlaylistChansons.Sum(pc => pc.Chanson?.DureeSecondes ?? 0);

    public string DureeTotaleFormatee()
    {
        int h = DureeTotal / 3600;
        int m = (DureeTotal % 3600) / 60;
        int s = DureeTotal % 60;
        return h > 0 ? $"{h}h {m:D2}min {s:D2}s" : $"{m:D2}min {s:D2}s";
    }

    [NotMapped]
    public int NombreChansons => PlaylistChansons.Count;
}
