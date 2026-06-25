namespace ParcoursCandidatApi.Tests.Fixtures;

/// <summary>
/// Données de référence du jeu de mock <c>Data/inscrits.json</c> (US-2.01).
/// Tout changement dans ce fichier doit être répercuté ici.
/// </summary>
internal static class InscritsFixtures
{
    public const string EvenementJobDatingRestauration = "evt-001";
    public const string EvenementForumNumerique = "evt-002";
    public const string EvenementBtp = "evt-003";
    public const string EvenementSansInscrit = "evt-005";
    public const string EvenementInexistant = "evt-inexistant";

    // Inscrits du job dating restauration (evt-001).
    public const int NombreInscritsRestauration = 7;
    public const int NombrePresentsRestauration = 3;
    public const int NombreAbsentsRestauration = 1;
    public const int NombreInconnusRestauration = 3;

    public static readonly string[] InscritsRestaurationTriesParNom =
    {
        "ins-002", // Bernard Thomas
        "ins-003", // Dubois Sophie
        "ins-006", // Durand Hugo
        "ins-007", // Leroy Camille
        "ins-001", // Martin Lucie
        "ins-005", // Petit Emma
        "ins-004"  // Robert Antoine
    };
}
