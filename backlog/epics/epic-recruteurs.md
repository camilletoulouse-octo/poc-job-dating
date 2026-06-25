## Epic 3

---
**[US-3.01] Consulter la liste des recruteurs avec compteur et barre de progression**

Conseiller terrain, je veux voir la liste des recruteurs avec compteur et barre afin de piloter le taux de présence des entreprises.

**Critères d'acceptation**
- Étant donné le contexte d'un événement, quand j'accède à « Les recruteurs », alors chaque carte affiche logo, contact, raison sociale, badge offres, badge statut et flèche.
- Étant donné des recruteurs enregistrés, quand la liste charge, alors titre « Liste des recruteurs (X/Y) » + barre + pourcentage.
- Étant donné tous présents, quand la liste s'affiche, alors barre en couleur de succès (vert).

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-3, 3pts
---

---
**[US-3.02] Filtrer la liste des recruteurs par statut de présence**

Conseiller terrain, je veux filtrer (Tous/Présent/Absent/Inconnu) afin de me concentrer sur les entreprises non pointées.

**Critères d'acceptation**
- Étant donné « Les recruteurs », quand j'appuie sur un filtre, alors mise à jour immédiate des cartes correspondantes.
- Étant donné « Absent » actif sans recruteur, quand filtré, alors état vide contextuel « Aucun recruteur absent ».
- Étant donné un changement de filtre, quand il change, alors compteur (X/Y) et barre conservent les valeurs globales.

**Estimation** : 2 pts
**Priorité** : SHOULD
**Tags** : epic-3, 2pts
---

---
**[US-3.03] Rechercher un recruteur par son nom ou son entreprise**

Conseiller terrain, je veux rechercher par contact ou raison sociale afin de le retrouver sans parcourir la liste.

**Critères d'acceptation**
- Étant donné « Les recruteurs », quand je saisis du texte, alors filtrage temps réel (contact/raison sociale) insensible casse/accents.
- Étant donné recherche + filtre « Présent », quand je saisis « BUR », alors seule la carte correspondante présente s'affiche.
- Étant donné le champ vidé, quand effacé, alors retour à l'état complet en conservant le filtre statut.

**Estimation** : 2 pts
**Priorité** : SHOULD
**Tags** : epic-3, 2pts
---

---
**[US-3.04] Modifier le statut de présence d'un recruteur via l'action sheet**

Conseiller terrain, je veux modifier le statut depuis la carte afin de pointer arrivée/absence sans quitter la vue.

**Critères d'acceptation**
- Étant donné « Les recruteurs », quand j'appuie sur la zone principale d'une carte (hors chevron), alors action sheet « Sélectionne l'état de présence » + contact/raison sociale + options.
- Étant donné l'action sheet, quand « Présent », alors badge vert immédiat + compteur et barre màj sans rechargement.
- Étant donné l'action sheet, quand « Annuler »/swipe, alors fermeture sans modification.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-3, 3pts
---

---
**[US-3.05] Pointer un recruteur présent par lecture de son QR code**

Conseiller terrain, je veux scanner le QR du badge recruteur afin de pointer son arrivée sans recherche manuelle.

**Critères d'acceptation**
- Étant donné « Les recruteurs » et l'accès scanner via l'icône QR, quand le scan s'ouvre, alors mode « recruteur » (titre distinct, contexte conservé).
- Étant donné le scan ouvert, quand je présente un QR recruteur valide, alors contact + raison sociale + coche verte + statut « Présent ».
- Étant donné le scan réussi, quand l'identité s'affiche, alors « Scan suivant » pour enchaîner.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-3, 5pts
---

---
**[US-3.06] Gérer un QR code recruteur illisible ou non reconnu lors du scan**

Conseiller terrain, je veux être guidé quand un QR recruteur est illisible/non reconnu afin de ne pas rester bloqué et pointer autrement.

**Critères d'acceptation**
- Étant donné le scan recruteur actif, quand le QR est illisible (timeout), alors message contextuel « QR code illisible — rapprochez le badge » et viseur ouvert.
- Étant donné un QR lisible hors événement, quand lu, alors « Badge non reconnu pour cet événement ».
- Étant donné un QR non reconnu, quand le message s'affiche, alors « Chercher manuellement » renvoie vers la liste champ activé.

**Estimation** : 2 pts
**Priorité** : SHOULD
**Tags** : epic-3, 2pts
---

---
**[US-3.07] Accéder au scan QR recruteur depuis le header de la liste**

Conseiller terrain, je veux accéder au scanner depuis l'icône du header recruteurs afin de lancer un scan sans changer d'onglet.

**Critères d'acceptation**
- Étant donné « Les recruteurs », quand je tape l'icône QR du header, alors scan en mode « recruteur » avec contexte préservé.
- Étant donné le scan terminé, quand je quitte, alors retour direct à « Les recruteurs » à la position de scroll précédente.

**Estimation** : 2 pts
**Priorité** : SHOULD
**Tags** : epic-3, 1pts
---

---
**[US-3.08] Consulter le détail d'un recruteur depuis la liste**

Conseiller terrain, je veux accéder au détail via la flèche afin d' obtenir ses infos et ses offres pour orienter les candidats.

**Critères d'acceptation**
- Étant donné « Les recruteurs », quand je tape le chevron, alors vue détail (logo, contact, raison sociale, statut, nombre d'offres, liste d'offres).
- Étant donné la vue détail, quand je tape retour, alors retour à la liste à la même position de scroll.
- Étant donné la vue détail, quand le statut change ailleurs, alors mise à jour en temps réel.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-3, 3pts
---

---
**[US-3.09] Ajouter un nouveau recruteur sur place depuis la liste**

Conseiller terrain, je veux enregistrer un recruteur non préinscrit afin d' intégrer des entreprises arrivées de façon imprévue.

**Critères d'acceptation**
- Étant donné « Les recruteurs », quand je tape « + », alors formulaire (Nom, Prénom, Raison sociale obligatoires ; Nombre d'offres optionnel ≥ 0).
- Étant donné les obligatoires renseignés, quand je valide, alors ajout avec statut « Inconnu », Y incrémenté, barre recalculée.
- Étant donné le formulaire ouvert, quand « Annuler »/swipe, alors aucun ajout.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-3, 5pts
---

---
**[US-3.11] État vide : liste des recruteurs sans aucun recruteur enregistré**

Conseiller terrain, je veux un état vide explicite quand aucun recruteur n'est enregistré afin de savoir que je dois en ajouter un.

**Critères d'acceptation**
- Étant donné aucun recruteur, quand j'accède à l'onglet, alors écran vide « Aucun recruteur enregistré pour cet événement » + « Ajouter un recruteur ».
- Étant donné l'état vide global, quand « Ajouter un recruteur », alors formulaire (US-3.09).
- Étant donné un filtre actif sans correspondance, quand filtré, alors état vide contextuel distinct du global.

**Estimation** : 2 pts
**Priorité** : SHOULD
**Tags** : epic-3, 2pts
---