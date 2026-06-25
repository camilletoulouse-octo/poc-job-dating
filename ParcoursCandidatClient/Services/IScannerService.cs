namespace ParcoursCandidatClient.Services;

/// <summary>
/// Service d'accès à la caméra du téléphone pour le scan de QR codes / badges.
/// </summary>
public interface IScannerService
{
    /// <summary>
    /// Ouvre le flux vidéo de la caméra dans l'élément &lt;video&gt; identifié par <paramref name="videoElementId"/>.
    /// </summary>
    /// <param name="videoElementId">L'id de l'élément HTML &lt;video&gt; cible.</param>
    /// <returns><c>true</c> si la caméra a démarré avec succès, <c>false</c> sinon.</returns>
    Task<bool> OuvrirCameraAsync(string videoElementId);

    /// <summary>
    /// Arrête le flux vidéo de la caméra sur l'élément identifié par <paramref name="videoElementId"/>.
    /// </summary>
    /// <param name="videoElementId">L'id de l'élément HTML &lt;video&gt; cible.</param>
    Task FermerCameraAsync(string videoElementId);

    /// <summary>
    /// Vérifie si le navigateur supporte l'accès à la caméra.
    /// </summary>
    /// <returns><c>true</c> si la caméra est disponible, <c>false</c> sinon.</returns>
    Task<bool> EstCameraDisponibleAsync();

    /// <summary>
    /// Démarre la boucle de décodage QR code sur le flux vidéo.
    /// Notifie <paramref name="dotNetRef"/> via <c>OnQrCodeDetecte(string data)</c> dès qu'un QR code est détecté.
    /// </summary>
    /// <param name="videoElementId">L'id de l'élément HTML &lt;video&gt; source.</param>
    /// <param name="canvasElementId">L'id de l'élément HTML &lt;canvas&gt; utilisé pour capturer les frames.</param>
    /// <param name="dotNetRef">Référence .NET recevant le callback de détection (doit exposer [JSInvokable] OnQrCodeDetecte).</param>
    Task DemarrerScanAsync<T>(string videoElementId, string canvasElementId, Microsoft.JSInterop.DotNetObjectReference<T> dotNetRef) where T : class;
}
