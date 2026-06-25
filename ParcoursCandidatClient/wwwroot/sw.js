const CACHE_NAME = "parcours-candidat-v2";

const ASSETS_TO_CACHE = [
    "/",
    "/index.html",
    "/manifest.json",
    "/css/app.css",
    "/favicon.png",
    "/icon-192.png",
    "/icon-512.png"
];

self.addEventListener("install", (event) => {
    event.waitUntil(
        caches.open(CACHE_NAME).then((cache) => cache.addAll(ASSETS_TO_CACHE))
    );
    self.skipWaiting();
});

self.addEventListener("activate", (event) => {
    event.waitUntil(
        caches.keys().then((cacheNames) =>
            Promise.all(
                cacheNames
                    .filter((name) => name !== CACHE_NAME)
                    .map((name) => caches.delete(name))
            )
        )
    );
    self.clients.claim();
});

self.addEventListener("fetch", (event) => {
    if (event.request.method !== "GET") return;

    const url = new URL(event.request.url);

    // Ne mettre en cache que les requêtes HTTP(S) — ignorer chrome-extension://, etc.
    if (url.protocol !== "http:" && url.protocol !== "https:") return;

    // Ne pas mettre en cache les appels API
    if (url.pathname.startsWith("/api/")) return;

    // Stratégie network-first pour les assets Blazor WASM :
    // ces fichiers changent à chaque build et doivent toujours être frais.
    const isBlazorAsset =
        url.pathname.startsWith("/_framework/") ||
        url.pathname.endsWith(".wasm") ||
        url.pathname.endsWith(".dll") ||
        url.pathname.endsWith(".pdb");

    if (isBlazorAsset) {
        event.respondWith(
            fetch(event.request).catch(() => caches.match(event.request))
        );
        return;
    }

    // Stratégie cache-first pour les autres assets statiques (CSS, images, JS)
    event.respondWith(
        caches.match(event.request).then((cached) => {
            if (cached) return cached;

            return fetch(event.request).then((response) => {
                if (!response || response.status !== 200 || response.type === "opaque") {
                    return response;
                }

                const responseToCache = response.clone();
                caches.open(CACHE_NAME).then((cache) => {
                    cache.put(event.request, responseToCache);
                });

                return response;
            });
        })
    );
});
