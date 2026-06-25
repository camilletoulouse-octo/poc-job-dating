using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ParcoursCandidatClient;
using ParcoursCandidatClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Adresse de l'API ParcoursCandidatApi (profil http par défaut).
// Configurable via la propriété "ApiBaseAddress" dans wwwroot/appsettings.json.
var apiBaseAddress = builder.Configuration["ApiBaseAddress"]
    ?? "http://localhost:5197/";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });
builder.Services.AddScoped<EvenementService>();
builder.Services.AddScoped<IInscritService, InscritService>();
builder.Services.AddScoped<IRecruteurService, RecruteurService>();
builder.Services.AddScoped<ITableauDeBordService, TableauDeBordService>();
builder.Services.AddScoped<IRdvRestantsService, RdvRestantsService>();
builder.Services.AddScoped<ICandidatsSansRdvService, CandidatsSansRdvService>();
builder.Services.AddScoped<IScannerService, ScannerService>();

await builder.Build().RunAsync();
