namespace ParcoursCandidatApi.Tests.Fixtures;

/// <summary>
/// Données de référence du jeu de mock <c>Data/evenements.json</c>.
/// Tout changement dans ce fichier doit être répercuté ici.
/// </summary>
internal static class EvenementsFixtures
{
    public const string AgenceLyon = "lyon-part-dieu";
    public const string AgenceParis = "paris-republique";
    public const string AgenceInconnue = "agence-inexistante";

    public const string EvenementJobDatingRestauration = "evt-001";
    public const string EvenementForumNumerique = "evt-002";
    public const string EvenementBtp = "evt-003";
    public const string EvenementAtelierCv = "evt-004";
    public const string EvenementJobDatingLogistique = "evt-005";

    public static readonly string[] EvenementsLyonDuJourTriesParHeure =
    {
        EvenementJobDatingRestauration, // 09:00
        EvenementForumNumerique,        // 10:30
        EvenementBtp,                   // 14:00
        EvenementAtelierCv              // 15:30
    };
}
