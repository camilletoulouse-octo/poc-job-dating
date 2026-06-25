using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Tests.Scanner;

/// <summary>
/// Données de test pour les tests de la page Scanner.
/// </summary>
public static class ScannerFixtures
{
    public static readonly Inscrit InscritMartin = new(
        Id: "ins-001",
        EvenementId: "evt-001",
        Nom: "Martin",
        Prenom: "Lucie",
        Statut: StatutInscrit.PRESENT
    );

    public static readonly Inscrit InscritBernard = new(
        Id: "ins-002",
        EvenementId: "evt-001",
        Nom: "Bernard",
        Prenom: "Thomas",
        Statut: StatutInscrit.PRESENT
    );
}
