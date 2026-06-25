## Epic 6

---
**[US-6.01] Accéder à l'onglet « Rechercher un événement »**

Conseiller terrain, je veux un point d'entrée dédié dans la barre de navigation afin de trouver tout événement du catalogue, au-delà de ma liste du jour.

**Critères d'acceptation**
- Étant donné n'importe quel écran applicatif, quand je tape l'onglet « Rechercher », alors écran avec champ vide, sans résultats ni filtres.
- Étant donné l'écran ouvert, quand il est prêt, alors focus automatique sur le champ.
- Étant donné l'onglet déjà actif, quand je retape son icône, alors réinitialisation (champ/filtres/résultats).

**Estimation** : 2 pts
**Priorité** : MUST
**Tags** : epic-6, 2pts
---

---
**[US-6.02] Saisir des mots-clés pour rechercher dans l'ensemble du catalogue**

Conseiller terrain, je veux taper des mots-clés afin de retrouver un événement par nom, organisme ou ville, même un autre jour.

**Critères d'acceptation**
- Étant donné l'écran de recherche, quand je saisis ≥ 2 caractères, alors résultats màj en quasi-temps réel (≤ 300 ms).
- Étant donné un terme saisi, quand les résultats s'affichent, alors recherche sur nom/organisme/ville insensible casse/accents.
- Étant donné une saisie supprimée, quand le champ est vide, alors résultats masqués + état initial (US-6.10).

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-6, 3pts
---

---
**[US-6.03] Afficher les résultats sous forme de cartes événements**

Conseiller terrain, je veux des résultats en cartes lisibles afin de comparer et identifier l'événement qui m'intéresse.

**Critères d'acceptation**
- Étant donné des résultats, quand la liste s'affiche, alors chaque carte présente titre, organisme, localisation, créneau et badge inscrits.
- Étant donné des résultats, quand je consulte le haut, alors compteur « N événements trouvés ».
- Étant donné une liste dépassant l'écran, quand je défile, alors chargement progressif sans rechargement complet.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-6, 3pts
---

---
**[US-6.04] Filtrer les résultats par date ou plage de dates**

Conseiller terrain, je veux restreindre à une date/plage afin de trouver les événements correspondant à mon planning.

**Critères d'acceptation**
- Étant donné l'écran, quand j'ouvre le filtre « Date », alors sélecteur date unique ou plage (début/fin).
- Étant donné une plage validée, quand les résultats s'affichent, alors seuls les événements inclus dans la plage.
- Étant donné un filtre date actif, quand je consulte les filtres, alors chip de valeur (ex. « 24–26 juin ») + bouton × ; à la suppression, recalcul avec critères restants.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-6, 5pts
---

---
**[US-6.05] Filtrer par agence ou localisation géographique**

Conseiller terrain, je veux filtrer par agence/localisation afin de trouver des événements de mon secteur.

**Critères d'acceptation**
- Étant donné l'écran, quand j'ouvre « Agence / Lieu », alors champ/liste pour saisir ou sélectionner.
- Étant donné une sélection validée, quand les résultats s'affichent, alors seuls les événements de ce périmètre.
- Étant donné un filtre localisation actif, quand je consulte les filtres, alors chip (ex. « 35000 Rennes ») + ×.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-6, 3pts
---

---
**[US-6.06] Filtrer par statut de l'événement**

Conseiller terrain, je veux filtrer par statut (à venir/en cours/passé) afin de cibler les événements ouverts ou clôturés.

**Critères d'acceptation**
- Étant donné l'écran, quand j'ouvre « Statut », alors trois options en sélection exclusive.
- Étant donné « En cours », quand les résultats s'affichent, alors seuls les événements encadrant l'heure actuelle.
- Étant donné « À venir », quand les résultats s'affichent, alors seuls les événements à date de début future.
- Étant donné un filtre statut actif, quand je consulte les filtres, alors chip coloré + ×.

**Estimation** : 2 pts
**Priorité** : SHOULD
**Tags** : epic-6, 2pts
---

---
**[US-6.07] Visualiser et supprimer individuellement les filtres actifs**

Conseiller terrain, je veux voir tous les filtres et en retirer un sans tout réinitialiser afin d' affiner progressivement.

**Critères d'acceptation**
- Étant donné ≥ 1 filtre actif, quand les résultats s'affichent, alors zone de chips listant chaque filtre + × individuel.
- Étant donné plusieurs filtres, quand je supprime un chip, alors retrait de ce filtre, autres conservés, recalcul immédiat.
- Étant donné tous les chips supprimés, quand la zone est vide, alors elle disparaît et retour à la recherche mots-clés seule.

**Estimation** : 3 pts
**Priorité** : COULD
**Tags** : epic-6, 5pts
---

---
**[US-6.08] Réinitialiser l'ensemble des critères en un seul geste**

Conseiller terrain, je veux tout effacer d'un geste afin de repartir d'une ardoise vierge rapidement.

**Critères d'acceptation**
- Étant donné ≥ 1 critère actif, quand j'appuie « Tout effacer », alors champ vidé, filtres désactivés, chips et résultats effacés.
- Étant donné la réinitialisation, quand je consulte l'écran, alors retour à l'état initial.
- Étant donné un écran vide sans filtre, quand je le consulte, alors « Tout effacer » masqué ou non interactif.

**Estimation** : 2 pts
**Priorité** : SHOULD
**Tags** : epic-6, 2pts
---

---
**[US-6.09] Sélectionner un événement dans les résultats pour accéder à son contexte**

Conseiller terrain, je veux taper une carte résultat afin d' accéder à ses onglets sans repasser par « Mes événements du jour ».

**Critères d'acceptation**
- Étant donné des résultats, quand je tape une carte, alors entrée dans le contexte (Inscrits/Recruteurs/Tableau de bord).
- Étant donné une navigation depuis les résultats, quand je tape retour, alors retour à la recherche avec mots-clés et filtres conservés.
- Étant donné un événement « Passé », quand j'y accède, alors bandeau « Événement terminé » + lecture seule.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-6, 3pts
---

---
**[US-6.10] Afficher l'état initial de l'écran avant toute saisie**

Conseiller terrain, je veux un état d'accueil clair avant saisie afin de comprendre comment utiliser la recherche.

**Critères d'acceptation**
- Étant donné l'onglet ouvert sans saisie, quand l'écran s'affiche, alors invite « Recherchez par nom, organisme ou ville ».
- Étant donné l'état initial, quand je consulte les filtres, alors ils sont présents mais grisés tant qu'aucune recherche n'est en cours.

**Estimation** : 2 pts
**Priorité** : COULD
**Tags** : epic-6, 1pts
---

---
**[US-6.11] Afficher l'état vide quand aucun événement ne correspond**

Conseiller terrain, je veux un message clair et actionnable sans résultat afin de comprendre l'absence et savoir comment modifier mes critères.

**Critères d'acceptation**
- Étant donné des mots-clés/filtres, quand rien ne correspond, alors « Aucun événement trouvé pour ces critères. ».
- Étant donné l'état vide, quand je le consulte, alors suggestion d'action + « Réinitialiser les filtres ».
- Étant donné l'état vide, quand je modifie ma saisie, alors résultats/état màj en temps réel.

**Estimation** : 2 pts
**Priorité** : MUST
**Tags** : epic-6, 2pts
---