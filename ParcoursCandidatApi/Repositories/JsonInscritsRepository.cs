using System.Text.Json;
using ParcoursCandidatApi.Models;

namespace ParcoursCandidatApi.Repositories;

/// <summary>
/// Implémentation lisant et écrivant les inscrits depuis le fichier mock
/// <c>Data/inscrits.json</c> (US-2.01 / US-2.04).
/// </summary>
public class JsonInscritsRepository : IInscritsRepository
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

    public JsonInscritsRepository(string? filePath = null)
    {
        _filePath = filePath ?? ResolveDefaultPath();
    }

    public IReadOnlyList<Inscrit> GetByEvenementId(string evenementId)
    {
        if (string.IsNullOrWhiteSpace(evenementId))
        {
            return Array.Empty<Inscrit>();
        }

        var inscrits = LoadAll();
        return inscrits
            .Where(i => i.EvenementId.Equals(evenementId, StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.Nom, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(i => i.Prenom, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
    }

    /// <inheritdoc/>
    public Inscrit? UpdateStatut(string inscritId, StatutInscrit nouveauStatut)
    {
        lock (_lock)
        {
            var inscrits = LoadAll();
            var index = inscrits.FindIndex(i =>
                i.Id.Equals(inscritId, StringComparison.OrdinalIgnoreCase));

            if (index < 0)
            {
                return null;
            }

            var inscritMisAJour = inscrits[index] with { Statut = nouveauStatut };
            inscrits[index] = inscritMisAJour;
            SaveAll(inscrits);
            return inscritMisAJour;
        }
    }

    private List<Inscrit> LoadAll()
    {
        using var stream = File.OpenRead(_filePath);
        return JsonSerializer.Deserialize<List<Inscrit>>(stream, JsonOptions) ?? new List<Inscrit>();
    }

    private void SaveAll(List<Inscrit> inscrits)
    {
        var json = JsonSerializer.Serialize(inscrits, JsonWriteOptions);
        File.WriteAllText(_filePath, json);
    }

    private static string ResolveDefaultPath()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "inscrits.json");
        if (File.Exists(path))
        {
            return path;
        }

        // Fallback : exécution depuis le dossier source (dotnet run).
        return Path.Combine(Directory.GetCurrentDirectory(), "Data", "inscrits.json");
    }
}
