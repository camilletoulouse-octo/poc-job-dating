using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ParcoursCandidatClient.Models;
using ParcoursCandidatClient.Services;
using ParcoursCandidatClient.Tests.Fixtures;
using EvenementRecruteursPage = ParcoursCandidatClient.Pages.EvenementRecruteurs;

namespace ParcoursCandidatClient.Tests.Recruteurs;

/// <summary>
/// Tests "E2E" (parcours utilisateur) de la page <see cref="EvenementRecruteursPage"/>
/// rendue avec bUnit (US-3.01 + US-3.02 + US-3.04).
/// </summary>
public class EvenementRecruteursPageTests : TestContext
{
    private sealed class RecruteurServiceStub : IRecruteurService
    {
        private readonly IReadOnlyList<Recruteur> _recruteurs;
        private readonly Exception? _exception;

        public RecruteurServiceStub(IReadOnlyList<Recruteur> recruteurs)
        {
            _recruteurs = recruteurs;
            _exception = null;
        }

        public RecruteurServiceStub(Exception exception)
        {
            _recruteurs = Array.Empty<Recruteur>();
            _exception = exception;
        }

        public Task<IReadOnlyList<Recruteur>> GetRecruteursAsync(string evenementId, CancellationToken cancellationToken = default)
        {
            if (_exception is not null)
            {
                return Task.FromException<IReadOnlyList<Recruteur>>(_exception);
            }

            return Task.FromResult(_recruteurs);
        }

        public Task<Recruteur?> UpdateStatutAsync(string recruteurId, StatutRecruteur nouveauStatut, CancellationToken cancellationToken = default)
        {
            var recruteur = _recruteurs.FirstOrDefault(r => r.Id == recruteurId);
            if (recruteur is null) return Task.FromResult<Recruteur?>(null);

            var misAJour = recruteur with { Statut = nouveauStatut };
            return Task.FromResult<Recruteur?>(misAJour);
        }
    }

    private void EnregistrerServices(IRecruteurService recruteurService, HttpResponseMessage evenementResponse)
    {
        var handler = new MockHttpMessageHandler(evenementResponse);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
        Services.AddSingleton(new EvenementService(httpClient));
        Services.AddSingleton(recruteurService);
        Services.AddSingleton(httpClient);
    }

    // ── US-3.02 : Filtrer la liste des recruteurs par statut ────────────────

    [Fact]
    public void Etant_donne_la_liste_affichee_quand_la_page_est_chargee_alors_les_quatre_chips_de_filtre_sont_visibles()
    {
        var service = new RecruteurServiceStub(new[]
        {
            RecruteursHttpFixtures.DupontMarie,
            RecruteursHttpFixtures.MorelClaire,
            RecruteursHttpFixtures.FontaineJulien
        });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonTroisRecruteurs));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-segmented-tab").Any());

        var tabs = cut.FindAll(".rec-segmented-tab");
        tabs.Should().HaveCount(4);
        tabs[0].TextContent.Trim().Should().Be("Tous");
        tabs[1].TextContent.Trim().Should().Be("Présent");
        tabs[2].TextContent.Trim().Should().Be("Absent");
        tabs[3].TextContent.Trim().Should().Be("Inconnu");
    }

    [Fact]
    public void Etant_donne_la_liste_affichee_quand_la_page_est_chargee_alors_le_chip_Tous_est_actif_par_defaut()
    {
        var service = new RecruteurServiceStub(new[]
        {
            RecruteursHttpFixtures.DupontMarie,
            RecruteursHttpFixtures.MorelClaire,
            RecruteursHttpFixtures.FontaineJulien
        });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonTroisRecruteurs));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-segmented-tab--actif").Any());

        cut.Find(".rec-segmented-tab--actif").TextContent.Trim().Should().Be("Tous");
    }

    [Fact]
    public void Etant_donne_le_filtre_Tous_quand_la_liste_se_met_a_jour_alors_tous_les_recruteurs_sont_affiches_avec_le_compteur_global()
    {
        var service = new RecruteurServiceStub(new[]
        {
            RecruteursHttpFixtures.DupontMarie,
            RecruteursHttpFixtures.MorelClaire,
            RecruteursHttpFixtures.FontaineJulien
        });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonTroisRecruteurs));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-segmented-tab").Any());

        cut.FindAll(".rec-row").Should().HaveCount(3);
        cut.Find(".rec-total").TextContent.Should().Be("3");
    }

    [Fact]
    public void Etant_donne_le_filtre_Present_selectionne_quand_la_liste_se_met_a_jour_alors_seuls_les_presents_sont_affiches()
    {
        var service = new RecruteurServiceStub(new[]
        {
            RecruteursHttpFixtures.DupontMarie,
            RecruteursHttpFixtures.MorelClaire,
            RecruteursHttpFixtures.FontaineJulien
        });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonTroisRecruteurs));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-segmented-tab").Any());

        cut.FindAll(".rec-segmented-tab").First(c => c.TextContent.Trim() == "Présent").Click();

        cut.FindAll(".rec-row").Should().HaveCount(1);
        cut.Find(".rec-badge--present").Should().NotBeNull();
        cut.Find(".rec-segmented-tab--actif").TextContent.Trim().Should().Be("Présent");
    }

    [Fact]
    public void Etant_donne_le_filtre_Absent_selectionne_quand_la_liste_se_met_a_jour_alors_seuls_les_absents_sont_affiches()
    {
        var service = new RecruteurServiceStub(new[]
        {
            RecruteursHttpFixtures.DupontMarie,
            RecruteursHttpFixtures.MorelClaire,
            RecruteursHttpFixtures.FontaineJulien
        });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonTroisRecruteurs));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-segmented-tab").Any());

        cut.FindAll(".rec-segmented-tab").First(c => c.TextContent.Trim() == "Absent").Click();

        cut.FindAll(".rec-row").Should().HaveCount(1);
        cut.Find(".rec-badge--absent").Should().NotBeNull();
        cut.Find(".rec-segmented-tab--actif").TextContent.Trim().Should().Be("Absent");
    }

    [Fact]
    public void Etant_donne_le_filtre_Inconnu_selectionne_quand_la_liste_se_met_a_jour_alors_seuls_les_indetermines_sont_affiches()
    {
        var service = new RecruteurServiceStub(new[]
        {
            RecruteursHttpFixtures.DupontMarie,
            RecruteursHttpFixtures.MorelClaire,
            RecruteursHttpFixtures.FontaineJulien
        });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonTroisRecruteurs));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-segmented-tab").Any());

        cut.FindAll(".rec-segmented-tab").First(c => c.TextContent.Trim() == "Inconnu").Click();

        cut.FindAll(".rec-row").Should().HaveCount(1);
        cut.Find(".rec-badge--inconnu").Should().NotBeNull();
        cut.Find(".rec-segmented-tab--actif").TextContent.Trim().Should().Be("Inconnu");
    }

    [Fact]
    public void Etant_donne_le_filtre_Absent_actif_sans_recruteur_absent_quand_filtre_alors_l_etat_vide_contextuel_est_affiche()
    {
        var service = new RecruteurServiceStub(new[]
        {
            RecruteursHttpFixtures.DupontMarie  // PRESENT uniquement
        });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonTroisRecruteurs));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-segmented-tab").Any());

        cut.FindAll(".rec-segmented-tab").First(c => c.TextContent.Trim() == "Absent").Click();

        cut.Find(".rec-empty-text").TextContent.Trim()
            .Should().Be("Aucun recruteur absent.");
    }

    [Fact]
    public void Etant_donne_le_filtre_Present_actif_sans_recruteur_present_quand_filtre_alors_l_etat_vide_contextuel_est_affiche()
    {
        var service = new RecruteurServiceStub(new[]
        {
            RecruteursHttpFixtures.FontaineJulien  // INCONNU uniquement
        });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonTroisRecruteurs));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-segmented-tab").Any());

        cut.FindAll(".rec-segmented-tab").First(c => c.TextContent.Trim() == "Présent").Click();

        cut.Find(".rec-empty-text").TextContent.Trim()
            .Should().Be("Aucun recruteur présent.");
    }

    [Fact]
    public void Etant_donne_un_filtre_actif_quand_je_retape_Tous_alors_tous_les_recruteurs_sont_de_nouveau_affiches()
    {
        var service = new RecruteurServiceStub(new[]
        {
            RecruteursHttpFixtures.DupontMarie,
            RecruteursHttpFixtures.MorelClaire,
            RecruteursHttpFixtures.FontaineJulien
        });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonTroisRecruteurs));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-segmented-tab").Any());

        cut.FindAll(".rec-segmented-tab").First(c => c.TextContent.Trim() == "Présent").Click();
        cut.FindAll(".rec-segmented-tab").First(c => c.TextContent.Trim() == "Tous").Click();

        cut.FindAll(".rec-row").Should().HaveCount(3);
        cut.Find(".rec-segmented-tab--actif").TextContent.Trim().Should().Be("Tous");
    }

    [Fact]
    public void Etant_donne_un_changement_de_filtre_quand_il_change_alors_le_compteur_et_la_barre_conservent_les_valeurs_globales()
    {
        var service = new RecruteurServiceStub(new[]
        {
            RecruteursHttpFixtures.DupontMarie,    // PRESENT
            RecruteursHttpFixtures.MorelClaire,    // ABSENT
            RecruteursHttpFixtures.FontaineJulien  // INCONNU
        });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonTroisRecruteurs));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-segmented-tab").Any());

        // Filtre sur "Absent" : seul MorelClaire est affiché
        cut.FindAll(".rec-segmented-tab").First(c => c.TextContent.Trim() == "Absent").Click();

        // Le compteur global (1 présent / 3 total) ne change pas
        cut.Find(".rec-counter").TextContent.Should().Be("1");
        cut.Find(".rec-total").TextContent.Should().Be("3");
        // La barre de progression reflète toujours le global
        cut.Find(".rec-progress-bar").GetAttribute("style").Should().Contain("width: 33%");
    }

    // ── US-3.04 : Modifier le statut via l'action sheet ─────────────────────

    [Fact]
    public void Etant_donne_la_liste_quand_j_appuie_sur_une_ligne_alors_l_action_sheet_s_ouvre_avec_la_raison_sociale_et_le_contact()
    {
        var service = new RecruteurServiceStub(new[]
        {
            RecruteursHttpFixtures.DupontMarie,
            RecruteursHttpFixtures.MorelClaire,
            RecruteursHttpFixtures.FontaineJulien
        });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-row").Any());

        cut.FindAll(".rec-row")[0].Click();

        cut.Find(".rec-action-sheet").Should().NotBeNull();
        cut.Find(".rec-action-sheet-title").TextContent.Trim()
            .Should().Be("Sélectionne l'état de présence");
        cut.Find(".rec-action-sheet-nom").TextContent.Trim()
            .Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Etant_donne_l_action_sheet_ouverte_quand_elle_est_affichee_alors_les_quatre_options_sont_presentes()
    {
        var service = new RecruteurServiceStub(new[] { RecruteursHttpFixtures.FontaineJulien });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-row").Any());

        cut.Find(".rec-row").Click();

        cut.Find(".rec-action-btn--present").TextContent.Trim().Should().Be("Présent");
        cut.Find(".rec-action-btn--absent").TextContent.Trim().Should().Be("Absent");
        cut.Find(".rec-action-btn--inconnu").TextContent.Trim().Should().Be("Participation inconnue");
        cut.Find(".rec-action-btn--annuler").TextContent.Trim().Should().Be("Annuler");
    }

    [Fact]
    public void Etant_donne_l_action_sheet_quand_Annuler_est_clique_alors_l_action_sheet_se_ferme_sans_modification()
    {
        var service = new RecruteurServiceStub(new[] { RecruteursHttpFixtures.FontaineJulien });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-row").Any());

        cut.Find(".rec-row").Click();
        cut.Find(".rec-action-btn--annuler").Click();

        cut.FindAll(".rec-action-sheet").Should().BeEmpty();
        cut.Find(".rec-badge--inconnu").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_l_action_sheet_quand_Present_est_choisi_alors_le_badge_devient_vert_et_l_action_sheet_se_ferme()
    {
        var service = new RecruteurServiceStub(new[] { RecruteursHttpFixtures.FontaineJulien });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-row").Any());

        cut.Find(".rec-row").Click();
        cut.Find(".rec-action-btn--present").Click();
        cut.WaitForState(() => !cut.FindAll(".rec-action-sheet").Any());

        cut.FindAll(".rec-action-sheet").Should().BeEmpty();
        cut.Find(".rec-badge--present").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_l_action_sheet_quand_Absent_est_choisi_alors_le_badge_devient_rouge_et_le_compteur_est_mis_a_jour()
    {
        var service = new RecruteurServiceStub(new[]
        {
            RecruteursHttpFixtures.FontaineJulien,  // INCONNU → sera mis à ABSENT
            RecruteursHttpFixtures.DupontMarie      // PRESENT
        });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-row").Any());

        // Avant : 1 présent (DupontMarie), FontaineJulien INCONNU
        cut.Find(".rec-counter").TextContent.Should().Be("1");

        // Clic sur FontaineJulien (Café de la Paix)
        cut.FindAll(".rec-row").First(r => r.TextContent.Contains("Café de la Paix")).Click();
        cut.Find(".rec-action-btn--absent").Click();
        cut.WaitForState(() => !cut.FindAll(".rec-action-sheet").Any());

        // Après : toujours 1 présent (DupontMarie), FontaineJulien ABSENT
        cut.Find(".rec-counter").TextContent.Should().Be("1");
        cut.FindAll(".rec-badge--absent").Should().HaveCount(1);
    }

    [Fact]
    public void Etant_donne_l_action_sheet_quand_Participation_inconnue_est_choisie_alors_le_badge_devient_gris()
    {
        var service = new RecruteurServiceStub(new[] { RecruteursHttpFixtures.DupontMarie });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-row").Any());

        cut.Find(".rec-row").Click();
        cut.Find(".rec-action-btn--inconnu").Click();
        cut.WaitForState(() => !cut.FindAll(".rec-action-sheet").Any());

        cut.Find(".rec-badge--inconnu").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_l_action_sheet_quand_Present_est_choisi_alors_le_compteur_est_mis_a_jour_sans_rechargement()
    {
        var service = new RecruteurServiceStub(new[]
        {
            RecruteursHttpFixtures.FontaineJulien,  // INCONNU → sera mis à PRESENT
            RecruteursHttpFixtures.MorelClaire      // ABSENT
        });
        EnregistrerServices(service, RecruteursHttpFixtures.ReponseOk(RecruteursHttpFixtures.ReponseJsonEvenementRestauration));

        var cut = RenderComponent<EvenementRecruteursPage>(parameters => parameters
            .Add(p => p.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".rec-row").Any());

        // Avant : 0 présent
        cut.Find(".rec-counter").TextContent.Should().Be("0");

        cut.FindAll(".rec-row").First(r => r.TextContent.Contains("Café de la Paix")).Click();
        cut.Find(".rec-action-btn--present").Click();
        cut.WaitForState(() => !cut.FindAll(".rec-action-sheet").Any());

        // Après : 1 présent
        cut.Find(".rec-counter").TextContent.Should().Be("1");
    }
}
