using Microsoft.EntityFrameworkCore;
using PlaylistAppEF.Data;
using PlaylistAppEF.Models;

namespace PlaylistAppEF.Repositories;

/// <summary>
/// Dépôt (Repository) – couche d'accès aux données via Entity Framework Core.
/// Toutes les opérations CRUD passent par le DbContext (PlaylistContext).
/// </summary>
public class MusiqueRepository
{
    private readonly PlaylistContext _ctx;

    public MusiqueRepository(PlaylistContext ctx) => _ctx = ctx;

    // ════════════════════════ CHANSONS ════════════════════════════════════════

    /// <summary>Toutes les chansons, triées par artiste puis titre.</summary>
    public async Task<List<Chanson>> ObtenirToutesChansonsAsync()
        => await _ctx.Chansons
            .OrderBy(c => c.Artiste)
            .ThenBy(c => c.Titre)
            .ToListAsync();

    /// <summary>Recherche textuelle multi-champs.</summary>
    public async Task<List<Chanson>> RechercherChansonsAsync(string terme)
        => await _ctx.Chansons
            .Where(c => EF.Functions.Like(c.Titre,   $"%{terme}%") ||
                        EF.Functions.Like(c.Artiste, $"%{terme}%") ||
                        EF.Functions.Like(c.Album,   $"%{terme}%"))
            .OrderBy(c => c.Artiste)
            .ToListAsync();

    /// <summary>Chansons filtrées par genre.</summary>
    public async Task<List<Chanson>> ObtenirParGenreAsync(string genre)
        => await _ctx.Chansons
            .Where(c => c.Genre == genre)
            .OrderByDescending(c => c.Note)
            .ToListAsync();

    /// <summary>Top N chansons les mieux notées.</summary>
    public async Task<List<Chanson>> TopChansonsAsync(int n = 5)
        => await _ctx.Chansons
            .OrderByDescending(c => c.Note)
            .ThenBy(c => c.Titre)
            .Take(n)
            .ToListAsync();

    /// <summary>Ajouter une nouvelle chanson.</summary>
    public async Task<Chanson> AjouterChansonAsync(Chanson chanson)
    {
        _ctx.Chansons.Add(chanson);
        await _ctx.SaveChangesAsync();
        return chanson;
    }

    /// <summary>Mettre à jour la note d'une chanson.</summary>
    public async Task<bool> ModifierNoteAsync(int id, int note)
    {
        var chanson = await _ctx.Chansons.FindAsync(id);
        if (chanson is null) return false;
        chanson.Note = note;
        await _ctx.SaveChangesAsync();
        return true;
    }

    /// <summary>Supprimer une chanson (échoue si elle est dans une playlist).</summary>
    public async Task<bool> SupprimerChansonAsync(int id)
    {
        var chanson = await _ctx.Chansons.FindAsync(id);
        if (chanson is null) return false;
        _ctx.Chansons.Remove(chanson);
        await _ctx.SaveChangesAsync();
        return true;
    }

    // ════════════════════════ PLAYLISTS ═══════════════════════════════════════

    /// <summary>Toutes les playlists avec leurs chansons chargées (eager loading).</summary>
    public async Task<List<Playlist>> ObtenirToutesPlaylistsAsync()
        => await _ctx.Playlists
            .Include(p => p.PlaylistChansons)
                .ThenInclude(pc => pc.Chanson)
            .OrderBy(p => p.Nom)
            .ToListAsync();

    /// <summary>Une playlist avec ses chansons, triées par Position.</summary>
    public async Task<Playlist?> ObtenirPlaylistAsync(int id)
        => await _ctx.Playlists
            .Include(p => p.PlaylistChansons.OrderBy(pc => pc.Position))
                .ThenInclude(pc => pc.Chanson)
            .FirstOrDefaultAsync(p => p.Id == id);

    /// <summary>Créer une nouvelle playlist.</summary>
    public async Task<Playlist> CreerPlaylistAsync(string nom, string description = "")
    {
        var playlist = new Playlist { Nom = nom, Description = description };
        _ctx.Playlists.Add(playlist);
        await _ctx.SaveChangesAsync();
        return playlist;
    }

    /// <summary>Ajouter une chanson à une playlist.</summary>
    public async Task AjouterChansonPlaylistAsync(int playlistId, int chansonId)
    {
        // Vérifier que la liaison n'existe pas déjà
        bool existe = await _ctx.PlaylistChansons
            .AnyAsync(pc => pc.PlaylistId == playlistId && pc.ChansonId == chansonId);
        if (existe) throw new InvalidOperationException("Cette chanson est déjà dans la playlist.");

        // Calculer la prochaine position
        int position = await _ctx.PlaylistChansons
            .Where(pc => pc.PlaylistId == playlistId)
            .CountAsync() + 1;

        _ctx.PlaylistChansons.Add(new PlaylistChanson
        {
            PlaylistId = playlistId,
            ChansonId  = chansonId,
            Position   = position
        });
        await _ctx.SaveChangesAsync();
    }

    /// <summary>Retirer une chanson d'une playlist.</summary>
    public async Task<bool> RetirerChansonPlaylistAsync(int playlistId, int chansonId)
    {
        var lien = await _ctx.PlaylistChansons
            .FirstOrDefaultAsync(pc => pc.PlaylistId == playlistId && pc.ChansonId == chansonId);
        if (lien is null) return false;
        _ctx.PlaylistChansons.Remove(lien);
        await _ctx.SaveChangesAsync();
        return true;
    }

    /// <summary>Supprimer une playlist (et ses liaisons via CASCADE).</summary>
    public async Task<bool> SupprimerPlaylistAsync(int id)
    {
        var pl = await _ctx.Playlists.FindAsync(id);
        if (pl is null) return false;
        _ctx.Playlists.Remove(pl);
        await _ctx.SaveChangesAsync();
        return true;
    }

    // ════════════════════════ STATISTIQUES ════════════════════════════════════

    public async Task AfficherStatistiquesAsync()
    {
        int nbChansons   = await _ctx.Chansons.CountAsync();
        int nbPlaylists  = await _ctx.Playlists.CountAsync();
        double noteMoy   = nbChansons > 0
            ? await _ctx.Chansons.AverageAsync(c => (double)c.Note)
            : 0;

        var topGenres = await _ctx.Chansons
            .GroupBy(c => c.Genre)
            .Select(g => new { Genre = g.Key, Nombre = g.Count() })
            .OrderByDescending(g => g.Nombre)
            .Take(5)
            .ToListAsync();

        var topArtiste = await _ctx.Chansons
            .GroupBy(c => c.Artiste)
            .Select(g => new { Artiste = g.Key, Nombre = g.Count() })
            .OrderByDescending(g => g.Nombre)
            .FirstOrDefaultAsync();

        Console.WriteLine("\n📊 Statistiques de la bibliothèque");
        Console.WriteLine(new string('═', 45));
        Console.WriteLine($"  Chansons    : {nbChansons}");
        Console.WriteLine($"  Playlists   : {nbPlaylists}");
        Console.WriteLine($"  Note moyenne : {noteMoy:F1} / 5");

        if (topGenres.Count > 0)
        {
            Console.WriteLine("\n  Top genres :");
            foreach (var g in topGenres)
                Console.WriteLine($"    • {g.Genre,-15} {g.Nombre} titre(s)");
        }

        if (topArtiste is not null)
            Console.WriteLine($"\n  Artiste le + représenté : {topArtiste.Artiste} ({topArtiste.Nombre} titres)");
    }
}
