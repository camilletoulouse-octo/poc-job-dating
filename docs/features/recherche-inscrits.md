# Feature — Recherche d'un inscrit par nom (US-2.03)

Depuis la liste des inscrits, le conseiller terrain peut saisir un nom ou un prénom
dans la barre de recherche pour filtrer la liste en temps réel. Un bouton `+` (icône QR)
à droite de la barre ouvre directement l'appareil photo pour scanner un QR code.

---

## 1. Vue d'ensemble

| | |
|---|---|
| Composant | [`Components/Evenement/InscritsContent.razor`](../../ParcoursCandidatClient/Components/Evenement/InscritsContent.razor) |
| CSS | [`Components/Evenement/InscritsContent.razor.css`](../../ParcoursCandidatClient/Components/Evenement/InscritsContent.razor.css) |
| Page parente | [`Pages/EvenementInscrits.razor`](../../ParcoursCandidatClient/Pages/EvenementInscrits.razor) |
| Tests | [`Tests/Candidats/RechercheInscritsTests.cs`](../../ParcoursCandidatClient.Tests/Candidats/RechercheInscritsTests.cs) |

## 2. Architecture

```
EvenementInscrits.razor
  └─ InscritsContent (EvenementId="evt-001", Inscrits="...")
       ├─ ins-header        : titre + barre de progression
       ├─ ins-recherche-row : champ de recherche + bouton scanner
       │   ├─ ins-recherche-wrapper  : input[type=search] + icône loupe
       │   └─ ins-btn-scanner        : <a href="/evenements/{id}/scanner">
       ├─ ins-segmented     : filtres Tous / Présent / Absent / Inconnu
       └─ ins-list          : liste filtrée (recherche ∩ filtre statut)
```

## 3. Comportement

### Barre de recherche

- Le filtrage est **temps réel** (`oninput`) : chaque frappe met à jour la liste.
- La recherche porte sur le **nom** et le **prénom** de l'inscrit.
- Elle est **insensible à la casse** et aux **accents** (normalisation Unicode FormD).
- Elle se **combine** avec le filtre de statut actif : seuls les inscrits satisfaisant
  les deux conditions sont affichés.
- Effacer le champ restaure la liste complète (en conservant le filtre statut).

### Bouton scanner (icône QR)

- Lien `<a>` vers `/evenements/{EvenementId}/scanner`.
- Ouvre la page `Scanner.razor` qui active l'appareil photo pour lire un QR code.
- L'`EvenementId` est transmis via le paramètre `EvenementId` du composant `InscritsContent`,
  lui-même passé depuis `EvenementInscrits.razor`.

## 4. Paramètre ajouté à `InscritsContent`

| Paramètre | Type | Description |
|---|---|---|
| `EvenementId` | `string` | Identifiant de l'événement, utilisé pour construire le lien vers le scanner. |

## 5. Logique de filtrage combiné

```csharp
// Filtre statut
var inscrits = _filtreActif switch {
    PRESENT => Inscrits.Where(i => i.Statut == PRESENT),
    ABSENT  => Inscrits.Where(i => i.Statut == ABSENT),
    INCONNU => Inscrits.Where(i => i.Statut == INCONNU),
    _       => Inscrits.AsEnumerable()
};

// Filtre recherche (si non vide)
if (!string.IsNullOrWhiteSpace(_recherche))
{
    var terme = NormaliserTexte(_recherche);
    inscrits = inscrits.Where(i =>
        NormaliserTexte(i.Nom).Contains(terme) ||
        NormaliserTexte(i.Prenom).Contains(terme));
}
```

La normalisation supprime les diacritiques via `String.Normalize(FormD)` et convertit en minuscules.

## 6. Tests

| Test | Description |
|---|---|
| Champ de recherche visible | Le `.ins-recherche-input` est présent dans le DOM |
| Bouton scanner avec bon lien | `.ins-btn-scanner` pointe vers `/evenements/evt-001/scanner` |
| Recherche par nom | Saisir "Bernard" → 1 résultat |
| Recherche par prénom | Saisir "Sophie" → 1 résultat |
| Insensibilité à la casse | Saisir "martin" → trouve "Martin" |
| Insensibilité aux accents | Saisir "leger" → trouve "Léger" |
| Effacement de la recherche | Vider le champ → tous les inscrits réapparaissent |
| Recherche + filtre statut | "b" + filtre Présent → uniquement Bernard Thomas (PRESENT) |
| Aucun résultat | Saisir "zzz" → message vide affiché |
