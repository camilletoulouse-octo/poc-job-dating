using ParcoursCandidatClient.Models;
using ParcoursCandidatClient.Services;

namespace ParcoursCandidatClient.Tests.Fixtures;

/// <summary>
/// Implémentation de test de <see cref="IInscritService"/> permettant de
/// simuler la récupération et la mise à jour des inscrits.
/// </summary>
public class InscritServiceMock : IInscritService
{
    private readonly Inscrit? _inscritRetourne;

    public string? UpdateStatutAppeleAvecId { get; private set; }
    public StatutInscrit? UpdateStatutAppeleAvecStatut { get; private set; }

    public InscritServiceMock(Inscrit? inscritRetourne = null)
    {
        _inscritRetourne = inscritRetourne;
    }

    public Task<IReadOnlyList<Inscrit>> GetInscritsAsync(string evenementId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Inscrit>>(Array.Empty<Inscrit>());
    }

    public Task<Inscrit?> UpdateStatutAsync(string inscritId, StatutInscrit nouveauStatut, CancellationToken cancellationToken = default)
    {
        UpdateStatutAppeleAvecId = inscritId;
        UpdateStatutAppeleAvecStatut = nouveauStatut;
        return Task.FromResult(_inscritRetourne);
    }
}
