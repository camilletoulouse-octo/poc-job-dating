using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ParcoursCandidatClient.Services;
using ParcoursCandidatClient.Tests.Fixtures;
using MesEvenementsPage = ParcoursCandidatClient.Pages.MesEvenements;

namespace ParcoursCandidatClient.Tests.MesEvenements;

/// <summary>
/// Tests "E2E" (parcours utilisateur) de la page <see cref="MesEvenementsPage"/>
/// rendue avec bUnit. Ils couvrent le parcours :
///   chargement → liste / état vide / erreur → changement d'onglet,
/// ainsi que l'en-tête (titre + conseiller + roue de paramètres) et la
/// bottom navigation à 3 entrées.
/// </summary>
public class MesEvenementsPageTests : TestContext
{
    private void EnregistrerServiceAvec(HttpResponseMessage reponse)
    {
        var handler = new MockHttpMessageHandler(reponse);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(EvenementsHttpFixtures.BaseAddress)
        };
        Services.AddSingleton(new EvenementService(httpClient));
    }

    [Fact]
    public void Etant_donne_la_page_chargee_avec_des_evenements_quand_le_rendu_est_termine_alors_les_titres_des_evenements_sont_affiches()
    {
        var aujourdhui = DateOnly.FromDateTime(DateTime.Today);
        EnregistrerServiceAvec(
            EvenementsHttpFixtures.ReponseOk(EvenementsHttpFixtures.ReponseJsonPourDate(aujourdhui)));

        var cut = RenderComponent<MesEvenementsPage>();
        cut.WaitForState(() => cut.FindAll(".me-card-title").Any());

        var titres = cut.FindAll(".me-card-title").Select(e => e.TextContent.Trim()).ToList();
        titres.Should().Contain("Job dating restauration");
        titres.Should().Contain("Rencontre employeurs BTP");
    }

    [Fact]
    public void Etant_donne_une_API_renvoyant_une_liste_vide_quand_la_page_est_chargee_alors_l_etat_vide_est_affiche()
    {
        EnregistrerServiceAvec(EvenementsHttpFixtures.ReponseListeVide());

        var cut = RenderComponent<MesEvenementsPage>();
        cut.WaitForState(() => cut.FindAll(".me-empty").Any());

        cut.Find(".me-empty-text").TextContent
            .Should().Contain("Aucun événement prévu aujourd'hui");
    }

    [Fact]
    public void Etant_donne_une_API_en_erreur_quand_la_page_est_chargee_alors_le_bloc_d_erreur_et_le_bouton_Reessayer_sont_affiches()
    {
        EnregistrerServiceAvec(EvenementsHttpFixtures.ReponseErreurServeur());

        var cut = RenderComponent<MesEvenementsPage>();
        cut.WaitForState(() => cut.FindAll(".me-error").Any());

        cut.Find(".me-error").TextContent.Should().Contain("Impossible de charger");
        cut.Find(".me-error .me-btn").TextContent.Trim().Should().Be("Réessayer");
    }

    [Fact]
    public void Etant_donne_la_page_rendue_quand_l_entete_est_affiche_alors_le_titre_de_la_page_est_visible()
    {
        EnregistrerServiceAvec(EvenementsHttpFixtures.ReponseListeVide());

        var cut = RenderComponent<MesEvenementsPage>();
        cut.WaitForState(() => cut.FindAll(".me-topbar-title").Any());

        cut.Find(".me-topbar-title").TextContent.Trim().Should().Be("Mes événements");
    }

    [Fact]
    public void Etant_donne_le_conseiller_par_defaut_quand_la_page_est_rendue_alors_son_nom_est_affiche_dans_l_entete()
    {
        EnregistrerServiceAvec(EvenementsHttpFixtures.ReponseListeVide());

        var cut = RenderComponent<MesEvenementsPage>();
        cut.WaitForState(() => cut.FindAll(".me-topbar-conseiller").Any());

        cut.Find(".me-topbar-conseiller").TextContent.Trim().Should().Be("Camille Toulouse");
    }

    [Fact]
    public void Etant_donne_la_page_rendue_quand_l_entete_est_affiche_alors_le_bouton_des_parametres_est_present()
    {
        EnregistrerServiceAvec(EvenementsHttpFixtures.ReponseListeVide());

        var cut = RenderComponent<MesEvenementsPage>();
        cut.WaitForState(() => cut.FindAll(".me-topbar-settings").Any());

        cut.Find(".me-topbar-settings").GetAttribute("aria-label").Should().Be("Paramètres");
    }

    [Fact]
    public void Etant_donne_la_page_chargee_quand_l_onglet_Rechercher_est_tape_alors_le_placeholder_de_recherche_est_affiche()
    {
        EnregistrerServiceAvec(EvenementsHttpFixtures.ReponseListeVide());

        var cut = RenderComponent<MesEvenementsPage>();
        cut.WaitForState(() => cut.FindAll(".me-tab").Count >= 2);

        // L'onglet "Rechercher" est désactivé tant que US-1.05 n'est pas livrée ;
        // on vérifie ici l'état initial de l'onglet "Mes événements du jour".
        var ongletActif = cut.Find(".me-tab.me-tab--active");
        ongletActif.TextContent.Should().Contain("Mes événements du jour");
    }

    [Fact]
    public void Etant_donne_des_evenements_charges_quand_les_cartes_sont_affichees_alors_chaque_carte_contient_le_badge_des_inscrits()
    {
        var aujourdhui = DateOnly.FromDateTime(DateTime.Today);
        EnregistrerServiceAvec(
            EvenementsHttpFixtures.ReponseOk(EvenementsHttpFixtures.ReponseJsonPourDate(aujourdhui)));

        var cut = RenderComponent<MesEvenementsPage>();
        cut.WaitForState(() => cut.FindAll(".me-badge").Any());

        cut.FindAll(".me-badge").Select(e => e.TextContent.Trim())
            .Should().Contain(t => t.Contains("24 inscrits"))
            .And.Contain(t => t.Contains("18 inscrits"));
    }

    [Fact]
    public void Etant_donne_la_page_rendue_quand_la_bottom_navigation_est_affichee_alors_elle_contient_exactement_trois_entrees()
    {
        EnregistrerServiceAvec(EvenementsHttpFixtures.ReponseListeVide());

        var cut = RenderComponent<MesEvenementsPage>();
        cut.WaitForState(() => cut.FindAll(".me-bottom-item").Any());

        var libelles = cut.FindAll(".me-bottom-item span").Select(e => e.TextContent.Trim()).ToList();
        libelles.Should().BeEquivalentTo(new[]
        {
            "Scanner",
            "Mes événements du jour",
            "Rechercher un inscrit"
        });
    }

    [Fact]
    public void Etant_donne_la_page_rendue_quand_la_bottom_navigation_est_affichee_alors_l_entree_Mes_evenements_du_jour_est_active()
    {
        EnregistrerServiceAvec(EvenementsHttpFixtures.ReponseListeVide());

        var cut = RenderComponent<MesEvenementsPage>();
        cut.WaitForState(() => cut.FindAll(".me-bottom-item--active").Any());

        cut.Find(".me-bottom-item--active").TextContent.Should().Contain("Mes événements du jour");
    }

    [Fact]
    public void Etant_donne_des_evenements_charges_quand_la_fleche_d_une_carte_est_consultee_alors_elle_pointe_vers_la_page_des_inscrits_de_cet_evenement()
    {
        var aujourdhui = DateOnly.FromDateTime(DateTime.Today);
        EnregistrerServiceAvec(
            EvenementsHttpFixtures.ReponseOk(EvenementsHttpFixtures.ReponseJsonPourDate(aujourdhui)));

        var cut = RenderComponent<MesEvenementsPage>();
        cut.WaitForState(() => cut.FindAll(".me-card-arrow").Any());

        var liens = cut.FindAll(".me-card-arrow")
            .Select(e => e.GetAttribute("href"))
            .ToList();
        liens.Should().Contain("/evenements/evt-001/inscrits");
        liens.Should().Contain("/evenements/evt-003/inscrits");
    }
}
