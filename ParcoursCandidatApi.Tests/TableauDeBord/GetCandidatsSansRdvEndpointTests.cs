using System.Net;
using System.Text.Json;
using FluentAssertions;
using ParcoursCandidatApi.Tests.Fixtures;

namespace ParcoursCandidatApi.Tests.TableauDeBord;

/// <summary>
/// Tests d'intégration pour l'endpoint <c>GET /api/evenements/{id}/candidats-sans-rdv</c>
/// utilisé par la page "Candidats sans RDV" (US-5.08).
/// Les candidats sans RDV sont calculés dynamiquement depuis inscrits.json
/// en filtrant les inscrits avec statut INCONNU.
/// </summary>
public class GetCandidatsSansRdvEndpointTests : IClassFixture<EvenementsApiFactory>
{
    private readonly HttpClient _client;

    public GetCandidatsSansRdvEndpointTests(EvenementsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Etant_donne_un_evenement_existant_quand_on_demande_ses_candidats_sans_rdv_alors_la_reponse_est_200()
    {
        var response = await _client.GetAsync(
            $"/api/evenements/{CandidatsSansRdvFixtures.EvenementJobDatingRestauration}/candidats-sans-rdv");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Etant_donne_l_evenement_job_dating_restauration_quand_on_demande_ses_candidats_sans_rdv_alors_les_trois_candidats_sont_renvoyes()
    {
        var json = await _client.GetStringAsync(
            $"/api/evenements/{CandidatsSansRdvFixtures.EvenementJobDatingRestauration}/candidats-sans-rdv");

        using var doc = JsonDocument.Parse(json);
        var candidats = doc.RootElement.GetProperty("candidats").EnumerateArray().ToList();

        candidats.Should().HaveCount(CandidatsSansRdvFixtures.NombreCandidatsSansRdvRestauration);
    }

    [Fact]
    public async Task Etant_donne_un_evenement_existant_quand_on_demande_ses_candidats_sans_rdv_alors_l_evenement_id_est_correct()
    {
        var json = await _client.GetStringAsync(
            $"/api/evenements/{CandidatsSansRdvFixtures.EvenementJobDatingRestauration}/candidats-sans-rdv");

        using var doc = JsonDocument.Parse(json);
        var evenementId = doc.RootElement.GetProperty("evenementId").GetString();

        evenementId.Should().Be(CandidatsSansRdvFixtures.EvenementJobDatingRestauration);
    }

    [Fact]
    public async Task Etant_donne_un_candidat_renvoye_quand_la_reponse_est_consultee_alors_les_champs_attendus_sont_renseignes()
    {
        var json = await _client.GetStringAsync(
            $"/api/evenements/{CandidatsSansRdvFixtures.EvenementJobDatingRestauration}/candidats-sans-rdv");

        using var doc = JsonDocument.Parse(json);
        var premier = doc.RootElement.GetProperty("candidats").EnumerateArray().First();

        premier.GetProperty("id").GetString().Should().NotBeNullOrWhiteSpace();
        premier.GetProperty("nom").GetString().Should().Be(CandidatsSansRdvFixtures.PremierCandidatNom);
        premier.GetProperty("prenom").GetString().Should().Be(CandidatsSansRdvFixtures.PremierCandidatPrenom);
        premier.GetProperty("telephone").GetString().Should().Be(CandidatsSansRdvFixtures.PremierCandidatTelephone);
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_d_evenement_inexistant_quand_on_demande_ses_candidats_sans_rdv_alors_la_liste_est_vide()
    {
        var json = await _client.GetStringAsync(
            $"/api/evenements/{CandidatsSansRdvFixtures.EvenementInexistant}/candidats-sans-rdv");

        using var doc = JsonDocument.Parse(json);
        var candidats = doc.RootElement.GetProperty("candidats").EnumerateArray().ToList();

        candidats.Should().BeEmpty();
    }
}
