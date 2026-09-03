using Microsoft.AspNetCore.Mvc;
using PlaylistAppAPI.Events;
using PlaylistAppEF.Data;
using PlaylistAppEF.Models;
using Microsoft.EntityFrameworkCore;

namespace PlaylistAppAPI.Controllers;

/// <summary>
/// Contrôleur REST pour la gestion des chansons.
/// Architecture SOA : chaque endpoint est un service REST indépendant.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ChansonsController(
    PlaylistContext ctx,
    IEventBus eventBus,
    ILogger<ChansonsController> logger) : ControllerBase
{
    // Constructeur primaire (C# 12+) : les paramètres ctx, eventBus et logger
    // sont directement utilisables dans toutes les méthodes de la classe.
    private readonly PlaylistContext _ctx = ctx;
    private readonly IEventBus _eventBus = eventBus;
    private readonly ILogger<ChansonsController> _logger = logger;

    // ── GET /api/chansons ─────────────────────────────────────────────────────
    /// <summary>Récupère toutes les chansons de la bibliothèque.</summary>
    /// <returns>Liste de toutes les chansons triées par artiste.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Chanson>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Chanson>>> GetAll(
        [FromQuery] string? genre = null,
        [FromQuery] int    page  = 1,
        [FromQuery] int    taille = 20)
    {
        var query = _ctx.Chansons.AsQueryable();

        if (!string.IsNullOrEmpty(genre))
            query = query.Where(c => c.Genre == genre);

        var chansons = await query
            .OrderBy(c => c.Artiste).ThenBy(c => c.Titre)
            .Skip((page - 1) * taille)
            .Take(taille)
            .ToListAsync();

        _logger.LogInformation("GET /api/chansons → {Count} résultats", chansons.Count);
        return Ok(chansons);
    }

    // ── GET /api/chansons/{id} ────────────────────────────────────────────────
    /// <summary>Récupère une chanson par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Chanson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Chanson>> GetById(int id)
    {
        var chanson = await _ctx.Chansons.FindAsync(id);
        return chanson is null ? NotFound(new { message = $"Chanson #{id} introuvable." }) : Ok(chanson);
    }

    // ── GET /api/chansons/recherche?q= ───────────────────────────────────────
    /// <summary>Recherche des chansons par titre, artiste ou album.</summary>
    [HttpGet("recherche")]
    [ProducesResponseType(typeof(IEnumerable<Chanson>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Chanson>>> Rechercher([FromQuery] string q = "")
    {
        var res = await _ctx.Chansons
            .Where(c => EF.Functions.Like(c.Titre,   $"%{q}%") ||
                        EF.Functions.Like(c.Artiste, $"%{q}%") ||
                        EF.Functions.Like(c.Album,   $"%{q}%"))
            .OrderBy(c => c.Artiste)
            .ToListAsync();
        return Ok(res);
    }

    // ── POST /api/chansons ────────────────────────────────────────────────────
    /// <summary>Ajoute une nouvelle chanson à la bibliothèque.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Chanson), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Chanson>> Create([FromBody] Chanson chanson)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        chanson.AjouteLe = DateTime.UtcNow;
        _ctx.Chansons.Add(chanson);
        await _ctx.SaveChangesAsync();

        // EOA : publier l'événement (les handlers réagissent de façon asynchrone)
        await _eventBus.PublishAsync(new ChansonAjouteeEvent(
            chanson.Id, chanson.Titre, chanson.Artiste, chanson.Genre, DateTime.UtcNow));

        return CreatedAtAction(nameof(GetById), new { id = chanson.Id }, chanson);
    }

    // ── PUT /api/chansons/{id} ────────────────────────────────────────────────
    /// <summary>Met à jour une chanson existante.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] Chanson chanson)
    {
        if (id != chanson.Id) return BadRequest(new { message = "L'ID ne correspond pas." });

        var existant = await _ctx.Chansons.FindAsync(id);
        if (existant is null) return NotFound();

        existant.Titre          = chanson.Titre;
        existant.Artiste        = chanson.Artiste;
        existant.Album          = chanson.Album;
        existant.Genre          = chanson.Genre;
        existant.DureeSecondes  = chanson.DureeSecondes;
        existant.Annee          = chanson.Annee;
        existant.Note           = chanson.Note;

        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    // ── DELETE /api/chansons/{id} ─────────────────────────────────────────────
    /// <summary>Supprime une chanson (échoue si présente dans une playlist).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var chanson = await _ctx.Chansons.FindAsync(id);
        if (chanson is null) return NotFound();

        try
        {
            _ctx.Chansons.Remove(chanson);
            await _ctx.SaveChangesAsync();

            await _eventBus.PublishAsync(new ChansonSupprimeeEvent(
                id, chanson.Titre, DateTime.UtcNow));
            return NoContent();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "Cette chanson est dans une playlist. Retirez-la d'abord." });
        }
    }
}
