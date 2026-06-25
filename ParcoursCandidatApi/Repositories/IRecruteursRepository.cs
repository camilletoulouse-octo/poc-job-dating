using ParcoursCandidatApi.Models;

namespace ParcoursCandidatApi.Repositories;

/// <summary>
/// Accès aux recruteurs d'un événement (US-3.01 / US-3.04).
/// </summary>
public interface IRecruteursRepository
{
    IReadOnlyList<Recruteur> GetByEvenementId(string evenementId);

    /// <summary>
    /// Met à jour le statut de présence d'un recruteur (US-3.04).
    /// Retourne le recruteur mis à jour, ou <c>null</c> si l'identifiant est introuvable.
    /// </summary>
    Recruteur? UpdateStatut(string recruteurId, StatutRecruteur nouveauStatut);
}
