using System.Net;
using System.Text;
using ParcoursCandidatClient.Models;

namespace ParcoursCandidatClient.Tests.Fixtures;

/// <summary>
/// Fixtures pour mocker les réponses HTTP renvoyées par l'API à l'écran "Mes événements".
/// </summary>
internal static class EvenementsHttpFixtures
{
    public const string AgenceLyon = "lyon-part-dieu";
    public const string BaseAddress = "http://localhost/";

    public static readonly Evenement JobDatingRestauration = new(
        Id: "evt-001",
        Titre: "Job dating restauration",
        Organisme: "France Travail",
        Ville: "Lyon",
        Departement: "69",
        HeureDebut: "09:00",
        HeureFin: "12:00",
        NombreInscrits: 24,
        AgenceId: AgenceLyon,
        Date: DateOnly.FromDateTime(DateTime.Today));

    public static readonly Evenement RencontreBtp = new(
        Id: "evt-003",
        Titre: "Rencontre employeurs BTP",
        Organisme: "France Travail",
        Ville: "Lyon",
        Departement: "69",
        HeureDebut: "14:00",
        HeureFin: "17:00",
        NombreInscrits: 18,
        AgenceId: AgenceLyon,
        Date: DateOnly.FromDateTime(DateTime.Today));

    public static readonly IReadOnlyList<Evenement> DeuxEvenementsLyon =
        new[] { JobDatingRestauration, RencontreBtp };

    public const string ReponseJsonDeuxEvenementsLyon = """
    [
      {
        "id": "evt-001",
        "titre": "Job dating restauration",
        "organisme": "France Travail",
        "ville": "Lyon",
        "departement": "69",
        "heureDebut": "09:00",
        "heureFin": "12:00",
        "nombreInscrits": 24,
        "agenceId": "lyon-part-dieu",
        "date": "DATE_PLACEHOLDER"
      },
      {
        "id": "evt-003",
        "titre": "Rencontre employeurs BTP",
        "organisme": "France Travail",
        "ville": "Lyon",
        "departement": "69",
        "heureDebut": "14:00",
        "heureFin": "17:00",
        "nombreInscrits": 18,
        "agenceId": "lyon-part-dieu",
        "date": "DATE_PLACEHOLDER"
      }
    ]
    """;

    public static string ReponseJsonPourDate(DateOnly date) =>
        ReponseJsonDeuxEvenementsLyon.Replace("DATE_PLACEHOLDER", date.ToString("yyyy-MM-dd"));

    public static HttpResponseMessage ReponseOk(string json) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

    public static HttpResponseMessage ReponseListeVide() => ReponseOk("[]");

    public static HttpResponseMessage ReponseErreurServeur() =>
        new(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };
}
