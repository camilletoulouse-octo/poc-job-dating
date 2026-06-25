# ft-poc-parcours-candidat

POC du parcours candidat pour France Travail (Job Dating).

Le projet est composé de deux applications **.NET 10** et de leurs projets de tests :

- **`ParcoursCandidatApi`** — API REST exposant les données (événements, candidats, recruteurs, etc.)
- **`ParcoursCandidatClient`** — Application **Blazor WebAssembly** consommant l'API
- **`ParcoursCandidatApi.Tests`** — Tests d'intégration de l'API (`WebApplicationFactory` + xUnit)
- **`ParcoursCandidatClient.Tests`** — Tests unitaires et "E2E" composant (xUnit + bUnit)

## 📋 Prérequis

- [.NET SDK 10.0+](https://dotnet.microsoft.com/download) (la version précise est fixée dans `global.json`)
- `make` (préinstallé sur macOS et Linux)
- [Android Studio](https://developer.android.com/studio) + SDK Android (pour tester sur émulateur)

## 🚀 Démarrage rapide

Toutes les commandes utiles sont regroupées dans un **Makefile** à la racine du projet.

```bash
make help        # Affiche la liste des commandes disponibles
make run         # Lance l'API + le Client en parallèle
make watch       # Lance l'API + le Client en mode hot reload
make test        # Lance toute la suite de tests
```

Une fois démarrée :
- API : <http://localhost:5197>
- Client : <http://localhost:5209>

## 🛠 Commandes disponibles

| Commande | Description |
|---|---|
| `make run-api` | Lance uniquement l'API |
| `make run-client` | Lance uniquement le client Blazor |
| `make run` | Lance l'API et le client en parallèle |
| `make watch` | Lance l'API et le client en mode watch (hot reload) |
| `make build` | Build toute la solution |
| `make restore` | Restaure les dépendances NuGet |
| `make clean` | Nettoie les artefacts de build (`bin/`, `obj/`) |
| `make test` | Lance les tests de la solution |
| `make test-api` | Lance uniquement les tests d'intégration de l'API |
| `make test-client` | Lance uniquement les tests du client (unitaires + bUnit) |
| `make kill-ports` | Libère les ports utilisés par l'API (5197) et le Client (5209) |
| `make adb-reverse` | Configure le port forwarding ADB sur l'émulateur Android (ports 5209 et 5197) |
| `make run-android` | Configure le port forwarding ADB puis lance l'API et le client en parallèle |

> 💡 Tu peux toujours utiliser directement la CLI `dotnet` (ex : `dotnet run --project ParcoursCandidatApi`).

## 📱 Test sur émulateur Android

Pour accéder à l'application depuis le navigateur d'un émulateur Android (ex : Pixel 10 dans Android Studio) :

```bash
make run-android
```

Cette commande :
1. Configure le **port forwarding ADB** (`adb reverse`) pour que `localhost:5209` et `localhost:5197` dans l'émulateur soient redirigés vers la machine hôte.
2. Lance l'API et le client Blazor en parallèle.

Ouvre ensuite `http://localhost:5209` dans le navigateur de l'émulateur.

> ⚠️ Les règles `adb reverse` sont temporaires : elles disparaissent si l'émulateur est redémarré.
> Relance `make adb-reverse` (ou `make run-android`) après chaque redémarrage de l'émulateur.

## 📁 Structure du projet

```
.
├── ParcoursCandidatApi/         # API backend (ASP.NET Core Minimal API)
│   ├── Data/                    # Données mockées (JSON)
│   ├── Models/                  # Modèles métier
│   └── Properties/launchSettings.json
├── ParcoursCandidatApi.Tests/   # Tests d'intégration de l'API
│   ├── Fixtures/                # WebApplicationFactory + données de test
│   └── MesEvenements/           # Tests par feature
├── ParcoursCandidatClient/      # Client Blazor WebAssembly
│   ├── Pages/                   # Pages Razor
│   ├── Components/              # Composants Razor réutilisables (par feature)
│   ├── Layout/                  # Layout & navigation
│   ├── Services/                # Services (appels API)
│   └── Models/
├── ParcoursCandidatClient.Tests/# Tests unitaires + composant (bUnit)
│   ├── Fixtures/                # Handler HTTP mocké + fixtures JSON
│   └── MesEvenements/           # Tests par feature
├── assets/                      # Maquettes SVG
├── backlog/epics/               # Epics & user stories
├── docs/features/               # Documentation détaillée par feature
├── Makefile                     # Raccourcis de commandes
├── global.json                  # Version SDK .NET
└── ft-poc-parcours-candidat.slnx
```

## ✨ Features

- **Scanner (accès caméra + décodage QR)** — Un clic sur le bouton **Scanner** de la barre
  de navigation ouvre la caméra arrière du téléphone dans une page dédiée (`/scanner`).
  La librairie **jsQR** analyse en continu le flux vidéo via un `<canvas>` caché et notifie
  Blazor dès qu'un QR code est détecté. Le résultat est affiché avec un bouton "Scanner à
  nouveau". La caméra est libérée automatiquement lors de la navigation.
  Pour tester sur l'émulateur Android : `~/Library/Android/sdk/platform-tools/adb emu virtualscene-image wall assets/test/qrcode-test.png`
  📄 Doc détaillée : [`docs/features/scanner.md`](docs/features/scanner.md).
- **PWA (Progressive Web App)** — L'application est installable sur mobile et desktop.
  Un service worker met en cache les assets statiques pour un fonctionnement hors-ligne.
  Un bouton d'installation (icône téléchargement) apparaît dans l'en-tête de "Mes événements"
  lorsque le navigateur propose l'installation (`beforeinstallprompt`).
  📄 Doc détaillée : [`docs/features/pwa.md`](docs/features/pwa.md).
- **Mes événements** — Liste des événements de job dating du jour pour
  l'agence du conseiller (page `MesEvenements.razor`). En-tête blanc avec
  titre + nom du conseiller + bouton installation PWA + bouton paramètres ;
  bottom navigation à 3 entrées (Scanner, Mes événements du jour, Rechercher un inscrit).
  La page est composée de 4 sous-composants réutilisables extraits dans
  `Components/MesEvenements/` (TopBar, Tabs, Content, BottomNav).
  📄 Doc détaillée : [`docs/features/mes-evenements.md`](docs/features/mes-evenements.md).
- **Les inscrits (contexte événement)** — Lorsqu'on clique sur la flèche
  cerclée d'une carte (US-1.03), on ouvre le contexte d'un événement
  (page `EvenementInscrits.razor`, route `/evenements/{id}/inscrits`) :
  en-tête persistant (titre + organisme + lieu + horaires), onglets
  de navigation et liste des inscrits avec compteur de pointage, barre
  de progression et badge statut (Présent / Absent / Inconnu). Filtrage
  par statut (US-2.02), modification du statut via action sheet (US-2.04),
  et **recherche temps réel par nom/prénom** insensible à la casse et aux
  accents (US-2.03). Un bouton QR à droite de la barre de recherche ouvre
  directement le scanner.
  📄 Doc détaillée : [`docs/features/inscrits.md`](docs/features/inscrits.md) ·
  [`docs/features/recherche-inscrits.md`](docs/features/recherche-inscrits.md).
- **Les recruteurs (contexte événement)** — Deuxième onglet du contexte
  d'un événement (page `EvenementRecruteurs.razor`, route
  `/evenements/{id}/recruteurs`) : liste des recruteurs avec compteur de
  présence, barre de progression (verte si 100 % présents), et carte
  affichant raison sociale, contact, nombre d'offres, badge statut et
  flèche (US-3.01). Les onglets "Les inscrits" et "Les recruteurs" sont
  désormais des liens de navigation actifs.
  📄 Doc détaillée : [`docs/features/recruteurs.md`](docs/features/recruteurs.md).
- **Tableau de bord (contexte événement)** — Troisième onglet du contexte
  d'un événement (page `EvenementTableauDeBord.razor`, route
  `/evenements/{id}/tableau-de-bord`). La bottom navigation passe à
  3 entrées (US-1.04). Avant génération du planning, un état d'attente
  illustré (cactus) est affiché avec le message "Planning non généré".
  📄 Doc détaillée : [`docs/features/tableau-de-bord.md`](docs/features/tableau-de-bord.md).

> Les autres epics (auth, paramètres, recherche d'événements) sont décrits dans `backlog/epics/`.

## 🚂 Déploiement Railway

Le projet est déployé sur [Railway](https://railway.app/) en deux services distincts. Chaque service possède son propre fichier `railway.toml` dans son répertoire :

| Service | Fichier de config | Commande de build | Commande de démarrage |
|---|---|---|---|
| API | [`ParcoursCandidatApi/railway.toml`](ParcoursCandidatApi/railway.toml) | `dotnet publish ParcoursCandidatApi.csproj -c Release -o out` | `dotnet out/ParcoursCandidatApi.dll --urls http://0.0.0.0:$PORT` |
| Client Blazor | [`ParcoursCandidatClient/railway.toml`](ParcoursCandidatClient/railway.toml) | `dotnet publish ParcoursCandidatClient.csproj -c Release -o out && dotnet tool install --tool-path tools dotnet-serve` | `./tools/dotnet-serve --directory out/wwwroot --address 0.0.0.0 --port $PORT --fallback-file index.html` |

> ⚠️ Le format `[[services]]` (tableau TOML) n'est pas supporté par Railway et est ignoré.
> Chaque service Railway doit pointer sur le sous-répertoire correspondant (`rootDirectory`) et
> disposer de son propre `railway.toml`.
>
> La commande de build `dotnet publish -c Release -o out` est nécessaire à la place de la commande
> auto-générée par Railpack (`--no-restore`) qui échoue avec **NETSDK1064** car le cache NuGet
> de l'étape de restore n'est pas disponible dans la couche de build suivante.
>
> ⚠️ **Ne pas coder en dur** le chemin `/usr/share/dotnet/dotnet` dans la `startCommand` :
> ce chemin n'existe pas dans l'image Railpack (l'erreur est `No such file or directory`).
> Il faut utiliser simplement `dotnet`, qui est résolu via le `PATH`.
>
> ⚠️ Le client est une application **Blazor WebAssembly** : `dotnet publish` produit des
> fichiers statiques dans `out/wwwroot/` et **aucun `.dll` exécutable côté serveur**. On les
> sert avec l'outil .NET global [`dotnet-serve`](https://github.com/natemcmaster/dotnet-serve)
> installé pendant la phase de build. Le flag `--fallback-file index.html` est nécessaire
> pour que les routes Blazor côté client fonctionnent (SPA fallback).
>
> ⚠️ Railway expose dynamiquement la variable d'environnement `$PORT` ; il faut donc que les
> deux services écoutent sur `0.0.0.0:$PORT` et non sur leur port de dev local.


## 🧪 Tests

| Type | Projet | Outils |
|---|---|---|
| Intégration API | `ParcoursCandidatApi.Tests` | xUnit + `Microsoft.AspNetCore.Mvc.Testing` + FluentAssertions |
| Unitaire | `ParcoursCandidatClient.Tests` | xUnit + `HttpMessageHandler` mocké + FluentAssertions |
| Composant ("E2E") | `ParcoursCandidatClient.Tests` | xUnit + [bUnit](https://bunit.dev/) + FluentAssertions |

Conventions de test (cf. [`.clinerules/testing.md`](.clinerules/testing.md)) :
- Intitulés en français au format `Etant_donne_..._quand_..._alors_...`.
- Données extraites dans des fixtures, dans le même module que les tests.
- Un seul *Given / When / Then* par test.

```bash
make test
```
