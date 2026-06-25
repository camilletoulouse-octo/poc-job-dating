using Bunit;
using FluentAssertions;
using ParcoursCandidatClient.Models;
using ParcoursCandidatClient.Tests.Fixtures;
using InscritsContentComponent = ParcoursCandidatClient.Components.Evenement.InscritsContent;

namespace ParcoursCandidatClient.Tests.Candidats;

/// <summary>
/// Tests unitaires de la recherche d'un inscrit par nom/prénom (US-2.03).
/// </summary>
public class RechercheInscritsTests : TestContext
{
    private static readonly Inscrit BernardThomas = InscritsHttpFixtures.BernardThomas;
    private static readonly Inscrit DuboisSophie = InscritsHttpFixtures.DuboisSophie;
    private static readonly Inscrit MartinLucie = InscritsHttpFixtures.MartinLucie;

    private IRenderedComponent<InscritsContentComponent> RenderAvecInscrits(IReadOnlyList<Inscrit> inscrits) =>
        RenderComponent<InscritsContentComponent>(parameters => parameters
            .Add(p => p.Inscrits, inscrits)
            .Add(p => p.Chargement, false)
            .Add(p => p.EvenementId, "evt-001"));

    [Fact]
    public void Etant_donne_la_liste_chargee_quand_la_page_est_affichee_alors_le_champ_de_recherche_est_visible()
    {
        var cut = RenderAvecInscrits(new[] { BernardThomas, DuboisSophie, MartinLucie });

        cut.Find(".ins-recherche-input").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_la_liste_chargee_quand_la_page_est_affichee_alors_le_bouton_scanner_est_visible_avec_le_bon_lien()
    {
        var cut = RenderAvecInscrits(new[] { BernardThomas });

        var lien = cut.Find(".ins-btn-scanner");
        lien.Should().NotBeNull();
        lien.GetAttribute("href").Should().Be("/evenements/evt-001/scanner");
    }

    [Fact]
    public void Etant_donne_la_liste_quand_je_tape_un_nom_alors_seuls_les_inscrits_correspondants_sont_affiches()
    {
        var cut = RenderAvecInscrits(new[] { BernardThomas, DuboisSophie, MartinLucie });

        cut.Find(".ins-recherche-input").Input("Bernard");

        cut.FindAll(".ins-row").Should().HaveCount(1);
        cut.Find(".ins-nom").TextContent.Trim().Should().Be("Bernard Thomas");
    }

    [Fact]
    public void Etant_donne_la_liste_quand_je_tape_un_prenom_alors_seuls_les_inscrits_correspondants_sont_affiches()
    {
        var cut = RenderAvecInscrits(new[] { BernardThomas, DuboisSophie, MartinLucie });

        cut.Find(".ins-recherche-input").Input("Sophie");

        cut.FindAll(".ins-row").Should().HaveCount(1);
        cut.Find(".ins-nom").TextContent.Trim().Should().Be("Dubois Sophie");
    }

    [Fact]
    public void Etant_donne_la_liste_quand_je_tape_en_minuscules_alors_la_recherche_est_insensible_a_la_casse()
    {
        var cut = RenderAvecInscrits(new[] { BernardThomas, DuboisSophie, MartinLucie });

        cut.Find(".ins-recherche-input").Input("martin");

        cut.FindAll(".ins-row").Should().HaveCount(1);
        cut.Find(".ins-nom").TextContent.Trim().Should().Be("Martin Lucie");
    }

    [Fact]
    public void Etant_donne_la_liste_quand_je_tape_sans_accent_alors_la_recherche_est_insensible_aux_accents()
    {
        var inscritAvecAccent = new Inscrit(
            Id: "ins-010",
            EvenementId: "evt-001",
            Nom: "Léger",
            Prenom: "Élodie",
            Statut: StatutInscrit.INCONNU);

        var cut = RenderAvecInscrits(new[] { BernardThomas, inscritAvecAccent });

        cut.Find(".ins-recherche-input").Input("leger");

        cut.FindAll(".ins-row").Should().HaveCount(1);
        cut.Find(".ins-nom").TextContent.Trim().Should().Be("Léger Élodie");
    }

    [Fact]
    public void Etant_donne_une_recherche_active_quand_j_efface_le_texte_alors_tous_les_inscrits_sont_de_nouveau_affiches()
    {
        var cut = RenderAvecInscrits(new[] { BernardThomas, DuboisSophie, MartinLucie });

        cut.Find(".ins-recherche-input").Input("Bernard");
        cut.FindAll(".ins-row").Should().HaveCount(1);

        cut.Find(".ins-recherche-input").Input("");

        cut.FindAll(".ins-row").Should().HaveCount(3);
    }

    [Fact]
    public void Etant_donne_une_recherche_et_un_filtre_statut_quand_les_deux_sont_actifs_alors_seuls_les_inscrits_satisfaisant_les_deux_conditions_sont_affiches()
    {
        var cut = RenderAvecInscrits(new[] { BernardThomas, DuboisSophie, MartinLucie });

        cut.Find(".ins-recherche-input").Input("b");
        cut.FindAll(".ins-segmented-tab").First(t => t.TextContent.Trim() == "Présent").Click();

        cut.FindAll(".ins-row").Should().HaveCount(1);
        cut.Find(".ins-nom").TextContent.Trim().Should().Be("Bernard Thomas");
        cut.Find(".ins-badge--present").Should().NotBeNull();
    }

    [Fact]
    public void Etant_donne_une_recherche_sans_resultat_quand_aucun_inscrit_ne_correspond_alors_le_message_vide_est_affiche()
    {
        var cut = RenderAvecInscrits(new[] { BernardThomas, DuboisSophie, MartinLucie });

        cut.Find(".ins-recherche-input").Input("zzz");

        cut.FindAll(".ins-row").Should().BeEmpty();
        cut.Find(".ins-empty-text").Should().NotBeNull();
    }
}
