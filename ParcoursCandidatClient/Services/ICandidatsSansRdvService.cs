using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Services;

/// <summary>
/// Service de lecture des candidats sans RDV d'un événement (US-5.08).
/// </summary>
public interface ICandidatsSansRdvService
{
    /// <summary>
    /// Récupère la liste des candidats sans entretien planifié pour un événement donné.
    /// Retourne <c>null</c> si l'événement est introuvable (404).
    /// </summary>
    Task<CandidatsSansRdv?> GetCandidatsSansRdvAsync(string evenementId, CancellationToken cancellationToken = default);
}
