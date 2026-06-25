namespace ParcoursCandidatApi.Models;

/// <summary>
/// Corps de la requête <c>PATCH /api/inscrits/{id}</c> (US-2.04).
/// </summary>
public record UpdateStatutRequest(StatutInscrit Statut);

/// <summary>
/// Corps de la requête <c>PATCH /api/recruteurs/{id}</c> (US-3.04).
/// </summary>
public record UpdateStatutRecruteurRequest(StatutRecruteur Statut);
