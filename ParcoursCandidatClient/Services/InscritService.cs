using System.Net;
using System.Net.Http.Json;
using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Services;

/// <summary>
/// Implémentation HTTP de <see cref="IInscritService"/> consommant
/// <c>GET /api/evenements/{id}/inscrits</c> (US-2.01) et
/// <c>PATCH /api/inscrits/{id}</c> (US-2.04).
/// </summary>
public class InscritService : IInscritService
{
    private readonly HttpClient _httpClient;

    public InscritService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<Inscrit>> GetInscritsAsync(string evenementId, CancellationToken cancellationToken = default)
    {
        var url = $"api/evenements/{Uri.EscapeDataString(evenementId)}/inscrits";
        var result = await _httpClient.GetFromJsonAsync<List<Inscrit>>(url, cancellationToken);
        return result ?? new List<Inscrit>();
    }

    public async Task<Inscrit?> UpdateStatutAsync(string inscritId, StatutInscrit nouveauStatut, CancellationToken cancellationToken = default)
    {
        var url = $"api/inscrits/{Uri.EscapeDataString(inscritId)}";
        var body = new { statut = nouveauStatut.ToString() };
        var response = await _httpClient.PatchAsJsonAsync(url, body, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Inscrit>(cancellationToken: cancellationToken);
    }
}
