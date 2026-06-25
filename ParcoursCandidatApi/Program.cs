using System.Text.Json;
using ParcoursCandidatApi.Models;
using ParcoursCandidatApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<IInscritsRepository>(_ => new JsonInscritsRepository());
builder.Services.AddSingleton<IRecruteursRepository>(_ => new JsonRecruteursRepository());

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Charge les événements depuis le fichier JSON et substitue les dates relatives.
List<Evenement> LoadEvenements()
{
    var path = Path.Combine(AppContext.BaseDirectory, "Data", "evenements.json");
    if (!File.Exists(path))
    {
        // Fallback: chemin source pendant le dev (dotnet run)
        path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "evenements.json");
    }

    using var stream = File.OpenRead(path);
    using var doc = JsonDocument.Parse(stream);

    var today = DateOnly.FromDateTime(DateTime.Today);
    var tomorrow = today.AddDays(1);
    var yesterday = today.AddDays(-1);

    var list = new List<Evenement>();
    foreach (var el in doc.RootElement.EnumerateArray())
    {
        var rawDate = el.GetProperty("date").GetString() ?? "TODAY";
        var date = rawDate switch
        {
            "TODAY" => today,
            "TOMORROW" => tomorrow,
            "YESTERDAY" => yesterday,
            _ => DateOnly.Parse(rawDate)
        };

        list.Add(new Evenement(
            el.GetProperty("id").GetString()!,
            el.GetProperty("titre").GetString()!,
            el.GetProperty("organisme").GetString()!,
            el.GetProperty("ville").GetString()!,
            el.GetProperty("departement").GetString()!,
            el.GetProperty("heureDebut").GetString()!,
            el.GetProperty("heureFin").GetString()!,
            el.GetProperty("nombreInscrits").GetInt32(),
            el.GetProperty("agenceId").GetString()!,
            date
        ));
    }

    return list;
}

app.MapGet("/api/evenements", (string? agenceId, DateOnly? date) =>
{
    var evenements = LoadEvenements();
    var dateFiltre = date ?? DateOnly.FromDateTime(DateTime.Today);
    var query = evenements.Where(e => e.Date == dateFiltre);

    if (!string.IsNullOrWhiteSpace(agenceId))
    {
        query = query.Where(e => e.AgenceId.Equals(agenceId, StringComparison.OrdinalIgnoreCase));
    }

    // Tri chronologique croissant (US-1.01)
    return query
        .OrderBy(e => e.HeureDebut, StringComparer.Ordinal)
        .ToArray();
})
.WithName("GetEvenements");

// US-1.03 — détail d'un événement à l'ouverture de son contexte ("Les inscrits").
app.MapGet("/api/evenements/{id}", (string id) =>
{
    var evenement = LoadEvenements()
        .FirstOrDefault(e => e.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

    return evenement is null ? Results.NotFound() : Results.Ok(evenement);
})
.WithName("GetEvenementById");

// US-2.01 — liste des inscrits d'un événement.
app.MapGet("/api/evenements/{id}/inscrits", (string id, IInscritsRepository repository) =>
{
    var inscrits = repository.GetByEvenementId(id);
    return Results.Ok(inscrits);
})
.WithName("GetInscritsByEvenementId");

// US-2.04 — modifier le statut de présence d'un inscrit.
app.MapPatch("/api/inscrits/{id}", (string id, UpdateStatutRequest body, IInscritsRepository repository) =>
{
    var inscrit = repository.UpdateStatut(id, body.Statut);
    return inscrit is null ? Results.NotFound() : Results.Ok(inscrit);
})
.WithName("UpdateStatutInscrit");

// US-3.01 — liste des recruteurs d'un événement.
app.MapGet("/api/evenements/{id}/recruteurs", (string id, IRecruteursRepository repository) =>
{
    var recruteurs = repository.GetByEvenementId(id);
    return Results.Ok(recruteurs);
})
.WithName("GetRecruteursByEvenementId");

// US-3.04 — modifier le statut de présence d'un recruteur.
app.MapPatch("/api/recruteurs/{id}", (string id, UpdateStatutRecruteurRequest body, IRecruteursRepository repository) =>
{
    var recruteur = repository.UpdateStatut(id, body.Statut);
    return recruteur is null ? Results.NotFound() : Results.Ok(recruteur);
})
.WithName("UpdateStatutRecruteur");

// US-5.01, US-5.02, US-5.03, US-5.04, US-5.05 — tableau de bord d'un événement.
app.MapGet("/api/evenements/{id}/tableau-de-bord", (string id, IInscritsRepository inscritsRepository, IRecruteursRepository recruteursRepository) =>
{
    var path = Path.Combine(AppContext.BaseDirectory, "Data", "tableau-de-bord.json");
    if (!File.Exists(path))
    {
        path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "tableau-de-bord.json");
    }

    using var stream = File.OpenRead(path);
    using var doc = JsonDocument.Parse(stream);

    foreach (var el in doc.RootElement.EnumerateArray())
    {
        var evenementId = el.GetProperty("evenementId").GetString() ?? "";
        if (!evenementId.Equals(id, StringComparison.OrdinalIgnoreCase)) continue;

        var visionGlobale = el.GetProperty("visionGlobale");

        // US-5.02 : calcul dynamique depuis inscrits.json pour rester cohérent avec la liste inscrits.
        var inscrits = inscritsRepository.GetByEvenementId(id);
        var inscritsPresents = inscrits.Count(i => i.Statut == StatutInscrit.PRESENT);
        var inscritsAbsents = inscrits.Count(i => i.Statut == StatutInscrit.ABSENT);
        var inscritsSansEntretien = inscrits.Count(i => i.Statut == StatutInscrit.INCONNU);

        // US-5.03 : calcul dynamique depuis recruteurs.json pour rester cohérent avec la liste recruteurs.
        var recruteurs = recruteursRepository.GetByEvenementId(id);
        var recruteursPresents = recruteurs.Count(r => r.Statut == StatutRecruteur.PRESENT);
        var recruteursAbsents = recruteurs.Count(r => r.Statut == StatutRecruteur.ABSENT);

        // US-5.04 : suites & recrutements depuis tableau-de-bord.json, proportions dynamiques selon inscrits présents.
        object? suitesRecrutements = null;
        if (el.TryGetProperty("suitesRecrutements", out var suitesEl))
        {
            var recrutements = suitesEl.GetProperty("recrutements").GetInt32();
            var deuxiemesEl = suitesEl.GetProperty("deuxiemesEntretiens");
            var deuxiemesOui = deuxiemesEl.GetProperty("oui").GetInt32();
            var deuxiemesPeutEtre = deuxiemesEl.GetProperty("peutEtre").GetInt32();
            var deuxiemesNon = deuxiemesEl.GetProperty("non").GetInt32();
            var immersions = suitesEl.GetProperty("immersions").GetInt32();
            var poei = suitesEl.GetProperty("poei").GetInt32();

            // Recalcul dynamique des proportions selon le nombre de candidats présents.
            var totalPresents = inscritsPresents > 0 ? inscritsPresents : 1;
            var totalSuites = recrutements + deuxiemesOui + deuxiemesPeutEtre + deuxiemesNon + immersions + poei;
            if (totalSuites > totalPresents)
            {
                var ratio = (double)totalPresents / totalSuites;
                recrutements = (int)Math.Round(recrutements * ratio);
                deuxiemesOui = (int)Math.Round(deuxiemesOui * ratio);
                deuxiemesPeutEtre = (int)Math.Round(deuxiemesPeutEtre * ratio);
                deuxiemesNon = (int)Math.Round(deuxiemesNon * ratio);
                immersions = (int)Math.Round(immersions * ratio);
                poei = (int)Math.Round(poei * ratio);
            }

            suitesRecrutements = new
            {
                recrutements,
                deuxiemesEntretiens = new
                {
                    oui = deuxiemesOui,
                    peutEtre = deuxiemesPeutEtre,
                    non = deuxiemesNon
                },
                immersions,
                poei
            };
        }

        // US-5.05 : notes de satisfaction depuis tableau-de-bord.json.
        // Les notes sont dynamiques : calculées en proportion du nombre de candidats/recruteurs présents.
        object? enquetesSatisfaction = null;
        if (el.TryGetProperty("enquetesSatisfaction", out var satisfactionEl))
        {
            double? noteCandidats = null;
            double? noteRecruteurs = null;

            if (satisfactionEl.TryGetProperty("noteCandidats", out var noteCandidatsEl)
                && noteCandidatsEl.ValueKind == JsonValueKind.Number
                && inscritsPresents > 0)
            {
                noteCandidats = noteCandidatsEl.GetDouble();
            }

            if (satisfactionEl.TryGetProperty("noteRecruteurs", out var noteRecruteursEl)
                && noteRecruteursEl.ValueKind == JsonValueKind.Number
                && recruteursPresents > 0)
            {
                noteRecruteurs = noteRecruteursEl.GetDouble();
            }

            enquetesSatisfaction = new { noteCandidats, noteRecruteurs };
        }

        var tdb = new
        {
            evenementId,
            visionGlobale = new
            {
                totalEntretiens = visionGlobale.GetProperty("totalEntretiens").GetInt32(),
                entretiensRestants = visionGlobale.GetProperty("entretiensRestants").GetInt32()
            },
            statutCandidats = new
            {
                sansEntretien = inscritsSansEntretien,
                presents = inscritsPresents,
                absents = inscritsAbsents
            },
            statutRecruteurs = new
            {
                presents = recruteursPresents,
                absents = recruteursAbsents
            },
            suitesRecrutements,
            enquetesSatisfaction
        };
        return Results.Ok(tdb);
    }

    return Results.NotFound();
})
.WithName("GetTableauDeBord");

// US-5.06 — RDV restants par recruteur pour un événement.
app.MapGet("/api/evenements/{id}/rdv-restants", (string id) =>
{
    var path = Path.Combine(AppContext.BaseDirectory, "Data", "rdv-restants.json");
    if (!File.Exists(path))
    {
        path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "rdv-restants.json");
    }

    using var stream = File.OpenRead(path);
    using var doc = JsonDocument.Parse(stream);

    foreach (var el in doc.RootElement.EnumerateArray())
    {
        var evenementId = el.GetProperty("evenementId").GetString() ?? "";
        if (!evenementId.Equals(id, StringComparison.OrdinalIgnoreCase)) continue;

        var recruteurs = el.GetProperty("recruteurs").EnumerateArray()
            .Select(r => new
            {
                recruteurId = r.GetProperty("recruteurId").GetString(),
                nom = r.GetProperty("nom").GetString(),
                prenom = r.GetProperty("prenom").GetString(),
                raisonSociale = r.GetProperty("raisonSociale").GetString(),
                creneaux = r.GetProperty("creneaux").EnumerateArray()
                    .Select(c => new
                    {
                        heure = c.GetProperty("heure").GetString(),
                        candidatNom = c.GetProperty("candidatNom").GetString(),
                        candidatPrenom = c.GetProperty("candidatPrenom").GetString()
                    })
                    .ToArray()
            })
            .ToArray();

        var rdvRestants = new { evenementId, recruteurs };
        return Results.Ok(rdvRestants);
    }

    return Results.NotFound();
})
.WithName("GetRdvRestants");

// US-5.08 — candidats sans RDV pour un événement (inscrits avec statut INCONNU).
app.MapGet("/api/evenements/{id}/candidats-sans-rdv", (string id, IInscritsRepository inscritsRepository) =>
{
    var inscrits = inscritsRepository.GetByEvenementId(id);

    var candidatsSansRdv = inscrits
        .Where(i => i.Statut == StatutInscrit.INCONNU)
        .Select(i => new
        {
            id = i.Id,
            nom = i.Nom,
            prenom = i.Prenom,
            telephone = i.Telephone
        })
        .ToArray();

    var result = new { evenementId = id, candidats = candidatsSansRdv };
    return Results.Ok(result);
})
.WithName("GetCandidatsSansRdv");

// US-5.01 — générer le planning d'un événement.
app.MapPost("/api/evenements/{id}/planning", (string id) =>
{
    var evenement = LoadEvenements()
        .FirstOrDefault(e => e.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

    return evenement is null ? Results.NotFound() : Results.Ok(new { evenementId = id, statut = "GENERE" });
})
.WithName("GenererPlanning");

app.Run();


record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Rend la classe Program visible aux projets de tests (WebApplicationFactory<Program>).
public partial class Program;
