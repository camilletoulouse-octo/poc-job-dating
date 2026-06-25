using System.Text.Json.Serialization;

namespace ParcoursCandidatClient.Models;

/// <summary>
/// Statut de présence d'un inscrit à un événement (US-2.01).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatutInscrit
{
    PRESENT,
    ABSENT,
    INCONNU
}
