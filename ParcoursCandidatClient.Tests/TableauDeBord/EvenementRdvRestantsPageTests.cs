using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ParcoursCandidatClient.Services;
using ParcoursCandidatClient.Tests.Fixtures;
using EvenementRdvRestantsPage = ParcoursCandidatClient.Pages.EvenementRdvRestants;

namespace ParcoursCandidatClient.Tests.TableauDeBord;

/// <summary>
/// Tests E2E de la page <see cref="EvenementRdvRestantsPage"/>
/// rendue avec bUnit.
/// Couvre US-5.06 : liste des recruteurs avec accordéon des créneaux RDV restants.
/// </summary>
public class EvenementRdvRestantsPageTests : TestContext
{
    // ── Helpers ─────────────────────────────────────────────────────────────

    private void EnregistrerServices(
        HttpResponseMessage evenementResponse,
        HttpResponseMessage? rdvRestantsResponse = null)
    {
        var rdvResponse = rdvRestantsResponse
            ?? RdvRestantsHttpFixtures.ReponseNotFound();

        var handler = new MockHttpMessageHandler(req =>
        {
            var path = req.RequestUri?.AbsolutePath ?? "";

            if (path.Contains("/rdv-restants"))
                return rdvResponse;

            return evenementResponse;
        });

        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
        Services.AddSingleton(new EvenementService(httpClient));
        Services.AddSingleton<IRdvRestantsService>(new RdvRestantsService(httpClient));
        Services.AddSingleton(httpClient);
    }

    // ── En-tête ─────────────────────────────────────────────────────────────

    [Fact]
    public void Etant_donne_la_page_rdv_restants_quand_elle_est_chargee_alors_le_titre_rdv_restants_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseOk(RdvRestantsHttpFixtures.ReponseJsonRdvRestantsEvt001));

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-header-titre").Any());

        cut.Find(".rdv-header-titre").TextContent.Trim()
            .Should().Be("RDV restants");
    }

    [Fact]
    public void Etant_donne_la_page_rdv_restants_quand_elle_est_chargee_alors_le_lien_retour_pointe_vers_le_tableau_de_bord()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseOk(RdvRestantsHttpFixtures.ReponseJsonRdvRestantsEvt001));

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-header-back").Any());

        cut.Find(".rdv-header-back")
            .GetAttribute("href")
            .Should().Be("/evenements/evt-001/tableau-de-bord");
    }

    // ── Barre de recherche ───────────────────────────────────────────────────

    [Fact]
    public void Etant_donne_la_page_rdv_restants_quand_elle_est_chargee_alors_la_barre_de_recherche_est_affichee()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseOk(RdvRestantsHttpFixtures.ReponseJsonRdvRestantsEvt001));

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-recherche-input").Any());

        cut.Find(".rdv-recherche-input").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_la_page_rdv_restants_quand_elle_est_chargee_alors_le_placeholder_rechercher_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseOk(RdvRestantsHttpFixtures.ReponseJsonRdvRestantsEvt001));

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-recherche-input").Any());

        cut.Find(".rdv-recherche-input")
            .GetAttribute("placeholder")
            .Should().Be("Rechercher");
    }

    // ── Liste des recruteurs ─────────────────────────────────────────────────

    [Fact]
    public void Etant_donne_des_rdv_restants_quand_la_page_est_chargee_alors_les_quatre_recruteurs_sont_affiches()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseOk(RdvRestantsHttpFixtures.ReponseJsonRdvRestantsEvt001));

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-recruteur-bloc").Any());

        cut.FindAll(".rdv-recruteur-bloc").Should().HaveCount(4);
    }

    [Fact]
    public void Etant_donne_des_rdv_restants_quand_la_page_est_chargee_alors_le_nom_du_premier_recruteur_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseOk(RdvRestantsHttpFixtures.ReponseJsonRdvRestantsEvt001));

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-recruteur-nom").Any());

        cut.FindAll(".rdv-recruteur-nom").First().TextContent.Trim()
            .Should().Be("Marie DUPONT");
    }

    [Fact]
    public void Etant_donne_des_rdv_restants_quand_la_page_est_chargee_alors_la_raison_sociale_du_premier_recruteur_est_affichee()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseOk(RdvRestantsHttpFixtures.ReponseJsonRdvRestantsEvt001));

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-recruteur-societe").Any());

        cut.FindAll(".rdv-recruteur-societe").First().TextContent.Trim()
            .Should().Be("BRASSERIE DU VIEUX LYON");
    }

    // ── Accordéon ────────────────────────────────────────────────────────────

    [Fact]
    public void Etant_donne_un_recruteur_ferme_quand_la_page_est_chargee_alors_ses_creneaux_ne_sont_pas_affiches()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseOk(RdvRestantsHttpFixtures.ReponseJsonRdvRestantsEvt001));

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-recruteur-bloc").Any());

        cut.FindAll(".rdv-creneaux-liste").Should().BeEmpty();
    }

    [Fact]
    public void Etant_donne_un_recruteur_ferme_quand_on_clique_dessus_alors_ses_creneaux_sont_affiches()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseOk(RdvRestantsHttpFixtures.ReponseJsonRdvRestantsEvt001));

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-recruteur-entete").Any());

        cut.FindAll(".rdv-recruteur-entete").First().Click();
        cut.WaitForState(() => cut.FindAll(".rdv-creneaux-liste").Any());

        cut.FindAll(".rdv-creneaux-liste").Should().HaveCount(1);
    }

    [Fact]
    public void Etant_donne_un_recruteur_ouvert_quand_on_clique_a_nouveau_alors_ses_creneaux_sont_masques()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseOk(RdvRestantsHttpFixtures.ReponseJsonRdvRestantsEvt001));

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-recruteur-entete").Any());

        cut.FindAll(".rdv-recruteur-entete").First().Click();
        cut.WaitForState(() => cut.FindAll(".rdv-creneaux-liste").Any());

        cut.FindAll(".rdv-recruteur-entete").First().Click();
        cut.WaitForState(() => !cut.FindAll(".rdv-creneaux-liste").Any());

        cut.FindAll(".rdv-creneaux-liste").Should().BeEmpty();
    }

    [Fact]
    public void Etant_donne_le_premier_recruteur_ouvert_quand_ses_creneaux_sont_affiches_alors_les_heures_sont_correctes()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseOk(RdvRestantsHttpFixtures.ReponseJsonRdvRestantsEvt001));

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-recruteur-entete").Any());

        cut.FindAll(".rdv-recruteur-entete").First().Click();
        cut.WaitForState(() => cut.FindAll(".rdv-creneau-heure").Any());

        var heures = cut.FindAll(".rdv-creneau-heure")
            .Select(e => e.TextContent.Trim())
            .ToList();

        heures.Should().Contain("10:00");
        heures.Should().Contain("10:15");
    }

    [Fact]
    public void Etant_donne_le_premier_recruteur_ouvert_quand_ses_creneaux_sont_affiches_alors_les_noms_candidats_sont_corrects()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseOk(RdvRestantsHttpFixtures.ReponseJsonRdvRestantsEvt001));

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-recruteur-entete").Any());

        cut.FindAll(".rdv-recruteur-entete").First().Click();
        cut.WaitForState(() => cut.FindAll(".rdv-creneau-candidat-nom").Any());

        var noms = cut.FindAll(".rdv-creneau-candidat-nom")
            .Select(e => e.TextContent.Trim())
            .ToList();

        noms.Should().Contain("ROBERT");
        noms.Should().Contain("PETIT");
    }

    [Fact]
    public void Etant_donne_le_premier_recruteur_ouvert_quand_ses_creneaux_sont_affiches_alors_les_prenoms_candidats_sont_corrects()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseOk(RdvRestantsHttpFixtures.ReponseJsonRdvRestantsEvt001));

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-recruteur-entete").Any());

        cut.FindAll(".rdv-recruteur-entete").First().Click();
        cut.WaitForState(() => cut.FindAll(".rdv-creneau-candidat-prenom").Any());

        var prenoms = cut.FindAll(".rdv-creneau-candidat-prenom")
            .Select(e => e.TextContent.Trim())
            .ToList();

        prenoms.Should().Contain("Antoine");
        prenoms.Should().Contain("Emma");
    }

    // ── État vide ────────────────────────────────────────────────────────────

    [Fact]
    public void Etant_donne_aucun_recruteur_quand_la_page_est_chargee_alors_le_message_aucun_rdv_restant_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseOk(RdvRestantsHttpFixtures.ReponseJsonRdvRestantsVide));

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-vide-message").Any());

        cut.Find(".rdv-vide-message").TextContent.Trim()
            .Should().Be("Aucun RDV restant.");
    }

    // ── État d'erreur ────────────────────────────────────────────────────────

    [Fact]
    public void Etant_donne_une_erreur_serveur_quand_la_page_est_chargee_alors_le_message_d_erreur_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseErreurServeur());

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-erreur").Any());

        cut.Find(".rdv-erreur-message").TextContent.Trim()
            .Should().Be("Impossible de charger les RDV restants.");
    }

    [Fact]
    public void Etant_donne_une_erreur_serveur_quand_la_page_est_chargee_alors_le_bouton_reessayer_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            RdvRestantsHttpFixtures.ReponseErreurServeur());

        var cut = RenderComponent<EvenementRdvRestantsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rdv-erreur-reessayer").Any());

        cut.Find(".rdv-erreur-reessayer").Should().NotBeNull();
    }
}
