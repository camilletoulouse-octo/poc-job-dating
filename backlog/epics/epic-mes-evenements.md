## Epic 1

---
DONE
**[US-1.01] Consulter la liste de mes événements du jour**

Conseiller terrain, je veux voir mes événements du jour dès l'ouverture afin de choisir rapidement lequel animer.

**Critères d'acceptation**
- Étant donné la première ouverture du jour, quand « Mes événements » se charge, alors l'onglet « Mes événements du jour » est actif et liste les cartes assignées à l'agence pour la date du jour.
- Étant donné la liste chargée, quand une carte est consultée, alors titre, organisme, ville+département, chip horaire et badge « X inscrits » sont affichés.
- Étant donné plusieurs événements, quand la liste s'affiche, alors tri chronologique croissant.
- Étant donné des données en chargement, quand la liste n'est pas prête, alors skeleton loaders.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-1, 3pts
---

---
**[US-1.02] Modifier l'agence de référence pour filtrer les événements**

Conseiller terrain, je veux modifier l'agence affichée afin de voir les événements de mon périmètre quand je couvre plusieurs agences.

**Critères d'acceptation**
- Étant donné l'écran « Mes événements », quand le conseiller tape le crayon près du libellé d'agence, alors un sélecteur/champ éditable s'ouvre.
- Étant donné une nouvelle agence validée, quand confirmée, alors la liste est rafraîchie sur la nouvelle agence.
- Étant donné l'agence modifiée, quand l'app est rouverte, alors l'agence est mémorisée.

**Estimation** : 2 pts
**Priorité** : SHOULD
**Tags** : epic-1, 2pts
---

---
DONE
**[US-1.03] Accéder au contexte d'un événement depuis sa carte**

Conseiller terrain, je veux taper la flèche d'une carte afin d' entrer dans le contexte opérationnel de l'événement.

**Critères d'acceptation**
- Étant donné la liste affichée, quand le conseiller tape la flèche cerclée, alors redirection vers l'événement, onglet « Les inscrits » actif.
- Étant donné l'événement ouvert, quand l'en-tête est consulté, alors titre/organisme/lieu/horaire cohérents avec la carte.
- Étant donné la navigation entre onglets, quand l'en-tête est observé, alors les infos clés restent persistantes.

**Estimation** : 2 pts
**Priorité** : MUST
**Tags** : epic-1, 2pts
---

---
**[US-1.04] Naviguer entre les onglets d'un événement ouvert**

Conseiller terrain, je veux basculer entre « Les inscrits », « Les recruteurs » et « Tableau de bord » afin d' accéder à toutes les dimensions sans perdre mon contexte.

**Critères d'acceptation**
- Étant donné l'onglet « Les inscrits » actif, quand le conseiller tape « Les recruteurs », alors ce dernier devient actif et chargé, l'autre inactif.
- Étant donné un filtre/scroll appliqué, quand on change d'onglet puis revient, alors position et filtres conservés.
- Étant donné « Tableau de bord » avant génération, quand consulté, alors état d'attente (cactus) affiché.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-1, 3pts
---

---
**[US-1.05] Rechercher un événement par mots-clés**

Conseiller terrain, je veux utiliser l'onglet « Rechercher un événement » afin de localiser un événement dont j'ignore la date ou l'agence.

**Critères d'acceptation**
- Étant donné l'onglet « Rechercher » tapé, quand il s'ouvre, alors champ focusé et clavier affiché.
- Étant donné ≥ 2 caractères saisis, quand on continue, alors résultats en temps réel (titre/organisme) sous forme de cartes.
- Étant donné des résultats, quand une carte est tapée, alors accès au contexte (onglet « Les inscrits »).
- Étant donné le champ effacé, quand il est vide, alors résultats masqués et état initial restauré.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-1, 3pts
---

---
**[US-1.06] Revenir à la liste d'événements depuis un événement ouvert**

Conseiller terrain, je veux revenir à ma liste depuis n'importe quel onglet afin de changer d'événement sans perdre mes données saisies.

**Critères d'acceptation**
- Étant donné un événement ouvert, quand le conseiller tape retour, alors retour à « Mes événements » sur l'onglet d'origine.
- Étant donné le retour à la liste, quand elle se réaffiche, alors le compteur « X inscrits » est rafraîchi.
- Étant donné le geste retour système, quand il est effectué, alors comportement identique au bouton retour.

**Estimation** : 2 pts
**Priorité** : MUST
**Tags** : epic-1, 2pts
---

---
**[US-1.07] Basculer d'un événement vers un autre en cours de journée**

Conseiller terrain, je veux sélectionner un autre événement après en avoir animé un premier afin de gérer plusieurs événements depuis un seul appareil.

**Critères d'acceptation**
- Étant donné le retour à la liste depuis A, quand le conseiller tape B, alors contexte B sans données résiduelles de A.
- Étant donné l'événement B ouvert, quand compteurs/listes sont consultés, alors infos exclusivement de B.
- Étant donné un travail successif A puis B, quand on retourne sur A, alors données A rechargées sans mélange.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-1, 3pts
---

---
**[US-1.08] Afficher le nom du conseiller connecté dans l'en-tête de la liste**

Conseiller terrain, je veux voir mon nom sous le titre de l'app afin de confirmer mon identité avant de démarrer.

**Critères d'acceptation**
- Étant donné l'authentification et « Mes événements » chargé, quand l'en-tête est consulté, alors le nom complet est affiché en sous-titre lecture seule.
- Étant donné la navigation entre onglets de liste, quand on bascule, alors le nom reste visible et inchangé.
- Étant donné l'entrée/sortie d'un événement, quand la liste se réaffiche, alors le nom reste cohérent.

**Estimation** : 2 pts
**Priorité** : MUST
**Tags** : epic-1, 1pts
---

---
**[US-1.09] État vide : aucun événement planifié pour aujourd'hui**

Conseiller terrain, je veux un message clair quand je n'ai aucun événement aujourd'hui afin de comprendre la situation et savoir quoi faire.

**Critères d'acceptation**
- Étant donné une liste vide retournée pour le jour, quand le chargement réussit, alors état vide illustré « Aucun événement prévu aujourd'hui » + bouton « Rechercher un événement ».
- Étant donné l'état vide, quand le bouton est tapé, alors redirection vers « Rechercher » champ focusé.
- Étant donné le passage à une nouvelle journée, quand on rafraîchit, alors les événements de la nouvelle date se chargent.

**Estimation** : 2 pts
**Priorité** : MUST
**Tags** : epic-1, 2pts
---