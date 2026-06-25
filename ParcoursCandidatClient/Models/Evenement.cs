namespace ParcoursCandidatClient.Models;

public record Evenement(
    string Id,
    string Titre,
    string Organisme,
    string Ville,
    string Departement,
    string HeureDebut,
    string HeureFin,
    int NombreInscrits,
    string AgenceId,
    DateOnly Date
);
