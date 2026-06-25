namespace ParcoursCandidatClient.Models;

/// <summary>
/// Inscrit à un événement de job dating (US-2.01).
/// </summary>
public record Inscrit(
    string Id,
    string EvenementId,
    string Nom,
    string Prenom,
    StatutInscrit Statut
);
