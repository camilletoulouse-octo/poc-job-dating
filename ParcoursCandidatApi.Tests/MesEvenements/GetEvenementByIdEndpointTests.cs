using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ParcoursCandidatApi.Models;
using ParcoursCandidatApi.Tests.Fixtures;

namespace ParcoursCandidatApi.Tests.MesEvenements;

/// <summary>
/// Tests d'intégration pour l'endpoint <c>GET /api/evenements/{id}</c>
/// utilisé à l'ouverture du contexte d'un événement (US-1.03).
/// </summary>
public class GetEvenementByIdEndpointTests : IClassFixture<EvenementsApiFactory>
{
    private readonly HttpClient _client;

    public GetEvenementByIdEndpointTests(EvenementsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_d_evenement_existant_quand_on_le_demande_alors_la_reponse_est_200()
    {
        var response = await _client.GetAsync(
            $"/api/evenements/{EvenementsFixtures.EvenementJobDatingRestauration}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_d_evenement_existant_quand_on_le_demande_alors_les_infos_de_l_evenement_sont_renvoyees()
    {
        var evenement = await _client.GetFromJsonAsync<Evenement>(
            $"/api/evenements/{EvenementsFixtures.EvenementJobDatingRestauration}");

        evenement.Should().NotBeNull();
        evenement!.Id.Should().Be(EvenementsFixtures.EvenementJobDatingRestauration);
        evenement.Titre.Should().NotBeNullOrWhiteSpace();
        evenement.Organisme.Should().NotBeNullOrWhiteSpace();
        evenement.Ville.Should().NotBeNullOrWhiteSpace();
        evenement.HeureDebut.Should().MatchRegex(@"^\d{2}:\d{2}$");
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_d_evenement_inexistant_quand_on_le_demande_alors_la_reponse_est_404()
    {
        var response = await _client.GetAsync("/api/evenements/evt-inexistant");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
