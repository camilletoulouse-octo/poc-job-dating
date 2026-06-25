# Feature — Mes événements

> Epic 1 (cf. [`backlog/epics/epic-mes-evenements.md`](../../backlog/epics/epic-mes-evenements.md))
> Maquette : [`assets/EcransSVG/6-Liste-evenements.svg`](../../assets/EcransSVG/6-Liste-evenements.svg)

Écran d'entrée du conseiller terrain : il affiche la liste des événements
de job dating de la journée pour son agence de référence et lui permet
ensuite d'entrer dans le contexte opérationnel d'un événement.

---

## 1. Vue d'ensemble

| | |
|---|---|
| URL | `/` et `/mes-evenements` |
| Page | [`Pages/MesEvenements.razor`](../../ParcoursCandidatClient/Pages/MesEvenements.razor) |
| Composants | [`Components/MesEvenements/`](../../ParcoursCandidatClient/Components/MesEvenements) (4 sous-composants extraits, voir §3) |
| Service | [`Services/EvenementService.cs`](../../ParcoursCandidatClient/Services/EvenementService.cs) |
| Endpoint API | `GET /api/evenements?agenceId={agenceId}&date={yyyy-MM-dd}` |
| Données mockées | [`ParcoursCandidatApi/Data/evenements.json`](../../ParcoursCandidatApi/Data/evenements.json) |

## 2. User stories couvertes

| US | Statut | Notes |
|---|---|---|
| **US-1.01** Consulter la liste de mes événements du jour | ✅ Livrée | Onglet "Mes événements du jour" par défaut, tri chronologique, skeleton loaders, badge "X inscrits". |
| **US-1.02** Modifier l'agence de référence | ⛔ À faire | Agence en dur `lyon-part-dieu` (plus de carte agence dans l'en-tête). |
| **US-1.03** Accéder au contexte d'un événement | ✅ Livrée | La flèche cerclée d'une carte navigue vers `/evenements/{id}/inscrits` (cf. [`docs/features/inscrits.md`](./inscrits.md)). |
| **US-1.04** Naviguer entre les onglets d'un événement | ✅ Livrée | Bottom nav à 3 onglets (Les inscrits / Les recruteurs / Tableau de bord). Onglet "Tableau de bord" : état d'attente cactus avant génération du planning (cf. [`docs/features/tableau-de-bord.md`](./tableau-de-bord.md)). |
| **US-1.05** Rechercher un événement par mots-clés | 🚧 Onglet visible | Onglet "Rechercher" désactivé, placeholder affiché. |
| **US-1.06** Revenir à la liste d'événements | ⛔ À faire | Bouton retour retiré de l'en-tête. |
| **US-1.07** Basculer d'un événement à un autre | ⛔ À faire | Dépend d'US-1.03 / 1.06. |
| **US-1.08** Afficher le nom du conseiller | 🚧 Partiel | Nom du conseiller affiché dans l'en-tête (`Camille Toulouse` en dur). |
| **US-1.09** État vide aucun événement | ✅ Livrée | Bloc "Aucun événement prévu aujourd'hui". |
| **US-1.10** Accéder aux paramètres | 🚧 UI préparée | Bouton roue dentée dans l'en-tête désactivé. |

## 3. Architecture

```
┌───────────────────────────┐         GET /api/evenements
│ Pages/MesEvenements.razor │ ──────────────────────────────►  ┌──────────────────────┐
│   - charge la liste       │                                  │ ParcoursCandidatApi  │
│   - skeleton / vide / err │ ◄──────────────────────────────  │  Program.cs          │
│   - onglets pilule        │           JSON                   │  Data/evenements.json│
└─────────────┬─────────────┘                                  └──────────────────────┘
              │ injecte
              ▼
   ┌──────────────────────┐
   │ EvenementService.cs  │  HttpClient enregistré dans Program.cs
   └──────────────────────┘
```

### Modèle partagé `Evenement`

| Champ | Type | Exemple |
|---|---|---|
| `Id` | `string` | `"evt-001"` |
| `Titre` | `string` | `"Job dating restauration"` |
| `Organisme` | `string` | `"France Travail"` |
| `Ville` | `string` | `"Lyon"` |
| `Departement` | `string` | `"69"` |
| `HeureDebut` | `string` (HH:mm) | `"09:00"` |
| `HeureFin` | `string` (HH:mm) | `"12:00"` |
| `NombreInscrits` | `int` | `24` |
| `AgenceId` | `string` | `"lyon-part-dieu"` |
| `Date` | `DateOnly` | `2025-06-23` |

### Endpoint API `GET /api/evenements`

| Param | Type | Optionnel | Comportement |
|---|---|---|---|
| `agenceId` | `string` | ✅ | Si absent, toutes agences. Comparaison insensible à la casse. |
| `date` | `DateOnly` (`yyyy-MM-dd`) | ✅ | Si absent, `DateTime.Today`. |

- Tri : par `HeureDebut` croissant (US-1.01).
- Le fichier JSON supporte les dates relatives `TODAY` / `TOMORROW` / `YESTERDAY`
  pour rester pertinent à toute date d'exécution.

### Composant Blazor

État interne de `MesEvenements.razor` (la page n'orchestre que l'état et
délègue l'affichage à 4 sous-composants — voir plus bas) :

| Variable | Rôle |
|---|---|
| `_ongletActif` | `OngletMesEvenements.MesEvenementsDuJour` ou `OngletMesEvenements.Rechercher`. |
| `_chargement` | Affiche les skeletons tant que la requête HTTP n'a pas répondu. |
| `_erreur` | Message d'erreur affiché avec un bouton "Réessayer". |
| `_evenements` | Liste retournée par `EvenementService`. |
| `_conseillerNom` | Nom du conseiller connecté affiché dans l'en-tête (en dur tant qu'US-1.08 n'est pas livrée). |

### Sous-composants extraits

La page se contente d'agencer 4 sous-composants situés dans
[`Components/MesEvenements/`](../../ParcoursCandidatClient/Components/MesEvenements),
chacun avec son propre fichier `.razor.css` scopé. Ce découpage permet de
les réutiliser dans d'autres écrans (cf. epics Scanner / Tableau de bord /
Recherche d'inscrits) sans dupliquer le markup ni les styles.

| Composant | Rôle | Paramètres |
|---|---|---|
| [`MesEvenementsTopBar.razor`](../../ParcoursCandidatClient/Components/MesEvenements/MesEvenementsTopBar.razor) | En-tête mobile blanc (titre + nom conseiller + bouton installation PWA + bouton paramètres) | `Titre`, `ConseillerNom` |
| [`MesEvenementsTabs.razor`](../../ParcoursCandidatClient/Components/MesEvenements/MesEvenementsTabs.razor) | Onglets pilule "Mes événements du jour" / "Rechercher un événement" | `OngletActif` (bind), `OngletActifChanged` |
| [`MesEvenementsContent.razor`](../../ParcoursCandidatClient/Components/MesEvenements/MesEvenementsContent.razor) | Contenu selon l'onglet : skeletons / erreur / vide / liste de cartes / placeholder | `OngletActif`, `Chargement`, `Erreur`, `Evenements`, `Reessayer` |
| [`MesEvenementsBottomNav.razor`](../../ParcoursCandidatClient/Components/MesEvenements/MesEvenementsBottomNav.razor) | Bottom navigation à 3 entrées (Scanner / Mes événements / Rechercher un inscrit) | — |

L'enum [`OngletMesEvenements`](../../ParcoursCandidatClient/Components/MesEvenements/OngletMesEvenements.cs)
est partagé entre la page et les sous-composants.

### Structure de la page

```
┌──────────────────────────────────────────┐
│  En-tête blanc                           │
│   ├─ Titre "Mes événements"              │
│   ├─ Nom du conseiller                   │
│   └─ ⚙️  Bouton paramètres (désactivé)   │
├──────────────────────────────────────────┤
│  Onglets pilule                          │
│   ├─ Mes événements du jour (actif)      │
│   └─ Rechercher un événement (désactivé) │
├──────────────────────────────────────────┤
│  Liste des cartes d'événement            │
│   (ou état vide / erreur / skeletons)    │
├──────────────────────────────────────────┤
│  Bottom navigation (3 entrées)           │
│   ├─ Scanner (désactivé)                 │
│   ├─ Mes événements du jour (actif)      │
│   └─ Rechercher un inscrit (désactivé)   │
└──────────────────────────────────────────┘
```

## 4. Tests automatisés

Trois projets de tests couvrent la feature :

| Projet | Type | Cible | Outils |
|---|---|---|---|
| [`ParcoursCandidatApi.Tests`](../../ParcoursCandidatApi.Tests) | Tests d'intégration | `GET /api/evenements` | `xUnit` + `WebApplicationFactory` + `FluentAssertions` |
| [`ParcoursCandidatClient.Tests/MesEvenements/EvenementServiceTests.cs`](../../ParcoursCandidatClient.Tests/MesEvenements/EvenementServiceTests.cs) | Tests unitaires | `EvenementService` | `xUnit` + `HttpMessageHandler` mocké + `FluentAssertions` |
| [`ParcoursCandidatClient.Tests/MesEvenements/MesEvenementsPageTests.cs`](../../ParcoursCandidatClient.Tests/MesEvenements/MesEvenementsPageTests.cs) | Tests "E2E" composant (parcours utilisateur) | `Pages/MesEvenements.razor` | `bUnit` + `xUnit` + `FluentAssertions` |

Conventions (`.clinerules/testing.md`) :
- Tous les intitulés en français au format `Etant_donne_..._quand_..._alors_...`.
- Données isolées dans des fixtures :
  - [`EvenementsFixtures.cs`](../../ParcoursCandidatApi.Tests/Fixtures/EvenementsFixtures.cs) — IDs et ordre attendu côté API.
  - [`EvenementsHttpFixtures.cs`](../../ParcoursCandidatClient.Tests/Fixtures/EvenementsHttpFixtures.cs) — JSON et `Evenement` côté client.
  - [`MockHttpMessageHandler.cs`](../../ParcoursCandidatClient.Tests/Fixtures/MockHttpMessageHandler.cs) — handler HTTP mocké réutilisable.

### Lancement

```bash
make test
# ou ciblé :
dotnet test ParcoursCandidatApi.Tests/ParcoursCandidatApi.Tests.csproj
dotnet test ParcoursCandidatClient.Tests/ParcoursCandidatClient.Tests.csproj
```

### Cas couverts

**API — `GetEvenementsEndpointTests`**
1. Réponse 200 pour `agenceId` + `date` valides.
2. Filtre `agenceId` strict.
3. Tri par `heureDebut` croissant.
4. Agence inconnue → liste vide.
5. Aucun événement pour une date donnée → liste vide.
6. Filtre `date` correct (TOMORROW / Paris).
7. Tous les champs sont renseignés et bien typés (HH:mm).
8. Sans `agenceId` → toutes les agences renvoyées pour la date.

**Client — `EvenementServiceTests`**
1. Désérialisation correcte de la réponse JSON.
2. Liste vide gérée.
3. Date par défaut = aujourd'hui.
4. Date personnalisée bien transmise.
5. `agenceId` correctement URL-encodé.
6. Route appelée = `/api/evenements`.
7. Une réponse 5xx lève `HttpRequestException`.

**Composant — `MesEvenementsPageTests` (bUnit)**
1. Affichage des titres après chargement.
2. État vide affiché (US-1.09).
3. Bloc d'erreur + bouton "Réessayer".
4. Titre "Mes événements" affiché dans l'en-tête.
5. Nom du conseiller affiché dans l'en-tête.
6. Bouton "Paramètres" (roue dentée) présent dans l'en-tête.
7. Onglet "Mes événements du jour" actif par défaut.
8. Badges "X inscrits" affichés.
9. Bottom navigation contient exactement 3 entrées (Scanner, Mes événements du jour, Rechercher un inscrit).
10. Entrée "Mes événements du jour" active par défaut dans la bottom navigation.

## 5. Évolutions à prévoir

- US-1.02 : permettre la sélection persistante d'une agence (localStorage)
  → ajouter un `IAgenceRepository` (l'agence est aujourd'hui en dur :
  `lyon-part-dieu`).
- US-1.03 / 1.06 : router vers une page `Evenement/{id}` et conserver
  l'onglet d'origine.
- US-1.05 : activer l'onglet "Rechercher" et brancher un endpoint
  `GET /api/evenements/search?q=...`.
- US-1.08 : remplacer `_conseillerNom` ("Camille Toulouse" en dur) par le
  nom du conseiller authentifié (cf. epic Auth).
- US-1.10 : implémenter la page Paramètres et activer le bouton ⚙️ de
  l'en-tête ainsi que l'entrée "Scanner" et "Rechercher un inscrit" de la
  bottom navigation (cf. epics correspondants).
