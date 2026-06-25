using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ParcoursCandidatClient;
using ParcoursCandidatClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Adresse de l'API ParcoursCandidatApi.
// - En production, l'API et le client Blazor sont co-hébergés sur le même service
//   (cf. `ParcoursCandidatApi/Program.cs` qui sert les fichiers du WASM) : on utilise
//   alors l'origine courante (`builder.HostEnvironment.BaseAddress`).
// - En développement local, on continue de pointer sur l'API démarrée séparément
//   (http://localhost:5197/), configurable via "ApiBaseAddress" dans wwwroot/appsettings.json.
var apiBaseAddress = builder.Configuration["ApiBaseAddress"]
    ?? builder.HostEnvironment.BaseAddress;

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });
builder.Services.AddScoped<EvenementService>();
builder.Services.AddScoped<IInscritService, InscritService>();
builder.Services.AddScoped<IRecruteurService, RecruteurService>();
builder.Services.AddScoped<ITableauDeBordService, TableauDeBordService>();
builder.Services.AddScoped<IRdvRestantsService, RdvRestantsService>();
builder.Services.AddScoped<ICandidatsSansRdvService, CandidatsSansRdvService>();
builder.Services.AddScoped<IScannerService, ScannerService>();

await builder.Build().RunAsync();
