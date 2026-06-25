using System.Net;
using System.Net.Http.Json;
using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Services;

/// <summary>
/// Implémentation HTTP de <see cref="IRdvRestantsService"/> consommant
/// <c>GET /api/evenements/{id}/rdv-restants</c> (US-5.06).
/// </summary>
public class RdvRestantsService : IRdvRestantsService
{
    private readonly HttpClient _httpClient;

    public RdvRestantsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<RdvRestants?> GetRdvRestantsAsync(string evenementId, CancellationToken cancellationToken = default)
    {
        var url = $"api/evenements/{Uri.EscapeDataString(evenementId)}/rdv-restants";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RdvRestants>(cancellationToken: cancellationToken);
    }
}
