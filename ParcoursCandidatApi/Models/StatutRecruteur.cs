using System.Text.Json.Serialization;

namespace ParcoursCandidatApi.Models;

/// <summary>
/// Statut de présence d'un recruteur lors d'un événement (US-3.01).
/// Sérialisé en chaîne lisible côté API pour rester explicite.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatutRecruteur
{
    PRESENT,
    ABSENT,
    INCONNU
}
