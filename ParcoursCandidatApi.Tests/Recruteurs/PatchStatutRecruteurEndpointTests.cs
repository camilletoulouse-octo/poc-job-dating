using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ParcoursCandidatApi.Models;
using ParcoursCandidatApi.Tests.Fixtures;

namespace ParcoursCandidatApi.Tests.Recruteurs;

/// <summary>
/// Tests d'intégration pour l'endpoint <c>PATCH /api/recruteurs/{id}</c>
/// utilisé par l'action sheet de modification de statut (US-3.04).
/// </summary>
public class PatchStatutRecruteurEndpointTests : IClassFixture<EvenementsApiFactory>
{
    private readonly HttpClient _client;

    public PatchStatutRecruteurEndpointTests(EvenementsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Etant_donne_un_recruteur_existant_quand_on_modifie_son_statut_alors_la_reponse_est_200()
    {
        var body = new { statut = "PRESENT" };

        var response = await _client.PatchAsJsonAsync(
            $"/api/recruteurs/{RecruteursFixtures.RecruteursRestaurationTriesParRaisonSociale[0]}", body);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Etant_donne_un_recruteur_existant_quand_on_lui_affecte_le_statut_Absent_alors_le_recruteur_renvoye_a_le_statut_Absent()
    {
        var recruteurId = RecruteursFixtures.RecruteursRestaurationTriesParRaisonSociale[1];
        var body = new { statut = "ABSENT" };

        var recruteur = await _client.PatchAsJsonAsync($"/api/recruteurs/{recruteurId}", body)
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<Recruteur>())
            .Unwrap();

        recruteur!.Statut.Should().Be(StatutRecruteur.ABSENT);
        recruteur.Id.Should().Be(recruteurId);
    }

    [Fact]
    public async Task Etant_donne_un_recruteur_existant_quand_on_lui_affecte_le_statut_Present_alors_le_recruteur_renvoye_a_le_statut_Present()
    {
        var recruteurId = RecruteursFixtures.RecruteursRestaurationTriesParRaisonSociale[2];
        var body = new { statut = "PRESENT" };

        var recruteur = await _client.PatchAsJsonAsync($"/api/recruteurs/{recruteurId}", body)
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<Recruteur>())
            .Unwrap();

        recruteur!.Statut.Should().Be(StatutRecruteur.PRESENT);
    }

    [Fact]
    public async Task Etant_donne_un_recruteur_existant_quand_on_lui_affecte_le_statut_Inconnu_alors_le_recruteur_renvoye_a_le_statut_Inconnu()
    {
        var recruteurId = RecruteursFixtures.RecruteursRestaurationTriesParRaisonSociale[3];
        var body = new { statut = "INCONNU" };

        var recruteur = await _client.PatchAsJsonAsync($"/api/recruteurs/{recruteurId}", body)
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<Recruteur>())
            .Unwrap();

        recruteur!.Statut.Should().Be(StatutRecruteur.INCONNU);
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_inexistant_quand_on_modifie_le_statut_alors_la_reponse_est_404()
    {
        var body = new { statut = "PRESENT" };

        var response = await _client.PatchAsJsonAsync(
            $"/api/recruteurs/{RecruteursFixtures.EvenementInexistant}", body);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
