namespace PlaylistAppAPI.Events;

// ══════════════════════════════════════════════════════════════════════════════
//  Architecture EOA (Event-Oriented Architecture)
//  Bus d'événements en mémoire – pour le TP (Kafka/RabbitMQ en production)
//
//  Principe :
//   1. Un service publie un événement (ex: ChansonAjouteeEvent)
//   2. Le bus notifie tous les handlers abonnés
//   3. Les handlers réagissent de façon indépendante
// ══════════════════════════════════════════════════════════════════════════════

// ── Interface du bus d'événements ─────────────────────────────────────────────
public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class;
    void Subscribe<T>(Func<T, Task> handler) where T : class;
}

// ── Implémentation en mémoire (simple, pour le TP) ────────────────────────────
public class InMemoryEventBus : IEventBus
{
    private readonly Dictionary<Type, List<Func<object, Task>>> _handlers = new();
    private readonly ILogger<InMemoryEventBus> _logger;

    public InMemoryEventBus(ILogger<InMemoryEventBus> logger) => _logger = logger;

    public void Subscribe<T>(Func<T, Task> handler) where T : class
    {
        var type = typeof(T);
        if (!_handlers.ContainsKey(type))
            _handlers[type] = new();
        _handlers[type].Add(e => handler((T)e));
    }

    public async Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class
    {
        var type = typeof(T);
        _logger.LogInformation("[EventBus] Publication : {EventType}", type.Name);

        if (!_handlers.TryGetValue(type, out var handlers)) return;

        var tasks = handlers.Select(h => h(@event));
        await Task.WhenAll(tasks);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
//  Événements du domaine PlaylistApp
//  Convention : NomÉvénementEvent (suffixe "Event", passé composé)
// ══════════════════════════════════════════════════════════════════════════════

/// <summary>Émis quand une chanson est ajoutée à la bibliothèque</summary>
public record ChansonAjouteeEvent(
    int     ChansonId,
    string  Titre,
    string  Artiste,
    string  Genre,
    DateTime Timestamp);

/// <summary>Émis quand une chanson est supprimée</summary>
public record ChansonSupprimeeEvent(
    int     ChansonId,
    string  Titre,
    DateTime Timestamp);

/// <summary>Émis quand une chanson est ajoutée à une playlist</summary>
public record ChansonAjouteePlaylistEvent(
    int     PlaylistId,
    string  NomPlaylist,
    int     ChansonId,
    string  TitreChanson,
    DateTime Timestamp);

/// <summary>Émis quand une playlist est créée</summary>
public record PlaylistCreeeEvent(
    int     PlaylistId,
    string  Nom,
    DateTime Timestamp);

// ══════════════════════════════════════════════════════════════════════════════
//  Handlers (consommateurs d'événements)
// ══════════════════════════════════════════════════════════════════════════════

/// <summary>Journalise toutes les actions (audit trail)</summary>
public class AuditHandler(ILogger<AuditHandler> logger)
{
    public Task HandleChansonAjoutee(ChansonAjouteeEvent e)
    {
        logger.LogInformation(
            "[AUDIT] {Timestamp:HH:mm:ss} | Chanson ajoutée : {Titre} – {Artiste} (#{Id})",
            e.Timestamp, e.Titre, e.Artiste, e.ChansonId);
        return Task.CompletedTask;
    }

    public Task HandleChansonAjouteePlaylist(ChansonAjouteePlaylistEvent e)
    {
        logger.LogInformation(
            "[AUDIT] {Timestamp:HH:mm:ss} | Chanson #{CId} ajoutée à playlist #{PId} ({Nom})",
            e.Timestamp, e.ChansonId, e.PlaylistId, e.NomPlaylist);
        return Task.CompletedTask;
    }

    public Task HandlePlaylistCreee(PlaylistCreeeEvent e)
    {
        logger.LogInformation(
            "[AUDIT] {Timestamp:HH:mm:ss} | Playlist créée : {Nom} (#{Id})",
            e.Timestamp, e.Nom, e.PlaylistId);
        return Task.CompletedTask;
    }
}

/// <summary>Gère les stats (cache invalidation) lors d'événements</summary>
public class StatistiquesHandler(ILogger<StatistiquesHandler> logger)
{
    public Task HandleChansonAjoutee(ChansonAjouteeEvent e)
    {
        // En production : invalider le cache Redis des statistiques
        logger.LogInformation("[STATS] Cache invalidé suite à l'ajout de '{Titre}'", e.Titre);
        return Task.CompletedTask;
    }
}

// ── Extension pour enregistrer le bus + les handlers ─────────────────────────
public static class EventBusExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryEventBus>();
        services.AddSingleton<IEventBus>(sp =>
        {
            var bus     = sp.GetRequiredService<InMemoryEventBus>();
            var audit   = sp.GetRequiredService<AuditHandler>();
            var stats   = sp.GetRequiredService<StatistiquesHandler>();

            // Abonnements des handlers
            bus.Subscribe<ChansonAjouteeEvent>(audit.HandleChansonAjoutee);
            bus.Subscribe<ChansonAjouteeEvent>(stats.HandleChansonAjoutee);
            bus.Subscribe<ChansonAjouteePlaylistEvent>(audit.HandleChansonAjouteePlaylist);
            bus.Subscribe<PlaylistCreeeEvent>(audit.HandlePlaylistCreee);

            return bus;
        });
        services.AddSingleton<AuditHandler>();
        services.AddSingleton<StatistiquesHandler>();
        return services;
    }
}
