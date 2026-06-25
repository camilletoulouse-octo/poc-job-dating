using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ParcoursCandidatApi.Tests.Fixtures;

/// <summary>
/// Fabrique de test pour l'API ParcoursCandidatApi.
/// Force l'environnement "Testing" et le ContentRoot vers le dossier source de l'API
/// afin que <c>Data/evenements.json</c> soit accessible.
/// </summary>
public class EvenementsApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        var apiProjectPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "ParcoursCandidatApi"));

        if (Directory.Exists(apiProjectPath))
        {
            builder.UseContentRoot(apiProjectPath);
        }
    }
}
