using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ParcoursCandidatClient.Models;
using ParcoursCandidatClient.Services;
using ParcoursCandidatClient.Tests.Fixtures;
using ScannerPage = ParcoursCandidatClient.Pages.Scanner;

namespace ParcoursCandidatClient.Tests.Scanner;

/// <summary>
/// Tests E2E (parcours utilisateur) de la page <see cref="ScannerPage"/>
/// rendue avec bUnit. Ils couvrent :
///   - l'affichage du viewfinder quand la caméra est disponible,
///   - l'affichage du message d'erreur quand la caméra est indisponible,
///   - l'appel au service à l'initialisation,
///   - la mise à jour du statut de l'inscrit après scan,
///   - la présence des éléments d'interface.
/// </summary>
public class ScannerPageTests : TestContext
{
    private ScannerServiceMock EnregistrerServiceMock(bool cameraDisponible = true, bool ouvertureReussie = true)
    {
        var mock = new ScannerServiceMock(cameraDisponible, ouvertureReussie);
        Services.AddSingleton<IScannerService>(mock);
        return mock;
    }

    private void EnregistrerInscritServiceMock(Inscrit? inscritRetourne = null)
    {
        var mock = new InscritServiceMock(inscritRetourne);
        Services.AddSingleton<IInscritService>(mock);
    }

    [Fact]
    public void Etant_donne_une_camera_disponible_quand_la_page_est_rendue_alors_le_viewfinder_est_affiche()
    {
        EnregistrerServiceMock(cameraDisponible: true, ouvertureReussie: true);
        EnregistrerInscritServiceMock();

        var cut = RenderComponent<ScannerPage>(p => p.Add(x => x.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".sc-viewfinder").Any());

        cut.FindAll(".sc-viewfinder").Should().HaveCount(1);
    }

    [Fact]
    public void Etant_donne_une_camera_indisponible_quand_la_page_est_rendue_alors_le_message_d_erreur_est_affiche()
    {
        EnregistrerServiceMock(cameraDisponible: false);
        EnregistrerInscritServiceMock();

        var cut = RenderComponent<ScannerPage>(p => p.Add(x => x.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".sc-erreur").Any());

        cut.Find(".sc-erreur").TextContent.Should().Contain("caméra");
    }

    [Fact]
    public void Etant_donne_une_ouverture_de_camera_echouee_quand_la_page_est_rendue_alors_le_message_d_erreur_est_affiche()
    {
        EnregistrerServiceMock(cameraDisponible: true, ouvertureReussie: false);
        EnregistrerInscritServiceMock();

        var cut = RenderComponent<ScannerPage>(p => p.Add(x => x.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".sc-erreur").Any());

        cut.Find(".sc-erreur").TextContent.Should().Contain("caméra");
    }

    [Fact]
    public void Etant_donne_une_camera_disponible_quand_la_page_est_initialisee_alors_OuvrirCameraAsync_est_appele()
    {
        var mock = EnregistrerServiceMock(cameraDisponible: true, ouvertureReussie: true);
        EnregistrerInscritServiceMock();

        var cut = RenderComponent<ScannerPage>(p => p.Add(x => x.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".sc-viewfinder").Any());

        mock.OuvrirCameraAppeleAvec.Should().Be("sc-video");
    }

    [Fact]
    public void Etant_donne_la_page_rendue_quand_l_entete_est_affiche_alors_le_titre_Scanner_est_visible()
    {
        EnregistrerServiceMock();
        EnregistrerInscritServiceMock();

        var cut = RenderComponent<ScannerPage>(p => p.Add(x => x.EvenementId, "evt-001"));

        cut.Find(".sc-header-titre").TextContent.Trim().Should().Be("Scanner mon événement");
    }

    [Fact]
    public void Etant_donne_la_page_rendue_quand_l_entete_est_affiche_alors_le_bouton_retour_est_present()
    {
        EnregistrerServiceMock();
        EnregistrerInscritServiceMock();

        var cut = RenderComponent<ScannerPage>(p => p.Add(x => x.EvenementId, "evt-001"));

        cut.Find(".sc-header-back").GetAttribute("aria-label").Should().Be("Retour");
    }

    [Fact]
    public void Etant_donne_une_camera_disponible_quand_le_viewfinder_est_affiche_alors_l_element_video_est_present()
    {
        EnregistrerServiceMock(cameraDisponible: true, ouvertureReussie: true);
        EnregistrerInscritServiceMock();

        var cut = RenderComponent<ScannerPage>(p => p.Add(x => x.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".sc-video").Any());

        cut.Find(".sc-video").Id.Should().Be("sc-video");
    }

    [Fact]
    public void Etant_donne_une_camera_disponible_quand_le_viewfinder_est_affiche_alors_l_instruction_est_visible()
    {
        EnregistrerServiceMock(cameraDisponible: true, ouvertureReussie: true);
        EnregistrerInscritServiceMock();

        var cut = RenderComponent<ScannerPage>(p => p.Add(x => x.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".sc-instruction").Any());

        cut.Find(".sc-instruction").TextContent.Should().Contain("QR code");
    }

    [Fact]
    public void Etant_donne_une_camera_disponible_quand_la_page_est_initialisee_alors_DemarrerScanAsync_est_appele()
    {
        var mock = EnregistrerServiceMock(cameraDisponible: true, ouvertureReussie: true);
        EnregistrerInscritServiceMock();

        var cut = RenderComponent<ScannerPage>(p => p.Add(x => x.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".sc-viewfinder").Any());

        mock.ScanDemarre.Should().BeTrue();
    }

    [Fact]
    public async Task Etant_donne_un_qr_code_detecte_quand_l_inscrit_est_trouve_alors_son_nom_est_affiche()
    {
        EnregistrerServiceMock(cameraDisponible: true, ouvertureReussie: true);
        EnregistrerInscritServiceMock(ScannerFixtures.InscritMartin);

        var cut = RenderComponent<ScannerPage>(p => p.Add(x => x.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".sc-viewfinder").Any());

        await cut.InvokeAsync(() => cut.Instance.OnQrCodeDetecte("ins-001"));
        cut.WaitForState(() => cut.FindAll(".sc-overlay").Any());

        cut.Find(".sc-resultat-nom").TextContent.Should().ContainEquivalentOf("Martin");
    }

    [Fact]
    public async Task Etant_donne_un_qr_code_detecte_quand_l_inscrit_est_trouve_alors_le_badge_succes_est_visible()
    {
        EnregistrerServiceMock(cameraDisponible: true, ouvertureReussie: true);
        EnregistrerInscritServiceMock(ScannerFixtures.InscritMartin);

        var cut = RenderComponent<ScannerPage>(p => p.Add(x => x.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".sc-viewfinder").Any());

        await cut.InvokeAsync(() => cut.Instance.OnQrCodeDetecte("ins-001"));
        cut.WaitForState(() => cut.FindAll(".sc-overlay").Any());

        cut.FindAll(".sc-badge-succes").Should().HaveCount(1);
    }

    [Fact]
    public async Task Etant_donne_un_qr_code_detecte_quand_l_inscrit_est_introuvable_alors_le_badge_erreur_est_visible()
    {
        EnregistrerServiceMock(cameraDisponible: true, ouvertureReussie: true);
        EnregistrerInscritServiceMock(inscritRetourne: null);

        var cut = RenderComponent<ScannerPage>(p => p.Add(x => x.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".sc-viewfinder").Any());

        await cut.InvokeAsync(() => cut.Instance.OnQrCodeDetecte("ins-inconnu"));
        cut.WaitForState(() => cut.FindAll(".sc-overlay").Any());

        cut.FindAll(".sc-badge-erreur").Should().HaveCount(1);
    }

    [Fact]
    public async Task Etant_donne_un_qr_code_detecte_quand_le_resultat_est_affiche_alors_le_bouton_scan_suivant_est_present()
    {
        EnregistrerServiceMock(cameraDisponible: true, ouvertureReussie: true);
        EnregistrerInscritServiceMock(ScannerFixtures.InscritMartin);

        var cut = RenderComponent<ScannerPage>(p => p.Add(x => x.EvenementId, "evt-001"));
        cut.WaitForState(() => cut.FindAll(".sc-viewfinder").Any());

        await cut.InvokeAsync(() => cut.Instance.OnQrCodeDetecte("ins-001"));
        cut.WaitForState(() => cut.FindAll(".sc-btn-nouveau-scan").Any());

        cut.Find(".sc-btn-nouveau-scan").TextContent.Trim().Should().Be("Scan suivant");
    }
}
