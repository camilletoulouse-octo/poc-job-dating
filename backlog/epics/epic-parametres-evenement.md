## Epic 4

---
**[US-4.01] Ouvrir le panneau des paramètres de l'événement**

Conseiller terrain, je veux accéder au bottom sheet des paramètres via l'engrenage afin de configurer les règles avant génération.

**Critères d'acceptation**
- Étant donné un onglet d'événement, quand je tape l'engrenage, alors bottom sheet « Paramètres de mon événement » avec glissement.
- Étant donné le sheet ouvert, quand je swipe/tape en dehors, alors fermeture sans modification.
- Étant donné des paramètres enregistrés, quand j'ouvre, alors valeurs pré-remplies (RDV, durée, référents).

**Estimation** : 2 pts
**Priorité** : MUST
**Tags** : epic-4, 2pts
---

---
**[US-4.02] Paramétrer les règles de planification**

Conseiller terrain, je veux définir le nombre de RDV par candidat et la durée d'entretien afin que le planning respecte les contraintes horaires.

**Critères d'acceptation**
- Étant donné le sheet ouvert, quand je modifie « RDV par candidats », alors la nouvelle valeur s'affiche.
- Étant donné le sheet ouvert, quand je modifie « Temps par entretien », alors valeur màj.
- Étant donné une valeur hors plage (0/négative), quand je quitte le champ, alors message de validation + « Enregistrer » désactivé.
- Étant donné des valeurs valides, quand « Enregistrer », alors sauvegarde + confirmation.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-4, 3pts
---

---
**[US-4.03] Enregistrer les paramètres de l'événement**

Conseiller terrain, je veux sauvegarder les paramètres via « Enregistrer » afin que les règles s'appliquent à la prochaine génération.

**Critères d'acceptation**
- Étant donné des paramètres valides, quand « Enregistrer », alors persistance + fermeture automatique du sheet.
- Étant donné l'enregistrement réussi, quand le sheet se ferme, alors toast « Paramètres enregistrés » 3 s.
- Étant donné des modifs non enregistrées, quand je ferme puis rouvre, alors valeurs antérieures restaurées.

**Estimation** : 2 pts
**Priorité** : MUST
**Tags** : epic-4, 2pts
---

---
**[US-4.04] Ajouter un conseiller référent à l'événement**

Conseiller terrain, je veux ajouter un référent depuis les paramètres afin qu' il soit identifié comme point de contact officiel.

**Critères d'acceptation**
- Étant donné le sheet ouvert, quand je tape « Ajouter un référent », alors formulaire identité (nom, prénom).
- Étant donné un nom/prénom valides, quand je valide, alors le référent apparaît dans la liste du panneau.
- Étant donné un référent ajouté et enregistré, quand je rouvre plus tard, alors il est toujours présent.
- Étant donné une liste vide, quand j'ouvre le sheet, alors « Aucun référent ajouté » + bouton d'ajout.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-4, 3pts
---

---
**[US-4.05] Modifier ou supprimer un conseiller référent**

Conseiller terrain, je veux éditer ou retirer un référent existant afin de corriger une erreur de saisie ou retirer un contact qui n'est plus rattaché.

**Critères d'acceptation**
- Étant donné la liste des référents, quand je tape le crayon, alors formulaire pré-rempli ; après validation, la liste affiche les nouvelles infos sans recharger.
- Étant donné la liste, quand je tape la corbeille, alors confirmation « Supprimer ce référent ? » (Confirmer/Annuler).
- Étant donné la confirmation, quand « Confirmer », alors le référent disparaît immédiatement ; quand « Annuler », alors liste inchangée.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-4, 3pts
---

---
**[US-4.07] Déclencher la génération du planning depuis les onglets Inscrits et Recruteurs**

Conseiller terrain, je veux lancer la génération depuis l'en-tête des onglets « Les inscrits » et « Les recruteurs » afin de déclencher l'action sans changer d'écran.

**Critères d'acceptation**
- Étant donné un de ces deux onglets chargé, quand il s'affiche, alors le bouton « Générer le planning » est visible et identique dans l'en-tête.
- Étant donné des recruteurs et inscrits présents, quand je tape « Générer le planning », alors boîte de confirmation « Générer le planning pour X recruteurs et Y inscrits présents ? » avant traitement (génération elle-même : US-4.08).
- Étant donné un planning déjà généré, quand je tape le bouton, alors confirmation « Régénérer le planning ? Les données existantes seront remplacées. ».

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-4, 3pts
---

---
**[US-4.08] Générer le planning, consulter le résultat et gérer les erreurs techniques**

Conseiller terrain, je veux lancer le calcul, être notifié de son achèvement et guidé en cas d'erreur afin que les candidats présents soient affectés à des créneaux sans rester bloqué.

**Critères d'acceptation**
- Étant donné des paramètres enregistrés et ≥ 1 recruteur présent, quand je confirme, alors indicateur de chargement pendant le traitement.
- Étant donné une génération réussie, quand elle se termine, alors toast « Planning généré avec succès » et « Tableau de bord » peuplé (entretiens totaux, restants, candidats sans RDV).
- Étant donné le planning généré, quand je navigue au dashboard, alors cartes KPI peuplées de valeurs réelles.
- Étant donné une génération précédente relancée après confirmation, quand elle réussit, alors le planning est remplacé et le dashboard mis à jour.
- Étant donné une erreur (réseau/timeout), quand elle survient, alors l'indicateur disparaît, message « La génération a échoué — vérifiez votre connexion et réessayez », bouton « Réessayer » relançant sans reconfigurer ; en cas de persistance, « Contacter le support ».

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-4, 5pts
---

---
**[US-4.09] Consulter l'état d'attente du tableau de bord avant génération**

Conseiller terrain, je veux un état d'attente explicite tant que le planning n'est pas généré afin de comprendre que les KPI ne sont pas encore disponibles.

**Critères d'acceptation**
- Étant donné un planning non généré, quand j'ouvre « Tableau de bord », alors écran d'attente (illustration cactus) + invitation à générer.
- Étant donné l'état d'attente, quand je lis l'écran, alors texte explicatif + CTA « Générer le planning » accessible.
- Étant donné le planning généré, quand je reviens au dashboard, alors l'état d'attente est remplacé par les KPI.

**Estimation** : 2 pts
**Priorité** : MUST
**Tags** : epic-4, 2pts
---

---
**[US-4.10] Bloquer la génération du planning si les conditions sont insuffisantes**

Conseiller terrain, je veux être alerté si je tente de générer sans paramètres ou sans présents suffisants afin d' éviter un planning incorrect ou vide.

**Critères d'acceptation**
- Étant donné un champ obligatoire non renseigné, quand je tape « Générer le planning », alors blocage + message des champs manquants.
- Étant donné aucun recruteur présent, quand je tente, alors « Aucun recruteur présent — le planning ne peut pas être généré ».
- Étant donné aucun inscrit présent, quand je tente, alors « Aucun candidat présent — le planning ne peut pas être généré ».

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-4, 3pts
---

---
**[US-4.12] Régénérer le planning après modification des paramètres**

Conseiller terrain, je veux régénérer le planning après modification des paramètres afin que les ajustements de dernière minute soient reflétés.

**Critères d'acceptation**
- Étant donné un planning généré, quand je modifie un paramètre et enregistre, alors « Générer le planning » redevient disponible avec indicateur de changement.
- Étant donné un tap « Générer » après modification, quand la confirmation « Régénérer le planning ? Les entretiens déjà planifiés seront recalculés. » s'affiche, alors confirmation requise.
- Étant donné une régénération réussie, quand le dashboard se rafraîchit, alors nouvelles données + toast « Planning mis à jour ».

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-4, 3pts
---






