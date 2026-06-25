using System.Net;
using System.Net.Http.Json;
using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Services;

/// <summary>
/// Implémentation HTTP de <see cref="IRecruteurService"/> consommant
/// <c>GET /api/evenements/{id}/recruteurs</c> (US-3.01) et
/// <c>PATCH /api/recruteurs/{id}</c> (US-3.04).
/// </summary>
public class RecruteurService : IRecruteurService
{
    private readonly HttpClient _httpClient;

    public RecruteurService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<Recruteur>> GetRecruteursAsync(string evenementId, CancellationToken cancellationToken = default)
    {
        var url = $"api/evenements/{Uri.EscapeDataString(evenementId)}/recruteurs";
        var result = await _httpClient.GetFromJsonAsync<List<Recruteur>>(url, cancellationToken);
        return result ?? new List<Recruteur>();
    }

    public async Task<Recruteur?> UpdateStatutAsync(string recruteurId, StatutRecruteur nouveauStatut, CancellationToken cancellationToken = default)
    {
        var url = $"api/recruteurs/{Uri.EscapeDataString(recruteurId)}";
        var body = new { statut = nouveauStatut.ToString() };
        var response = await _httpClient.PatchAsJsonAsync(url, body, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Recruteur>(cancellationToken: cancellationToken);
    }
}
