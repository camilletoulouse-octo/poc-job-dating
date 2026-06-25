using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Services;

/// <summary>
/// Service de lecture et de mise à jour des recruteurs d'un événement (US-3.01 / US-3.04).
/// </summary>
public interface IRecruteurService
{
    Task<IReadOnlyList<Recruteur>> GetRecruteursAsync(string evenementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour le statut de présence d'un recruteur (US-3.04).
    /// Retourne le recruteur mis à jour, ou <c>null</c> si l'identifiant est introuvable (404).
    /// </summary>
    Task<Recruteur?> UpdateStatutAsync(string recruteurId, StatutRecruteur nouveauStatut, CancellationToken cancellationToken = default);
}
