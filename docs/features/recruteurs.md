# Feature — Les recruteurs (contexte événement)

> Epic 3 (cf. [`backlog/epics/epic-recruteurs.md`](../../backlog/epics/epic-recruteurs.md))
> Maquette : [`assets/EcransSVG/13-Liste-recruteurs.svg`](../../assets/EcransSVG/13-Liste-recruteurs.svg)

Deuxième onglet du contexte d'un événement ouvert : il affiche l'en-tête
persistant de l'événement, les onglets de navigation, puis la liste des
recruteurs avec un compteur de présence et une barre de progression (verte
quand tous les recruteurs sont présents).

---

## 1. Vue d'ensemble

| | |
|---|---|
| URL | `/evenements/{evenementId}/recruteurs` |
| Page | [`Pages/EvenementRecruteurs.razor`](../../ParcoursCandidatClient/Pages/EvenementRecruteurs.razor) |
| Composants | [`Components/Evenement/`](../../ParcoursCandidatClient/Components/Evenement) (composants partagés avec l'onglet inscrits) |
| Services | [`Services/EvenementService.cs`](../../ParcoursCandidatClient/Services/EvenementService.cs) — `GetEvenementByIdAsync` <br/> [`Services/RecruteurService.cs`](../../ParcoursCandidatClient/Services/RecruteurService.cs) — `GetRecruteursAsync`, `UpdateStatutAsync` |
| Endpoints API | `GET /api/evenements/{id}` <br/> `GET /api/evenements/{id}/recruteurs` <br/> `PATCH /api/recruteurs/{id}` |
| Données mockées | [`ParcoursCandidatApi/Data/evenements.json`](../../ParcoursCandidatApi/Data/evenements.json) <br/> [`ParcoursCandidatApi/Data/recruteurs.json`](../../ParcoursCandidatApi/Data/recruteurs.json) |

## 2. User stories couvertes

| US | Statut | Notes |
|---|---|---|
| **US-3.01** Consulter la liste des recruteurs avec compteur et barre de progression | ✅ Livrée | Titre "Liste des recruteurs (N/Total)" où N = présents, barre de progression (violette → verte si 100 %), carte avec raison sociale, contact, badge offres, badge statut et flèche. |
| **US-3.02** Filtrer la liste des recruteurs par statut de présence | ✅ Livrée | Segmented control Tous / Présent / Absent / Inconnu. Mise à jour immédiate des cartes. État vide contextuel par statut. Compteur et barre conservent les valeurs globales. |
| **US-3.04** Modifier le statut de présence d'un recruteur via l'action sheet | ✅ Livrée | Tap sur la zone principale d'une carte ouvre l'action sheet. Badge mis à jour immédiatement. Compteur et barre recalculés sans rechargement. Annuler/swipe ferme sans modification. |

## 3. Architecture

```
┌──────────────────────────────────┐  GET /api/evenements/{id}
│ Pages/EvenementRecruteurs.razor  │ ─────────────────────────►  ┌──────────────────────────┐
│   - charge l'événement           │  GET /api/evenements/{id}/  │ ParcoursCandidatApi      │
│   - charge les recruteurs        │      recruteurs             │  Program.cs              │
│   - modifie le statut (US-3.04)  │  PATCH /api/recruteurs/{id} │  Data/recruteurs.json    │
│   - skeleton / erreur / vide     │ ◄─────────────────────────  │  Data/evenements.json    │
└─────────────┬────────────────────┘            JSON             └──────────────────────────┘
              │ injecte
              ▼
   ┌──────────────────────┐
   │ EvenementService.cs  │  GetEvenementByIdAsync
   │ RecruteurService.cs  │  GetRecruteursAsync
   │                      │  UpdateStatutAsync (US-3.04)
   └──────────────────────┘
```

### Modèle `Recruteur`

| Champ | Type | Exemple |
|---|---|---|
| `Id` | `string` | `"rec-001"` |
| `EvenementId` | `string` | `"evt-001"` |
| `Nom` | `string` | `"Dupont"` |
| `Prenom` | `string` | `"Marie"` |
| `RaisonSociale` | `string` | `"Brasserie du Vieux Lyon"` |
| `NombreOffres` | `int` | `3` |
| `Statut` | `StatutRecruteur` (`PRESENT`, `ABSENT`, `INCONNU`) | `"PRESENT"` |

### Endpoints API

| Endpoint | Description | Codes |
|---|---|---|
| `GET /api/evenements/{id}/recruteurs` | Liste des recruteurs d'un événement, triée par raison sociale puis nom (US-3.01). Une liste vide est renvoyée si l'événement n'existe pas. | `200` |
| `PATCH /api/recruteurs/{id}` | Met à jour le statut de présence d'un recruteur (US-3.04). Corps : `{ "statut": "PRESENT" \| "ABSENT" \| "INCONNU" }`. | `200`, `404` |

### Persistance mockée

L'API utilise un repository JSON `JsonRecruteursRepository` qui lit et écrit
[`Data/recruteurs.json`](../../ParcoursCandidatApi/Data/recruteurs.json).
Le fichier `evenements.json` est partagé avec les autres features.

### Composants réutilisés / créés

| Composant | Rôle | Paramètres |
|---|---|---|
| [`EvenementHeader.razor`](../../ParcoursCandidatClient/Components/Evenement/EvenementHeader.razor) | En-tête persistant (partagé avec l'onglet inscrits) | `Titre`, `Organisme`, `Ville`, `Departement`, `HeureDebut`, `HeureFin` |
| [`EvenementTabs.razor`](../../ParcoursCandidatClient/Components/Evenement/EvenementTabs.razor) | Onglets de navigation — "Les inscrits" et "Les recruteurs" sont désormais actifs | `OngletActif`, `EvenementId` |
| [`RecruteursContent.razor`](../../ParcoursCandidatClient/Components/Evenement/RecruteursContent.razor) | Titre + barre de progression (verte si 100 %) + segmented control de filtre + liste filtrée avec raison sociale, contact, badge offres, badge statut, flèche + action sheet de modification de statut (US-3.04) | `Chargement`, `Erreur`, `Recruteurs`, `Reessayer`, `StatutModifie` |
| [`FiltreStatutRecruteur.cs`](../../ParcoursCandidatClient/Components/Evenement/FiltreStatutRecruteur.cs) | Enum des filtres de présence (US-3.02) : `TOUS`, `PRESENT`, `ABSENT`, `INCONNU` | — |

### Calcul du compteur et de la barre

`RecruteursContent` calcule :
- `NombrePresents = Recruteurs.Count(r => r.Statut == PRESENT)`
- `PourcentagePresents = round(NombrePresents / Total * 100)` (`0` si la liste est vide)
- La barre passe en **vert** (`rec-progress--complet`) quand `PourcentagePresents == 100`
- Le compteur et la barre sont toujours calculés sur la **liste globale**, quel que soit le filtre actif (US-3.02)

### Filtre par statut (US-3.02)

`RecruteursContent` maintient un état `_filtreActif` (`FiltreStatutRecruteur`, défaut `TOUS`).
La propriété calculée `RecruteursFiltres` filtre la liste en mémoire :

```
TOUS    → Recruteurs (liste complète)
PRESENT → Recruteurs où Statut == PRESENT
ABSENT  → Recruteurs où Statut == ABSENT
INCONNU → Recruteurs où Statut == INCONNU
```

Quand la liste filtrée est vide et que la liste globale ne l'est pas, un message contextuel est affiché :
- « Aucun recruteur présent. »
- « Aucun recruteur absent. »
- « Aucun recruteur au statut inconnu. »

### Action sheet de modification de statut (US-3.04)

Un tap sur la **zone principale** d'une carte recruteur (hors chevron) ouvre une action sheet modale :
- Titre : « Sélectionne l'état de présence »
- Sous-titre : raison sociale — nom prénom du contact
- Options : **Présent** / **Absent** / **Participation inconnue** / **Annuler**

Quand un statut est choisi :
1. L'action sheet se ferme immédiatement.
2. `PATCH /api/recruteurs/{id}` est appelé avec le nouveau statut.
3. La liste locale est mise à jour sans rechargement (`_recruteurs` remplacé par projection).
4. Le badge de la carte, le compteur et la barre de progression sont recalculés.

Un tap sur **Annuler** ou sur l'overlay ferme l'action sheet sans modification.

### Structure de la page

```
┌──────────────────────────────────────────┐
│  En-tête persistant                      │
│   ├─ ◀  Bouton retour                    │
│   ├─ Titre événement                     │
│   ├─ Organisme                           │
│   ├─ Ville (département)                 │
│   └─ Horaires                            │
├──────────────────────────────────────────┤
│  Onglets pilule                          │
│   ├─ Les inscrits (lien actif)           │
│   ├─ Les recruteurs (actif)              │
│   └─ Tableau de bord (désactivé)         │
├──────────────────────────────────────────┤
│  Liste des recruteurs                    │
│   ├─ Titre "Liste des recruteurs (N/T)"  │
│   ├─ Barre de progression (verte si 100%)│
│   ├─ Filtre : Tous | Présent | Absent    │
│   │           | Inconnu                  │
│   └─ Lignes (cliquables) :               │
│              Raison sociale              │
│              Contact (Nom Prénom)        │
│              Badge offres + badge statut │
│              Flèche › (non cliquable)    │
├──────────────────────────────────────────┤
│  Action sheet (US-3.04, si ouverte)      │
│   ├─ Titre + contact/raison sociale      │
│   ├─ Présent                             │
│   ├─ Absent                              │
│   ├─ Participation inconnue              │
│   └─ Annuler                             │
└──────────────────────────────────────────┘
```

## 4. Tests automatisés

| Projet | Type | Cible | Outils |
|---|---|---|---|
| [`ParcoursCandidatApi.Tests/Recruteurs/GetRecruteursEndpointTests.cs`](../../ParcoursCandidatApi.Tests/Recruteurs/GetRecruteursEndpointTests.cs) | Tests d'intégration API | `GET /api/evenements/{id}/recruteurs` | `xUnit` + `WebApplicationFactory` + `FluentAssertions` |
| [`ParcoursCandidatApi.Tests/Recruteurs/PatchStatutRecruteurEndpointTests.cs`](../../ParcoursCandidatApi.Tests/Recruteurs/PatchStatutRecruteurEndpointTests.cs) | Tests d'intégration API | `PATCH /api/recruteurs/{id}` | `xUnit` + `WebApplicationFactory` + `FluentAssertions` |
| [`ParcoursCandidatClient.Tests/Recruteurs/RecruteurServiceTests.cs`](../../ParcoursCandidatClient.Tests/Recruteurs/RecruteurServiceTests.cs) | Tests unitaires | `RecruteurService` | `xUnit` + `HttpMessageHandler` mocké + `FluentAssertions` |
| [`ParcoursCandidatClient.Tests/Recruteurs/EvenementRecruteursPageTests.cs`](../../ParcoursCandidatClient.Tests/Recruteurs/EvenementRecruteursPageTests.cs) | Tests E2E composant | `EvenementRecruteurs` (page + filtre US-3.02 + action sheet US-3.04) | `bUnit` + `FluentAssertions` |

Fixtures dédiées :
- [`RecruteursFixtures.cs`](../../ParcoursCandidatApi.Tests/Fixtures/RecruteursFixtures.cs) — IDs et ordre attendu côté API.
- [`RecruteursHttpFixtures.cs`](../../ParcoursCandidatClient.Tests/Fixtures/RecruteursHttpFixtures.cs) — JSON et `Recruteur` côté client.

### Cas couverts (synthèse)

**API — `GetRecruteursEndpointTests`**
1. `200` pour un événement existant.
2. Bon nombre de recruteurs pour `evt-001` (5).
3. Tous les recruteurs sont rattachés à l'événement demandé.
4. Tri par raison sociale puis nom.
5. Liste vide pour un événement sans recruteur.
6. Liste vide pour un identifiant d'événement inexistant.
7. Champs renseignés (id, nom, prénom, raison sociale, nombre d'offres, statut).
8. Nombre de présents correct.

**API — `PatchStatutRecruteurEndpointTests`** (US-3.04)
1. `200` pour un recruteur existant.
2. Statut `ABSENT` correctement persisté et renvoyé.
3. Statut `PRESENT` correctement persisté et renvoyé.
4. Statut `INCONNU` correctement persisté et renvoyé.
5. `404` pour un identifiant inexistant.

**Client — `RecruteurServiceTests`**
1. Désérialisation correcte de trois recruteurs.
2. Désérialisation correcte des trois statuts.
3. Désérialisation correcte de la raison sociale.
4. Liste vide gérée.
5. Route appelée = `/api/evenements/{id}/recruteurs`.
6. Identifiant URL-encodé.
7. Une réponse 5xx lève `HttpRequestException`.
8. `UpdateStatutAsync` : recruteur mis à jour désérialisé (US-3.04).
9. `UpdateStatutAsync` : route appelée = `/api/recruteurs/{id}` (US-3.04).
10. `UpdateStatutAsync` : méthode HTTP = `PATCH` (US-3.04).
11. `UpdateStatutAsync` : `null` renvoyé si 404 (US-3.04).
12. `UpdateStatutAsync` : `HttpRequestException` si 5xx (US-3.04).

**Client — `EvenementRecruteursPageTests`** (US-3.02 + US-3.04)
1. Les quatre chips de filtre sont visibles au chargement.
2. Le chip « Tous » est actif par défaut.
3. Filtre « Tous » : tous les recruteurs affichés, compteur global conservé.
4. Filtre « Présent » : seuls les présents affichés, chip actif mis à jour.
5. Filtre « Absent » : seuls les absents affichés, chip actif mis à jour.
6. Filtre « Inconnu » : seuls les inconnus affichés, chip actif mis à jour.
7. État vide contextuel « Aucun recruteur absent. » quand filtre Absent sans résultat.
8. État vide contextuel « Aucun recruteur présent. » quand filtre Présent sans résultat.
9. Retour à « Tous » après un filtre : tous les recruteurs réaffichés.
10. Changement de filtre : compteur et barre conservent les valeurs globales.
11. Tap sur une ligne : action sheet ouverte avec titre et contact/raison sociale (US-3.04).
12. Action sheet : quatre options présentes (US-3.04).
13. Annuler : action sheet fermée sans modification (US-3.04).
14. Choisir « Présent » : badge vert, action sheet fermée (US-3.04).
15. Choisir « Absent » : badge rouge, compteur inchangé (US-3.04).
16. Choisir « Participation inconnue » : badge gris (US-3.04).
17. Choisir « Présent » : compteur mis à jour sans rechargement (US-3.04).

## 5. Évolutions à prévoir

- **US-3.03** : recherche d'un recruteur par contact ou raison sociale.
- **US-3.05+** : scan QR code, ajout d'un recruteur sur place, etc.
