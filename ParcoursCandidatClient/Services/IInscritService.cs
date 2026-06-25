using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Services;

/// <summary>
/// Service de lecture et de mise à jour des inscrits d'un événement (US-2.01 / US-2.04).
/// </summary>
public interface IInscritService
{
    Task<IReadOnlyList<Inscrit>> GetInscritsAsync(string evenementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour le statut de présence d'un inscrit (US-2.04).
    /// Retourne l'inscrit mis à jour, ou <c>null</c> si l'identifiant est introuvable (404).
    /// </summary>
    Task<Inscrit?> UpdateStatutAsync(string inscritId, StatutInscrit nouveauStatut, CancellationToken cancellationToken = default);
}
