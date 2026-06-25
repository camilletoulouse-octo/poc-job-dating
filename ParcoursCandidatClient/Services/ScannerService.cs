using Microsoft.JSInterop;

namespace ParcoursCandidatClient.Services;

/// <summary>
/// Implémentation de <see cref="IScannerService"/> utilisant l'interop JS
/// via <c>wwwroot/js/scanner.js</c> pour accéder à la caméra du téléphone
/// et décoder les QR codes via la librairie jsQR.
/// </summary>
public class ScannerService : IScannerService
{
    private readonly IJSRuntime _js;

    public ScannerService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<bool> OuvrirCameraAsync(string videoElementId)
    {
        return await _js.InvokeAsync<bool>("scanner.ouvrirCamera", videoElementId);
    }

    public async Task FermerCameraAsync(string videoElementId)
    {
        await _js.InvokeVoidAsync("scanner.fermerCamera", videoElementId);
    }

    public async Task<bool> EstCameraDisponibleAsync()
    {
        return await _js.InvokeAsync<bool>("scanner.estCameraDisponible");
    }

    public async Task DemarrerScanAsync<T>(string videoElementId, string canvasElementId, Microsoft.JSInterop.DotNetObjectReference<T> dotNetRef) where T : class
    {
        await _js.InvokeVoidAsync("scanner.demarrerScan", videoElementId, canvasElementId, dotNetRef);
    }
}
