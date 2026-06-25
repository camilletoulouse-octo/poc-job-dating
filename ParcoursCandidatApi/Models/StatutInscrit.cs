using System.Text.Json.Serialization;

namespace ParcoursCandidatApi.Models;

/// <summary>
/// Statut de présence d'un inscrit lors d'un événement (US-2.01).
/// Sérialisé en chaîne lisible côté API pour rester explicite.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatutInscrit
{
    PRESENT,
    ABSENT,
    INCONNU
}
