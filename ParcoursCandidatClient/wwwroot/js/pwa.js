let _deferredPrompt = null;
let _dotNetRef = null;

// Capture beforeinstallprompt le plus tôt possible,
// avant même que Blazor soit initialisé.
window.addEventListener("beforeinstallprompt", (event) => {
    event.preventDefault();
    _deferredPrompt = event;
    if (_dotNetRef) {
        _dotNetRef.invokeMethodAsync("OnInstallPromptAvailable");
    }
});

window.addEventListener("appinstalled", () => {
    _deferredPrompt = null;
    if (_dotNetRef) {
        _dotNetRef.invokeMethodAsync("OnAppInstalled");
    }
});

window.pwa = {
    registerServiceWorker: function () {
        if ("serviceWorker" in navigator) {
            navigator.serviceWorker.register("/sw.js").catch((error) => {
                console.error("Erreur lors de l'enregistrement du service worker :", error);
            });
        }
    },

    initInstallPrompt: function (dotNetRef) {
        _dotNetRef = dotNetRef;
        // Si beforeinstallprompt a déjà été capturé avant l'init Blazor,
        // on notifie immédiatement le composant.
        if (_deferredPrompt) {
            dotNetRef.invokeMethodAsync("OnInstallPromptAvailable");
        }
    },

    isInstallPromptAvailable: function () {
        return _deferredPrompt !== null;
    },

    showInstallPrompt: async function () {
        if (!_deferredPrompt) return false;

        _deferredPrompt.prompt();
        const { outcome } = await _deferredPrompt.userChoice;
        _deferredPrompt = null;

        return outcome === "accepted";
    }
};
