using System.Net;
using System.Text;

namespace ParcoursCandidatClient.Tests.Fixtures;

/// <summary>
/// Fixtures pour mocker les réponses HTTP renvoyées par l'API
/// pour le tableau de bord (US-5.01, US-5.02, US-5.03, US-5.04, US-5.05).
/// </summary>
internal static class TableauDeBordHttpFixtures
{
    /// <summary>
    /// Données cohérentes avec inscrits.json et recruteurs.json pour evt-001 :
    /// Inscrits : 3 PRESENT (ins-001, ins-002, ins-006), 1 ABSENT (ins-003), 3 INCONNU (ins-004, ins-005, ins-007).
    /// Recruteurs : 2 PRESENT (rec-001, rec-002), 1 ABSENT (rec-003), 2 INCONNU (rec-004, rec-005).
    /// Suites : 5 recrutements, 2e entretiens (8 OUI / 4 PEUT-ÊTRE / 3 NON), 2 immersions, 1 POEI.
    /// </summary>
    public const string ReponseJsonTableauDeBordEvt001 = """
    {
      "evenementId": "evt-001",
      "visionGlobale": {
        "totalEntretiens": 24,
        "entretiensRestants": 9
      },
      "statutCandidats": {
        "sansEntretien": 3,
        "presents": 3,
        "absents": 1
      },
      "statutRecruteurs": {
        "presents": 2,
        "absents": 1
      },
      "suitesRecrutements": {
        "recrutements": 5,
        "deuxiemesEntretiens": {
          "oui": 8,
          "peutEtre": 4,
          "non": 3
        },
        "immersions": 2,
        "poei": 1
      }
    }
    """;

    /// <summary>
    /// Données avec tous les recruteurs présents (absents = 0) pour vérifier
    /// que la valeur "0" est bien affichée et non masquée (critère US-5.03).
    /// </summary>
    public const string ReponseJsonTableauDeBordEvt001TousPresents = """
    {
      "evenementId": "evt-001",
      "visionGlobale": {
        "totalEntretiens": 24,
        "entretiensRestants": 9
      },
      "statutCandidats": {
        "sansEntretien": 3,
        "presents": 3,
        "absents": 1
      },
      "statutRecruteurs": {
        "presents": 5,
        "absents": 0
      },
      "suitesRecrutements": {
        "recrutements": 5,
        "deuxiemesEntretiens": {
          "oui": 8,
          "peutEtre": 4,
          "non": 3
        },
        "immersions": 2,
        "poei": 1
      }
    }
    """;

    /// <summary>
    /// Données avec plusieurs recruteurs absents pour tester l'affichage d'absents > 0.
    /// </summary>
    public const string ReponseJsonTableauDeBordEvt001AvecAbsents = """
    {
      "evenementId": "evt-001",
      "visionGlobale": {
        "totalEntretiens": 18,
        "entretiensRestants": 5
      },
      "statutCandidats": {
        "sansEntretien": 1,
        "presents": 1,
        "absents": 1
      },
      "statutRecruteurs": {
        "presents": 1,
        "absents": 3
      },
      "suitesRecrutements": {
        "recrutements": 3,
        "deuxiemesEntretiens": {
          "oui": 5,
          "peutEtre": 2,
          "non": 4
        },
        "immersions": 1,
        "poei": 0
      }
    }
    """;

    /// <summary>
    /// Données sans la section suitesRecrutements pour tester l'absence de la carte (US-5.04).
    /// </summary>
    public const string ReponseJsonTableauDeBordEvt001SansSuites = """
    {
      "evenementId": "evt-001",
      "visionGlobale": {
        "totalEntretiens": 24,
        "entretiensRestants": 9
      },
      "statutCandidats": {
        "sansEntretien": 3,
        "presents": 3,
        "absents": 1
      },
      "statutRecruteurs": {
        "presents": 2,
        "absents": 1
      }
    }
    """;

    /// <summary>
    /// Données avec 2e entretiens tous à zéro pour tester la barre vide (US-5.04).
    /// </summary>
    public const string ReponseJsonTableauDeBordEvt001SuitesDeuxiemesVides = """
    {
      "evenementId": "evt-001",
      "visionGlobale": {
        "totalEntretiens": 24,
        "entretiensRestants": 9
      },
      "statutCandidats": {
        "sansEntretien": 3,
        "presents": 3,
        "absents": 1
      },
      "statutRecruteurs": {
        "presents": 2,
        "absents": 1
      },
      "suitesRecrutements": {
        "recrutements": 0,
        "deuxiemesEntretiens": {
          "oui": 0,
          "peutEtre": 0,
          "non": 0
        },
        "immersions": 0,
        "poei": 0
      }
    }
    """;

    /// <summary>
    /// Données avec enquêtes de satisfaction renseignées pour candidats et recruteurs (US-5.05).
    /// Note candidats : 3,5 — Note recruteurs : 3,2.
    /// </summary>
    public const string ReponseJsonTableauDeBordEvt001AvecSatisfaction = """
    {
      "evenementId": "evt-001",
      "visionGlobale": {
        "totalEntretiens": 24,
        "entretiensRestants": 9
      },
      "statutCandidats": {
        "sansEntretien": 3,
        "presents": 3,
        "absents": 1
      },
      "statutRecruteurs": {
        "presents": 2,
        "absents": 1
      },
      "suitesRecrutements": {
        "recrutements": 5,
        "deuxiemesEntretiens": {
          "oui": 8,
          "peutEtre": 4,
          "non": 3
        },
        "immersions": 2,
        "poei": 1
      },
      "enquetesSatisfaction": {
        "noteCandidats": 3.5,
        "noteRecruteurs": 3.2
      }
    }
    """;

    /// <summary>
    /// Données avec enquêtes de satisfaction sans aucune note soumise (US-5.05).
    /// Les deux notes sont null : affichage attendu « — » plutôt que zéro.
    /// </summary>
    public const string ReponseJsonTableauDeBordEvt001SatisfactionVide = """
    {
      "evenementId": "evt-001",
      "visionGlobale": {
        "totalEntretiens": 24,
        "entretiensRestants": 9
      },
      "statutCandidats": {
        "sansEntretien": 3,
        "presents": 3,
        "absents": 1
      },
      "statutRecruteurs": {
        "presents": 2,
        "absents": 1
      },
      "enquetesSatisfaction": {
        "noteCandidats": null,
        "noteRecruteurs": null
      }
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
