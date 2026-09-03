using PlaylistAppEF.Models;
using Xunit;

namespace PlaylistAppEF.Tests;

/// <summary>
/// Tests unitaires pour l'entité Chanson.
/// Ces tests vérifient le comportement des méthodes de la classe.
/// </summary>
public class ChansonTests
{
    // ── Tests de DureeFormatee() ─────────────────────────────────────────────

    [Fact(DisplayName = "DureeFormatee retourne mm:ss pour 354 secondes")]
    public void DureeFormatee_354secondes_Retourne05_54()
    {
        // Arrange
        var chanson = new Chanson { DureeSecondes = 354 };

        // Act
        string resultat = chanson.DureeFormatee();

        // Assert
        Assert.Equal("05:54", resultat);
    }

    [Fact(DisplayName = "DureeFormatee retourne 00:00 pour zéro seconde")]
    public void DureeFormatee_0secondes_Retourne00_00()
    {
        var chanson = new Chanson { DureeSecondes = 0 };
        Assert.Equal("00:00", chanson.DureeFormatee());
    }

    [Fact(DisplayName = "DureeFormatee retourne 01:00 pour 60 secondes")]
    public void DureeFormatee_60secondes_Retourne01_00()
    {
        var chanson = new Chanson { DureeSecondes = 60 };
        Assert.Equal("01:00", chanson.DureeFormatee());
    }

    [Theory(DisplayName = "DureeFormatee formate correctement plusieurs durées")]
    [InlineData(0,   "00:00")]
    [InlineData(30,  "00:30")]
    [InlineData(60,  "01:00")]
    [InlineData(90,  "01:30")]
    [InlineData(200, "03:20")]
    [InlineData(354, "05:54")]
    [InlineData(391, "06:31")]
    public void DureeFormatee_DiversesDurees_FormatageCorrect(int secondes, string attendu)
    {
        var chanson = new Chanson { DureeSecondes = secondes };
        Assert.Equal(attendu, chanson.DureeFormatee());
    }

    // ── Tests de ToString() ──────────────────────────────────────────────────

    [Fact(DisplayName = "ToString contient le titre et l'artiste")]
    public void ToString_ChansonComplete_ContientTitreEtArtiste()
    {
        var chanson = new Chanson
        {
            Id       = 1,
            Titre    = "Bohemian Rhapsody",
            Artiste  = "Queen",
            Album    = "A Night at the Opera",
            DureeSecondes = 354,
            Genre    = "Rock",
            Annee    = 1975,
            Note     = 5
        };

        string resultat = chanson.ToString();

        Assert.Contains("Bohemian Rhapsody", resultat);
        Assert.Contains("Queen", resultat);
    }

    // ── Tests de validation des propriétés ───────────────────────────────────

    [Fact(DisplayName = "Note par défaut est 3")]
    public void Note_NouvelleInstance_Vaut3()
    {
        var chanson = new Chanson();
        Assert.Equal(3, chanson.Note);
    }

    [Fact(DisplayName = "AjouteLe est renseigné à la création")]
    public void AjouteLe_NouvelleInstance_EstRenseigne()
    {
        var avant   = DateTime.UtcNow.AddSeconds(-1);
        var chanson = new Chanson();
        var apres   = DateTime.UtcNow.AddSeconds(1);

        Assert.InRange(chanson.AjouteLe, avant, apres);
    }
}
