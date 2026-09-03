using Microsoft.EntityFrameworkCore;
using PlaylistAppEF.Data;
using PlaylistAppEF.Models;
using PlaylistAppEF.Repositories;
using Xunit;

namespace PlaylistAppEF.Tests;

/// <summary>
/// Tests d'intégration pour MusiqueRepository avec SQLite InMemory.
/// Chaque test repart d'une base de données vide (isolation totale).
/// </summary>
public class RepositoryTests
{
    // ── Factory : crée un DbContext en mémoire pour chaque test ─────────────
    private static PlaylistContext CreerContextTest()
    {
        var options = new DbContextOptionsBuilder<PlaylistContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // BD unique par test
            .Options;
        return new PlaylistContext(options);
    }

    // ════════════════════════ TESTS CHANSONS ═════════════════════════════════

    [Fact(DisplayName = "AjouterChanson – la chanson est bien persistée")]
    public async Task AjouterChansonAsync_NouvelleChanson_EstPersistee()
    {
        // Arrange
        using var ctx  = CreerContextTest();
        var repo = new MusiqueRepository(ctx);
        var nouvelle = new Chanson
        {
            Titre = "Chanson Test", Artiste = "Artiste Test",
            Album = "Test Album", DureeSecondes = 180,
            Genre = "Pop", Annee = 2024, Note = 4
        };

        // Act
        var ajoutee = await repo.AjouterChansonAsync(nouvelle);

        // Assert
        Assert.True(ajoutee.Id > 0, "L'ID doit être généré automatiquement");
        var enBase = await ctx.Chansons.FindAsync(ajoutee.Id);
        Assert.NotNull(enBase);
        Assert.Equal("Chanson Test", enBase.Titre);
    }

    [Fact(DisplayName = "ObtenirToutesChansons – retourne les chansons triées par artiste")]
    public async Task ObtenirToutesChansonsAsync_TriParArtiste()
    {
        using var ctx  = CreerContextTest();
        var repo = new MusiqueRepository(ctx);
        await repo.AjouterChansonAsync(new Chanson { Titre="B", Artiste="Zorro", Album="Z", DureeSecondes=100, Genre="Rock", Annee=2020 });
        await repo.AjouterChansonAsync(new Chanson { Titre="A", Artiste="Alpha", Album="A", DureeSecondes=100, Genre="Pop",  Annee=2020 });

        var chansons = await repo.ObtenirToutesChansonsAsync();

        Assert.Equal(2, chansons.Count);
        Assert.Equal("Alpha", chansons[0].Artiste); // Tri alphabétique
        Assert.Equal("Zorro", chansons[1].Artiste);
    }

    [Fact(DisplayName = "RechercherChansons – trouve par titre")]
    public async Task RechercherChansonsAsync_ParTitre_TrouveLesBons()
    {
        using var ctx  = CreerContextTest();
        var repo = new MusiqueRepository(ctx);
        await repo.AjouterChansonAsync(new Chanson { Titre="Bohemian Rhapsody", Artiste="Queen", Album="A", DureeSecondes=354, Genre="Rock", Annee=1975 });
        await repo.AjouterChansonAsync(new Chanson { Titre="Hotel California",  Artiste="Eagles",Album="B", DureeSecondes=391, Genre="Rock", Annee=1977 });

        var resultats = await repo.RechercherChansonsAsync("Bohemian");

        Assert.Single(resultats);
        Assert.Equal("Bohemian Rhapsody", resultats[0].Titre);
    }

    [Fact(DisplayName = "RechercherChansons – trouve par artiste")]
    public async Task RechercherChansonsAsync_ParArtiste_TrouveLesBons()
    {
        using var ctx  = CreerContextTest();
        var repo = new MusiqueRepository(ctx);
        await repo.AjouterChansonAsync(new Chanson { Titre="One More Time", Artiste="Daft Punk", Album="Discovery", DureeSecondes=321, Genre="Electro", Annee=2000 });
        await repo.AjouterChansonAsync(new Chanson { Titre="Get Lucky",     Artiste="Daft Punk", Album="RAM",       DureeSecondes=369, Genre="Electro", Annee=2013 });
        await repo.AjouterChansonAsync(new Chanson { Titre="Blinding Lights",Artiste="The Weeknd",Album="After Hours",DureeSecondes=200,Genre="Pop",    Annee=2019 });

        var resultats = await repo.RechercherChansonsAsync("Daft Punk");

        Assert.Equal(2, resultats.Count);
        Assert.All(resultats, c => Assert.Equal("Daft Punk", c.Artiste));
    }

    [Fact(DisplayName = "ModifierNote – la note est bien mise à jour")]
    public async Task ModifierNoteAsync_NoteValide_MiseAJour()
    {
        using var ctx  = CreerContextTest();
        var repo = new MusiqueRepository(ctx);
        var c = await repo.AjouterChansonAsync(new Chanson { Titre="Test", Artiste="A", Album="B", DureeSecondes=100, Genre="Pop", Annee=2020, Note=3 });

        bool ok = await repo.ModifierNoteAsync(c.Id, 5);

        Assert.True(ok);
        var enBase = await ctx.Chansons.FindAsync(c.Id);
        Assert.Equal(5, enBase!.Note);
    }

    [Fact(DisplayName = "SupprimerChanson – la chanson est retirée")]
    public async Task SupprimerChansonAsync_ExistantSansPlaylist_Supprime()
    {
        using var ctx  = CreerContextTest();
        var repo = new MusiqueRepository(ctx);
        var c = await repo.AjouterChansonAsync(new Chanson { Titre="A", Artiste="B", Album="C", DureeSecondes=100, Genre="Pop", Annee=2020 });

        bool ok = await repo.SupprimerChansonAsync(c.Id);

        Assert.True(ok);
        Assert.Empty(await repo.ObtenirToutesChansonsAsync());
    }

    [Fact(DisplayName = "SupprimerChanson – retourne false si introuvable")]
    public async Task SupprimerChansonAsync_Inexistant_RetourneFalse()
    {
        using var ctx  = CreerContextTest();
        var repo = new MusiqueRepository(ctx);

        bool ok = await repo.SupprimerChansonAsync(9999);

        Assert.False(ok);
    }

    // ════════════════════════ TESTS PLAYLISTS ════════════════════════════════

    [Fact(DisplayName = "CreerPlaylist – la playlist est bien créée")]
    public async Task CreerPlaylistAsync_Nouvelle_EstCreee()
    {
        using var ctx  = CreerContextTest();
        var repo = new MusiqueRepository(ctx);

        var pl = await repo.CreerPlaylistAsync("Rock Classics", "Les grands classiques");

        Assert.True(pl.Id > 0);
        Assert.Equal("Rock Classics", pl.Nom);
        Assert.Equal("Les grands classiques", pl.Description);
    }

    [Fact(DisplayName = "AjouterChansonPlaylist – la liaison est créée")]
    public async Task AjouterChansonPlaylistAsync_ChansonEtPlaylist_LiaisonCreee()
    {
        using var ctx  = CreerContextTest();
        var repo = new MusiqueRepository(ctx);
        var c  = await repo.AjouterChansonAsync(new Chanson { Titre="T", Artiste="A", Album="B", DureeSecondes=100, Genre="Pop", Annee=2020 });
        var pl = await repo.CreerPlaylistAsync("Ma Playlist");

        await repo.AjouterChansonPlaylistAsync(pl.Id, c.Id);

        var plAvecChansons = await repo.ObtenirPlaylistAsync(pl.Id);
        Assert.NotNull(plAvecChansons);
        Assert.Single(plAvecChansons!.PlaylistChansons);
    }

    [Fact(DisplayName = "AjouterChansonPlaylist – doublon rejeté")]
    public async Task AjouterChansonPlaylistAsync_Doublon_LeveException()
    {
        using var ctx  = CreerContextTest();
        var repo = new MusiqueRepository(ctx);
        var c  = await repo.AjouterChansonAsync(new Chanson { Titre="T", Artiste="A", Album="B", DureeSecondes=100, Genre="Pop", Annee=2020 });
        var pl = await repo.CreerPlaylistAsync("Ma Playlist");
        await repo.AjouterChansonPlaylistAsync(pl.Id, c.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            repo.AjouterChansonPlaylistAsync(pl.Id, c.Id));
    }

    [Fact(DisplayName = "RetirerChansonPlaylist – la liaison est supprimée")]
    public async Task RetirerChansonPlaylistAsync_Existant_Supprime()
    {
        using var ctx  = CreerContextTest();
        var repo = new MusiqueRepository(ctx);
        var c  = await repo.AjouterChansonAsync(new Chanson { Titre="T", Artiste="A", Album="B", DureeSecondes=100, Genre="Pop", Annee=2020 });
        var pl = await repo.CreerPlaylistAsync("Ma Playlist");
        await repo.AjouterChansonPlaylistAsync(pl.Id, c.Id);

        bool ok = await repo.RetirerChansonPlaylistAsync(pl.Id, c.Id);

        Assert.True(ok);
        var plVide = await repo.ObtenirPlaylistAsync(pl.Id);
        Assert.Empty(plVide!.PlaylistChansons);
    }

    [Fact(DisplayName = "TopChansons – retourne les N mieux notées")]
    public async Task TopChansonsAsync_3Chansons_ReturnsTopN()
    {
        using var ctx  = CreerContextTest();
        var repo = new MusiqueRepository(ctx);
        await repo.AjouterChansonAsync(new Chanson { Titre="A", Artiste="x", Album="x", DureeSecondes=100, Genre="Pop", Annee=2020, Note=5 });
        await repo.AjouterChansonAsync(new Chanson { Titre="B", Artiste="x", Album="x", DureeSecondes=100, Genre="Pop", Annee=2020, Note=3 });
        await repo.AjouterChansonAsync(new Chanson { Titre="C", Artiste="x", Album="x", DureeSecondes=100, Genre="Pop", Annee=2020, Note=4 });

        var top2 = await repo.TopChansonsAsync(2);

        Assert.Equal(2, top2.Count);
        Assert.Equal(5, top2[0].Note); // La mieux notée en premier
        Assert.Equal(4, top2[1].Note);
    }
}
