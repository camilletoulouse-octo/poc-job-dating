using System.Net;
using System.Text;
using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Tests.Fixtures;

/// <summary>
/// Fixtures pour mocker les réponses HTTP renvoyées par l'API
/// pour l'onglet "Les recruteurs" (US-3.01).
/// </summary>
internal static class RecruteursHttpFixtures
{
    public const string EvenementIdRestauration = "evt-001";

    public static readonly Recruteur DupontMarie = new(
        Id: "rec-001",
        EvenementId: EvenementIdRestauration,
        Nom: "Dupont",
        Prenom: "Marie",
        RaisonSociale: "Brasserie du Vieux Lyon",
        NombreOffres: 3,
        Statut: StatutRecruteur.PRESENT);

    public static readonly Recruteur MorelClaire = new(
        Id: "rec-003",
        EvenementId: EvenementIdRestauration,
        Nom: "Morel",
        Prenom: "Claire",
        RaisonSociale: "Groupe Sodexo",
        NombreOffres: 8,
        Statut: StatutRecruteur.ABSENT);

    public static readonly Recruteur FontaineJulien = new(
        Id: "rec-004",
        EvenementId: EvenementIdRestauration,
        Nom: "Fontaine",
        Prenom: "Julien",
        RaisonSociale: "Café de la Paix",
        NombreOffres: 2,
        Statut: StatutRecruteur.INCONNU);

    public const string ReponseJsonTroisRecruteurs = """
    [
      {
        "id": "rec-001",
        "evenementId": "evt-001",
        "nom": "Dupont",
        "prenom": "Marie",
        "raisonSociale": "Brasserie du Vieux Lyon",
        "nombreOffres": 3,
        "statut": "PRESENT"
      },
      {
        "id": "rec-003",
        "evenementId": "evt-001",
        "nom": "Morel",
        "prenom": "Claire",
        "raisonSociale": "Groupe Sodexo",
        "nombreOffres": 8,
        "statut": "ABSENT"
      },
      {
        "id": "rec-004",
        "evenementId": "evt-001",
        "nom": "Fontaine",
        "prenom": "Julien",
        "raisonSociale": "Café de la Paix",
        "nombreOffres": 2,
        "statut": "INCONNU"
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
      "nombreInscrits": 5,
      "agenceId": "lyon-part-dieu",
      "date": "2030-01-01"
    }
    """;

    /// <summary>
    /// Réponse JSON d'un recruteur unique avec statut PRESENT (US-3.04).
    /// </summary>
    public const string ReponseJsonRecruteurPresent = """
    {
      "id": "rec-001",
      "evenementId": "evt-001",
      "nom": "Dupont",
      "prenom": "Marie",
      "raisonSociale": "Brasserie du Vieux Lyon",
      "nombreOffres": 3,
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
