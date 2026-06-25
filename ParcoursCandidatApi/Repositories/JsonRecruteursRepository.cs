using System.Text.Json;
using ParcoursCandidatApi.Models;

namespace ParcoursCandidatApi.Repositories;

/// <summary>
/// Implémentation lisant et écrivant les recruteurs depuis le fichier mock
/// <c>Data/recruteurs.json</c> (US-3.01 / US-3.04).
/// </summary>
public class JsonRecruteursRepository : IRecruteursRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions JsonWriteOptions = new()
    {
        WriteIndented = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    private readonly string _filePath;
    private readonly Lock _lock = new();

    public JsonRecruteursRepository(string? filePath = null)
    {
        _filePath = filePath ?? ResolveDefaultPath();
    }

    public IReadOnlyList<Recruteur> GetByEvenementId(string evenementId)
    {
        if (string.IsNullOrWhiteSpace(evenementId))
        {
            return Array.Empty<Recruteur>();
        }

        var recruteurs = LoadAll();
        return recruteurs
            .Where(r => r.EvenementId.Equals(evenementId, StringComparison.OrdinalIgnoreCase))
            .OrderBy(r => r.RaisonSociale, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(r => r.Nom, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
    }

    /// <inheritdoc/>
    public Recruteur? UpdateStatut(string recruteurId, StatutRecruteur nouveauStatut)
    {
        lock (_lock)
        {
            var recruteurs = LoadAll();
            var index = recruteurs.FindIndex(r =>
                r.Id.Equals(recruteurId, StringComparison.OrdinalIgnoreCase));

            if (index < 0)
            {
                return null;
            }

            var recruteurMisAJour = recruteurs[index] with { Statut = nouveauStatut };
            recruteurs[index] = recruteurMisAJour;
            SaveAll(recruteurs);
            return recruteurMisAJour;
        }
    }

    private List<Recruteur> LoadAll()
    {
        using var stream = File.OpenRead(_filePath);
        return JsonSerializer.Deserialize<List<Recruteur>>(stream, JsonOptions) ?? new List<Recruteur>();
    }

    private void SaveAll(List<Recruteur> recruteurs)
    {
        var json = JsonSerializer.Serialize(recruteurs, JsonWriteOptions);
        File.WriteAllText(_filePath, json);
    }

    private static string ResolveDefaultPath()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "recruteurs.json");
        if (File.Exists(path))
        {
            return path;
        }

        // Fallback : exécution depuis le dossier source (dotnet run).
        return Path.Combine(Directory.GetCurrentDirectory(), "Data", "recruteurs.json");
    }
}
