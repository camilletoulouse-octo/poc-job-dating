using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ParcoursCandidatApi.Models;
using ParcoursCandidatApi.Tests.Fixtures;

namespace ParcoursCandidatApi.Tests.Candidats;

/// <summary>
/// Tests d'intégration pour l'endpoint <c>PATCH /api/inscrits/{id}</c>
/// utilisé par l'action sheet de modification de statut (US-2.04).
/// </summary>
public class PatchStatutInscritEndpointTests : IClassFixture<EvenementsApiFactory>
{
    private readonly HttpClient _client;

    public PatchStatutInscritEndpointTests(EvenementsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Etant_donne_un_inscrit_existant_quand_on_modifie_son_statut_alors_la_reponse_est_200()
    {
        var body = new { statut = "PRESENT" };

        var response = await _client.PatchAsJsonAsync(
            $"/api/inscrits/{InscritsFixtures.InscritsRestaurationTriesParNom[0]}", body);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Etant_donne_un_inscrit_existant_quand_on_lui_affecte_le_statut_Absent_alors_l_inscrit_renvoye_a_le_statut_Absent()
    {
        var inscritId = InscritsFixtures.InscritsRestaurationTriesParNom[1];
        var body = new { statut = "ABSENT" };

        var inscrit = await _client.PatchAsJsonAsync($"/api/inscrits/{inscritId}", body)
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<Inscrit>())
            .Unwrap();

        inscrit!.Statut.Should().Be(StatutInscrit.ABSENT);
        inscrit.Id.Should().Be(inscritId);
    }

    [Fact]
    public async Task Etant_donne_un_inscrit_existant_quand_on_lui_affecte_le_statut_Present_alors_l_inscrit_renvoye_a_le_statut_Present()
    {
        var inscritId = InscritsFixtures.InscritsRestaurationTriesParNom[2];
        var body = new { statut = "PRESENT" };

        var inscrit = await _client.PatchAsJsonAsync($"/api/inscrits/{inscritId}", body)
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<Inscrit>())
            .Unwrap();

        inscrit!.Statut.Should().Be(StatutInscrit.PRESENT);
    }

    [Fact]
    public async Task Etant_donne_un_inscrit_existant_quand_on_lui_affecte_le_statut_Inconnu_alors_l_inscrit_renvoye_a_le_statut_Inconnu()
    {
        var inscritId = InscritsFixtures.InscritsRestaurationTriesParNom[3];
        var body = new { statut = "INCONNU" };

        var inscrit = await _client.PatchAsJsonAsync($"/api/inscrits/{inscritId}", body)
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<Inscrit>())
            .Unwrap();

        inscrit!.Statut.Should().Be(StatutInscrit.INCONNU);
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_inexistant_quand_on_modifie_le_statut_alors_la_reponse_est_404()
    {
        var body = new { statut = "PRESENT" };

        var response = await _client.PatchAsJsonAsync(
            $"/api/inscrits/{InscritsFixtures.EvenementInexistant}", body);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
