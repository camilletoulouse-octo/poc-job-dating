# Feature — PWA (Progressive Web App)

L'application Parcours Candidat est une PWA installable sur mobile et desktop.
Elle peut fonctionner hors-ligne grâce à un service worker qui met en cache
les assets statiques.

---

## 1. Vue d'ensemble

| | |
|---|---|
| Manifest | [`wwwroot/manifest.json`](../../ParcoursCandidatClient/wwwroot/manifest.json) |
| Service worker | [`wwwroot/sw.js`](../../ParcoursCandidatClient/wwwroot/sw.js) |
| Script JS | [`wwwroot/js/pwa.js`](../../ParcoursCandidatClient/wwwroot/js/pwa.js) |
| Composant bouton | [`Components/Pwa/PwaInstallButton.razor`](../../ParcoursCandidatClient/Components/Pwa/PwaInstallButton.razor) |
| Point d'entrée HTML | [`wwwroot/index.html`](../../ParcoursCandidatClient/wwwroot/index.html) |

## 2. Architecture

```
index.html
  ├─ <link rel="manifest" href="manifest.json" />   ← décrit l'app installable
  ├─ <meta name="theme-color" content="#4833E3" />
  ├─ <link rel="apple-touch-icon" href="icon-192.png" />
  ├─ js/pwa.js                                       ← logique d'installation
  └─ pwa.registerServiceWorker()                     ← enregistre sw.js

sw.js (Service Worker)
  ├─ install  → met en cache les assets statiques (Cache API)
  ├─ activate → supprime les anciens caches
  └─ fetch    → stratégie Cache-first pour les GET (hors /api/)

PwaInstallButton.razor (Composant Blazor)
  ├─ écoute beforeinstallprompt via pwa.initInstallPrompt()
  ├─ s'affiche uniquement si l'installation est disponible
  └─ déclenche pwa.showInstallPrompt() au clic
```

## 3. Fichiers

### `manifest.json`

Décrit l'application pour les navigateurs compatibles PWA :

| Champ | Valeur |
|---|---|
| `name` | `"Parcours Candidat"` |
| `short_name` | `"Parcours Candidat"` |
| `start_url` | `"/"` |
| `display` | `"standalone"` |
| `theme_color` | `"#4833E3"` |
| `background_color` | `"#FFFFFF"` |
| `orientation` | `"portrait"` |
| `lang` | `"fr"` |
| `icons` | `icon-192.png` (192×192) et `icon-512.png` (512×512) |

### `sw.js` — Service Worker

- **Install** : met en cache les assets listés dans `ASSETS_TO_CACHE` (HTML, CSS, icônes, manifest).
- **Activate** : supprime les caches dont le nom diffère de `CACHE_NAME` (nettoyage des anciennes versions).
- **Fetch** : stratégie *cache-first* — répond depuis le cache si disponible, sinon effectue la requête réseau et met le résultat en cache. Les appels `GET /api/...` sont exclus du cache.

### `js/pwa.js`

Expose l'objet `window.pwa` avec 4 méthodes appelées depuis Blazor via `IJSRuntime` :

| Méthode | Rôle |
|---|---|
| `registerServiceWorker()` | Enregistre `sw.js` au démarrage de l'app |
| `initInstallPrompt(dotNetRef)` | Écoute `beforeinstallprompt` et `appinstalled`, notifie le composant Blazor |
| `isInstallPromptAvailable()` | Retourne `true` si le prompt d'installation est en attente |
| `showInstallPrompt()` | Affiche le prompt natif du navigateur, retourne `true` si l'utilisateur accepte |

### `PwaInstallButton.razor`

Composant Blazor autonome situé dans `Components/Pwa/` :

- S'affiche uniquement lorsque `beforeinstallprompt` a été intercepté.
- Se masque automatiquement après installation (`appinstalled`).
- Utilise `DotNetObjectReference` pour recevoir les callbacks JS.
- Implémente `IAsyncDisposable` pour libérer la référence .NET.

## 4. Intégration dans l'en-tête

Le composant `PwaInstallButton` est intégré dans
[`MesEvenementsTopBar.razor`](../../ParcoursCandidatClient/Components/MesEvenements/MesEvenementsTopBar.razor)
dans un conteneur `.me-topbar-actions` aux côtés du bouton paramètres :

```
┌──────────────────────────────────────────────────┐
│  En-tête blanc                                   │
│   ├─ Titre "Mes événements"                      │
│   ├─ Nom du conseiller                           │
│   └─ Actions :                                   │
│       ├─ ⬇️  Bouton installation PWA (si dispo)  │
│       └─ ⚙️  Bouton paramètres (désactivé)       │
└──────────────────────────────────────────────────┘
```

Le bouton d'installation n'est visible que lorsque le navigateur supporte
l'installation PWA et que l'application n'est pas encore installée.

## 5. Compatibilité

| Navigateur | Installation | Service Worker |
|---|---|---|
| Chrome / Edge (Android & Desktop) | ✅ `beforeinstallprompt` | ✅ |
| Safari iOS (≥ 16.4) | ⚠️ Via "Ajouter à l'écran d'accueil" (pas de prompt natif) | ✅ |
| Firefox | ❌ Pas de prompt natif | ✅ |

> Sur iOS/Safari, le bouton d'installation ne s'affiche pas (pas de `beforeinstallprompt`).
> L'utilisateur peut tout de même installer l'app via le menu "Partager → Ajouter à l'écran d'accueil".
