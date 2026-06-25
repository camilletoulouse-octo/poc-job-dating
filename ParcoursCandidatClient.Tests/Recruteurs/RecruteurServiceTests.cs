using FluentAssertions;
using ParcoursCandidatClient.Models;
using ParcoursCandidatClient.Services;
using ParcoursCandidatClient.Tests.Fixtures;

namespace ParcoursCandidatClient.Tests.Recruteurs;

/// <summary>
/// Tests unitaires du service <see cref="RecruteurService"/> consommé
/// par l'onglet "Les recruteurs" (US-3.01).
/// </summary>
public class RecruteurServiceTests
{
    private const string BaseAddress = "http://localhost/";

    private static RecruteurService CreerService(MockHttpMessageHandler handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri(BaseAddress) });

    [Fact]
    public async Task Etant_donne_une_API_repondant_trois_recruteurs_quand_on_appelle_GetRecruteursAsync_alors_les_recruteurs_sont_desserialises()
    {
        var handler = new MockHttpMessageHandler(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonTroisRecruteurs));
        var service = CreerService(handler);

        var recruteurs = await service.GetRecruteursAsync(RecruteursHttpFixtures.EvenementIdRestauration);

        recruteurs.Should().HaveCount(3);
    }

    [Fact]
    public async Task Etant_donne_une_API_repondant_des_recruteurs_quand_on_appelle_GetRecruteursAsync_alors_le_statut_est_correctement_desserialise()
    {
        var handler = new MockHttpMessageHandler(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonTroisRecruteurs));
        var service = CreerService(handler);

        var recruteurs = await service.GetRecruteursAsync(RecruteursHttpFixtures.EvenementIdRestauration);

        recruteurs.Select(r => r.Statut).Should().Contain(new[]
        {
            StatutRecruteur.PRESENT,
            StatutRecruteur.ABSENT,
            StatutRecruteur.INCONNU
        });
    }

    [Fact]
    public async Task Etant_donne_une_API_repondant_des_recruteurs_quand_on_appelle_GetRecruteursAsync_alors_la_raison_sociale_est_correctement_deserialisee()
    {
        var handler = new MockHttpMessageHandler(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonTroisRecruteurs));
        var service = CreerService(handler);

        var recruteurs = await service.GetRecruteursAsync(RecruteursHttpFixtures.EvenementIdRestauration);

        recruteurs.First().RaisonSociale.Should().Be("Brasserie du Vieux Lyon");
    }

    [Fact]
    public async Task Etant_donne_une_API_renvoyant_une_liste_vide_quand_on_appelle_GetRecruteursAsync_alors_le_resultat_est_vide()
    {
        var handler = new MockHttpMessageHandler(RecruteursHttpFixtures.ReponseListeVide());
        var service = CreerService(handler);

        var recruteurs = await service.GetRecruteursAsync(RecruteursHttpFixtures.EvenementIdRestauration);

        recruteurs.Should().BeEmpty();
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_d_evenement_quand_on_appelle_GetRecruteursAsync_alors_la_route_appelee_contient_cet_identifiant()
    {
        var handler = new MockHttpMessageHandler(RecruteursHttpFixtures.ReponseListeVide());
        var service = CreerService(handler);

        await service.GetRecruteursAsync(RecruteursHttpFixtures.EvenementIdRestauration);

        handler.DerniereRequeteUri!.AbsolutePath
            .Should().Be($"/api/evenements/{RecruteursHttpFixtures.EvenementIdRestauration}/recruteurs");
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_d_evenement_contenant_un_espace_quand_on_appelle_GetRecruteursAsync_alors_il_est_correctement_encode()
    {
        var handler = new MockHttpMessageHandler(RecruteursHttpFixtures.ReponseListeVide());
        var service = CreerService(handler);

        await service.GetRecruteursAsync("evt avec espace");

        handler.DerniereRequeteUri!.AbsolutePath.Should().Contain("evt%20avec%20espace");
    }

    [Fact]
    public async Task Etant_donne_une_API_en_erreur_quand_on_appelle_GetRecruteursAsync_alors_une_HttpRequestException_est_levee()
    {
        var handler = new MockHttpMessageHandler(RecruteursHttpFixtures.ReponseErreurServeur());
        var service = CreerService(handler);

        var act = async () => await service.GetRecruteursAsync(RecruteursHttpFixtures.EvenementIdRestauration);

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    // ── US-3.04 : UpdateStatutAsync ─────────────────────────────────────────

    [Fact]
    public async Task Etant_donne_une_API_renvoyant_le_recruteur_mis_a_jour_quand_on_appelle_UpdateStatutAsync_alors_le_recruteur_est_desserialise()
    {
        var handler = new MockHttpMessageHandler(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonRecruteurPresent));
        var service = CreerService(handler);

        var recruteur = await service.UpdateStatutAsync("rec-001", StatutRecruteur.PRESENT);

        recruteur.Should().NotBeNull();
        recruteur!.Id.Should().Be("rec-001");
        recruteur.Statut.Should().Be(StatutRecruteur.PRESENT);
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_de_recruteur_quand_on_appelle_UpdateStatutAsync_alors_la_route_appelee_contient_cet_identifiant()
    {
        var handler = new MockHttpMessageHandler(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonRecruteurPresent));
        var service = CreerService(handler);

        await service.UpdateStatutAsync("rec-001", StatutRecruteur.PRESENT);

        handler.DerniereRequeteUri!.AbsolutePath.Should().Be("/api/recruteurs/rec-001");
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_de_recruteur_quand_on_appelle_UpdateStatutAsync_alors_la_methode_HTTP_est_PATCH()
    {
        HttpMethod? methodeUtilisee = null;
        var handler = new MockHttpMessageHandler(req =>
        {
            methodeUtilisee = req.Method;
            return RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonRecruteurPresent);
        });
        var service = CreerService(handler);

        await service.UpdateStatutAsync("rec-001", StatutRecruteur.PRESENT);

        methodeUtilisee.Should().Be(HttpMethod.Patch);
    }

    [Fact]
    public async Task Etant_donne_une_API_renvoyant_404_quand_on_appelle_UpdateStatutAsync_alors_null_est_renvoye()
    {
        var handler = new MockHttpMessageHandler(RecruteursHttpFixtures.ReponseNotFound());
        var service = CreerService(handler);

        var recruteur = await service.UpdateStatutAsync("rec-inexistant", StatutRecruteur.PRESENT);

        recruteur.Should().BeNull();
    }

    [Fact]
    public async Task Etant_donne_une_API_en_erreur_quand_on_appelle_UpdateStatutAsync_alors_une_HttpRequestException_est_levee()
    {
        var handler = new MockHttpMessageHandler(RecruteursHttpFixtures.ReponseErreurServeur());
        var service = CreerService(handler);

        var act = async () => await service.UpdateStatutAsync("rec-001", StatutRecruteur.PRESENT);

        await act.Should().ThrowAsync<HttpRequestException>();
    }
}
