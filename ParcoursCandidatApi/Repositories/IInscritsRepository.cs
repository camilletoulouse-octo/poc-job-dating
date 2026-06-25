using ParcoursCandidatApi.Models;

namespace ParcoursCandidatApi.Repositories;

/// <summary>
/// Accès aux inscrits d'un événement (US-2.01 / US-2.04).
/// </summary>
public interface IInscritsRepository
{
    IReadOnlyList<Inscrit> GetByEvenementId(string evenementId);

    /// <summary>
    /// Met à jour le statut de présence d'un inscrit (US-2.04).
    /// Retourne l'inscrit mis à jour, ou <c>null</c> si l'identifiant est introuvable.
    /// </summary>
    Inscrit? UpdateStatut(string inscritId, StatutInscrit nouveauStatut);
}
