using System.Net;
using System.Text;

namespace ParcoursCandidatClient.Tests.Fixtures;

/// <summary>
/// Fixtures pour mocker les réponses HTTP renvoyées par l'API
/// pour les RDV restants (US-5.06).
/// </summary>
internal static class RdvRestantsHttpFixtures
{
    /// <summary>
    /// Données pour evt-001 : 4 recruteurs avec leurs créneaux RDV restants.
    /// Recruteur rec-001 (Marie Dupont / Brasserie du Vieux Lyon) : 2 créneaux.
    /// Recruteur rec-002 (Pierre Lefebvre / Hôtel Bellecour) : 1 créneau.
    /// Recruteur rec-004 (Julien Fontaine / Café de la Paix) : 3 créneaux.
    /// Recruteur rec-005 (Isabelle Girard / Traiteur Prestige) : 1 créneau.
    /// </summary>
    public const string ReponseJsonRdvRestantsEvt001 = """
    {
      "evenementId": "evt-001",
      "recruteurs": [
        {
          "recruteurId": "rec-001",
          "nom": "Dupont",
          "prenom": "Marie",
          "raisonSociale": "Brasserie du Vieux Lyon",
          "creneaux": [
            { "heure": "10:00", "candidatNom": "ROBERT", "candidatPrenom": "Antoine" },
            { "heure": "10:15", "candidatNom": "PETIT", "candidatPrenom": "Emma" }
          ]
        },
        {
          "recruteurId": "rec-002",
          "nom": "Lefebvre",
          "prenom": "Pierre",
          "raisonSociale": "Hôtel Bellecour",
          "creneaux": [
            { "heure": "10:30", "candidatNom": "LEROY", "candidatPrenom": "Camille" }
          ]
        },
        {
          "recruteurId": "rec-004",
          "nom": "Fontaine",
          "prenom": "Julien",
          "raisonSociale": "Café de la Paix",
          "creneaux": [
            { "heure": "11:00", "candidatNom": "ROBERT", "candidatPrenom": "Antoine" },
            { "heure": "11:15", "candidatNom": "PETIT", "candidatPrenom": "Emma" },
            { "heure": "11:30", "candidatNom": "LEROY", "candidatPrenom": "Camille" }
          ]
        },
        {
          "recruteurId": "rec-005",
          "nom": "Girard",
          "prenom": "Isabelle",
          "raisonSociale": "Traiteur Prestige",
          "creneaux": [
            { "heure": "14:00", "candidatNom": "ROBERT", "candidatPrenom": "Antoine" }
          ]
        }
      ]
    }
    """;

    /// <summary>
    /// Données avec un seul recruteur et un seul créneau.
    /// </summary>
    public const string ReponseJsonRdvRestantsUnRecruteur = """
    {
      "evenementId": "evt-001",
      "recruteurs": [
        {
          "recruteurId": "rec-001",
          "nom": "Dupont",
          "prenom": "Marie",
          "raisonSociale": "Brasserie du Vieux Lyon",
          "creneaux": [
            { "heure": "10:00", "candidatNom": "ROBERT", "candidatPrenom": "Antoine" }
          ]
        }
      ]
    }
    """;

    /// <summary>
    /// Données avec aucun recruteur (liste vide).
    /// </summary>
    public const string ReponseJsonRdvRestantsVide = """
    {
      "evenementId": "evt-001",
      "recruteurs": []
    }
    """;

    public static HttpResponseMessage ReponseOk(string json) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

    public static HttpResponseMessage ReponseNotFound() =>
        new(HttpStatusCode.NotFound)
        {
            Content = new StringContent("", Encoding.UTF8, "application/json")
        };

    public static HttpResponseMessage ReponseErreurServeur() =>
        new(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };
}
