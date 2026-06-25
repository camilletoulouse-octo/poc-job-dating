## Epic 2

---
DONE
**[US-2.01] Consulter la liste des inscrits avec compteur et barre de progression**

Conseiller terrain, je veux voir la liste des inscrits avec compteur et barre afin de savoir combien sont pointés sur le total.

**Critères d'acceptation**
- Étant donné le contexte d'un événement, quand j'ouvre « Les inscrits », alors la liste affiche nom, prénom et badge statut coloré pour chacun.
- Étant donné la liste chargée, quand l'onglet s'affiche, alors titre `Liste des inscrits (N/Total)` + barre proportionnelle.
- Étant donné un inscrit pointé, quand son statut change, alors compteur et barre se mettent à jour sans rechargement.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-2, 3pts
---

---
DONE
**[US-2.02] Filtrer la liste des inscrits par statut de présence**

Conseiller terrain, je veux filtrer via segmented control (Tous/Présent/Absent/Inconnu) afin de me concentrer sur les non-pointés ou vérifier les absents.

**Critères d'acceptation**
- Étant donné la liste affichée, quand « Tous » est tapé, alors tous les inscrits + compteur global.
- Étant donné « Présent » sélectionné, quand la liste se met à jour, alors seuls les présents et sous-total filtré.
- Étant donné « Absent » sélectionné, quand la liste se met à jour, alors seuls les absents.
- Étant donné « Inconnu » sélectionné, quand la liste se met à jour, alors seuls les indéterminés.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-2, 3pts
---

---
DONE
**[US-2.03] Rechercher un inscrit par son nom dans la liste**

Conseiller terrain, je veux saisir un nom/prénom dans la recherche afin de retrouver un participant sans défiler.

**Critères d'acceptation**
- Étant donné la liste affichée, quand je tape ≥ 1 caractère, alors filtrage temps réel insensible casse/accents.
- Étant donné recherche + filtre statut, quand les deux s'appliquent, alors seuls les inscrits satisfaisant les deux conditions.
- Étant donné une recherche active, quand j'efface, alors retour à l'état complet en conservant le filtre statut.

**Estimation** : 2 pts
**Priorité** : SHOULD
**Tags** : epic-2, 2pts
---

---
DONE
**[US-2.04] Modifier le statut de présence d'un inscrit via l'action sheet**

Conseiller terrain, je veux appuyer sur un inscrit pour ouvrir une action sheet de statut afin de pointer manuellement ou corriger un pointage.

**Critères d'acceptation**
- Étant donné la liste, quand j'appuie sur une ligne, alors action sheet « Sélectionne l'état de présence » + nom + options Présent/Absent/Participation inconnue/Annuler.
- Étant donné l'action sheet, quand « Présent », alors fermeture, badge vert et compteur incrémenté.
- Étant donné l'action sheet, quand « Absent », alors badge rouge et compteur màj.
- Étant donné l'action sheet, quand « Participation inconnue », alors badge gris.
- Étant donné l'action sheet, quand « Annuler », alors fermeture sans modification.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-2, 5pts
---

---
**[US-2.05] Pointer un inscrit présent par lecture de QR code**

Conseiller terrain, je veux scanner le QR du badge afin de pointer « Présent » sans manipulation manuelle.

**Critères d'acceptation**
- Étant donné l'écran de scan ouvert, quand je présente un QR valide, alors décodage, identification et enregistrement « Présent ».
- Étant donné le scan réussi, quand le QR est reconnu, alors confirmation (coche verte), nom/prénom et bouton « Scan suivant ».
- Étant donné la confirmation, quand « Scan suivant », alors retour immédiat au viseur.
- Étant donné un inscrit pointé par QR, quand je retourne à « Les inscrits », alors badge vert et compteur incrémenté.

**Estimation** : 8 pts
**Priorité** : MUST
**Tags** : epic-2, 8pts
---

---
**[US-2.06] Gérer un QR code non reconnu ou illisible lors du scan**

Conseiller terrain, je veux être averti quand un QR ne correspond à aucun inscrit ou est illisible afin de ne pas créer de pointages erronés.

**Critères d'acceptation**
- Étant donné le viseur actif, quand je scanne un QR hors liste, alors état d'erreur (icône rouge) « Inscrit non trouvé pour cet événement » + « Réessayer ».
- Étant donné l'état d'erreur, quand « Réessayer », alors retour immédiat au viseur.
- Étant donné un QR invalide, quand le décodage échoue, alors « QR code non lisible — repositionnez le badge » + retour auto après 2 s.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-2, 3pts
---

---
**[US-2.07] Accéder à l'écran de scan depuis la navigation principale et depuis un événement**

Conseiller terrain, je veux ouvrir le scan depuis « Scanner tous les événements » et depuis l'icône QR recruteurs afin de lancer un pointage quel que soit mon contexte.

**Critères d'acceptation**
- Étant donné l'accueil, quand je tape « Scanner tous les événements », alors viseur actif.
- Étant donné l'onglet « Les recruteurs », quand je tape l'icône QR, alors scan ouvert dans le contexte de l'événement.
- Étant donné un scan ouvert depuis un événement, quand il se termine, alors retour à l'écran précédent (contexte événement).

**Estimation** : 2 pts
**Priorité** : MUST
**Tags** : epic-2, 2pts
---

---
**[US-2.08] Inscrire un nouveau participant sur place depuis la liste des inscrits**

Conseiller terrain, je veux ajouter un participant via le bouton `+` afin de l'intégrer et le pointer présent immédiatement.

**Critères d'acceptation**
- Étant donné « Les inscrits », quand je tape `+`, alors formulaire Prénom + Nom (obligatoires) + validation.
- Étant donné des données valides, quand je valide, alors ajout avec statut « Présent », total et pointés incrémentés.
- Étant donné le formulaire ouvert, quand « Annuler »/fermeture, alors aucune inscription créée.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-2, 5pts
---

---
**[US-2.09] Consulter le détail d'un inscrit depuis la liste**

Conseiller terrain, je veux taper le chevron d'un inscrit afin de consulter sa fiche sans rien modifier.

**Critères d'acceptation**
- Étant donné la liste, quand je tape le chevron, alors écran détail (nom, prénom, statut actuel au minimum).
- Étant donné l'écran détail, quand je tape retour, alors retour à la liste avec filtre et scroll conservés.

**Estimation** : 3 pts
**Priorité** : COULD
**Tags** : epic-2, 3pts
---

---
DONE
**[US-2.10] État vide : liste des inscrits sans aucun inscrit**

Conseiller terrain, je veux un état vide explicite quand aucun inscrit n'est enregistré afin de comprendre que la liste est vide et en ajouter un.

**Critères d'acceptation**
- Étant donné un événement sans inscrit, quand j'ouvre « Les inscrits », alors état vide « Aucun inscrit pour le moment » + « Ajouter un inscrit ».
- Étant donné l'état vide, quand « Ajouter un inscrit », alors formulaire d'inscription (US-2.08).
- Étant donné un compteur 0/0, quand la liste est vide, alors barre à 0 % sans NaN ni division par zéro.

**Estimation** : 2 pts
**Priorité** : SHOULD
**Tags** : epic-2, 2pts
---