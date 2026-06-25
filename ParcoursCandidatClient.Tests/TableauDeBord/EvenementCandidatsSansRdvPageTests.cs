using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ParcoursCandidatClient.Services;
using ParcoursCandidatClient.Tests.Fixtures;
using EvenementCandidatsSansRdvPage = ParcoursCandidatClient.Pages.EvenementCandidatsSansRdv;

namespace ParcoursCandidatClient.Tests.TableauDeBord;

/// <summary>
/// Tests E2E de la page <see cref="EvenementCandidatsSansRdvPage"/>
/// rendue avec bUnit.
/// Couvre US-5.08 : liste des candidats sans entretien planifié.
/// </summary>
public class EvenementCandidatsSansRdvPageTests : TestContext
{
    // ── Helpers ─────────────────────────────────────────────────────────────

    private void EnregistrerServices(
        HttpResponseMessage evenementResponse,
        HttpResponseMessage? candidatsSansRdvResponse = null)
    {
        var candidatsResponse = candidatsSansRdvResponse
            ?? CandidatsSansRdvHttpFixtures.ReponseNotFound();

        var handler = new MockHttpMessageHandler(req =>
        {
            var path = req.RequestUri?.AbsolutePath ?? "";

            if (path.Contains("/candidats-sans-rdv"))
                return candidatsResponse;

            return evenementResponse;
        });

        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
        Services.AddSingleton(new EvenementService(httpClient));
        Services.AddSingleton<ICandidatsSansRdvService>(new CandidatsSansRdvService(httpClient));
        Services.AddSingleton(httpClient);
    }

    // ── En-tête ─────────────────────────────────────────────────────────────

    [Fact]
    public void Etant_donne_la_page_candidats_sans_rdv_quand_elle_est_chargee_alors_le_titre_candidats_sans_rdv_est_affiche()
    {
        EnregistrerServices(
            InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration),
            CandidatsSansRdvHttpFixtures.ReponseOk(CandidatsSansRdvHttpFixtures.ReponseJsonCandidatsSansRdvEvt001));

        var cut = RenderComponent<EvenementCandidatsSansRdvPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".csr-header-titre").Any());

        cut.Find(".csr-header-titre").TextContent.Trim()
            .Should().Be("Candidats sans RDV");
    }

    [Fact]
    public void Etant_donne_la_page_candidats_sans_rdv_quand_elle_est_chargee_alors_le_lien_retour_pointe_vers_le_tableau_de_bord()
    {
        EnregistrerServices(
            InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration),
            CandidatsSansRdvHttpFixtures.ReponseOk(CandidatsSansRdvHttpFixtures.ReponseJsonCandidatsSansRdvEvt001));

        var cut = RenderComponent<EvenementCandidatsSansRdvPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".csr-header-back").Any());

        cut.Find(".csr-header-back")
            .GetAttribute("href")
            .Should().Be("/evenements/evt-001/tableau-de-bord");
    }

    // ── Liste des candidats ──────────────────────────────────────────────────

    [Fact]
    public void Etant_donne_des_candidats_sans_rdv_quand_la_page_est_chargee_alors_les_trois_candidats_sont_affiches()
    {
        EnregistrerServices(
            InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration),
            CandidatsSansRdvHttpFixtures.ReponseOk(CandidatsSansRdvHttpFixtures.ReponseJsonCandidatsSansRdvEvt001));

        var cut = RenderComponent<EvenementCandidatsSansRdvPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".csr-candidat-carte").Any());

        cut.FindAll(".csr-candidat-carte").Should().HaveCount(3);
    }

    [Fact]
    public void Etant_donne_des_candidats_sans_rdv_quand_la_page_est_chargee_alors_le_nom_du_premier_candidat_est_affiche_en_majuscules()
    {
        EnregistrerServices(
            InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration),
            CandidatsSansRdvHttpFixtures.ReponseOk(CandidatsSansRdvHttpFixtures.ReponseJsonCandidatsSansRdvEvt001));

        var cut = RenderComponent<EvenementCandidatsSansRdvPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".csr-candidat-nom").Any());

        cut.FindAll(".csr-candidat-nom").First().TextContent.Trim()
            .Should().Be("LEROY CAMILLE");
    }

    [Fact]
    public void Etant_donne_des_candidats_sans_rdv_quand_la_page_est_chargee_alors_chaque_candidat_a_un_bouton_appel()
    {
        EnregistrerServices(
            InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration),
            CandidatsSansRdvHttpFixtures.ReponseOk(CandidatsSansRdvHttpFixtures.ReponseJsonCandidatsSansRdvEvt001));

        var cut = RenderComponent<EvenementCandidatsSansRdvPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".csr-candidat-appel").Any());

        cut.FindAll(".csr-candidat-appel").Should().HaveCount(3);
    }

    [Fact]
    public void Etant_donne_des_candidats_sans_rdv_quand_la_page_est_chargee_alors_le_bouton_appel_contient_le_numero_de_telephone()
    {
        EnregistrerServices(
            InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration),
            CandidatsSansRdvHttpFixtures.ReponseOk(CandidatsSansRdvHttpFixtures.ReponseJsonCandidatsSansRdvEvt001));

        var cut = RenderComponent<EvenementCandidatsSansRdvPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".csr-candidat-appel").Any());

        cut.FindAll(".csr-candidat-appel").First()
            .GetAttribute("href")
            .Should().Be("tel:+33607080910");
    }

    // ── État vide ────────────────────────────────────────────────────────────

    [Fact]
    public void Etant_donne_aucun_candidat_quand_la_page_est_chargee_alors_le_message_aucun_candidat_sans_rdv_est_affiche()
    {
        EnregistrerServices(
            InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration),
            CandidatsSansRdvHttpFixtures.ReponseOk(CandidatsSansRdvHttpFixtures.ReponseJsonCandidatsSansRdvVide));

        var cut = RenderComponent<EvenementCandidatsSansRdvPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".csr-vide-message").Any());

        cut.Find(".csr-vide-message").TextContent.Trim()
            .Should().Be("Aucun candidat sans RDV.");
    }

    // ── État d'erreur ────────────────────────────────────────────────────────

    [Fact]
    public void Etant_donne_une_erreur_serveur_quand_la_page_est_chargee_alors_le_message_d_erreur_est_affiche()
    {
        EnregistrerServices(
            InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration),
            CandidatsSansRdvHttpFixtures.ReponseErreurServeur());

        var cut = RenderComponent<EvenementCandidatsSansRdvPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".csr-erreur").Any());

        cut.Find(".csr-erreur-message").TextContent.Trim()
            .Should().Be("Impossible de charger les candidats sans RDV.");
    }

    [Fact]
    public void Etant_donne_une_erreur_serveur_quand_la_page_est_chargee_alors_le_bouton_reessayer_est_affiche()
    {
        EnregistrerServices(
            InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration),
            CandidatsSansRdvHttpFixtures.ReponseErreurServeur());

        var cut = RenderComponent<EvenementCandidatsSansRdvPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".csr-erreur-reessayer").Any());

        cut.Find(".csr-erreur-reessayer").Should().NotBeNull();
    }
}
