namespace ParcoursCandidatClient.Models;

/// <summary>
/// Candidat sans entretien planifié pour un événement donné (US-5.08).
/// </summary>
public record CandidatSansRdv(
    string Id,
    string Nom,
    string Prenom,
    string? Telephone
);

/// <summary>
/// Données de la page « Candidats sans RDV » d'un événement (US-5.08).
/// </summary>
public record CandidatsSansRdv(
    string EvenementId,
    IReadOnlyList<CandidatSansRdv> Candidats
);
