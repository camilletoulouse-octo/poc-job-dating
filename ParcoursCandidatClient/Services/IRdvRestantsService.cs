using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Services;

/// <summary>
/// Service de lecture des RDV restants d'un événement (US-5.06).
/// </summary>
public interface IRdvRestantsService
{
    /// <summary>
    /// Récupère les créneaux avec entretiens restants pour un événement donné.
    /// Retourne <c>null</c> si l'événement est introuvable (404).
    /// </summary>
    Task<RdvRestants?> GetRdvRestantsAsync(string evenementId, CancellationToken cancellationToken = default);
}
