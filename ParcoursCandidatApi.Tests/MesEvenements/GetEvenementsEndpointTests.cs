using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ParcoursCandidatApi.Models;
using ParcoursCandidatApi.Tests.Fixtures;

namespace ParcoursCandidatApi.Tests.MesEvenements;

/// <summary>
/// Tests d'intégration pour l'endpoint <c>GET /api/evenements</c>
/// utilisé par l'écran "Mes événements" (epic 1).
/// </summary>
public class GetEvenementsEndpointTests : IClassFixture<EvenementsApiFactory>
{
    private readonly HttpClient _client;

    public GetEvenementsEndpointTests(EvenementsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Etant_donne_l_agence_de_Lyon_quand_on_demande_les_evenements_du_jour_alors_la_liste_est_renvoyee_en_200()
    {
        var aujourdhui = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd");

        var response = await _client.GetAsync(
            $"/api/evenements?agenceId={EvenementsFixtures.AgenceLyon}&date={aujourdhui}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Etant_donne_l_agence_de_Lyon_quand_on_demande_les_evenements_du_jour_alors_seuls_les_evenements_de_cette_agence_sont_renvoyes()
    {
        var aujourdhui = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd");

        var evenements = await _client.GetFromJsonAsync<List<Evenement>>(
            $"/api/evenements?agenceId={EvenementsFixtures.AgenceLyon}&date={aujourdhui}");

        evenements.Should().NotBeNull();
        evenements!.Should().OnlyContain(e => e.AgenceId == EvenementsFixtures.AgenceLyon);
    }

    [Fact]
    public async Task Etant_donne_plusieurs_evenements_quand_la_liste_est_renvoyee_alors_elle_est_triee_par_heure_de_debut_croissante()
    {
        var aujourdhui = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd");

        var evenements = await _client.GetFromJsonAsync<List<Evenement>>(
            $"/api/evenements?agenceId={EvenementsFixtures.AgenceLyon}&date={aujourdhui}");

        evenements!.Select(e => e.Id)
            .Should()
            .ContainInOrder(EvenementsFixtures.EvenementsLyonDuJourTriesParHeure);
    }

    [Fact]
    public async Task Etant_donne_une_agence_inconnue_quand_on_demande_les_evenements_du_jour_alors_la_liste_renvoyee_est_vide()
    {
        var aujourdhui = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd");

        var evenements = await _client.GetFromJsonAsync<List<Evenement>>(
            $"/api/evenements?agenceId={EvenementsFixtures.AgenceInconnue}&date={aujourdhui}");

        evenements.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Etant_donne_l_agence_de_Lyon_quand_on_demande_les_evenements_de_demain_alors_aucun_evenement_de_Lyon_n_est_renvoye()
    {
        var demain = DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString("yyyy-MM-dd");

        var evenements = await _client.GetFromJsonAsync<List<Evenement>>(
            $"/api/evenements?agenceId={EvenementsFixtures.AgenceLyon}&date={demain}");

        evenements.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Etant_donne_l_agence_de_Paris_quand_on_demande_les_evenements_de_demain_alors_le_job_dating_logistique_est_renvoye()
    {
        var demain = DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString("yyyy-MM-dd");

        var evenements = await _client.GetFromJsonAsync<List<Evenement>>(
            $"/api/evenements?agenceId={EvenementsFixtures.AgenceParis}&date={demain}");

        evenements!.Select(e => e.Id)
            .Should()
            .ContainSingle()
            .Which.Should().Be(EvenementsFixtures.EvenementJobDatingLogistique);
    }

    [Fact]
    public async Task Etant_donne_une_carte_evenement_quand_la_reponse_est_consultee_alors_les_champs_attendus_sont_renseignes()
    {
        var aujourdhui = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd");

        var evenements = await _client.GetFromJsonAsync<List<Evenement>>(
            $"/api/evenements?agenceId={EvenementsFixtures.AgenceLyon}&date={aujourdhui}");

        var premier = evenements!.First();
        premier.Id.Should().NotBeNullOrWhiteSpace();
        premier.Titre.Should().NotBeNullOrWhiteSpace();
        premier.Organisme.Should().NotBeNullOrWhiteSpace();
        premier.Ville.Should().NotBeNullOrWhiteSpace();
        premier.Departement.Should().NotBeNullOrWhiteSpace();
        premier.HeureDebut.Should().MatchRegex(@"^\d{2}:\d{2}$");
        premier.HeureFin.Should().MatchRegex(@"^\d{2}:\d{2}$");
        premier.NombreInscrits.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task Etant_donne_aucun_parametre_agenceId_quand_on_demande_les_evenements_alors_toutes_les_agences_sont_renvoyees()
    {
        var aujourdhui = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd");

        var evenements = await _client.GetFromJsonAsync<List<Evenement>>(
            $"/api/evenements?date={aujourdhui}");

        evenements!.Should().OnlyContain(e => e.Date.ToString("yyyy-MM-dd") == aujourdhui);
    }
}
