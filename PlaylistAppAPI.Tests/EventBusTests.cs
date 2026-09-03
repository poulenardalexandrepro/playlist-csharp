using Microsoft.Extensions.Logging.Abstractions;
using PlaylistAppAPI.Events;
using Xunit;

namespace PlaylistAppAPI.Tests;

/// <summary>
/// Tests unitaires du bus d'événements — TP4 (architecture orientée événements, EOA).
/// Vérifient la mécanique publish / subscribe de <see cref="InMemoryEventBus"/> :
/// un événement publié notifie tous les handlers abonnés à SON type, et seulement eux.
/// </summary>
public class EventBusTests
{
    private static InMemoryEventBus NouveauBus()
        => new(NullLogger<InMemoryEventBus>.Instance);

    [Fact(DisplayName = "Publish appelle le handler abonné au type d'événement")]
    public async Task Publish_appelle_le_handler_abonne()
    {
        var bus = NouveauBus();
        var recu = false;
        bus.Subscribe<ChansonAjouteeEvent>(_ => { recu = true; return Task.CompletedTask; });

        await bus.PublishAsync(new ChansonAjouteeEvent(1, "Imagine", "Lennon", "Rock", DateTime.UtcNow));

        Assert.True(recu);
    }

    [Fact(DisplayName = "Publish notifie TOUS les handlers abonnés au même type")]
    public async Task Publish_notifie_tous_les_handlers()
    {
        var bus = NouveauBus();
        var compteur = 0;
        bus.Subscribe<PlaylistCreeeEvent>(_ => { compteur++; return Task.CompletedTask; });
        bus.Subscribe<PlaylistCreeeEvent>(_ => { compteur++; return Task.CompletedTask; });

        await bus.PublishAsync(new PlaylistCreeeEvent(1, "Favoris", DateTime.UtcNow));

        Assert.Equal(2, compteur);
    }

    [Fact(DisplayName = "Publish sans abonné ne lève pas d'exception")]
    public async Task Publish_sans_abonne_ne_leve_pas_d_exception()
    {
        var bus = NouveauBus();

        var exception = await Record.ExceptionAsync(
            () => bus.PublishAsync(new ChansonSupprimeeEvent(1, "Yesterday", DateTime.UtcNow)));

        Assert.Null(exception);
    }

    [Fact(DisplayName = "Publish ne notifie QUE les handlers du bon type d'événement")]
    public async Task Publish_ne_notifie_que_le_bon_type()
    {
        var bus = NouveauBus();
        var mauvaisHandlerAppele = false;
        bus.Subscribe<ChansonSupprimeeEvent>(_ => { mauvaisHandlerAppele = true; return Task.CompletedTask; });

        await bus.PublishAsync(new PlaylistCreeeEvent(1, "Favoris", DateTime.UtcNow));

        Assert.False(mauvaisHandlerAppele);
    }

    [Fact(DisplayName = "Le handler reçoit les données de l'événement publié")]
    public async Task Handler_recoit_les_bonnes_donnees()
    {
        var bus = NouveauBus();
        ChansonAjouteeEvent? recu = null;
        bus.Subscribe<ChansonAjouteeEvent>(e => { recu = e; return Task.CompletedTask; });

        await bus.PublishAsync(
            new ChansonAjouteeEvent(42, "Bohemian Rhapsody", "Queen", "Rock", DateTime.UtcNow));

        Assert.NotNull(recu);
        Assert.Equal(42, recu!.ChansonId);
        Assert.Equal("Bohemian Rhapsody", recu.Titre);
        Assert.Equal("Queen", recu.Artiste);
    }
}
