using System.Net.Http.Json;
using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Services;

public class EvenementService
{
    private readonly HttpClient _httpClient;

    public EvenementService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Récupère les événements pour l'agence donnée et la date (par défaut : aujourd'hui).
    /// Le résultat est trié chronologiquement (US-1.01).
    /// </summary>
    public async Task<IReadOnlyList<Evenement>> GetEvenementsDuJourAsync(string agenceId, DateOnly? date = null, CancellationToken cancellationToken = default)
    {
        var dateParam = (date ?? DateOnly.FromDateTime(DateTime.Today)).ToString("yyyy-MM-dd");
        var url = $"api/evenements?agenceId={Uri.EscapeDataString(agenceId)}&date={dateParam}";

        var result = await _httpClient.GetFromJsonAsync<List<Evenement>>(url, cancellationToken);
        return result ?? new List<Evenement>();
    }

    /// <summary>
    /// Récupère le détail d'un événement par son identifiant (US-1.03).
    /// Renvoie <c>null</c> si l'événement n'existe pas (404).
    /// </summary>
    public async Task<Evenement?> GetEvenementByIdAsync(string evenementId, CancellationToken cancellationToken = default)
    {
        var url = $"api/evenements/{Uri.EscapeDataString(evenementId)}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Evenement>(cancellationToken: cancellationToken);
    }
}
