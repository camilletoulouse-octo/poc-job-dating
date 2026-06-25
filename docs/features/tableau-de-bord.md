# Feature — Tableau de bord

> Epic 5 (cf. [`backlog/epics/epic-tableau-de-bord.md`](../../backlog/epics/epic-tableau-de-bord.md))
> Maquettes : [`assets/EcransSVG/15-Tableau-de-bord.svg`](../../assets/EcransSVG/15-Tableau-de-bord.svg), [`assets/EcransSVG/17-CandidatssansRDV.svg`](../../assets/EcransSVG/17-CandidatssansRDV.svg), [`assets/EcransSVG/18-RDVrestants.svg`](../../assets/EcransSVG/18-RDVrestants.svg)

Troisième onglet du contexte événement. Affiche un état d'attente (cactus)
tant que le planning des entretiens n'a pas été généré, puis les cartes de
pilotage une fois le planning disponible.

---

## 1. Vue d'ensemble

| | |
|---|---|
| URL | `/evenements/{id}/tableau-de-bord` |
| Page | [`Pages/EvenementTableauDeBord.razor`](../../ParcoursCandidatClient/Pages/EvenementTableauDeBord.razor) |
| Composants réutilisés | [`EvenementHeader`](../../ParcoursCandidatClient/Components/Evenement/EvenementHeader.razor), [`EvenementBottomNav`](../../ParcoursCandidatClient/Components/Evenement/EvenementBottomNav.razor) |
| Services | [`Services/EvenementService.cs`](../../ParcoursCandidatClient/Services/EvenementService.cs), [`Services/ITableauDeBordService.cs`](../../ParcoursCandidatClient/Services/ITableauDeBordService.cs) |
| Modèle | [`Models/TableauDeBord.cs`](../../ParcoursCandidatClient/Models/TableauDeBord.cs) |
| Données mock | [`ParcoursCandidatApi/Data/tableau-de-bord.json`](../../ParcoursCandidatApi/Data/tableau-de-bord.json) |

## 2. User stories couvertes

| US | Statut | Notes |
|---|---|---|
| **US-1.04** Naviguer entre les onglets d'un événement | ✅ Livrée | Bottom nav à 3 onglets. Onglet "Tableau de bord" actif sur cette page. |
| **US-5.01** Consulter la carte « Vision globale » des entretiens | ✅ Livrée | Carte affichant total + restants + chevron vers drill-down RDV restants. |
| **US-5.02** Consulter la carte « Statut candidats » | ✅ Livrée | Carte affichant candidats sans entretien (+ chevron) et colonnes présents/absents. |
| **US-5.03** Consulter la carte « Statut recruteurs » | ✅ Livrée | Carte affichant présents/absents en deux colonnes avec labels textuels. Absents affiché même à 0. |
| **US-5.04** Consulter les suites données aux entretiens | ✅ Livrée | Carte « Suites & recrutements » : recrutements, segment tri-couleur 2e entretiens (OUI/PEUT-ÊTRE/NON), immersions, POEI. Proportions dynamiques selon candidats présents. |
| **US-5.05** Consulter les scores de satisfaction candidats et recruteurs | ✅ Livrée | Carte « Enquêtes de satisfaction » : notes candidats et recruteurs avec libellés + étoile SVG + valeur. Affiche « — » si aucune enquête soumise. Notes dynamiques selon présence. |
| **US-5.06** Consulter le détail des RDV restants par créneau | ✅ Livrée | Page dédiée `/evenements/{id}/rdv-restants` accessible via le chevron « Restants » de la carte Vision globale. |

## 3. Architecture

### Bouton « Générer le planning »

Le composant `EvenementHeader` expose un bouton **« Générer le planning »**
(paramètre `AfficherBoutonPlanning="true"`) toujours visible pour permettre
la régénération après modification des paramètres.

- Au clic, il appelle `POST /api/evenements/{id}/planning` via le callback
  `OnGenererPlanning`.
- Après la génération, il recharge les données du tableau de bord.
- Pendant la génération, le bouton affiche un spinner et est désactivé.

### État d'attente (cactus)

Affiché quand `GET /api/evenements/{id}/tableau-de-bord` retourne 404 :

- Illustration cactus (`images/cactus.png`)
- Titre : **"Planning non généré"**
- Description explicative

### Carte « Vision globale » (US-5.01)

Affichée quand le planning est généré (`GET /api/evenements/{id}/tableau-de-bord` retourne 200) :

- **Total entretiens planifiés** : nombre affiché en grand
- **Entretiens restants** : nombre en violet avec chevron → lien vers
  `/evenements/{id}/rdv-restants` (US-5.06)

### Carte « Statut recruteurs » (US-5.03)

Affichée entre la carte Vision globale et la carte Statut candidats :

- **Deux colonnes** avec labels textuels :
  - **Présents** : nombre en vert (`#006F46`)
  - **Absents** : nombre en rouge (`#E01F1F`) — affiché même si la valeur est `0`
- Les valeurs sont **calculées dynamiquement** depuis `recruteurs.json` via `IRecruteursRepository` :
  - `presents` = recruteurs avec statut `PRESENT`
  - `absents` = recruteurs avec statut `ABSENT`
  - Les recruteurs `INCONNU` ne sont comptés ni dans présents ni dans absents
- La cohérence avec la liste recruteurs est ainsi garantie : modifier le statut d'un recruteur se reflète immédiatement dans la carte.

### Carte « Enquêtes de satisfaction » (US-5.05)

Affichée **après** la carte « Suites & recrutements » (ordre : Vision globale → Statut candidats → Statut recruteurs → Suites & recrutements → Enquêtes de satisfaction), quand `enquetesSatisfaction` est présent dans la réponse API :

- **Deux colonnes** avec libellés en majuscules :
  - **CANDIDATS** : note moyenne sur 5 avec étoile SVG dorée (`#F5C842`) + valeur numérique au format français (ex. `3,5`)
  - **RECRUTEURS** : note moyenne sur 5 avec étoile SVG dorée + valeur numérique au format français (ex. `3,2`)
- Si une note est `null` (aucune enquête soumise), affiche **« — »** à la place du chiffre (jamais un zéro trompeur)
- Les notes sont **dynamiques** : une note n'est affichée que si le nombre de candidats/recruteurs présents est > 0 (sinon `null`)
- La carte est **absente** si `enquetesSatisfaction` est `null` dans la réponse (rétrocompatibilité)

### Carte « Suites & recrutements » (US-5.04)

Affichée **après** la carte « Enquêtes de satisfaction » (ordre : Vision globale → Statut candidats → Statut recruteurs → Enquêtes de satisfaction → Suites & recrutements), quand `suitesRecrutements` est présent dans la réponse API :

- **Liste de lignes** (libellé + valeur + chevron) :
  - **Nombre de recrutements**
  - **Nombre de 2nd entretiens** — section spéciale avec 3 blocs colorés côte à côte :
    - **OUI** : fond vert clair (`#D1FAE5`), texte vert (`#006F46`)
    - **PEUT-ÊTRE** : fond bleu clair (`#DBEAFE`), texte bleu (`#1D4ED8`)
    - **NON** : fond rose clair (`#FEE2E2`), texte rouge (`#DC2626`)
  - **Nombre d'immersion**
  - **Nombre de POEI**
- Les **valeurs absolues** (recrutements, immersions, POEI, 2e entretiens) sont **recalculées dynamiquement** : si leur somme dépasse le nombre de candidats présents, elles sont ramenées proportionnellement à ce plafond
- La carte est **absente** si `suitesRecrutements` est `null` dans la réponse (rétrocompatibilité)

### Carte « Statut candidats » (US-5.02)

Affichée sous la carte Statut recruteurs quand le planning est généré :

- **Candidats sans entretien** : nombre en violet avec chevron → lien vers
  `/evenements/{id}/candidats-sans-rdv` (US-5.08)
- **Séparateur horizontal**
- **Deux colonnes** avec labels textuels :
  - **Présents** : nombre en vert (`#006F46`)
  - **Absents** : nombre en rouge (`#E01F1F`)
- Les valeurs sont **calculées dynamiquement** depuis `inscrits.json` via `IInscritsRepository` :
  - `presents` = inscrits avec statut `PRESENT`
  - `absents` = inscrits avec statut `ABSENT`
  - `sansEntretien` = inscrits avec statut `INCONNU` (non encore pointés)
- La cohérence avec la liste inscrits est ainsi garantie : modifier le statut d'un inscrit se reflète immédiatement dans la carte.

### Navigation entre onglets

`EvenementBottomNav` expose **3 entrées** (US-1.04) :

| Onglet | URL | Enum |
|---|---|---|
| Les inscrits | `/evenements/{id}/inscrits` | `OngletEvenement.LesInscrits` |
| Les recruteurs | `/evenements/{id}/recruteurs` | `OngletEvenement.LesRecruteurs` |
| Tableau de bord | `/evenements/{id}/tableau-de-bord` | `OngletEvenement.TableauDeBord` |

### Structure de la page

```
┌──────────────────────────────────────────────────────┐
│  En-tête événement (EvenementHeader)                 │
│   ├─ ← Retour à mes événements                       │
│   ├─ Titre / Organisme / Lieu / Horaires             │
│   └─ [Générer le planning]                           │
├──────────────────────────────────────────────────────┤
│  Si planning non généré :                            │
│   ├─ 🌵 Illustration cactus                          │
│   ├─ "Planning non généré"                           │
│   └─ Description                                     │
│                                                      │
│  Si planning généré :                                │
│   ├─ Carte « Vision globale »                        │
│   │   ├─ Total entretiens planifiés                  │
│   │   └─ Restants (chevron → RDV restants)           │
│   ├─ Carte « Statut candidats »                      │
│   │   ├─ Sans entretien (chevron → Candidats s/ RDV) │
│   │   ├─ ───────────────────────────────────────── │
│   │   ├─ Présents (vert)                             │
│   │   └─ Absents (rouge)                             │
│   ├─ Carte « Statut recruteurs »                     │
│   │   ├─ Présents (vert)                             │
│   │   └─ Absents (rouge, affiché même à 0)           │
│   ├─ Carte « Suites & recrutements » (si présente)   │
│   │   ├─ Recrutements | Immersions | POEI            │
│   │   └─ 2e entretiens : OUI / PEUT-ÊTRE / NON       │
│   └─ Carte « Enquêtes de satisfaction » (si présente)│
│       ├─ CANDIDATS : ★ 3,5                           │
│       └─ RECRUTEURS : ★ 3,2  (ou « — » si null)     │
├──────────────────────────────────────────────────────┤
│  Bottom navigation (3 onglets)                       │
│   ├─ Les inscrits                                    │
│   ├─ Les recruteurs                                  │
│   └─ Tableau de bord (actif)                         │
└──────────────────────────────────────────────────────┘
```

## 4. Page « Candidats sans RDV » (US-5.08)

| | |
|---|---|
| URL | `/evenements/{id}/candidats-sans-rdv` |
| Page | [`Pages/EvenementCandidatsSansRdv.razor`](../../ParcoursCandidatClient/Pages/EvenementCandidatsSansRdv.razor) |
| Service | [`Services/ICandidatsSansRdvService.cs`](../../ParcoursCandidatClient/Services/ICandidatsSansRdvService.cs) |
| Modèle | [`Models/CandidatSansRdv.cs`](../../ParcoursCandidatClient/Models/CandidatSansRdv.cs) |
| Données mock | [`ParcoursCandidatApi/Data/candidats-sans-rdv.json`](../../ParcoursCandidatApi/Data/candidats-sans-rdv.json) |
| Maquette | [`assets/EcransSVG/17-CandidatssansRDV.svg`](../../assets/EcransSVG/17-CandidatssansRDV.svg) |

Accessible via le chevron **« Sans entretien »** de la carte Statut candidats du tableau de bord.

### Structure de la page

```
┌──────────────────────────────────────────────────────┐
│  En-tête                                             │
│   ├─ ← Retour au tableau de bord                     │
│   └─ "Candidats sans RDV"                            │
├──────────────────────────────────────────────────────┤
│  Liste des candidats                                 │
│   ├─ ROBERT ANTOINE                          📞      │
│   │   Alice                                          │
│   ├─ PETIT EMMA                              📞      │
│   │   Alice                                          │
│   └─ LEROY CAMILLE                           📞      │
│       Alice                                          │
└──────────────────────────────────────────────────────┘
```

### Affichage des candidats

- **Nom et prénom** affichés en majuscules (ex. `ROBERT ANTOINE`)
- **Conseiller** affiché en dessous du nom
- **Bouton d'appel** (icône téléphone violet) à droite de chaque candidat

### États

- **Chargement** : message "Chargement…"
- **Données disponibles** : liste des candidats avec bouton d'appel
- **Aucun candidat** : message "Aucun candidat sans RDV."
- **Erreur** : message d'erreur + bouton "Réessayer"

## 5. Page « RDV restants » (US-5.06)

| | |
|---|---|
| URL | `/evenements/{id}/rdv-restants` |
| Page | [`Pages/EvenementRdvRestants.razor`](../../ParcoursCandidatClient/Pages/EvenementRdvRestants.razor) |
| Service | [`Services/IRdvRestantsService.cs`](../../ParcoursCandidatClient/Services/IRdvRestantsService.cs) |
| Modèle | [`Models/RdvRestants.cs`](../../ParcoursCandidatClient/Models/RdvRestants.cs) |
| Données mock | [`ParcoursCandidatApi/Data/rdv-restants.json`](../../ParcoursCandidatApi/Data/rdv-restants.json) |
| Maquette | [`assets/EcransSVG/18-RDVrestants.svg`](../../assets/EcransSVG/18-RDVrestants.svg) |

Accessible via le chevron **« Restants »** de la carte Vision globale du tableau de bord.

### Structure de la page

```
┌──────────────────────────────────────────────────────┐
│  En-tête                                             │
│   ├─ ← Retour au tableau de bord                     │
│   └─ "RDV restants"                                  │
├──────────────────────────────────────────────────────┤
│  Barre de recherche                                  │
│   └─ [Rechercher          🔍]                        │
├──────────────────────────────────────────────────────┤
│  Liste des recruteurs (accordéon)                    │
│   ├─ 🖥 Marie DUPONT                          ▼      │
│   │   BRASSERIE DU VIEUX LYON                        │
│   ├─ 🖥 Pierre LEFEBVRE                       ▲      │
│   │   HÔTEL BELLECOUR                                │
│   │   ┌─────────────────────────────────────────┐   │
│   │   │ 🕐 10:30 │ LEROY          Camille       │   │
│   │   └─────────────────────────────────────────┘   │
│   └─ …                                               │
└──────────────────────────────────────────────────────┘
```

### Comportement de l'accordéon

- Par défaut, tous les recruteurs sont **fermés** (créneaux masqués)
- Un clic sur un recruteur **ouvre** son accordéon et affiche ses créneaux
- Un second clic **ferme** l'accordéon
- Un seul recruteur peut être ouvert à la fois

### Recherche

- La barre de recherche filtre les recruteurs par **nom**, **prénom** ou **raison sociale** (insensible à la casse)
- Si aucun recruteur ne correspond, affiche "Aucun résultat."

### États

- **Chargement** : message "Chargement…"
- **Données disponibles** : barre de recherche + liste des recruteurs avec accordéon
- **Aucun recruteur** : message "Aucun RDV restant."
- **Erreur** : message d'erreur + bouton "Réessayer"

## 6. API

| Méthode | URL | Description |
|---|---|---|
| `GET` | `/api/evenements/{id}/tableau-de-bord` | Données du tableau de bord (US-5.01 à US-5.05). 404 si planning non généré. |
| `GET` | `/api/evenements/{id}/candidats-sans-rdv` | Liste des candidats sans entretien planifié (US-5.08). 404 si événement introuvable. |
| `GET` | `/api/evenements/{id}/rdv-restants` | Créneaux avec entretiens restants (US-5.06). 404 si événement introuvable. |
| `POST` | `/api/evenements/{id}/planning` | Génère le planning de l'événement. |

### Modèle de réponse `GET /api/evenements/{id}/tableau-de-bord`

```json
{
  "evenementId": "evt-001",
  "visionGlobale": { "totalEntretiens": 24, "entretiensRestants": 9 },
  "statutCandidats": { "sansEntretien": 3, "presents": 3, "absents": 1 },
  "statutRecruteurs": { "presents": 5, "absents": 0 },
  "suitesRecrutements": {
    "recrutements": 5,
    "deuxiemesEntretiens": { "oui": 8, "peutEtre": 4, "non": 3 },
    "immersions": 2,
    "poei": 1
  },
  "enquetesSatisfaction": { "noteCandidats": 3.5, "noteRecruteurs": 3.2 }
}
```

### Modèle de réponse `GET /api/evenements/{id}/rdv-restants`

```json
{
  "evenementId": "evt-001",
  "recruteurs": [
    {
      "recruteurId": "rec-001",
      "nom": "Dupont",
      "prenom": "Marie",
      "raisonSociale": "Brasserie du Vieux Lyon",
      "creneaux": [
        { "heure": "10:00", "candidatNom": "ROBERT", "candidatPrenom": "Antoine" },
        { "heure": "10:15", "candidatNom": "PETIT", "candidatPrenom": "Emma" }
      ]
    }
  ]
}
```

Les données sont mockées dans
[`ParcoursCandidatApi/Data/tableau-de-bord.json`](../../ParcoursCandidatApi/Data/tableau-de-bord.json) et
[`ParcoursCandidatApi/Data/rdv-restants.json`](../../ParcoursCandidatApi/Data/rdv-restants.json).

## 7. Tests automatisés

| Fichier | Type | Outils |
|---|---|---|
| [`ParcoursCandidatClient.Tests/TableauDeBord/EvenementTableauDeBordPageTests.cs`](../../ParcoursCandidatClient.Tests/TableauDeBord/EvenementTableauDeBordPageTests.cs) | Tests E2E composant — tableau de bord | `bUnit` + `xUnit` + `FluentAssertions` |
| [`ParcoursCandidatClient.Tests/TableauDeBord/EvenementCandidatsSansRdvPageTests.cs`](../../ParcoursCandidatClient.Tests/TableauDeBord/EvenementCandidatsSansRdvPageTests.cs) | Tests E2E composant — Candidats sans RDV (US-5.08) | `bUnit` + `xUnit` + `FluentAssertions` |
| [`ParcoursCandidatClient.Tests/TableauDeBord/EvenementRdvRestantsPageTests.cs`](../../ParcoursCandidatClient.Tests/TableauDeBord/EvenementRdvRestantsPageTests.cs) | Tests E2E composant — RDV restants (US-5.06) | `bUnit` + `xUnit` + `FluentAssertions` |
| [`ParcoursCandidatApi.Tests/TableauDeBord/GetCandidatsSansRdvEndpointTests.cs`](../../ParcoursCandidatApi.Tests/TableauDeBord/GetCandidatsSansRdvEndpointTests.cs) | Tests d'intégration API — Candidats sans RDV (US-5.08) | `xUnit` + `FluentAssertions` + `WebApplicationFactory` |

### Cas couverts — tableau de bord (55 tests)

**État d'attente / bouton planning / navigation** : tests 1–7, 50–55 (voir fichier).

**US-5.01 — Carte Vision globale** : total, restants, chevron vers `/rdv-restants`.

**US-5.02 — Carte Statut candidats** : sans entretien, présents, absents, chevron vers `/candidats-sans-rdv`.

**US-5.03 — Carte Statut recruteurs** : présents, absents (affiché même à 0).

**US-5.04 — Carte Suites & recrutements** : recrutements, 2e entretiens (OUI/PEUT-ÊTRE/NON), immersions, POEI.

**US-5.05 — Carte Enquêtes de satisfaction** : notes candidats/recruteurs, tiret si null, absente si non fournie.

### Cas couverts — Candidats sans RDV (9 tests client + 5 tests API, US-5.08)

**En-tête**
1. Le titre "Candidats sans RDV" est affiché.
2. Le lien retour pointe vers `/evenements/{id}/tableau-de-bord`.

**Liste des candidats**
3. Les 3 candidats sont affichés.
4. Le nom du premier candidat est affiché en majuscules (ex. "ROBERT ANTOINE").
5. Le conseiller du premier candidat est affiché (ex. "Alice").
6. Chaque candidat a un bouton d'appel.

**État vide**
7. Le message "Aucun candidat sans RDV." est affiché quand la liste est vide.

**État d'erreur**
8. Le message d'erreur est affiché en cas d'erreur serveur.
9. Le bouton "Réessayer" est affiché en cas d'erreur serveur.

**Tests d'intégration API**
1. La réponse est 200 pour un événement existant.
2. Les 3 candidats sont renvoyés pour evt-001.
3. L'`evenementId` est correct dans la réponse.
4. Les champs `id`, `nom`, `prenom`, `conseiller` sont renseignés.
5. La réponse est 404 pour un événement inexistant.

### Cas couverts — RDV restants (16 tests, US-5.06)

**En-tête**
1. Le titre "RDV restants" est affiché.
2. Le lien retour pointe vers `/evenements/{id}/tableau-de-bord`.

**Barre de recherche**
3. La barre de recherche est affichée.
4. Le placeholder "Rechercher" est affiché.

**Liste des recruteurs**
5. Les 4 recruteurs sont affichés.
6. Le nom du premier recruteur est affiché au format "Prénom NOM" (ex. "Marie DUPONT").
7. La raison sociale du premier recruteur est affichée en majuscules (ex. "BRASSERIE DU VIEUX LYON").

**Accordéon**
8. Par défaut, aucun créneau n'est affiché (tous fermés).
9. Un clic sur un recruteur ouvre son accordéon et affiche ses créneaux.
10. Un second clic sur un recruteur ouvert ferme son accordéon.
11. Les heures des créneaux sont affichées (ex. "10:00", "10:15").
12. Les noms des candidats sont affichés (ex. "ROBERT", "PETIT").
13. Les prénoms des candidats sont affichés (ex. "Antoine", "Emma").

**État vide**
14. Le message "Aucun RDV restant." est affiché quand la liste est vide.

**État d'erreur**
15. Le message d'erreur est affiché en cas d'erreur serveur.
16. Le bouton "Réessayer" est affiché en cas d'erreur serveur.

## 8. Évolutions à prévoir

- Rafraîchissement automatique des KPI (US-5.10).
