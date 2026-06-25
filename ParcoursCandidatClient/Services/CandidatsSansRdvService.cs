using System.Net;
using System.Net.Http.Json;
using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Services;

/// <summary>
/// Implémentation HTTP de <see cref="ICandidatsSansRdvService"/> consommant
/// <c>GET /api/evenements/{id}/candidats-sans-rdv</c> (US-5.08).
/// </summary>
public class CandidatsSansRdvService : ICandidatsSansRdvService
{
    private readonly HttpClient _httpClient;

    public CandidatsSansRdvService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CandidatsSansRdv?> GetCandidatsSansRdvAsync(string evenementId, CancellationToken cancellationToken = default)
    {
        var url = $"api/evenements/{Uri.EscapeDataString(evenementId)}/candidats-sans-rdv";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CandidatsSansRdv>(cancellationToken: cancellationToken);
    }
}
