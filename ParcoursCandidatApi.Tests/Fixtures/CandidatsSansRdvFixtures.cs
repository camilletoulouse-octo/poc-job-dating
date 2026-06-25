namespace ParcoursCandidatApi.Tests.Fixtures;

/// <summary>
/// Données de référence pour les candidats sans RDV (US-5.08).
/// Les candidats sans RDV sont calculés dynamiquement depuis <c>Data/inscrits.json</c>
/// en filtrant les inscrits avec statut INCONNU.
/// Tout changement dans inscrits.json doit être répercuté ici.
/// </summary>
internal static class CandidatsSansRdvFixtures
{
    public const string EvenementJobDatingRestauration = "evt-001";
    public const string EvenementForumNumerique = "evt-002";
    public const string EvenementBtp = "evt-003";
    public const string EvenementInexistant = "evt-inexistant";

    // Candidats sans RDV du job dating restauration (evt-001) = inscrits avec statut INCONNU.
    // Triés par nom : ins-007 (Leroy Camille), ins-005 (Petit Emma), ins-004 (Robert Antoine)
    public const int NombreCandidatsSansRdvRestauration = 3;

    // Premier candidat trié par nom alphabétique = Leroy Camille
    public const string PremierCandidatNom = "Leroy";
    public const string PremierCandidatPrenom = "Camille";
    public const string PremierCandidatTelephone = "+33607080910";
}
