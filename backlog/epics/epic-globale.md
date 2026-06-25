# Directives — 23/06/2026

> Projet : **1782202092829-um1s0e** · 82 stories · Source : Loom (édition manuelle)

## Epic 0

---
**[US-0.01] Suite Tokens — couleurs, typographie, espacement, radius et ombres**

Conseiller terrain, je veux interagir avec une interface visuellement cohérente sur tous les écrans afin de lire rapidement les informations importantes sans effort cognitif.

**Critères d'acceptation**
- Étant donné que l'application est ouverte, quand n'importe quel écran s'affiche, alors couleurs, polices, espacements, radius et ombres proviennent du système de tokens centralisé (aucune valeur en dur).
- Étant donné que les tokens sont définis, quand un token primaire est modifié, alors tous les composants se mettent à jour automatiquement.
- Étant donné la palette définie, quand les ratios de contraste sont mesurés, alors toute paire texte/fond atteint ≥ 4,5:1 (normal) ou 3:1 (large/UI).

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-0, 5pts
---

---
**[US-0.02] Suite Boutons — variants, tailles et états**

Conseiller terrain, je veux des boutons clairs et réactifs au tactile afin de déclencher les actions clés sans hésitation.

**Critères d'acceptation**
- Étant donné la suite intégrée, quand un `Button` est rendu, alors il supporte `primary`, `secondary`, `ghost`, `danger`, visuellement distincts.
- Étant donné un appui maintenu, quand l'état `pressed` se déclenche, alors le retour visuel est perceptible en < 100 ms.
- Étant donné l'état `loading`, quand un appel est en cours, alors un indicateur remplace le libellé et le bouton ne répond plus.
- Étant donné l'état `disabled`, quand il s'affiche, alors opacité 38 % et aucune action au tap.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-0, 3pts
---

---
**[US-0.03] Suite Form — champs, sélecteurs, labels et validation**

Conseiller terrain, je veux saisir les paramètres dans des formulaires guidants afin de configurer l'événement sans erreurs.

**Critères d'acceptation**
- Étant donné la suite intégrée, quand un `TextInput`/`NumberInput`/`Select` est rendu, alors il affiche un `label` visible et les états `default`, `focused`, `filled`, `error`, `disabled`.
- Étant donné un champ requis vide soumis, quand la validation s'exécute, alors un message d'erreur inline rouge s'affiche sous le champ.
- Étant donné un champ numérique hors plage (0 RDV, durée négative), quand la validation s'exécute, alors un message spécifique s'affiche et la soumission est bloquée.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-0, 5pts
---

---
**[US-0.04] Suite Card — cartes événement, recruteur et KPI**

Conseiller terrain, je veux des informations regroupées en cartes structurées afin de scanner en un coup d'œil un événement, un recruteur ou un indicateur.

**Critères d'acceptation**
- Étant donné la suite intégrée, quand un `Card` est rendu, alors il supporte `EventCard`, `RecruiterCard` et `KPICard` avec leurs sous-éléments respectifs.
- Étant donné une `EventCard` à titre long, quand le texte dépasse, alors troncature ellipsis sur une ligne sans casser le layout.
- Étant donné une `KPICard` avec chevron, quand le conseiller tape la carte, alors la navigation de drill-down est déclenchée.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-0, 5pts
---

---
**[US-0.05] Suite Navigation — bottom tab bar, onglets et segmented control**

Conseiller terrain, je veux naviguer via une barre d'onglets persistante et filtrer via contrôles segmentés afin d' atteindre toute section ou vue filtrée en un geste.

**Critères d'acceptation**
- Étant donné la suite intégrée, quand l'app est sur une liste, alors la `BottomTabBar` affiche les onglets (icône + libellé), surligne l'actif et reste visible sauf en vue Scan plein écran.
- Étant donné un tap d'onglet, quand la navigation s'effectue, alors transition < 300 ms sans rechargement complet.
- Étant donné le `SegmentedControl` (Tous/Présent/Absent/Inconnu), quand un segment est choisi, alors il est mis en valeur et la liste est filtrée instantanément.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-0, 5pts
---

---
**[US-0.06] Suite Action & Bottom Sheet — panneaux modaux et actions contextuelles**

Conseiller terrain, je veux des panneaux glissants pour les actions contextuelles afin d' effectuer des opérations rapides sans perdre le contexte.

**Critères d'acceptation**
- Étant donné la suite intégrée, quand un `ActionSheet` est déclenché, alors un panneau s'élève avec titre contextuel et options (Présent/Absent/Inconnu/Annuler).
- Étant donné le `BottomSheet` Paramètres ouvert, quand le conseiller swipe vers le bas ou tape l'arrière-plan, alors fermeture animée < 250 ms.
- Étant donné une option sélectionnée, quand elle est confirmée, alors le panneau se ferme et l'action s'exécute avec retour visuel.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-0, 5pts
---

---
**[US-0.07] Suite Feedback — toasts, alertes et états vide/chargement/erreur**

Conseiller terrain, je veux être informé du résultat de mes actions et de l'état des listes afin de savoir si une opération a réussi ou pourquoi une liste est vide.

**Critères d'acceptation**
- Étant donné une action réussie, quand la confirmation arrive, alors un `Toast` succès (vert, coche) s'affiche 3 s puis disparaît.
- Étant donné une liste en chargement, quand les données manquent, alors un skeleton/indicateur circulaire s'affiche.
- Étant donné une liste vide après chargement, quand l'état vide est atteint, alors illustration + message explicite.
- Étant donné une erreur réseau, quand la requête échoue, alors état d'erreur + bouton « Réessayer ».

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-0, 3pts
---

---
**[US-0.08] Suite Indicateurs visuels — badges statut, barres de progression, chips, compteurs**

Conseiller terrain, je veux lire d'un coup d'œil l'état de présence et l'avancement du pointage afin de piloter l'événement en temps réel.

**Critères d'acceptation**
- Étant donné la suite intégrée, quand un `BadgeStatut` est rendu, alors `Présent`/`Absent`/`Inconnu` = pastille verte/rouge/grise **avec libellé** (jamais la couleur seule).
- Étant donné le compteur `(13/25)` + barre, quand un pointage est enregistré, alors barre et texte se mettent à jour sans rechargement.
- Étant donné un `ChipHoraire`, quand il s'affiche, alors hauteur ≥ 28 dp et texte ≥ 14 sp.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-0, 3pts
---

---
**[US-0.09] Accessibilité — conformité WCAG AA globale du design system**

Conseiller terrain, je veux utiliser l'app avec VoiceOver/TalkBack afin d' accéder à tout sans dépendre de la vision ou d'une motricité fine.

**Critères d'acceptation**
- Étant donné un écran avec lecteur d'écran, quand il parcourt l'UI, alors tout élément interactif a un `accessibilityLabel` non vide annonçant son rôle.
- Étant donné un Sheet ouvert, quand le conseiller navigue, alors le focus est piégé dans le panneau.
- Étant donné les indicateurs colorés, quand ils sont audités, alors chacun inclut libellé/icône alternative.
- Étant donné les composants livrés, quand le contraste est audité, alors 100 % des paires respectent 4,5:1 / 3:1.

**Estimation** : 8 pts
**Priorité** : MUST
**Tags** : epic-0, 8pts
---

---
**[US-0.10] Internationalisation FR — setup, tokens de chaînes et extractions**

Conseiller terrain, je veux que tous les textes s'affichent en français afin d' utiliser une app sans faute ni mélange d'anglais.

**Critères d'acceptation**
- Étant donné l'i18n configuré, quand l'app démarre, alors langue par défaut `fr` et aucun texte anglais non traduit visible.
- Étant donné la suite livrée, quand elle est auditée, alors 100 % des chaînes visibles sont externalisées (aucune chaîne en dur).
- Étant donné une clé absente de `fr.json`, quand un composant l'affiche, alors la clé brute est visible + alerte en dev.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-0, 3pts
---

---
**[US-0.11] Theming — mode clair/sombre et custom properties**

Conseiller terrain, je veux que l'app s'adapte au thème de mon smartphone afin de l'utiliser confortablement en intérieur comme en plein air.

**Critères d'acceptation**
- Étant donné le theming configuré, quand l'OS passe en sombre, alors tous les composants basculent sans action manuelle.
- Étant donné les tokens des deux modes, quand le contraste est mesuré, alors AA respecté en clair et sombre.
- Étant donné des custom properties, quand un token est modifié, alors la propagation est globale.

**Estimation** : 3 pts
**Priorité** : COULD
**Tags** : epic-0, 3pts
---







