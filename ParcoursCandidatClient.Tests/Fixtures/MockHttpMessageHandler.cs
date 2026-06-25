namespace ParcoursCandidatClient.Tests.Fixtures;

/// <summary>
/// HttpMessageHandler de test qui retourne une réponse pré-configurée
/// et capture l'URI de la dernière requête HTTP émise.
/// </summary>
internal sealed class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

    public MockHttpMessageHandler(HttpResponseMessage response)
        : this(_ => response)
    {
    }

    public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        _responder = responder;
    }

    public Uri? DerniereRequeteUri { get; private set; }
    public int NombreAppels { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        NombreAppels++;
        DerniereRequeteUri = request.RequestUri;
        return Task.FromResult(_responder(request));
    }
}
