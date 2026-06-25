using System.Net;
using System.Text;
using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Tests.Fixtures;

/// <summary>
/// Fixtures pour mocker les réponses HTTP renvoyées par l'API
/// pour l'onglet "Les inscrits" (US-2.01).
/// </summary>
internal static class InscritsHttpFixtures
{
    public const string EvenementIdRestauration = "evt-001";

    public static readonly Inscrit BernardThomas = new(
        Id: "ins-002",
        EvenementId: EvenementIdRestauration,
        Nom: "Bernard",
        Prenom: "Thomas",
        Statut: StatutInscrit.PRESENT);

    public static readonly Inscrit DuboisSophie = new(
        Id: "ins-003",
        EvenementId: EvenementIdRestauration,
        Nom: "Dubois",
        Prenom: "Sophie",
        Statut: StatutInscrit.ABSENT);

    public static readonly Inscrit MartinLucie = new(
        Id: "ins-001",
        EvenementId: EvenementIdRestauration,
        Nom: "Martin",
        Prenom: "Lucie",
        Statut: StatutInscrit.INCONNU);

    public const string ReponseJsonQuatreInscrits = """
    [
      {
        "id": "ins-002",
        "evenementId": "evt-001",
        "nom": "Bernard",
        "prenom": "Thomas",
        "statut": "PRESENT"
      },
      {
        "id": "ins-003",
        "evenementId": "evt-001",
        "nom": "Dubois",
        "prenom": "Sophie",
        "statut": "ABSENT"
      },
      {
        "id": "ins-001",
        "evenementId": "evt-001",
        "nom": "Martin",
        "prenom": "Lucie",
        "statut": "INCONNU"
      },
      {
        "id": "ins-004",
        "evenementId": "evt-001",
        "nom": "Robert",
        "prenom": "Antoine",
        "statut": "PRESENT"
      }
    ]
    """;

    public const string ReponseJsonEvenementRestauration = """
    {
      "id": "evt-001",
      "titre": "Job dating restauration",
      "organisme": "France Travail",
      "ville": "Lyon",
      "departement": "69",
      "heureDebut": "09:00",
      "heureFin": "12:00",
      "nombreInscrits": 4,
      "agenceId": "lyon-part-dieu",
      "date": "2030-01-01"
    }
    """;

    /// <summary>
    /// Réponse JSON d'un inscrit unique avec statut PRESENT (US-2.04).
    /// </summary>
    public const string ReponseJsonInscritPresent = """
    {
      "id": "ins-002",
      "evenementId": "evt-001",
      "nom": "Bernard",
      "prenom": "Thomas",
      "statut": "PRESENT"
    }
    """;

    public static HttpResponseMessage ReponseOk(string json) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

    public static HttpResponseMessage ReponseListeVide() => ReponseOk("[]");

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
