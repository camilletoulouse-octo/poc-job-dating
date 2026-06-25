namespace ParcoursCandidatApi.Tests.Fixtures;

/// <summary>
/// Données de référence du jeu de mock <c>Data/recruteurs.json</c> (US-3.01).
/// Tout changement dans ce fichier doit être répercuté ici.
/// </summary>
internal static class RecruteursFixtures
{
    public const string EvenementJobDatingRestauration = "evt-001";
    public const string EvenementForumNumerique = "evt-002";
    public const string EvenementBtp = "evt-003";
    public const string EvenementSansRecruteur = "evt-005";
    public const string EvenementInexistant = "evt-inexistant";

    // Recruteurs du job dating restauration (evt-001).
    public const int NombreRecruteursRestauration = 5;
    public const int NombrePresentsRestauration = 2;
    public const int NombreAbsentsRestauration = 1;
    public const int NombreInconnusRestauration = 2;

    // Triés par raison sociale puis nom (ordre attendu de l'API).
    public static readonly string[] RecruteursRestaurationTriesParRaisonSociale =
    {
        "rec-001", // Brasserie du Vieux Lyon — Dupont Marie
        "rec-004", // Café de la Paix — Fontaine Julien
        "rec-003", // Groupe Sodexo — Morel Claire
        "rec-002", // Hôtel Bellecour — Lefebvre Pierre
        "rec-005"  // Traiteur Prestige — Girard Isabelle
    };
}
