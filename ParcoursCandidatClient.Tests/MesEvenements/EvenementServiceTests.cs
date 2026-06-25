using FluentAssertions;
using ParcoursCandidatClient.Services;
using ParcoursCandidatClient.Tests.Fixtures;

namespace ParcoursCandidatClient.Tests.MesEvenements;

/// <summary>
/// Tests unitaires du service <see cref="EvenementService"/> consommé
/// par l'écran "Mes événements" (epic 1).
/// </summary>
public class EvenementServiceTests
{
    private static EvenementService CreerService(MockHttpMessageHandler handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri(EvenementsHttpFixtures.BaseAddress) });

    [Fact]
    public async Task Etant_donne_une_API_repondant_deux_evenements_quand_on_appelle_GetEvenementsDuJourAsync_alors_les_evenements_sont_desserialises()
    {
        var aujourdhui = DateOnly.FromDateTime(DateTime.Today);
        var json = EvenementsHttpFixtures.ReponseJsonPourDate(aujourdhui);
        var handler = new MockHttpMessageHandler(EvenementsHttpFixtures.ReponseOk(json));
        var service = CreerService(handler);

        var evenements = await service.GetEvenementsDuJourAsync(EvenementsHttpFixtures.AgenceLyon);

        evenements.Should().HaveCount(2);
        evenements[0].Id.Should().Be("evt-001");
        evenements[1].Id.Should().Be("evt-003");
    }

    [Fact]
    public async Task Etant_donne_une_API_renvoyant_une_liste_vide_quand_on_appelle_GetEvenementsDuJourAsync_alors_le_resultat_est_vide()
    {
        var handler = new MockHttpMessageHandler(EvenementsHttpFixtures.ReponseListeVide());
        var service = CreerService(handler);

        var evenements = await service.GetEvenementsDuJourAsync(EvenementsHttpFixtures.AgenceLyon);

        evenements.Should().BeEmpty();
    }

    [Fact]
    public async Task Etant_donne_aucune_date_fournie_quand_on_appelle_GetEvenementsDuJourAsync_alors_la_date_d_aujourd_hui_est_utilisee_dans_l_URL()
    {
        var aujourdhui = DateOnly.FromDateTime(DateTime.Today);
        var handler = new MockHttpMessageHandler(EvenementsHttpFixtures.ReponseListeVide());
        var service = CreerService(handler);

        await service.GetEvenementsDuJourAsync(EvenementsHttpFixtures.AgenceLyon);

        handler.DerniereRequeteUri!.Query
            .Should()
            .Contain($"date={aujourdhui:yyyy-MM-dd}");
    }

    [Fact]
    public async Task Etant_donne_une_date_specifique_quand_on_appelle_GetEvenementsDuJourAsync_alors_cette_date_est_passee_dans_l_URL()
    {
        var dateCible = new DateOnly(2030, 7, 14);
        var handler = new MockHttpMessageHandler(EvenementsHttpFixtures.ReponseListeVide());
        var service = CreerService(handler);

        await service.GetEvenementsDuJourAsync(EvenementsHttpFixtures.AgenceLyon, dateCible);

        handler.DerniereRequeteUri!.Query.Should().Contain("date=2030-07-14");
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_d_agence_quand_on_appelle_GetEvenementsDuJourAsync_alors_il_est_passe_dans_l_URL_et_correctement_encode()
    {
        var handler = new MockHttpMessageHandler(EvenementsHttpFixtures.ReponseListeVide());
        var service = CreerService(handler);

        await service.GetEvenementsDuJourAsync("lyon part dieu");

        handler.DerniereRequeteUri!.Query.Should().Contain("agenceId=lyon%20part%20dieu");
    }

    [Fact]
    public async Task Etant_donne_le_service_quand_on_appelle_GetEvenementsDuJourAsync_alors_la_route_appelee_est_api_evenements()
    {
        var handler = new MockHttpMessageHandler(EvenementsHttpFixtures.ReponseListeVide());
        var service = CreerService(handler);

        await service.GetEvenementsDuJourAsync(EvenementsHttpFixtures.AgenceLyon);

        handler.DerniereRequeteUri!.AbsolutePath.Should().Be("/api/evenements");
    }

    [Fact]
    public async Task Etant_donne_une_API_en_erreur_quand_on_appelle_GetEvenementsDuJourAsync_alors_une_HttpRequestException_est_levee()
    {
        var handler = new MockHttpMessageHandler(EvenementsHttpFixtures.ReponseErreurServeur());
        var service = CreerService(handler);

        var act = async () => await service.GetEvenementsDuJourAsync(EvenementsHttpFixtures.AgenceLyon);

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_d_evenement_quand_on_appelle_GetEvenementByIdAsync_alors_la_route_appelee_est_api_evenements_id()
    {
        var handler = new MockHttpMessageHandler(InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));
        var service = CreerService(handler);

        await service.GetEvenementByIdAsync("evt-001");

        handler.DerniereRequeteUri!.AbsolutePath.Should().Be("/api/evenements/evt-001");
    }

    [Fact]
    public async Task Etant_donne_une_API_repondant_un_evenement_quand_on_appelle_GetEvenementByIdAsync_alors_l_evenement_est_desserialise()
    {
        var handler = new MockHttpMessageHandler(InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));
        var service = CreerService(handler);

        var evenement = await service.GetEvenementByIdAsync("evt-001");

        evenement.Should().NotBeNull();
        evenement!.Id.Should().Be("evt-001");
        evenement.Titre.Should().Be("Job dating restauration");
    }

    [Fact]
    public async Task Etant_donne_une_API_renvoyant_404_quand_on_appelle_GetEvenementByIdAsync_alors_le_resultat_est_null()
    {
        var handler = new MockHttpMessageHandler(InscritsHttpFixtures.ReponseNotFound());
        var service = CreerService(handler);

        var evenement = await service.GetEvenementByIdAsync("evt-inexistant");

        evenement.Should().BeNull();
    }
}
