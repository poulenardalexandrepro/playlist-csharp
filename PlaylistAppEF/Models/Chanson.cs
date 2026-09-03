using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaylistAppEF.Models;

/// <summary>
/// Entité Chanson mappée sur la table "Chansons" via Entity Framework Core.
/// Les annotations DataAnnotations configurent la base de données.
/// </summary>
[Table("Chansons")]                          // ← Nom de la table en BD
public class Chanson
{
    // ── Clé primaire ─────────────────────────────────────────────────────────
    [Key]                                    // ← Clé primaire (PK)
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // ← Auto-incrément
    public int Id { get; set; }

    // ── Propriétés obligatoires ───────────────────────────────────────────────
    [Required]
    [MaxLength(200)]
    public string Titre { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Artiste { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Album { get; set; } = string.Empty;

    // ── Propriétés optionnelles ───────────────────────────────────────────────
    [Range(1, 7200)]                         // ← Validation : 1s à 2h max
    public int DureeSecondes { get; set; }

    [MaxLength(50)]
    public string Genre { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int Annee { get; set; }

    [Range(1, 5)]
    public int Note { get; set; } = 3;       // ← Note de 1 à 5 (défaut : 3)

    public DateTime AjouteLe { get; set; } = DateTime.UtcNow;

    // ── Navigation EF Core ────────────────────────────────────────────────────
    // Une chanson peut apparaître dans plusieurs playlists (relation N-N)
    public ICollection<PlaylistChanson> PlaylistChansons { get; set; } = new List<PlaylistChanson>();

    // ── Méthodes utilitaires ──────────────────────────────────────────────────
    public string DureeFormatee()
    {
        int m = DureeSecondes / 60;
        int s = DureeSecondes % 60;
        return $"{m:D2}:{s:D2}";
    }

    public override string ToString()
        => $"[{Id:D3}] {Titre,-30} {Artiste,-20} | {Album,-25} ({Annee}) | {DureeFormatee()} | {Genre} | ★{Note}";
}
