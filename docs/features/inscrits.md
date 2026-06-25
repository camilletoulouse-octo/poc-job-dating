# Feature — Les inscrits (contexte événement)

> Epics 1 et 2 (cf. [`backlog/epics/epic-mes-evenements.md`](../../backlog/epics/epic-mes-evenements.md)
> et [`backlog/epics/epic-candidats.md`](../../backlog/epics/epic-candidats.md))
> Maquette : [`assets/EcransSVG/11-Liste-inscrits.svg`](../../assets/EcransSVG/11-Liste-inscrits.svg)

Premier écran du contexte d'un événement ouvert : il affiche l'en-tête
persistant de l'événement (titre, organisme, lieu, horaires), les onglets
"Les inscrits / Les recruteurs / Tableau de bord" — seul **Les inscrits**
est livré — puis la liste des inscrits avec un compteur de pointage et
une barre de progression.

---

## 1. Vue d'ensemble

| | |
|---|---|
| URL | `/evenements/{evenementId}/inscrits` |
| Page | [`Pages/EvenementInscrits.razor`](../../ParcoursCandidatClient/Pages/EvenementInscrits.razor) |
| Composants | [`Components/Evenement/`](../../ParcoursCandidatClient/Components/Evenement) (3 sous-composants extraits, voir §3) |
| Services | [`Services/EvenementService.cs`](../../ParcoursCandidatClient/Services/EvenementService.cs) — `GetEvenementByIdAsync` <br/> [`Services/InscritService.cs`](../../ParcoursCandidatClient/Services/InscritService.cs) — `GetInscritsAsync`, `UpdateStatutAsync` |
| Endpoints API | `GET /api/evenements/{id}` <br/> `GET /api/evenements/{id}/inscrits` <br/> `PATCH /api/inscrits/{id}` |
| Données mockées | [`ParcoursCandidatApi/Data/evenements.json`](../../ParcoursCandidatApi/Data/evenements.json) <br/> [`ParcoursCandidatApi/Data/inscrits.json`](../../ParcoursCandidatApi/Data/inscrits.json) |

## 2. User stories couvertes

| US | Statut | Notes |
|---|---|---|
| **US-1.03** Accéder au contexte d'un événement | ✅ Livrée | Navigation déclenchée par la flèche cerclée des cartes de l'écran "Mes événements". |
| **US-1.04** Naviguer entre les onglets d'un événement | ✅ Partiellement livrée | Onglets "Les inscrits" et "Les recruteurs" actifs (liens de navigation) ; "Tableau de bord" présent mais désactivé. |
| **US-1.06** Revenir à la liste d'événements | ✅ Livrée | Bouton retour (chevron) dans l'en-tête persistant vers `/mes-evenements`. |
| **US-2.01** Consulter la liste des inscrits | ✅ Livrée | Titre "Liste des inscrits (N/Total)", barre de progression proportionnelle, badge statut coloré pour chaque inscrit. |
| **US-2.02** Filtrer la liste des inscrits par statut | ✅ Livrée | Chips Tous / Présent / Absent / Inconnu dans `InscritsContent`. Filtrage client-side, compteur global inchangé. |
| **US-2.04** Modifier le statut de présence via l'action sheet | ✅ Livrée | Tap sur une ligne → action sheet « Sélectionne l'état de présence » + nom + options Présent / Absent / Participation inconnue / Annuler. Badge et compteur mis à jour sans rechargement. |

## 3. Architecture

```
┌─────────────────────────────────┐  GET /api/evenements/{id}
│ Pages/EvenementInscrits.razor   │ ─────────────────────────►  ┌──────────────────────┐
│   - charge l'événement          │  GET /api/evenements/{id}/  │ ParcoursCandidatApi  │
│   - charge les inscrits         │      inscrits               │  Program.cs          │
│   - skeleton / erreur / vide    │ ◄─────────────────────────  │  Data/inscrits.json  │
└─────────────┬───────────────────┘            JSON             │  Data/evenements.json│
              │ injecte                                         └──────────────────────┘
              ▼
   ┌──────────────────────┐
   │ EvenementService.cs  │  GetEvenementByIdAsync
   │ InscritService.cs    │  GetInscritsAsync
   └──────────────────────┘
```

### Modèle partagé `Inscrit`

| Champ | Type | Exemple |
|---|---|---|
| `Id` | `string` | `"ins-001"` |
| `EvenementId` | `string` | `"evt-001"` |
| `Nom` | `string` | `"Martin"` |
| `Prenom` | `string` | `"Lucie"` |
| `Statut` | `StatutInscrit` (`PRESENT`, `ABSENT`, `INCONNU`) | `"PRESENT"` |

### Endpoints API

| Endpoint | Description | Codes |
|---|---|---|
| `GET /api/evenements/{id}` | Détail d'un événement (US-1.03) | `200`, `404` |
| `GET /api/evenements/{id}/inscrits` | Liste des inscrits d'un événement, triée par nom puis prénom (US-2.01). Une liste vide est renvoyée si l'événement n'existe pas. | `200` |
| `PATCH /api/inscrits/{id}` | Met à jour le statut de présence d'un inscrit (US-2.04). Corps : `{ "statut": "PRESENT" \| "ABSENT" \| "INCONNU" }`. Retourne l'inscrit mis à jour. | `200`, `404` |

### Persistance mockée

L'API utilise un repository JSON `JsonInscritsRepository` qui lit
[`Data/inscrits.json`](../../ParcoursCandidatApi/Data/inscrits.json) au
démarrage. Le fichier `evenements.json` est partagé avec la feature "Mes
événements" (epic 1).

### Sous-composants extraits

La page agence 3 sous-composants situés dans
[`Components/Evenement/`](../../ParcoursCandidatClient/Components/Evenement),
chacun avec son propre fichier `.razor.css` scopé. Ce découpage prépare
la réutilisation par les autres onglets (Les recruteurs / Tableau de
bord) lorsqu'ils seront livrés.

| Composant | Rôle | Paramètres |
|---|---|---|
| [`EvenementHeader.razor`](../../ParcoursCandidatClient/Components/Evenement/EvenementHeader.razor) | En-tête persistant : bouton retour + titre + organisme + lieu + horaires | `Titre`, `Organisme`, `Ville`, `Departement`, `HeureDebut`, `HeureFin` |
| [`EvenementTabs.razor`](../../ParcoursCandidatClient/Components/Evenement/EvenementTabs.razor) | Onglets pilule "Les inscrits / Les recruteurs / Tableau de bord" | `OngletActif` |
| [`InscritsContent.razor`](../../ParcoursCandidatClient/Components/Evenement/InscritsContent.razor) | Titre + barre de progression + chips de filtre (US-2.02) + liste filtrée avec badge statut + action sheet de modification de statut (US-2.04) (gère skeletons / erreur / vide) | `Chargement`, `Erreur`, `Inscrits`, `Reessayer`, `StatutModifie` |

L'enum [`OngletEvenement`](../../ParcoursCandidatClient/Components/Evenement/OngletEvenement.cs)
est partagé entre la page et les sous-composants.

### Calcul du pointage

`InscritsContent` calcule :
- `NombrePointes = Inscrits.Count(i => i.Statut != INCONNU)`
- `PourcentagePointes = round(NombrePointes / Total * 100)` (`0` si la liste est vide).

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
│   ├─ Les inscrits (actif)                │
│   ├─ Les recruteurs (désactivé)          │
│   └─ Tableau de bord (désactivé)         │
├──────────────────────────────────────────┤
│  Liste des inscrits                      │
│   ├─ Titre "Liste des inscrits (N/T)"    │
│   ├─ Barre de progression                │
│   ├─ Chips : [Tous] [Présent] [Absent]   │
│   │          [Inconnu]                   │
│   └─ Lignes filtrées : Nom + badge statut│
└──────────────────────────────────────────┘
```

## 4. Tests automatisés

| Projet | Type | Cible | Outils |
|---|---|---|---|
| [`ParcoursCandidatApi.Tests/Candidats/GetInscritsEndpointTests.cs`](../../ParcoursCandidatApi.Tests/Candidats/GetInscritsEndpointTests.cs) | Tests d'intégration API | `GET /api/evenements/{id}/inscrits` | `xUnit` + `WebApplicationFactory` + `FluentAssertions` |
| [`ParcoursCandidatApi.Tests/Candidats/PatchStatutInscritEndpointTests.cs`](../../ParcoursCandidatApi.Tests/Candidats/PatchStatutInscritEndpointTests.cs) | Tests d'intégration API | `PATCH /api/inscrits/{id}` | idem |
| [`ParcoursCandidatApi.Tests/MesEvenements/GetEvenementByIdEndpointTests.cs`](../../ParcoursCandidatApi.Tests/MesEvenements/GetEvenementByIdEndpointTests.cs) | Tests d'intégration API | `GET /api/evenements/{id}` | idem |
| [`ParcoursCandidatClient.Tests/Candidats/InscritServiceTests.cs`](../../ParcoursCandidatClient.Tests/Candidats/InscritServiceTests.cs) | Tests unitaires | `InscritService` | `xUnit` + `HttpMessageHandler` mocké + `FluentAssertions` |
| [`ParcoursCandidatClient.Tests/MesEvenements/EvenementServiceTests.cs`](../../ParcoursCandidatClient.Tests/MesEvenements/EvenementServiceTests.cs) | Tests unitaires | `EvenementService.GetEvenementByIdAsync` (entre autres) | idem |
| [`ParcoursCandidatClient.Tests/Candidats/EvenementInscritsPageTests.cs`](../../ParcoursCandidatClient.Tests/Candidats/EvenementInscritsPageTests.cs) | Tests "E2E" composant | `Pages/EvenementInscrits.razor` | `bUnit` + `xUnit` + `FluentAssertions` |

Fixtures dédiées :
- [`InscritsFixtures.cs`](../../ParcoursCandidatApi.Tests/Fixtures/InscritsFixtures.cs) — IDs et ordre attendu côté API.
- [`InscritsHttpFixtures.cs`](../../ParcoursCandidatClient.Tests/Fixtures/InscritsHttpFixtures.cs) — JSON et `Inscrit` côté client.

### Cas couverts (synthèse)

**API — `GetInscritsEndpointTests`**
1. `200` pour un événement existant.
2. Bon nombre d'inscrits pour `evt-001`.
3. Tous les inscrits sont rattachés à l'événement demandé.
4. Tri par nom puis prénom.
5. Liste vide pour un événement sans inscrit.
6. Liste vide pour un identifiant d'événement inexistant.
7. Champs renseignés (id, nom, prénom, statut).

**API — `GetEvenementByIdEndpointTests`**
1. `200` pour un identifiant existant.
2. Désérialisation correcte de l'événement.
3. `404` pour un identifiant inexistant.

**Client — `InscritServiceTests`**
1. Désérialisation correcte de quatre inscrits.
2. Désérialisation correcte des trois statuts.
3. Liste vide gérée.
4. Route appelée = `/api/evenements/{id}/inscrits`.
5. Identifiant URL-encodé.
6. Une réponse 5xx lève `HttpRequestException`.

**Client — `EvenementServiceTests` (ajouts US-1.03)**
1. Route appelée = `/api/evenements/{id}`.
2. Désérialisation correcte de l'événement.
3. Une réponse 404 renvoie `null`.

**Composant — `EvenementInscritsPageTests` (bUnit)**

*US-2.01*
1. L'en-tête affiche le titre de l'événement.
2. L'onglet "Les inscrits" est actif.
3. Le compteur "(N/T)" affiche les bons nombres (pointés vs total).
4. Barre de progression à `0%` quand aucun inscrit.
5. Chaque ligne d'inscrit possède un badge statut adapté.
6. Le nom et le prénom de l'inscrit sont visibles.
7. Le bouton retour pointe vers `/mes-evenements`.
8. Bloc d'erreur + bouton "Réessayer" en cas d'échec HTTP.

*US-2.02*
9. Les quatre chips Tous / Présent / Absent / Inconnu sont visibles.
10. Le chip "Tous" est actif par défaut.
11. Filtre "Tous" : tous les inscrits affichés avec le compteur global.
12. Filtre "Présent" : seuls les présents affichés, chip actif mis à jour.
13. Filtre "Absent" : seuls les absents affichés, chip actif mis à jour.
14. Filtre "Inconnu" : seuls les indéterminés affichés, chip actif mis à jour.
15. Filtre sans résultat : message "Aucun inscrit ne correspond à ce filtre."
16. Retour sur "Tous" après un filtre : tous les inscrits de nouveau affichés.

*US-2.04*
17. Tap sur une ligne → action sheet avec titre « Sélectionne l'état de présence » et nom de l'inscrit.
18. Action sheet affiche les quatre options : Présent / Absent / Participation inconnue / Annuler.
19. « Annuler » ferme l'action sheet sans modifier le badge.
20. « Présent » ferme l'action sheet et le badge devient vert.
21. « Absent » ferme l'action sheet, badge rouge, compteur incrémenté.
22. « Participation inconnue » ferme l'action sheet et le badge devient gris.

**API — `PatchStatutInscritEndpointTests`**
1. `200` pour un inscrit existant.
2. Statut `ABSENT` correctement persisté et renvoyé.
3. Statut `PRESENT` correctement persisté et renvoyé.
4. Statut `INCONNU` correctement persisté et renvoyé.
5. `404` pour un identifiant inexistant.

**Client — `InscritServiceTests` (ajouts US-2.04)**
7. Désérialisation correcte de l'inscrit mis à jour.
8. Route appelée = `/api/inscrits/{id}`.
9. Méthode HTTP = `PATCH`.
10. Une réponse 404 renvoie `null`.
11. Une réponse 5xx lève `HttpRequestException`.

## 5. Évolutions à prévoir

- **US-1.04** : activer les onglets "Les recruteurs" et "Tableau de bord".
- **US-2.03** : recherche d'un inscrit par nom (filtrage temps réel, combinable avec le filtre statut).
- **US-2.05+** : scan QR code, ajout d'un inscrit sur place, etc.
