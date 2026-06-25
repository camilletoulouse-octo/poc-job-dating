# Feature — Scanner (accès caméra + pointage automatique)

Lorsque l'utilisateur clique sur le bouton **Scanner** de la barre de navigation
d'un événement, l'application ouvre la caméra arrière du téléphone dans une page
dédiée. Après détection d'un QR code encodant l'ID d'un inscrit, son statut est
automatiquement mis à jour à **PRÉSENT** via l'API et son nom s'affiche à l'écran.

---

## 1. Vue d'ensemble

| | |
|---|---|
| Page Blazor | [`Pages/Scanner.razor`](../../ParcoursCandidatClient/Pages/Scanner.razor) |
| CSS | [`Pages/Scanner.razor.css`](../../ParcoursCandidatClient/Pages/Scanner.razor.css) |
| Interface service scanner | [`Services/IScannerService.cs`](../../ParcoursCandidatClient/Services/IScannerService.cs) |
| Implémentation service scanner | [`Services/ScannerService.cs`](../../ParcoursCandidatClient/Services/ScannerService.cs) |
| Script JS | [`wwwroot/js/scanner.js`](../../ParcoursCandidatClient/wwwroot/js/scanner.js) |
| Librairie QR | [`wwwroot/lib/jsqr.js`](../../ParcoursCandidatClient/wwwroot/lib/jsqr.js) (v1.4.0) |
| Bouton déclencheur | [`Components/Evenement/EvenementBottomNav.razor`](../../ParcoursCandidatClient/Components/Evenement/EvenementBottomNav.razor) |
| Enum onglets | [`Components/Evenement/OngletEvenement.cs`](../../ParcoursCandidatClient/Components/Evenement/OngletEvenement.cs) |
| Mock scanner | [`Tests/Fixtures/ScannerServiceMock.cs`](../../ParcoursCandidatClient.Tests/Fixtures/ScannerServiceMock.cs) |
| Mock inscrits | [`Tests/Fixtures/InscritServiceMock.cs`](../../ParcoursCandidatClient.Tests/Fixtures/InscritServiceMock.cs) |
| Fixtures | [`Tests/Scanner/ScannerFixtures.cs`](../../ParcoursCandidatClient.Tests/Scanner/ScannerFixtures.cs) |
| Tests | [`Tests/Scanner/ScannerPageTests.cs`](../../ParcoursCandidatClient.Tests/Scanner/ScannerPageTests.cs) |

## 2. Architecture

```
EvenementBottomNav.razor
  └─ clic sur "Scanner" → /evenements/{id}/scanner

Scanner.razor  (route : /evenements/{EvenementId}/scanner)
  ├─ OnAfterRenderAsync
  │   ├─ IScannerService.EstCameraDisponibleAsync()       → vérifie le support navigateur
  │   ├─ IScannerService.OuvrirCameraAsync("sc-video")    → démarre le flux vidéo
  │   └─ IScannerService.DemarrerScanAsync(...)           → lance la boucle de décodage jsQR
  ├─ Affiche .sc-viewfinder  (vidéo + canvas caché + cadre de visée + instruction)
  │   ou .sc-overlay         (QR code détecté : image QR + badge ✓ + nom candidat + "Scan suivant")
  │   ou .sc-overlay--erreur (inscrit introuvable : badge ✗ + message + "Scan suivant")
  │   ou .sc-erreur          (si caméra indisponible / accès refusé)
  ├─ [JSInvokable] OnQrCodeDetecte(inscritId)
  │   └─ IInscritService.UpdateStatutAsync(inscritId, PRESENT)
  │       ├─ succès → affiche nom+prénom de l'inscrit, badge vert ✓
  │       └─ null   → affiche message d'erreur, badge rouge ✗
  └─ Bouton "Retour" → FermerCameraAsync + NavigateTo("/evenements/{id}/inscrits")

ScannerService.cs
  └─ Délègue à window.scanner (scanner.js) via IJSRuntime

jsqr.js  (lib tierce, v1.4.0)
  └─ jsQR(imageData, width, height) → décode un QR code depuis des pixels

scanner.js  (window.scanner)
  ├─ ouvrirCamera(videoElementId)              → getUserMedia({ facingMode: "environment" })
  ├─ fermerCamera(videoElementId)              → arrête tous les tracks + annule requestAnimationFrame
  ├─ estCameraDisponible()                     → vérifie navigator.mediaDevices.getUserMedia
  └─ demarrerScan(videoId, canvasId, dotNetRef)→ boucle requestAnimationFrame + jsQR
                                                  → dotNetRef.invokeMethodAsync("OnQrCodeDetecte", inscritId)
```

## 3. Format du QR code

Le QR code doit encoder l'**identifiant de l'inscrit** (ex : `ins-001`).
Lors du scan, l'app appelle `PATCH /api/inscrits/{inscritId}` avec `{ "statut": "PRESENT" }`.

Le QR code de test encode `ins-001` (Martin Lucie, evt-001) :
```bash
# Générer un QR code de test
curl "https://api.qrserver.com/v1/create-qr-code/?size=300x300&data=ins-001" -o assets/test/qrcode-test.png

# Injecter dans la scène virtuelle de l'émulateur Android
~/Library/Android/sdk/platform-tools/adb emu virtualscene-image table assets/test/qrcode-test.png
```

## 4. Fichiers

### `wwwroot/js/scanner.js`

Expose l'objet `window.scanner` avec 4 méthodes appelées depuis Blazor via `IJSRuntime` :

| Méthode | Rôle |
|---|---|
| `ouvrirCamera(videoElementId)` | Démarre le flux vidéo de la caméra arrière dans l'élément `<video>` cible. Retourne `true` si succès. |
| `fermerCamera(videoElementId)` | Arrête tous les tracks du flux vidéo et libère la caméra. |
| `estCameraDisponible()` | Retourne `true` si `navigator.mediaDevices.getUserMedia` est disponible. |
| `demarrerScan(videoId, canvasId, dotNetRef)` | Boucle `requestAnimationFrame` + jsQR. Appelle `OnQrCodeDetecte` dès détection. |

### `IScannerService` / `ScannerService`

Le service encapsule les appels JS interop. Il est enregistré en `Scoped` dans `Program.cs` :

```csharp
builder.Services.AddScoped<IScannerService, ScannerService>();
```

### `Pages/Scanner.razor`

- Route : `/evenements/{EvenementId}/scanner`
- Injecte `IScannerService`, `IInscritService` et `NavigationManager`
- Implémente `IAsyncDisposable` pour fermer la caméra lors de la navigation
- Trois états d'affichage :
  - **`.sc-viewfinder`** : flux vidéo plein écran + cadre de visée + instruction
  - **`.sc-overlay`** : résultat après scan (image QR + badge vert + nom candidat + bouton "Scan suivant")
  - **`.sc-erreur`** : message d'erreur si la caméra est indisponible ou l'accès refusé

### `EvenementBottomNav.razor`

Le bouton **Scanner** navigue vers `/evenements/{id}/scanner` :

```razor
<a href="/evenements/@EvenementId/scanner" class="evt-bottom-item">
    ...
    <span>Scanner</span>
</a>
```

## 5. Gestion des erreurs

| Cas | Comportement |
|---|---|
| Navigateur sans support `getUserMedia` | `estCameraDisponible()` retourne `false` → affichage `.sc-erreur` |
| Utilisateur refuse l'accès à la caméra | `ouvrirCamera()` retourne `false` → affichage `.sc-erreur` |
| ID inscrit introuvable (404) | `UpdateStatutAsync` retourne `null` → badge rouge ✗ + message d'erreur |
| Navigation hors de la page | `IAsyncDisposable.DisposeAsync()` ferme automatiquement la caméra |

## 5. Compatibilité

| Navigateur | Accès caméra |
|---|---|
| Chrome / Edge (Android & Desktop) | ✅ `getUserMedia` avec `facingMode: "environment"` |
| Safari iOS (≥ 14.3) | ✅ Requiert HTTPS ou localhost |
| Firefox | ✅ |

> **Note** : L'accès à la caméra requiert une connexion sécurisée (HTTPS) en production.
> En développement local (`localhost`), HTTP est autorisé par les navigateurs.
