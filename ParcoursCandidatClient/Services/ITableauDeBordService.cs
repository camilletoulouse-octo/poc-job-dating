using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Services;

/// <summary>
/// Service de lecture du tableau de bord d'un événement (US-5.01).
/// </summary>
public interface ITableauDeBordService
{
    /// <summary>
    /// Récupère les données du tableau de bord pour un événement donné.
    /// Retourne <c>null</c> si l'événement est introuvable (404).
    /// </summary>
    Task<TableauDeBord?> GetTableauDeBordAsync(string evenementId, CancellationToken cancellationToken = default);
}
