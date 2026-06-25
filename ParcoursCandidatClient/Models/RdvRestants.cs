namespace ParcoursCandidatClient.Models;

/// <summary>
/// Créneau RDV restant pour un candidat donné (US-5.06).
/// </summary>
public record CreneauRdvRestant(
    string Heure,
    string CandidatNom,
    string CandidatPrenom
);

/// <summary>
/// Recruteur avec ses créneaux RDV restants (US-5.06).
/// </summary>
public record RecruteurRdvRestant(
    string RecruteurId,
    string Nom,
    string Prenom,
    string RaisonSociale,
    IReadOnlyList<CreneauRdvRestant> Creneaux
);

/// <summary>
/// Données de la page « RDV restants » d'un événement (US-5.06).
/// </summary>
public record RdvRestants(
    string EvenementId,
    IReadOnlyList<RecruteurRdvRestant> Recruteurs
);
