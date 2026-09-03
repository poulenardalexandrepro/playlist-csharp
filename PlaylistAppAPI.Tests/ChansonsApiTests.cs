using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlaylistAppEF.Data;
using PlaylistAppEF.Models;
using Xunit;

namespace PlaylistAppAPI.Tests;

/// <summary>
/// Tests d'intégration de l'API REST TP3.
/// WebApplicationFactory démarre l'API en mémoire – pas besoin de docker run.
/// </summary>
public class ChansonsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ChansonsApiTests(WebApplicationFactory<Program> factory)
    {
        // Remplacer SQLite par InMemory pour les tests
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Retirer TOUS les services liés au DbContext SQLite existant
                var aRetirer = services.Where(d =>
                    d.ServiceType == typeof(DbContextOptions<PlaylistContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(PlaylistContext) ||
                    (d.ServiceType.FullName?.Contains("DbContextOptions") ?? false)
                ).ToList();
                foreach (var d in aRetirer) services.Remove(d);

                // Base InMemory partagée pour la durée de la factory
                services.AddDbContext<PlaylistContext>(options =>
                    options.UseInMemoryDatabase("TestDB_Partagee"));
            });
        });
        _client = _factory.CreateClient();

        // Isolation : on repart d'une base vide AVANT chaque test
        // (le constructeur xUnit s'exécute avant chaque méthode de test)
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<PlaylistContext>();
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();
    }

    // ── Tests GET ─────────────────────────────────────────────────────────────

    [Fact(DisplayName = "GET /api/chansons → 200 OK")]
    public async Task GetAllChansons_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/chansons");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "GET /api/chansons → Content-Type JSON")]
    public async Task GetAllChansons_ReturnsJson()
    {
        var response = await _client.GetAsync("/api/chansons");

        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact(DisplayName = "GET /api/chansons/{id} → 404 si inexistant")]
    public async Task GetChansonById_Inexistant_Returns404()
    {
        var response = await _client.GetAsync("/api/chansons/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── Tests POST ────────────────────────────────────────────────────────────

    [Fact(DisplayName = "POST /api/chansons → 201 Created")]
    public async Task PostChanson_NouvelleValide_Returns201()
    {
        var nouvelle = new
        {
            Titre        = "Chanson Test API",
            Artiste      = "Artiste Test",
            Album        = "Test Album",
            DureeSecondes = 180,
            Genre        = "Pop",
            Annee        = 2024,
            Note         = 4
        };

        var response = await _client.PostAsJsonAsync("/api/chansons", nouvelle);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location); // Header Location doit être présent
    }

    [Fact(DisplayName = "POST puis GET → la chanson créée est récupérable")]
    public async Task PostEtGet_ChansonCreee_EstRecuperable()
    {
        // Créer une chanson
        var nouvelle = new
        {
            Titre="Chanson Test Unique", Artiste="Artiste Test",
            Album="Test", DureeSecondes=200, Genre="Rock", Annee=2024, Note=5
        };
        var postResponse = await _client.PostAsJsonAsync("/api/chansons", nouvelle);
        var creee = await postResponse.Content.ReadFromJsonAsync<Chanson>();
        Assert.NotNull(creee);

        // La récupérer par ID
        var getResponse = await _client.GetAsync($"/api/chansons/{creee.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var recup = await getResponse.Content.ReadFromJsonAsync<Chanson>();
        Assert.Equal("Chanson Test Unique", recup?.Titre);
    }

    // ── Tests DELETE ──────────────────────────────────────────────────────────

    [Fact(DisplayName = "DELETE /api/chansons/{id} → 204 No Content")]
    public async Task DeleteChanson_Existant_Returns204()
    {
        // Créer une chanson d'abord
        var nouvelle = new { Titre="To Delete", Artiste="A", Album="B", DureeSecondes=100, Genre="Pop", Annee=2024, Note=3 };
        var postResponse = await _client.PostAsJsonAsync("/api/chansons", nouvelle);
        var creee = await postResponse.Content.ReadFromJsonAsync<Chanson>();

        // La supprimer
        var deleteResponse = await _client.DeleteAsync($"/api/chansons/{creee!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact(DisplayName = "DELETE /api/chansons/{id} → 404 si inexistant")]
    public async Task DeleteChanson_Inexistant_Returns404()
    {
        var response = await _client.DeleteAsync("/api/chansons/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── Tests recherche ───────────────────────────────────────────────────────

    [Fact(DisplayName = "GET /api/chansons/recherche?q=queen → filtre correctement")]
    public async Task RechercherChansons_ParArtiste_FiltreBien()
    {
        // Insérer des données
        await _client.PostAsJsonAsync("/api/chansons", new { Titre="Bohemian Rhapsody", Artiste="Queen", Album="A Night at the Opera", DureeSecondes=354, Genre="Rock", Annee=1975, Note=5 });
        await _client.PostAsJsonAsync("/api/chansons", new { Titre="Hotel California", Artiste="Eagles", Album="Hotel California", DureeSecondes=391, Genre="Rock", Annee=1977, Note=5 });

        var response = await _client.GetAsync("/api/chansons/recherche?q=Queen");
        var resultats = await response.Content.ReadFromJsonAsync<List<Chanson>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(resultats);
        Assert.All(resultats, c => Assert.Contains("Queen", c.Artiste, StringComparison.OrdinalIgnoreCase));
    }
}
