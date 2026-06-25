using System.Net;
using System.Net.Http.Json;
using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Services;

/// <summary>
/// Implémentation HTTP de <see cref="ITableauDeBordService"/> consommant
/// <c>GET /api/evenements/{id}/tableau-de-bord</c> (US-5.01).
/// </summary>
public class TableauDeBordService : ITableauDeBordService
{
    private readonly HttpClient _httpClient;

    public TableauDeBordService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TableauDeBord?> GetTableauDeBordAsync(string evenementId, CancellationToken cancellationToken = default)
    {
        var url = $"api/evenements/{Uri.EscapeDataString(evenementId)}/tableau-de-bord";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TableauDeBord>(cancellationToken: cancellationToken);
    }
}
