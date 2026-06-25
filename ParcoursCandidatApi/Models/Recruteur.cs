namespace ParcoursCandidatApi.Models;

/// <summary>
/// Recruteur participant à un événement de job dating (US-3.01).
/// </summary>
public record Recruteur(
    string Id,
    string EvenementId,
    string Nom,
    string Prenom,
    string RaisonSociale,
    int NombreOffres,
    StatutRecruteur Statut
);
