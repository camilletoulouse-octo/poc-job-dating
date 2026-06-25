using FluentAssertions;
using ParcoursCandidatClient.Models;
using ParcoursCandidatClient.Services;
using ParcoursCandidatClient.Tests.Fixtures;

namespace ParcoursCandidatClient.Tests.Candidats;

/// <summary>
/// Tests unitaires du service <see cref="InscritService"/> consommé
/// par l'onglet "Les inscrits" (US-2.01).
/// </summary>
public class InscritServiceTests
{
    private const string BaseAddress = "http://localhost/";

    private static InscritService CreerService(MockHttpMessageHandler handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri(BaseAddress) });

    [Fact]
    public async Task Etant_donne_une_API_repondant_quatre_inscrits_quand_on_appelle_GetInscritsAsync_alors_les_inscrits_sont_desserialises()
    {
        var handler = new MockHttpMessageHandler(
            InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonQuatreInscrits));
        var service = CreerService(handler);

        var inscrits = await service.GetInscritsAsync(InscritsHttpFixtures.EvenementIdRestauration);

        inscrits.Should().HaveCount(4);
    }

    [Fact]
    public async Task Etant_donne_une_API_repondant_des_inscrits_quand_on_appelle_GetInscritsAsync_alors_le_statut_est_correctement_desserialise()
    {
        var handler = new MockHttpMessageHandler(
            InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonQuatreInscrits));
        var service = CreerService(handler);

        var inscrits = await service.GetInscritsAsync(InscritsHttpFixtures.EvenementIdRestauration);

        inscrits.Select(i => i.Statut).Should().Contain(new[]
        {
            StatutInscrit.PRESENT,
            StatutInscrit.ABSENT,
            StatutInscrit.INCONNU
        });
    }

    [Fact]
    public async Task Etant_donne_une_API_renvoyant_une_liste_vide_quand_on_appelle_GetInscritsAsync_alors_le_resultat_est_vide()
    {
        var handler = new MockHttpMessageHandler(InscritsHttpFixtures.ReponseListeVide());
        var service = CreerService(handler);

        var inscrits = await service.GetInscritsAsync(InscritsHttpFixtures.EvenementIdRestauration);

        inscrits.Should().BeEmpty();
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_d_evenement_quand_on_appelle_GetInscritsAsync_alors_la_route_appelee_contient_cet_identifiant()
    {
        var handler = new MockHttpMessageHandler(InscritsHttpFixtures.ReponseListeVide());
        var service = CreerService(handler);

        await service.GetInscritsAsync(InscritsHttpFixtures.EvenementIdRestauration);

        handler.DerniereRequeteUri!.AbsolutePath
            .Should().Be($"/api/evenements/{InscritsHttpFixtures.EvenementIdRestauration}/inscrits");
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_d_evenement_contenant_un_espace_quand_on_appelle_GetInscritsAsync_alors_il_est_correctement_encode()
    {
        var handler = new MockHttpMessageHandler(InscritsHttpFixtures.ReponseListeVide());
        var service = CreerService(handler);

        await service.GetInscritsAsync("evt avec espace");

        handler.DerniereRequeteUri!.AbsolutePath.Should().Contain("evt%20avec%20espace");
    }

    [Fact]
    public async Task Etant_donne_une_API_en_erreur_quand_on_appelle_GetInscritsAsync_alors_une_HttpRequestException_est_levee()
    {
        var handler = new MockHttpMessageHandler(InscritsHttpFixtures.ReponseErreurServeur());
        var service = CreerService(handler);

        var act = async () => await service.GetInscritsAsync(InscritsHttpFixtures.EvenementIdRestauration);

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    // ── US-2.04 : UpdateStatutAsync ─────────────────────────────────────────

    [Fact]
    public async Task Etant_donne_une_API_renvoyant_l_inscrit_mis_a_jour_quand_on_appelle_UpdateStatutAsync_alors_l_inscrit_est_desserialise()
    {
        var handler = new MockHttpMessageHandler(
            InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonInscritPresent));
        var service = CreerService(handler);

        var inscrit = await service.UpdateStatutAsync("ins-002", StatutInscrit.PRESENT);

        inscrit.Should().NotBeNull();
        inscrit!.Id.Should().Be("ins-002");
        inscrit.Statut.Should().Be(StatutInscrit.PRESENT);
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_d_inscrit_quand_on_appelle_UpdateStatutAsync_alors_la_route_appelee_contient_cet_identifiant()
    {
        var handler = new MockHttpMessageHandler(
            InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonInscritPresent));
        var service = CreerService(handler);

        await service.UpdateStatutAsync("ins-002", StatutInscrit.PRESENT);

        handler.DerniereRequeteUri!.AbsolutePath.Should().Be("/api/inscrits/ins-002");
    }

    [Fact]
    public async Task Etant_donne_un_identifiant_d_inscrit_quand_on_appelle_UpdateStatutAsync_alors_la_methode_HTTP_est_PATCH()
    {
        HttpMethod? methodeUtilisee = null;
        var handler = new MockHttpMessageHandler(req =>
        {
            methodeUtilisee = req.Method;
            return InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonInscritPresent);
        });
        var service = CreerService(handler);

        await service.UpdateStatutAsync("ins-002", StatutInscrit.PRESENT);

        methodeUtilisee.Should().Be(HttpMethod.Patch);
    }

    [Fact]
    public async Task Etant_donne_une_API_renvoyant_404_quand_on_appelle_UpdateStatutAsync_alors_null_est_renvoye()
    {
        var handler = new MockHttpMessageHandler(InscritsHttpFixtures.ReponseNotFound());
        var service = CreerService(handler);

        var inscrit = await service.UpdateStatutAsync("ins-inexistant", StatutInscrit.PRESENT);

        inscrit.Should().BeNull();
    }

    [Fact]
    public async Task Etant_donne_une_API_en_erreur_quand_on_appelle_UpdateStatutAsync_alors_une_HttpRequestException_est_levee()
    {
        var handler = new MockHttpMessageHandler(InscritsHttpFixtures.ReponseErreurServeur());
        var service = CreerService(handler);

        var act = async () => await service.UpdateStatutAsync("ins-002", StatutInscrit.PRESENT);

        await act.Should().ThrowAsync<HttpRequestException>();
    }
}
