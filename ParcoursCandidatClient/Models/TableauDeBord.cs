namespace ParcoursCandidatClient.Models;

/// <summary>
/// Données de la carte « Vision globale » (US-5.01).
/// </summary>
public record VisionGlobale(
    int TotalEntretiens,
    int EntretiensRestants
);

/// <summary>
/// Données de la carte « Statut candidats » (US-5.02).
/// </summary>
public record StatutCandidats(
    int SansEntretien,
    int Presents,
    int Absents
);

/// <summary>
/// Données de la carte « Statut recruteurs » (US-5.03).
/// </summary>
public record StatutRecruteurs(
    int Presents,
    int Absents
);

/// <summary>
/// Données des 2e entretiens (OUI / PEUT-ÊTRE / NON) pour la carte « Suites & recrutements » (US-5.04).
/// </summary>
public record DeuxiemesEntretiens(
    int Oui,
    int PeutEtre,
    int Non
);

/// <summary>
/// Données de la carte « Suites &amp; recrutements » (US-5.04).
/// </summary>
public record SuitesRecrutements(
    int Recrutements,
    DeuxiemesEntretiens DeuxiemesEntretiens,
    int Immersions,
    int Poei
);

/// <summary>
/// Données de la carte « Enquêtes de satisfaction » (US-5.05).
/// </summary>
public record EnquetesSatisfaction(
    double? NoteCandidats,
    double? NoteRecruteurs
);

/// <summary>
/// Données du tableau de bord d'un événement (US-5.01, US-5.02, US-5.03, US-5.04, US-5.05).
/// </summary>
public record TableauDeBord(
    string EvenementId,
    VisionGlobale VisionGlobale,
    StatutCandidats StatutCandidats,
    StatutRecruteurs StatutRecruteurs,
    SuitesRecrutements? SuitesRecrutements,
    EnquetesSatisfaction? EnquetesSatisfaction
);
