using System.Text.Json.Serialization;

namespace ParcoursCandidatClient.Models;

/// <summary>
/// Statut de présence d'un recruteur à un événement (US-3.01).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatutRecruteur
{
    PRESENT,
    ABSENT,
    INCONNU
}
