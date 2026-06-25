using System.Net;
using System.Text;

namespace ParcoursCandidatClient.Tests.Fixtures;

/// <summary>
/// Fixtures pour mocker les réponses HTTP renvoyées par l'API
/// pour les candidats sans RDV (US-5.08).
/// </summary>
internal static class CandidatsSansRdvHttpFixtures
{
    /// <summary>
    /// Données pour evt-001 : 3 candidats sans RDV (inscrits avec statut INCONNU).
    /// Triés par nom : Leroy Camille, Petit Emma, Robert Antoine.
    /// </summary>
    public const string ReponseJsonCandidatsSansRdvEvt001 = """
    {
      "evenementId": "evt-001",
      "candidats": [
        {
          "id": "ins-007",
          "nom": "Leroy",
          "prenom": "Camille",
          "telephone": "+33607080910"
        },
        {
          "id": "ins-005",
          "nom": "Petit",
          "prenom": "Emma",
          "telephone": "+33605060708"
        },
        {
          "id": "ins-004",
          "nom": "Robert",
          "prenom": "Antoine",
          "telephone": "+33604050607"
        }
      ]
    }
    """;

    /// <summary>
    /// Données avec un seul candidat sans RDV.
    /// </summary>
    public const string ReponseJsonCandidatsSansRdvUnCandidat = """
    {
      "evenementId": "evt-001",
      "candidats": [
        {
          "id": "ins-007",
          "nom": "Leroy",
          "prenom": "Camille",
          "telephone": "+33607080910"
        }
      ]
    }
    """;

    /// <summary>
    /// Données avec aucun candidat (liste vide).
    /// </summary>
    public const string ReponseJsonCandidatsSansRdvVide = """
    {
      "evenementId": "evt-001",
      "candidats": []
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
