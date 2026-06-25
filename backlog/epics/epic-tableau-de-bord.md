## Epic 5

---
DONE
**[US-5.01] Consulter la carte « Vision globale » des entretiens**

Conseiller terrain, je veux voir le total d'entretiens planifiés et restants afin de piloter l'avancement en un coup d'œil.

**Critères d'acceptation**
- Étant donné un planning généré, quand j'ouvre « Tableau de bord », alors la carte « Vision globale » affiche total + restants + chevron sur « restants ».
- Étant donné la carte, quand je tape le chevron « restants », alors redirection vers le drill-down « RDV restants » (US-5.06).

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-5, 3pts
---

---
DONE
**[US-5.02] Consulter la carte « Statut candidats »**

Conseiller terrain, je veux voir les candidats sans entretien et la répartition présents/absents afin de repérer les candidats à risque.

**Critères d'acceptation**
- Étant donné le dashboard après génération, quand la carte s'affiche, alors candidats sans entretien (+ chevron) et colonnes présents/absents avec labels textuels.
- Étant donné la carte, quand je tape le chevron « sans entretien », alors drill-down « Candidats sans RDV » (US-5.08).

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-5, 3pts
---

---
DONE
**[US-5.03] Consulter la carte « Statut recruteurs »**

Conseiller terrain, je veux voir la répartition présents/absents des recruteurs afin de m'assurer que les entreprises sont arrivées avant de lancer les entretiens.

**Critères d'acceptation**
- Étant donné le dashboard, quand la carte s'affiche, alors présents et absents en deux colonnes avec labels textuels.
- Étant donné tous présents (ex. 5/0), quand la carte s'affiche, alors « Absents » affiche « 0 » sans être masquée.

**Estimation** : 2 pts
**Priorité** : MUST
**Tags** : epic-5, 2pts
---

---
DONE
**[US-5.04] Consulter les suites données aux entretiens**

Conseiller terrain, je veux voir la carte « Suites & recrutements » (recrutements, 2e entretiens OUI/PEUT-ÊTRE/NON, immersions, POEI) afin de mesurer les résultats en temps réel et les communiquer.

**Critères d'acceptation**
- Étant donné le dashboard, quand la carte s'affiche, alors recrutements, segment tri-couleur « 2e entretiens » avec libellés + valeurs, immersions et POEI.
- Étant donné de nouvelles données, quand une valeur OUI/PEUT-ÊTRE/NON change, alors valeur et proportion màj sans rechargement.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-5, 3pts
---

---
Done
**[US-5.05] Consulter les scores de satisfaction candidats et recruteurs**

Conseiller terrain, je veux voir les notes moyennes de satisfaction afin de détecter une expérience dégradée et ajuster l'animation.

**Critères d'acceptation**
- Étant donné le dashboard, quand la carte « Enquêtes de satisfaction » s'affiche, alors notes candidats et recruteurs avec libellés + étoiles/valeur explicite.
- Étant donné aucune enquête soumise, quand la carte s'affiche, alors « — »/« N/A » plutôt qu'un zéro trompeur.

**Estimation** : 2 pts
**Priorité** : SHOULD
**Tags** : epic-5, 2pts
---

---
DONE
**[US-5.06] Accéder au drill-down « RDV restants » et consulter les créneaux par recruteur**

Conseiller terrain, je veux voir les RDV restants groupés par recruteur en accordéon afin d' identifier les créneaux non honorés et intervenir.

**Critères d'acceptation**
- Étant donné le chevron « entretiens restants », quand je le tape, alors écran « RDV restants » listant les recruteurs et créneaux non honorés.
- Étant donné l'écran ouvert, quand je tape un recruteur, alors accordéon déployé (créneaux horodatés + candidat affecté).
- Étant donné un accordéon déployé, quand je retape le même recruteur, alors il se referme.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-5, 5pts
---

---
DONE
**[US-5.07] Rechercher dans la liste des RDV restants**

Conseiller terrain, je veux filtrer les RDV restants par recruteur/candidat afin de localiser un créneau sans tout défiler.

**Critères d'acceptation**
- Étant donné « RDV restants », quand je saisis un terme, alors seuls les recruteurs/candidats correspondants s'affichent.
- Étant donné une recherche active, quand j'efface le champ, alors la liste complète est restaurée.

**Estimation** : 2 pts
**Priorité** : COULD
**Tags** : epic-5, 2pts
---

---
**[US-5.08] Accéder au drill-down « Candidats sans RDV » et consulter la liste**

Conseiller terrain, je veux voir les candidats présents sans RDV afin de les identifier et leur attribuer un créneau avant la fin.

**Critères d'acceptation**
- Étant donné le chevron « candidats sans entretien », quand je le tape, alors écran « Candidats sans RDV » (nom, prénom, bouton d'appel).
- Étant donné la liste affichée, quand je défile, alors tous les candidats non planifiés apparaissent sans troncature silencieuse.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-5, 3pts
---

---
**[US-5.09] Appeler un candidat sans RDV directement depuis le drill-down**

Conseiller terrain, je veux lancer un appel depuis sa ligne afin de le recontacter pour l'orienter vers un créneau disponible.

**Critères d'acceptation**
- Étant donné « Candidats sans RDV », quand je tape l'icône téléphone, alors l'app téléphonie s'ouvre avec le numéro pré-rempli.
- Étant donné un candidat sans numéro, quand je consulte sa ligne, alors bouton désactivé + « Numéro non disponible ».

**Estimation** : 2 pts
**Priorité** : SHOULD
**Tags** : epic-5, 2pts
---

---
DONE
**[US-5.10] Rafraîchir automatiquement les données du tableau de bord en temps réel**

Conseiller terrain, je veux que les KPI se mettent à jour sans action afin de piloter avec des données toujours à jour même en pointage simultané.

**Critères d'acceptation**
- Étant donné le dashboard ouvert et le réseau stable, quand de nouvelles données arrivent, alors les cartes impactées se mettent à jour sans rechargement.
- Étant donné aucune action pendant 30 s, quand le délai passe, alors rafraîchissement automatique (polling/push).
- Étant donné une valeur changée, quand la màj s'affiche, alors transition visuelle subtile sans perturber la lecture.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-5, 5pts
---

---
**[US-5.11] Gérer l'état d'erreur du tableau de bord après génération du planning**

Conseiller terrain, je veux un message explicite et une relance si le dashboard ne charge pas afin de ne pas rester bloqué pendant l'événement.

**Critères d'acceptation**
- Étant donné un planning généré, quand l'API des KPI échoue, alors écran d'erreur « Impossible de charger le tableau de bord » + « Réessayer » + horodatage.
- Étant donné l'écran d'erreur global, quand « Réessayer », alors rechargement complet ; si succès, dashboard normal ; sinon erreur maintenue mise à jour.
- Étant donné une seule carte sur cinq en échec, quand le reste s'affiche, alors les 4 valides s'affichent et la carte en erreur affiche « Données indisponibles » + retry local à cette carte.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-5, 3pts
---