using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ParcoursCandidatApi.Models;
using ParcoursCandidatApi.Tests.Fixtures;

namespace ParcoursCandidatApi.Tests.Recruteurs;

/// <summary>
/// Tests d'intégration pour l'endpoint <c>GET /api/evenements/{id}/recruteurs</c>
/// utilisé par l'onglet "Les recruteurs" (US-3.01).
/// </summary>
public class GetRecruteursEndpointTests : IClassFixture<EvenementsApiFactory>
{
    private readonly HttpClient _client;

    public GetRecruteursEndpointTests(EvenementsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Etant_donne_un_evenement_existant_quand_on_demande_ses_recruteurs_alors_la_reponse_est_200()
    {
        var response = await _client.GetAsync(
            $"/api/evenements/{RecruteursFixtures.EvenementJobDatingRestauration}/recruteurs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Etant_donne_l_evenement_job_dating_restauration_quand_on_demande_ses_recruteurs_alors_les_cinq_recruteurs_sont_renvoyes()
    {
        var recruteurs = await _client.GetFromJsonAsync<List<Recruteur>>(
            $"/api/evenements/{RecruteursFixtures.EvenementJobDatingRestauration}/recruteurs");

        recruteurs.Should().HaveCount(RecruteursFixtures.NombreRecruteursRestauration);
    }

    [Fact]
    public async Task Etant_donne_un_evenement_existant_quand_on_demande_ses_recruteurs_alors_ils_sont_tous_rattaches_a_cet_evenement()
    {
        var recruteurs = await _client.GetFromJsonAsync<List<Recruteur>>(
            $"/api/evenements/{RecruteursFixtures.EvenementJobDatingRestauration}/recruteurs");

        recruteurs!.Should().OnlyContain(r =>
            r.EvenementId == RecruteursFixtures.EvenementJobDatingRestauration);
    }

    [Fact]
    public async Task Etant_donne_plusieurs_recruteurs_quand_la_liste_est_renvoyee_alors_elle_est_triee_par_raison_sociale()
    {
        var recruteurs = await _client.GetFromJsonAsync<List<Recruteur>>(
            $"/api/evenements/{RecruteursFixtures.EvenementJobDatingRestauration}/recruteurs");

        recruteurs!.Select(r => r.Id)
            .Should()
            .ContainInOrder(RecruteursFixtures.RecruteursRestaurationTriesParRaisonSociale);
    }

    [Fact]
    public async Task Etant_donne_un_evenement_sans_recruteur_quand_on_demande_ses_recruteurs_alors_la_liste_renvoyee_est_vide()
    {
        var recruteurs = await _client.GetFromJsonAsync<List<Recruteur>>(
            $"/api/evenements/{RecruteursFixtures.EvenementSansRecruteur}/recruteurs");

        recruteurs.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_d_evenement_inexistant_quand_on_demande_ses_recruteurs_alors_la_liste_renvoyee_est_vide()
    {
        var recruteurs = await _client.GetFromJsonAsync<List<Recruteur>>(
            $"/api/evenements/{RecruteursFixtures.EvenementInexistant}/recruteurs");

        recruteurs.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Etant_donne_un_recruteur_renvoye_quand_la_reponse_est_consultee_alors_les_champs_attendus_sont_renseignes()
    {
        var recruteurs = await _client.GetFromJsonAsync<List<Recruteur>>(
            $"/api/evenements/{RecruteursFixtures.EvenementJobDatingRestauration}/recruteurs");

        var premier = recruteurs!.First();
        premier.Id.Should().NotBeNullOrWhiteSpace();
        premier.EvenementId.Should().NotBeNullOrWhiteSpace();
        premier.Nom.Should().NotBeNullOrWhiteSpace();
        premier.Prenom.Should().NotBeNullOrWhiteSpace();
        premier.RaisonSociale.Should().NotBeNullOrWhiteSpace();
        premier.NombreOffres.Should().BeGreaterThanOrEqualTo(0);
        premier.Statut.Should().BeOneOf(StatutRecruteur.PRESENT, StatutRecruteur.ABSENT, StatutRecruteur.INCONNU);
    }

    [Fact]
    public async Task Etant_donne_l_evenement_restauration_quand_on_demande_ses_recruteurs_alors_le_nombre_de_presents_est_correct()
    {
        var recruteurs = await _client.GetFromJsonAsync<List<Recruteur>>(
            $"/api/evenements/{RecruteursFixtures.EvenementJobDatingRestauration}/recruteurs");

        recruteurs!.Count(r => r.Statut == StatutRecruteur.PRESENT)
            .Should().Be(RecruteursFixtures.NombrePresentsRestauration);
    }
}
