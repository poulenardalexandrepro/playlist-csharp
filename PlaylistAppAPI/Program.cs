using Microsoft.EntityFrameworkCore;
using PlaylistAppAPI.Events;
using PlaylistAppEF.Data;

// ══════════════════════════════════════════════════════════════════════════════
//  🎵 PlaylistApp API – TP3 : Architecture SOA + EOA
//  ASP.NET Core Web API  |  Cas pratique PlaylistApp
//
//  Architecture SOA : API REST avec Controllers, Services et Repository
//  Architecture EOA : Bus d'événements InMemory (→ Kafka en production)
// ══════════════════════════════════════════════════════════════════════════════

var builder = WebApplication.CreateBuilder(args);

// ── Couche Base de Données (EF Core + SQLite) ─────────────────────────────────
string dbPath = Path.Combine(
    Environment.GetEnvironmentVariable("DB_PATH") ?? ".",
    "playlist.db");

builder.Services.AddDbContext<PlaylistContext>(opt =>
    opt.UseSqlite($"Data Source={dbPath}",
        // Les migrations vivent dans le projet PlaylistAppEF, pas dans l'API.
        // On indique à EF Core où les trouver.
        sql => sql.MigrationsAssembly("PlaylistAppEF")));

// ── Bus d'événements EOA ──────────────────────────────────────────────────────
builder.Services.AddEventBus();

// ── Couche API (Controllers) ──────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger / OpenAPI (documentation SOA) ────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "🎵 PlaylistApp API",
        Version     = "v1",
        Description = "API REST de gestion de playlists musicales\n\n" +
                      "Architecture SOA + EOA\n\n" +
                      "Référence : github.com/jasonsturges/sqlite-dotnet-core",
        Contact     = new() { Name = "SLAM", Email = "contact@example.org" }
    });

    // Inclure les commentaires XML dans Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

// ── CORS (Cross-Origin Resource Sharing) ─────────────────────────────────────
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// ── Migrations automatiques au démarrage ─────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<PlaylistContext>();
    // MigrateAsync() exige un fournisseur relationnel (SQLite).
    // En test (InMemory), on bascule sur EnsureCreatedAsync().
    if (ctx.Database.IsRelational())
        await ctx.Database.MigrateAsync();
    else
        await ctx.Database.EnsureCreatedAsync();
}

// ── Middleware pipeline ───────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    // Swagger disponible en développement
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlaylistApp API v1");
        c.RoutePrefix = string.Empty;   // Swagger à la racine : http://localhost:5000
        c.DocumentTitle = "🎵 PlaylistApp API";
    });
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Logger.LogInformation("🎵 PlaylistApp API démarrée – Swagger : http://localhost:5000");

app.Run();

// Rendre Program accessible aux tests d'intégration (WebApplicationFactory<Program>)
public partial class Program { }
