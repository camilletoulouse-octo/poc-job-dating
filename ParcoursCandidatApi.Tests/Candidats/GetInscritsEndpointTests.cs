using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ParcoursCandidatApi.Models;
using ParcoursCandidatApi.Tests.Fixtures;

namespace ParcoursCandidatApi.Tests.Candidats;

/// <summary>
/// Tests d'intégration pour l'endpoint <c>GET /api/evenements/{id}/inscrits</c>
/// utilisé par l'onglet "Les inscrits" (US-2.01).
/// </summary>
public class GetInscritsEndpointTests : IClassFixture<EvenementsApiFactory>
{
    private readonly HttpClient _client;

    public GetInscritsEndpointTests(EvenementsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Etant_donne_un_evenement_existant_quand_on_demande_ses_inscrits_alors_la_reponse_est_200()
    {
        var response = await _client.GetAsync(
            $"/api/evenements/{InscritsFixtures.EvenementJobDatingRestauration}/inscrits");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Etant_donne_l_evenement_job_dating_restauration_quand_on_demande_ses_inscrits_alors_les_sept_inscrits_sont_renvoyes()
    {
        var inscrits = await _client.GetFromJsonAsync<List<Inscrit>>(
            $"/api/evenements/{InscritsFixtures.EvenementJobDatingRestauration}/inscrits");

        inscrits.Should().HaveCount(InscritsFixtures.NombreInscritsRestauration);
    }

    [Fact]
    public async Task Etant_donne_un_evenement_existant_quand_on_demande_ses_inscrits_alors_ils_sont_tous_rattaches_a_cet_evenement()
    {
        var inscrits = await _client.GetFromJsonAsync<List<Inscrit>>(
            $"/api/evenements/{InscritsFixtures.EvenementJobDatingRestauration}/inscrits");

        inscrits!.Should().OnlyContain(i =>
            i.EvenementId == InscritsFixtures.EvenementJobDatingRestauration);
    }

    [Fact]
    public async Task Etant_donne_plusieurs_inscrits_quand_la_liste_est_renvoyee_alors_elle_est_triee_par_nom_de_famille()
    {
        var inscrits = await _client.GetFromJsonAsync<List<Inscrit>>(
            $"/api/evenements/{InscritsFixtures.EvenementJobDatingRestauration}/inscrits");

        inscrits!.Select(i => i.Id)
            .Should()
            .ContainInOrder(InscritsFixtures.InscritsRestaurationTriesParNom);
    }

    [Fact]
    public async Task Etant_donne_un_evenement_sans_inscrit_quand_on_demande_ses_inscrits_alors_la_liste_renvoyee_est_vide()
    {
        var inscrits = await _client.GetFromJsonAsync<List<Inscrit>>(
            $"/api/evenements/{InscritsFixtures.EvenementSansInscrit}/inscrits");

        inscrits.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_d_evenement_inexistant_quand_on_demande_ses_inscrits_alors_la_liste_renvoyee_est_vide()
    {
        var inscrits = await _client.GetFromJsonAsync<List<Inscrit>>(
            $"/api/evenements/{InscritsFixtures.EvenementInexistant}/inscrits");

        inscrits.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Etant_donne_un_inscrit_renvoye_quand_la_reponse_est_consultee_alors_les_champs_attendus_sont_renseignes()
    {
        var inscrits = await _client.GetFromJsonAsync<List<Inscrit>>(
            $"/api/evenements/{InscritsFixtures.EvenementJobDatingRestauration}/inscrits");

        var premier = inscrits!.First();
        premier.Id.Should().NotBeNullOrWhiteSpace();
        premier.EvenementId.Should().NotBeNullOrWhiteSpace();
        premier.Nom.Should().NotBeNullOrWhiteSpace();
        premier.Prenom.Should().NotBeNullOrWhiteSpace();
        premier.Statut.Should().BeOneOf(StatutInscrit.PRESENT, StatutInscrit.ABSENT, StatutInscrit.INCONNU);
    }
}
