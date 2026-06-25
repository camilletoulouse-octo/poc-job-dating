using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ParcoursCandidatClient.Models;
using ParcoursCandidatClient.Services;
using ParcoursCandidatClient.Tests.Fixtures;
using EvenementInscritsPage = ParcoursCandidatClient.Pages.EvenementInscrits;

namespace ParcoursCandidatClient.Tests.Candidats;

/// <summary>
/// Tests "E2E" (parcours utilisateur) de la page <see cref="EvenementInscritsPage"/>
/// rendue avec bUnit (US-1.03 + US-2.01 + US-2.02).
/// </summary>
public class EvenementInscritsPageTests : TestContext
{
    private sealed class InscritServiceStub : IInscritService
    {
        private readonly IReadOnlyList<Inscrit> _inscrits;
        private readonly Exception? _exception;

        public InscritServiceStub(IReadOnlyList<Inscrit> inscrits)
        {
            _inscrits = inscrits;
            _exception = null;
        }

        public InscritServiceStub(Exception exception)
        {
            _inscrits = Array.Empty<Inscrit>();
            _exception = exception;
        }

        public Task<IReadOnlyList<Inscrit>> GetInscritsAsync(string evenementId, CancellationToken cancellationToken = default)
        {
            if (_exception is not null)
            {
                return Task.FromException<IReadOnlyList<Inscrit>>(_exception);
            }

            return Task.FromResult(_inscrits);
        }

        public Task<Inscrit?> UpdateStatutAsync(string inscritId, StatutInscrit nouveauStatut, CancellationToken cancellationToken = default)
        {
            var inscrit = _inscrits.FirstOrDefault(i => i.Id == inscritId);
            if (inscrit is null) return Task.FromResult<Inscrit?>(null);

            var misAJour = inscrit with { Statut = nouveauStatut };
            return Task.FromResult<Inscrit?>(misAJour);
        }
    }

    private void EnregistrerServices(IInscritService inscritService, HttpResponseMessage evenementResponse)
    {
        var handler = new MockHttpMessageHandler(evenementResponse);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
        Services.AddSingleton(new EvenementService(httpClient));
        Services.AddSingleton(inscritService);
        Services.AddSingleton(httpClient);
    }

    [Fact]
    public void Etant_donne_un_evenement_avec_quatre_inscrits_quand_la_page_est_rendue_alors_l_entete_affiche_le_titre_de_l_evenement()
    {
        var service = new InscritServiceStub(new[]
        {
            InscritsHttpFixtures.BernardThomas,
            InscritsHttpFixtures.DuboisSophie,
            InscritsHttpFixtures.MartinLucie
        });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".evt-header-title").Any()
                               && !cut.Find(".evt-header-title").TextContent.Trim().Equals(string.Empty));

        cut.Find(".evt-header-title").TextContent.Trim().Should().Be("Job dating restauration");
    }

    [Fact]
    public void Etant_donne_un_evenement_quand_la_page_est_rendue_alors_l_onglet_Les_inscrits_est_actif()
    {
        var service = new InscritServiceStub(Array.Empty<Inscrit>());
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));

        // EvenementBottomNav est rendu immédiatement (pas de dépendance async)
        cut.FindAll(".evt-bottom-item--active").Should().NotBeEmpty();
        cut.Find(".evt-bottom-item--active").TextContent.Trim().Should().Be("Les inscrits");
    }

    [Fact]
    public void Etant_donne_trois_inscrits_dont_deux_pointes_quand_la_liste_est_chargee_alors_le_titre_affiche_2_sur_3()
    {
        var service = new InscritServiceStub(new[]
        {
            InscritsHttpFixtures.BernardThomas,   // PRESENT  → pointé
            InscritsHttpFixtures.DuboisSophie,    // ABSENT   → pointé
            InscritsHttpFixtures.MartinLucie      // INCONNU  → non pointé
        });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-counter").Any());

        cut.Find(".ins-title").TextContent.Should().Contain("Liste des inscrits");
        cut.Find(".ins-counter").TextContent.Should().Be("2");
        cut.Find(".ins-total").TextContent.Should().Be("3");
    }

    [Fact]
    public void Etant_donne_aucun_inscrit_quand_la_liste_est_chargee_alors_la_barre_de_progression_est_a_zero_pourcent()
    {
        var service = new InscritServiceStub(Array.Empty<Inscrit>());
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-progress-bar").Any());

        cut.Find(".ins-progress-bar").GetAttribute("style").Should().Contain("width: 0%");
    }

    [Fact]
    public void Etant_donne_la_liste_chargee_quand_les_lignes_sont_affichees_alors_chaque_inscrit_a_un_badge_statut()
    {
        var service = new InscritServiceStub(new[]
        {
            InscritsHttpFixtures.BernardThomas,
            InscritsHttpFixtures.DuboisSophie,
            InscritsHttpFixtures.MartinLucie
        });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-row").Any());

        cut.FindAll(".ins-badge").Should().HaveCount(3);
        cut.FindAll(".ins-badge--present").Should().HaveCount(1);
        cut.FindAll(".ins-badge--absent").Should().HaveCount(1);
        cut.FindAll(".ins-badge--inconnu").Should().HaveCount(1);
    }

    [Fact]
    public void Etant_donne_un_inscrit_quand_sa_ligne_est_affichee_alors_son_nom_et_son_prenom_sont_visibles()
    {
        var service = new InscritServiceStub(new[] { InscritsHttpFixtures.BernardThomas });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-nom").Any());

        cut.Find(".ins-nom").TextContent.Trim().Should().Be("Bernard Thomas");
    }

    [Fact]
    public void Etant_donne_l_evenement_charge_quand_l_entete_est_affiche_alors_le_bouton_retour_pointe_vers_mes_evenements()
    {
        var service = new InscritServiceStub(Array.Empty<Inscrit>());
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".evt-header-back").Any());

        cut.Find(".evt-header-back").GetAttribute("href").Should().Be("/mes-evenements");
    }

    [Fact]
    public void Etant_donne_le_service_des_inscrits_en_erreur_quand_la_page_est_rendue_alors_le_bloc_d_erreur_et_le_bouton_Reessayer_sont_affiches()
    {
        var service = new InscritServiceStub(new HttpRequestException("Boom"));
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-error").Any());

        cut.Find(".ins-error").TextContent.Should().Contain("Impossible de charger");
        cut.Find(".ins-error .ins-btn").TextContent.Trim().Should().Be("Réessayer");
    }

    // ── US-2.02 : Filtrer la liste des inscrits par statut ──────────────────

    [Fact]
    public void Etant_donne_la_liste_affichee_quand_la_page_est_chargee_alors_les_quatre_chips_de_filtre_sont_visibles()
    {
        var service = new InscritServiceStub(new[]
        {
            InscritsHttpFixtures.BernardThomas,
            InscritsHttpFixtures.DuboisSophie,
            InscritsHttpFixtures.MartinLucie
        });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-segmented-tab").Any());

        var tabs = cut.FindAll(".ins-segmented-tab");
        tabs.Should().HaveCount(4);
        tabs[0].TextContent.Trim().Should().Be("Tous");
        tabs[1].TextContent.Trim().Should().Be("Présent");
        tabs[2].TextContent.Trim().Should().Be("Absent");
        tabs[3].TextContent.Trim().Should().Be("Inconnu");
    }

    [Fact]
    public void Etant_donne_la_liste_affichee_quand_la_page_est_chargee_alors_le_chip_Tous_est_actif_par_defaut()
    {
        var service = new InscritServiceStub(new[]
        {
            InscritsHttpFixtures.BernardThomas,
            InscritsHttpFixtures.DuboisSophie,
            InscritsHttpFixtures.MartinLucie
        });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-segmented-tab--actif").Any());

        cut.Find(".ins-segmented-tab--actif").TextContent.Trim().Should().Be("Tous");
    }

    [Fact]
    public void Etant_donne_le_filtre_Tous_quand_la_liste_se_met_a_jour_alors_tous_les_inscrits_sont_affiches_avec_le_compteur_global()
    {
        var service = new InscritServiceStub(new[]
        {
            InscritsHttpFixtures.BernardThomas,
            InscritsHttpFixtures.DuboisSophie,
            InscritsHttpFixtures.MartinLucie
        });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-segmented-tab").Any());

        cut.FindAll(".ins-row").Should().HaveCount(3);
        cut.Find(".ins-total").TextContent.Should().Be("3");
    }

    [Fact]
    public void Etant_donne_le_filtre_Present_selectionne_quand_la_liste_se_met_a_jour_alors_seuls_les_presents_sont_affiches()
    {
        var service = new InscritServiceStub(new[]
        {
            InscritsHttpFixtures.BernardThomas,
            InscritsHttpFixtures.DuboisSophie,
            InscritsHttpFixtures.MartinLucie
        });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-segmented-tab").Any());

        cut.FindAll(".ins-segmented-tab").First(c => c.TextContent.Trim() == "Présent").Click();

        cut.FindAll(".ins-row").Should().HaveCount(1);
        cut.Find(".ins-badge--present").Should().NotBeNull();
        cut.Find(".ins-segmented-tab--actif").TextContent.Trim().Should().Be("Présent");
    }

    [Fact]
    public void Etant_donne_le_filtre_Absent_selectionne_quand_la_liste_se_met_a_jour_alors_seuls_les_absents_sont_affiches()
    {
        var service = new InscritServiceStub(new[]
        {
            InscritsHttpFixtures.BernardThomas,
            InscritsHttpFixtures.DuboisSophie,
            InscritsHttpFixtures.MartinLucie
        });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-segmented-tab").Any());

        cut.FindAll(".ins-segmented-tab").First(c => c.TextContent.Trim() == "Absent").Click();

        cut.FindAll(".ins-row").Should().HaveCount(1);
        cut.Find(".ins-badge--absent").Should().NotBeNull();
        cut.Find(".ins-segmented-tab--actif").TextContent.Trim().Should().Be("Absent");
    }

    [Fact]
    public void Etant_donne_le_filtre_Inconnu_selectionne_quand_la_liste_se_met_a_jour_alors_seuls_les_indetermines_sont_affiches()
    {
        var service = new InscritServiceStub(new[]
        {
            InscritsHttpFixtures.BernardThomas,
            InscritsHttpFixtures.DuboisSophie,
            InscritsHttpFixtures.MartinLucie
        });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-segmented-tab").Any());

        cut.FindAll(".ins-segmented-tab").First(c => c.TextContent.Trim() == "Inconnu").Click();

        cut.FindAll(".ins-row").Should().HaveCount(1);
        cut.Find(".ins-badge--inconnu").Should().NotBeNull();
        cut.Find(".ins-segmented-tab--actif").TextContent.Trim().Should().Be("Inconnu");
    }

    [Fact]
    public void Etant_donne_un_filtre_actif_quand_aucun_inscrit_ne_correspond_alors_le_message_vide_de_filtre_est_affiche()
    {
        var service = new InscritServiceStub(new[]
        {
            InscritsHttpFixtures.BernardThomas  // PRESENT uniquement
        });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-segmented-tab").Any());

        cut.FindAll(".ins-segmented-tab").First(c => c.TextContent.Trim() == "Absent").Click();

        cut.Find(".ins-empty-text").TextContent.Trim()
            .Should().Be("Aucun inscrit ne correspond à ce filtre.");
    }

    [Fact]
    public void Etant_donne_un_filtre_actif_quand_je_retape_Tous_alors_tous_les_inscrits_sont_de_nouveau_affiches()
    {
        var service = new InscritServiceStub(new[]
        {
            InscritsHttpFixtures.BernardThomas,
            InscritsHttpFixtures.DuboisSophie,
            InscritsHttpFixtures.MartinLucie
        });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-segmented-tab").Any());

        cut.FindAll(".ins-segmented-tab").First(c => c.TextContent.Trim() == "Présent").Click();
        cut.FindAll(".ins-segmented-tab").First(c => c.TextContent.Trim() == "Tous").Click();

        cut.FindAll(".ins-row").Should().HaveCount(3);
        cut.Find(".ins-segmented-tab--actif").TextContent.Trim().Should().Be("Tous");
    }

    // ── US-2.04 : Modifier le statut via l'action sheet ─────────────────────

    [Fact]
    public void Etant_donne_la_liste_quand_j_appuie_sur_une_ligne_alors_l_action_sheet_s_ouvre_avec_le_nom_de_l_inscrit()
    {
        var service = new InscritServiceStub(new[]
        {
            InscritsHttpFixtures.BernardThomas,
            InscritsHttpFixtures.DuboisSophie,
            InscritsHttpFixtures.MartinLucie
        });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-row").Any());

        cut.FindAll(".ins-row")[0].Click();

        cut.Find(".ins-action-sheet").Should().NotBeNull();
        cut.Find(".ins-action-sheet-title").TextContent.Trim()
            .Should().Be("Sélectionne l'état de présence");
        cut.Find(".ins-action-sheet-nom").TextContent.Trim()
            .Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Etant_donne_l_action_sheet_ouverte_quand_elle_est_affichee_alors_les_quatre_options_sont_presentes()
    {
        var service = new InscritServiceStub(new[] { InscritsHttpFixtures.BernardThomas });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-row").Any());

        cut.Find(".ins-row").Click();

        cut.Find(".ins-action-btn--present").TextContent.Trim().Should().Be("Présent");
        cut.Find(".ins-action-btn--absent").TextContent.Trim().Should().Be("Absent");
        cut.Find(".ins-action-btn--inconnu").TextContent.Trim().Should().Be("Participation inconnue");
        cut.Find(".ins-action-btn--annuler").TextContent.Trim().Should().Be("Annuler");
    }

    [Fact]
    public void Etant_donne_l_action_sheet_quand_Annuler_est_clique_alors_l_action_sheet_se_ferme_sans_modification()
    {
        var service = new InscritServiceStub(new[] { InscritsHttpFixtures.MartinLucie });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-row").Any());

        cut.Find(".ins-row").Click();
        cut.Find(".ins-action-btn--annuler").Click();

        cut.FindAll(".ins-action-sheet").Should().BeEmpty();
        cut.Find(".ins-badge--inconnu").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_l_action_sheet_quand_Present_est_choisi_alors_le_badge_devient_vert_et_l_action_sheet_se_ferme()
    {
        var service = new InscritServiceStub(new[] { InscritsHttpFixtures.MartinLucie });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-row").Any());

        cut.Find(".ins-row").Click();
        cut.Find(".ins-action-btn--present").Click();
        cut.WaitForState(() => !cut.FindAll(".ins-action-sheet").Any());

        cut.FindAll(".ins-action-sheet").Should().BeEmpty();
        cut.Find(".ins-badge--present").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_l_action_sheet_quand_Absent_est_choisi_alors_le_badge_devient_rouge_et_le_compteur_est_mis_a_jour()
    {
        var service = new InscritServiceStub(new[]
        {
            InscritsHttpFixtures.MartinLucie,   // INCONNU → sera mis à ABSENT
            InscritsHttpFixtures.BernardThomas  // PRESENT
        });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-row").Any());

        // Avant : 1 pointé (BernardThomas PRESENT), MartinLucie INCONNU
        cut.Find(".ins-counter").TextContent.Should().Be("1");

        // Clic sur MartinLucie (première ligne = tri alphabétique : Martin avant Bernard)
        cut.FindAll(".ins-row").First(r => r.TextContent.Contains("Martin")).Click();
        cut.Find(".ins-action-btn--absent").Click();
        cut.WaitForState(() => !cut.FindAll(".ins-action-sheet").Any());

        // Après : 2 pointés (BernardThomas PRESENT + MartinLucie ABSENT)
        cut.Find(".ins-counter").TextContent.Should().Be("2");
        cut.FindAll(".ins-badge--absent").Should().HaveCount(1);
    }

    [Fact]
    public void Etant_donne_l_action_sheet_quand_Participation_inconnue_est_choisie_alors_le_badge_devient_gris()
    {
        var service = new InscritServiceStub(new[] { InscritsHttpFixtures.BernardThomas });
        EnregistrerServices(service, InscritsHttpFixtures.ReponseOk(InscritsHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementInscritsPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".ins-row").Any());

        cut.Find(".ins-row").Click();
        cut.Find(".ins-action-btn--inconnu").Click();
        cut.WaitForState(() => !cut.FindAll(".ins-action-sheet").Any());

        cut.Find(".ins-badge--inconnu").Should().NotBeNull();
    }
}
