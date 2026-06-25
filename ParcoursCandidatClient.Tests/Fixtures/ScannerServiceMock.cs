using Microsoft.JSInterop;
using ParcoursCandidatClient.Services;

namespace ParcoursCandidatClient.Tests.Fixtures;

/// <summary>
/// Implémentation de test de <see cref="IScannerService"/> permettant de
/// simuler la disponibilité de la caméra et de tracer les appels effectués.
/// </summary>
public class ScannerServiceMock : IScannerService
{
    private readonly bool _cameraDisponible;
    private readonly bool _ouvertureReussie;

    public string? OuvrirCameraAppeleAvec { get; private set; }
    public string? FermerCameraAppeleAvec { get; private set; }
    public bool ScanDemarre { get; private set; }

    public ScannerServiceMock(bool cameraDisponible = true, bool ouvertureReussie = true)
    {
        _cameraDisponible = cameraDisponible;
        _ouvertureReussie = ouvertureReussie;
    }

    public Task<bool> OuvrirCameraAsync(string videoElementId)
    {
        OuvrirCameraAppeleAvec = videoElementId;
        return Task.FromResult(_ouvertureReussie);
    }

    public Task FermerCameraAsync(string videoElementId)
    {
        FermerCameraAppeleAvec = videoElementId;
        return Task.CompletedTask;
    }

    public Task<bool> EstCameraDisponibleAsync()
    {
        return Task.FromResult(_cameraDisponible);
    }

    public Task DemarrerScanAsync<T>(string videoElementId, string canvasElementId, DotNetObjectReference<T> dotNetRef) where T : class
    {
        ScanDemarre = true;
        return Task.CompletedTask;
    }
}
