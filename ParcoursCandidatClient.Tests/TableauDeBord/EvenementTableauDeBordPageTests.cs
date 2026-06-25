using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ParcoursCandidatClient.Services;
using ParcoursCandidatClient.Tests.Fixtures;
using EvenementTableauDeBordPage = ParcoursCandidatClient.Pages.EvenementTableauDeBord;

namespace ParcoursCandidatClient.Tests.TableauDeBord;

/// <summary>
/// Tests "E2E" (parcours utilisateur) de la page <see cref="EvenementTableauDeBordPage"/>
/// rendue avec bUnit.
/// Couvre US-1.04 (navigation), US-5.01 (carte Vision globale) et le bouton
/// « Générer le planning ».
/// </summary>
public class EvenementTableauDeBordPageTests : TestContext
{
    // ── Helpers ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Enregistre les services avec un handler qui route les requêtes selon l'URL :
    /// - GET /api/evenements/{id}          → <paramref name="evenementResponse"/>
    /// - GET /api/evenements/{id}/tableau-de-bord → <paramref name="tableauDeBordResponse"/>
    /// - POST /api/evenements/{id}/planning → 200 OK
    /// </summary>
    private void EnregistrerServices(
        HttpResponseMessage evenementResponse,
        HttpResponseMessage? tableauDeBordResponse = null)
    {
        var tdbResponse = tableauDeBordResponse
            ?? TableauDeBordHttpFixtures.ReponseNotFound();

        var handler = new MockHttpMessageHandler(req =>
        {
            var path = req.RequestUri?.AbsolutePath ?? "";

            if (req.Method == HttpMethod.Post && path.Contains("/planning"))
                return new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new System.Net.Http.StringContent(
                        """{"evenementId":"evt-001","statut":"GENERE"}""",
                        System.Text.Encoding.UTF8, "application/json")
                };

            if (path.Contains("/tableau-de-bord"))
                return tdbResponse;

            return evenementResponse;
        });

        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
        Services.AddSingleton(new EvenementService(httpClient));
        Services.AddSingleton<ITableauDeBordService>(new TableauDeBordService(httpClient));
        Services.AddSingleton(httpClient);
    }

    // ── US-1.04 : État d'attente avant génération du planning ───────────────

    [Fact]
    public void Etant_donne_le_tableau_de_bord_avant_generation_quand_la_page_est_chargee_alors_l_etat_cactus_est_affiche()
    {
        EnregistrerServices(RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-attente").Any());

        cut.Find(".tdb-attente").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_le_tableau_de_bord_avant_generation_quand_la_page_est_chargee_alors_le_titre_planning_non_genere_est_affiche()
    {
        EnregistrerServices(RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-attente-titre").Any());

        cut.Find(".tdb-attente-titre").TextContent.Trim()
            .Should().Be("Planning non généré");
    }

    [Fact]
    public void Etant_donne_le_tableau_de_bord_avant_generation_quand_la_page_est_chargee_alors_la_description_est_affichee()
    {
        EnregistrerServices(RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-attente-description").Any());

        cut.Find(".tdb-attente-description").TextContent.Trim()
            .Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Etant_donne_le_tableau_de_bord_quand_la_page_est_chargee_alors_l_entete_de_l_evenement_est_affiche()
    {
        EnregistrerServices(RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".evt-header-title").Any());

        cut.Find(".evt-header-title").TextContent.Trim()
            .Should().Be("Job dating restauration");
    }

    // ── Bouton « Générer le planning » ──────────────────────────────────────

    [Fact]
    public void Etant_donne_le_planning_non_genere_quand_la_page_est_chargee_alors_le_bouton_generer_le_planning_est_affiche()
    {
        EnregistrerServices(RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".evt-header-btn-planning").Any());

        cut.Find(".evt-header-btn-planning").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_le_planning_non_genere_quand_la_page_est_chargee_alors_le_libelle_du_bouton_est_correct()
    {
        EnregistrerServices(RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".evt-header-btn-planning").Any());

        cut.Find(".evt-header-btn-planning").TextContent.Trim()
            .Should().Be("Générer le planning");
    }

    [Fact]
    public void Etant_donne_le_planning_deja_genere_quand_la_page_est_chargee_alors_le_bouton_generer_le_planning_est_toujours_present()
    {
        // Le bouton est toujours visible pour permettre la régénération après modification des paramètres.
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-carte").Any());

        cut.Find(".evt-header-btn-planning").Should().NotBeNull();
        cut.Find(".evt-header-btn-planning").TextContent.Trim().Should().Be("Générer le planning");
    }

    // ── US-5.01 : Carte Vision globale ──────────────────────────────────────

    [Fact]
    public void Etant_donne_un_planning_genere_quand_le_tableau_de_bord_est_affiche_alors_la_carte_vision_globale_est_presente()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-carte").Any());

        cut.Find(".tdb-carte").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_un_planning_genere_quand_la_carte_vision_globale_est_affichee_alors_le_titre_vision_globale_est_present()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-carte-titre").Any());

        cut.Find(".tdb-carte-titre").TextContent.Trim()
            .Should().Be("Vision globale");
    }

    [Fact]
    public void Etant_donne_un_planning_genere_quand_la_carte_vision_globale_est_affichee_alors_le_total_entretiens_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-vision-globale").Any());

        var stats = cut.FindAll(".tdb-vision-stat-valeur");
        stats.Should().HaveCountGreaterOrEqualTo(1);
        stats[0].TextContent.Trim().Should().Be("24");
    }

    [Fact]
    public void Etant_donne_un_planning_genere_quand_la_carte_vision_globale_est_affichee_alors_les_entretiens_restants_sont_affiches()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-vision-globale").Any());

        var stats = cut.FindAll(".tdb-vision-stat-valeur");
        stats.Should().HaveCountGreaterOrEqualTo(2);
        stats[1].TextContent.Trim().Should().Be("9");
    }

    [Fact]
    public void Etant_donne_la_carte_vision_globale_quand_je_tape_le_chevron_restants_alors_le_lien_pointe_vers_rdv_restants()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-vision-stat-lien").Any());

        cut.Find(".tdb-vision-stat-lien")
            .GetAttribute("href")
            .Should().Be("/evenements/evt-001/rdv-restants");
    }

    [Fact]
    public void Etant_donne_un_planning_genere_quand_le_tableau_de_bord_est_affiche_alors_l_etat_cactus_est_absent()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-carte").Any());

        cut.FindAll(".tdb-attente").Should().BeEmpty();
    }

    // ── US-5.02 : Carte Statut candidats ────────────────────────────────────

    [Fact]
    public void Etant_donne_un_planning_genere_quand_le_tableau_de_bord_est_affiche_alors_la_carte_statut_candidats_est_presente()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-statut-candidats").Any());

        cut.Find(".tdb-statut-candidats").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_un_planning_genere_quand_la_carte_statut_candidats_est_affichee_alors_le_titre_statut_candidats_est_present()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-carte-titre").Any());

        cut.FindAll(".tdb-carte-titre")
            .Select(e => e.TextContent.Trim())
            .Should().Contain("Statut candidats");
    }

    [Fact]
    public void Etant_donne_la_carte_statut_candidats_quand_elle_est_affichee_alors_le_nombre_de_candidats_sans_entretien_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-statut-sans-entretien-valeur").Any());

        cut.Find(".tdb-statut-sans-entretien-valeur").TextContent.Trim()
            .Should().Be("3");
    }

    [Fact]
    public void Etant_donne_la_carte_statut_candidats_quand_elle_est_affichee_alors_le_libelle_sans_entretien_est_present()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-statut-sans-entretien-libelle").Any());

        cut.Find(".tdb-statut-sans-entretien-libelle").TextContent.Trim()
            .Should().Contain("Sans entretien");
    }

    [Fact]
    public void Etant_donne_la_carte_statut_candidats_quand_je_tape_le_chevron_sans_entretien_alors_le_lien_pointe_vers_candidats_sans_rdv()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-statut-sans-entretien-lien").Any());

        cut.Find(".tdb-statut-sans-entretien-lien")
            .GetAttribute("href")
            .Should().Be("/evenements/evt-001/candidats-sans-rdv");
    }

    [Fact]
    public void Etant_donne_la_carte_statut_candidats_quand_elle_est_affichee_alors_le_nombre_de_presents_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-statut-candidats").Any());

        var candidatsCarte = cut.Find(".tdb-statut-candidats");
        candidatsCarte.QuerySelectorAll(".tdb-statut-colonne-valeur--presents")
            .First().TextContent.Trim()
            .Should().Be("3");
    }

    [Fact]
    public void Etant_donne_la_carte_statut_candidats_quand_elle_est_affichee_alors_le_nombre_d_absents_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-statut-candidats").Any());

        var candidatsCarte = cut.Find(".tdb-statut-candidats");
        candidatsCarte.QuerySelectorAll(".tdb-statut-colonne-valeur--absents")
            .First().TextContent.Trim()
            .Should().Be("1");
    }

    [Fact]
    public void Etant_donne_la_carte_statut_candidats_quand_elle_est_affichee_alors_les_libelles_presents_et_absents_sont_affiches()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-statut-candidats").Any());

        var candidatsCarte = cut.Find(".tdb-statut-candidats");
        var libelles = candidatsCarte.QuerySelectorAll(".tdb-statut-colonne-libelle")
            .Select(e => e.TextContent.Trim())
            .ToList();
        libelles.Should().BeEquivalentTo(new[] { "Présents", "Absents" });
    }

    // ── US-5.03 : Carte Statut recruteurs ───────────────────────────────────

    [Fact]
    public void Etant_donne_un_planning_genere_quand_le_tableau_de_bord_est_affiche_alors_la_carte_statut_recruteurs_est_presente()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-statut-recruteurs").Any());

        cut.Find(".tdb-statut-recruteurs").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_un_planning_genere_quand_la_carte_statut_recruteurs_est_affichee_alors_le_titre_statut_recruteurs_est_present()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-carte-titre").Any());

        cut.FindAll(".tdb-carte-titre")
            .Select(e => e.TextContent.Trim())
            .Should().Contain("Statut recruteurs");
    }

    [Fact]
    public void Etant_donne_la_carte_statut_recruteurs_quand_elle_est_affichee_alors_le_nombre_de_presents_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-statut-recruteurs").Any());

        var recruteursCarte = cut.Find(".tdb-statut-recruteurs");
        recruteursCarte.QuerySelectorAll(".tdb-statut-colonne-valeur--presents")
            .First().TextContent.Trim()
            .Should().Be("2");
    }

    [Fact]
    public void Etant_donne_la_carte_statut_recruteurs_quand_elle_est_affichee_alors_le_nombre_d_absents_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-statut-recruteurs").Any());

        var recruteursCarte = cut.Find(".tdb-statut-recruteurs");
        recruteursCarte.QuerySelectorAll(".tdb-statut-colonne-valeur--absents")
            .First().TextContent.Trim()
            .Should().Be("1");
    }

    [Fact]
    public void Etant_donne_la_carte_statut_recruteurs_quand_elle_est_affichee_alors_les_libelles_presents_et_absents_sont_affiches()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-statut-recruteurs").Any());

        var recruteursCarte = cut.Find(".tdb-statut-recruteurs");
        var libelles = recruteursCarte.QuerySelectorAll(".tdb-statut-colonne-libelle")
            .Select(e => e.TextContent.Trim())
            .ToList();
        libelles.Should().BeEquivalentTo(new[] { "Présents", "Absents" });
    }

    [Fact]
    public void Etant_donne_tous_recruteurs_presents_quand_la_carte_statut_recruteurs_est_affichee_alors_absents_affiche_zero()
    {
        // Fixture avec absents = 0 pour vérifier que la valeur "0" est bien affichée (non masquée).
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001TousPresents));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-statut-recruteurs").Any());

        var recruteursCarte = cut.Find(".tdb-statut-recruteurs");
        recruteursCarte.QuerySelectorAll(".tdb-statut-colonne-valeur--absents")
            .First().TextContent.Trim()
            .Should().Be("0");
    }

    [Fact]
    public void Etant_donne_des_recruteurs_absents_quand_la_carte_statut_recruteurs_est_affichee_alors_le_nombre_d_absents_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001AvecAbsents));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-statut-recruteurs").Any());

        var recruteursCarte = cut.Find(".tdb-statut-recruteurs");
        recruteursCarte.QuerySelectorAll(".tdb-statut-colonne-valeur--absents")
            .First().TextContent.Trim()
            .Should().Be("3");
    }

    // ── US-5.04 : Carte Suites & recrutements ───────────────────────────────

    [Fact]
    public void Etant_donne_un_planning_genere_quand_le_tableau_de_bord_est_affiche_alors_la_carte_suites_recrutements_est_presente()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-suites-liste").Any());

        cut.Find(".tdb-suites-liste").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_un_planning_genere_quand_la_carte_suites_recrutements_est_affichee_alors_le_titre_est_present()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-carte-titre").Any());

        cut.FindAll(".tdb-carte-titre")
            .Select(e => e.TextContent.Trim())
            .Should().Contain("Suites & recrutements");
    }

    [Fact]
    public void Etant_donne_la_carte_suites_recrutements_quand_elle_est_affichee_alors_le_nombre_de_recrutements_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-suites-stat-valeur--recrutements").Any());

        cut.Find(".tdb-suites-stat-valeur--recrutements").TextContent.Trim()
            .Should().Be("5");
    }

    [Fact]
    public void Etant_donne_la_carte_suites_recrutements_quand_elle_est_affichee_alors_le_nombre_d_immersions_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-suites-stat-valeur--immersions").Any());

        cut.Find(".tdb-suites-stat-valeur--immersions").TextContent.Trim()
            .Should().Be("2");
    }

    [Fact]
    public void Etant_donne_la_carte_suites_recrutements_quand_elle_est_affichee_alors_le_nombre_de_poei_est_affiche()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-suites-stat-valeur--poei").Any());

        cut.Find(".tdb-suites-stat-valeur--poei").TextContent.Trim()
            .Should().Be("1");
    }

    [Fact]
    public void Etant_donne_la_carte_suites_recrutements_quand_elle_est_affichee_alors_les_libelles_recrutements_immersions_poei_sont_presents()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-suites-ligne-libelle").Any());

        var libelles = cut.FindAll(".tdb-suites-ligne-libelle")
            .Select(e => e.TextContent.Trim())
            .ToList();
        libelles.Should().Contain("Nombre de recrutements");
        libelles.Should().Contain("Nombre d'immersion");
        libelles.Should().Contain("Nombre de POEI");
    }

    [Fact]
    public void Etant_donne_la_carte_suites_recrutements_quand_elle_est_affichee_alors_les_trois_blocs_2e_entretiens_sont_presents()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-suites-entretien-bloc--oui").Any());

        cut.Find(".tdb-suites-entretien-bloc--oui").Should().NotBeNull();
        cut.Find(".tdb-suites-entretien-bloc--peut-etre").Should().NotBeNull();
        cut.Find(".tdb-suites-entretien-bloc--non").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_la_carte_suites_recrutements_quand_elle_est_affichee_alors_les_valeurs_oui_peut_etre_non_sont_affichees()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-suites-entretien-valeur").Any());

        var valeurs = cut.FindAll(".tdb-suites-entretien-valeur")
            .Select(e => e.TextContent.Trim())
            .ToList();
        valeurs.Should().BeEquivalentTo(new[] { "8", "4", "3" });
    }

    [Fact]
    public void Etant_donne_la_carte_suites_recrutements_quand_elle_est_affichee_alors_les_libelles_oui_peut_etre_non_sont_affiches()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-suites-entretien-label").Any());

        var labels = cut.FindAll(".tdb-suites-entretien-label")
            .Select(e => e.TextContent.Trim())
            .ToList();
        labels.Should().BeEquivalentTo(new[] { "OUI", "PEUT-ÊTRE", "NON" });
    }

    [Fact]
    public void Etant_donne_des_donnees_sans_suites_recrutements_quand_le_tableau_de_bord_est_affiche_alors_la_carte_suites_est_absente()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001SansSuites));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-carte").Any());

        cut.FindAll(".tdb-suites-liste").Should().BeEmpty();
    }

    [Fact]
    public void Etant_donne_la_carte_suites_recrutements_quand_elle_est_affichee_alors_le_libelle_2nd_entretiens_est_present()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-suites-entretiens-titre").Any());

        cut.Find(".tdb-suites-entretiens-titre").TextContent.Trim()
            .Should().Be("Nombre de 2nd entretiens");
    }

    // ── US-5.05 : Carte Enquêtes de satisfaction ─────────────────────────────

    [Fact]
    public void Etant_donne_un_planning_genere_avec_satisfaction_quand_le_tableau_de_bord_est_affiche_alors_la_carte_enquetes_satisfaction_est_presente()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001AvecSatisfaction));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-satisfaction-colonnes").Any());

        cut.Find(".tdb-satisfaction-colonnes").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_la_carte_satisfaction_quand_elle_est_affichee_alors_le_titre_enquetes_de_satisfaction_est_present()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001AvecSatisfaction));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-carte-titre").Any());

        cut.FindAll(".tdb-carte-titre")
            .Select(e => e.TextContent.Trim())
            .Should().Contain("Enquêtes de satisfaction");
    }

    [Fact]
    public void Etant_donne_la_carte_satisfaction_quand_elle_est_affichee_alors_les_libelles_candidats_et_recruteurs_sont_presents()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001AvecSatisfaction));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-satisfaction-libelle").Any());

        var libelles = cut.FindAll(".tdb-satisfaction-libelle")
            .Select(e => e.TextContent.Trim())
            .ToList();
        libelles.Should().BeEquivalentTo(new[] { "CANDIDATS", "RECRUTEURS" });
    }

    [Fact]
    public void Etant_donne_la_carte_satisfaction_quand_elle_est_affichee_alors_la_note_candidats_est_affichee()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001AvecSatisfaction));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-satisfaction-note").Any());

        var notes = cut.FindAll(".tdb-satisfaction-note")
            .Select(e => e.TextContent.Trim())
            .ToList();
        notes[0].Should().Be("3,5");
    }

    [Fact]
    public void Etant_donne_la_carte_satisfaction_quand_elle_est_affichee_alors_la_note_recruteurs_est_affichee()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001AvecSatisfaction));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-satisfaction-note").Any());

        var notes = cut.FindAll(".tdb-satisfaction-note")
            .Select(e => e.TextContent.Trim())
            .ToList();
        notes[1].Should().Be("3,2");
    }

    [Fact]
    public void Etant_donne_aucune_enquete_soumise_quand_la_carte_satisfaction_est_affichee_alors_les_notes_affichent_le_tiret()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001SatisfactionVide));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-satisfaction-note").Any());

        var notes = cut.FindAll(".tdb-satisfaction-note")
            .Select(e => e.TextContent.Trim())
            .ToList();
        notes.Should().AllBe("—");
    }

    [Fact]
    public void Etant_donne_des_donnees_sans_enquetes_satisfaction_quand_le_tableau_de_bord_est_affiche_alors_la_carte_satisfaction_est_absente()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-carte").Any());

        cut.FindAll(".tdb-satisfaction-colonnes").Should().BeEmpty();
    }

    [Fact]
    public void Etant_donne_la_carte_satisfaction_quand_elle_est_affichee_alors_deux_etoiles_svg_sont_presentes()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001AvecSatisfaction));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-satisfaction-etoile-svg").Any());

        cut.FindAll(".tdb-satisfaction-etoile-svg").Should().HaveCount(2);
    }

    [Fact]
    public void Etant_donne_la_carte_satisfaction_quand_elle_est_affichee_alors_la_carte_est_positionnee_apres_suites_recrutements()
    {
        EnregistrerServices(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration),
            TableauDeBordHttpFixtures.ReponseOk(TableauDeBordHttpFixtures.ReponseJsonTableauDeBordEvt001AvecSatisfaction));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".tdb-carte-titre").Any());

        var titres = cut.FindAll(".tdb-carte-titre")
            .Select(e => e.TextContent.Trim())
            .ToList();
        var indexSatisfaction = titres.IndexOf("Enquêtes de satisfaction");
        var indexSuites = titres.IndexOf("Suites & recrutements");
        indexSatisfaction.Should().BeGreaterThan(indexSuites);
    }

    // ── US-1.04 : Navigation entre onglets ──────────────────────────────────

    [Fact]
    public void Etant_donne_le_tableau_de_bord_quand_la_bottom_nav_est_affichee_alors_elle_contient_trois_entrees()
    {
        EnregistrerServices(RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".evt-bottom-item").Any());

        cut.FindAll(".evt-bottom-item").Should().HaveCount(3);
    }

    [Fact]
    public void Etant_donne_le_tableau_de_bord_quand_la_bottom_nav_est_affichee_alors_l_onglet_tableau_de_bord_est_actif()
    {
        EnregistrerServices(RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".evt-bottom-item--active").Any());

        cut.Find(".evt-bottom-item--active").TextContent.Trim()
            .Should().Contain("Tableau de bord");
    }

    [Fact]
    public void Etant_donne_le_tableau_de_bord_quand_la_bottom_nav_est_affichee_alors_les_libelles_des_trois_onglets_sont_corrects()
    {
        EnregistrerServices(RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".evt-bottom-item").Any());

        var libelles = cut.FindAll(".evt-bottom-item span")
            .Select(e => e.TextContent.Trim())
            .ToList();
        libelles.Should().BeEquivalentTo(new[]
        {
            "Les inscrits",
            "Les recruteurs",
            "Tableau de bord"
        });
    }

    [Fact]
    public void Etant_donne_le_tableau_de_bord_quand_le_lien_les_inscrits_est_consulte_alors_il_pointe_vers_la_bonne_url()
    {
        EnregistrerServices(RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".evt-bottom-item").Any());

        var lienInscrits = cut.FindAll(".evt-bottom-item")
            .First(a => a.TextContent.Contains("Les inscrits"));
        lienInscrits.GetAttribute("href").Should().Be("/evenements/evt-001/inscrits");
    }

    [Fact]
    public void Etant_donne_le_tableau_de_bord_quand_le_lien_les_recruteurs_est_consulte_alors_il_pointe_vers_la_bonne_url()
    {
        EnregistrerServices(RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementTableauDeBordPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".evt-bottom-item").Any());

        var lienRecruteurs = cut.FindAll(".evt-bottom-item")
            .First(a => a.TextContent.Contains("Les recruteurs"));
        lienRecruteurs.GetAttribute("href").Should().Be("/evenements/evt-001/recruteurs");
    }

    [Fact]
    public void Etant_donne_la_page_inscrits_quand_la_bottom_nav_est_affichee_alors_l_onglet_tableau_de_bord_pointe_vers_la_bonne_url()
    {
        var handler = new MockHttpMessageHandler(
            RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
        Services.AddSingleton(new EvenementService(httpClient));
        Services.AddSingleton<IInscritService>(new InscritServiceStub());
        Services.AddSingleton(httpClient);

        var cut = RenderComponent<ParcoursCandidatClient.Pages.EvenementInscrits>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".evt-bottom-item").Any());

        var lienTdb = cut.FindAll(".evt-bottom-item")
            .First(a => a.TextContent.Contains("Tableau de bord"));
        lienTdb.GetAttribute("href").Should().Be("/evenements/evt-001/tableau-de-bord");
    }

    private sealed class InscritServiceStub : IInscritService
    {
        public Task<IReadOnlyList<ParcoursCandidatClient.Models.Inscrit>> GetInscritsAsync(
            string evenementId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ParcoursCandidatClient.Models.Inscrit>>(
                Array.Empty<ParcoursCandidatClient.Models.Inscrit>());

        public Task<ParcoursCandidatClient.Models.Inscrit?> UpdateStatutAsync(
            string inscritId,
            ParcoursCandidatClient.Models.StatutInscrit nouveauStatut,
            CancellationToken cancellationToken = default)
            => Task.FromResult<ParcoursCandidatClient.Models.Inscrit?>(null);
    }

    private sealed class TableauDeBordServiceStub : ITableauDeBordService
    {
        public Task<ParcoursCandidatClient.Models.TableauDeBord?> GetTableauDeBordAsync(
            string evenementId, CancellationToken cancellationToken = default)
            => Task.FromResult<ParcoursCandidatClient.Models.TableauDeBord?>(null);
    }
}
