## Epic 7

---
**[US-7.01] Se connecter à l'application avec ses identifiants France Travail**

Conseiller terrain, je veux me connecter avec mes identifiants France Travail afin d' accéder à mes événements et exercer mes actions.

**Critères d'acceptation**
- Étant donné aucune session active, quand je saisis des identifiants valides et valide, alors redirection vers « Mes événements du jour » + nom affiché.
- Étant donné un champ vide, quand je tape « Se connecter », alors validation inline avant tout envoi.
- Étant donné l'authentification, quand je navigue, alors je reste authentifié sans retour à la connexion.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-7, 5pts
---

---
**[US-7.02] Gérer le cycle de vie de la session — persistance et expiration**

Conseiller terrain, je veux retrouver ma session ouverte et être averti clairement à son expiration afin de ne pas perdre de temps ni d'actions en plein événement.

**Critères d'acceptation**
- Étant donné un token valide, quand je rouvre l'app (ou la sors d'arrière-plan), alors atterrissage direct sur « Mes événements du jour » sans saisie de mot de passe.
- Étant donné un token expiré à la réouverture, quand l'app s'ouvre, alors redirection vers l'écran de connexion.
- Étant donné un token expirant en cours d'usage, quand une requête renvoie 401, alors modale « Session expirée — reconnectez-vous pour continuer » (« Se reconnecter »/« Annuler »).
- Étant donné « Se reconnecter », quand je m'authentifie, alors retour à l'écran où j'étais avant l'expiration.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-7, 5pts
---

---
**[US-7.03] Se déconnecter de l'application de façon sécurisée**

Conseiller terrain, je veux me déconnecter explicitement afin de protéger les données quand je passe mon téléphone ou en fin de journée.

**Critères d'acceptation**
- Étant donné une session connectée, quand je sélectionne « Se déconnecter », alors confirmation « Voulez-vous vraiment vous déconnecter ? » (Confirmer/Annuler).
- Étant donné la confirmation, quand validée, alors session invalidée serveur, token local supprimé, écran de connexion.
- Étant donné une déconnexion effectuée, quand j'appuie « Retour » système, alors je reste sur la connexion sans accès aux écrans protégés.

**Estimation** : 2 pts
**Priorité** : MUST
**Tags** : epic-7, 2pts
---

---
**[US-7.05] Limiter l'accès aux seules données du conseiller connecté**

Conseiller terrain, je veux ne voir que mes événements et données rattachées afin de ne pas accéder par erreur à ceux d'un autre conseiller.

**Critères d'acceptation**
- Étant donné l'authentification, quand je consulte « Mes événements du jour », alors seuls mes événements assignés s'affichent.
- Étant donné une tentative d'accès à un événement non attribué, quand la requête part, alors 403 + écran « Accès non autorisé » + « Retour à mes événements ».
- Étant donné la recherche (Epic 6), quand je tente d'ouvrir la gestion interne d'un événement non attribué, alors accès refusé « Vous n'êtes pas affecté à cet événement. ».

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-7, 3pts
---

---
**[US-7.06] Gérer la connectivité réseau — bannière hors ligne et resynchronisation**

Conseiller terrain, je veux voir immédiatement la perte de connexion et que les données se resynchronisent au retour afin de comprendre l'état et retrouver un affichage cohérent sans relancer chaque écran.

**Critères d'acceptation**
- Étant donné une connexion active, quand elle est coupée, alors bannière persistante « Hors ligne — les données peuvent être obsolètes » sur toutes les vues.
- Étant donné la bannière hors ligne, quand la connexion revient, alors bannière verte transitoire « Connexion rétablie » 3 s ; les données de l'écran actif sont rechargées automatiquement (≤ 3 s) avec indicateur subtil sans bloquer la navigation.
- Étant donné une connexion active, quand je fais un pull-to-refresh sur une liste, alors rechargement à la demande.
- Étant donné un hors ligne, quand je tente une action d'écriture, alors toast « Action impossible hors ligne » et action non exécutée.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-7, 5pts
---

---
**[US-7.07] Consulter les données mises en cache en mode hors ligne**

Conseiller terrain, je veux consulter inscrits et recruteurs sans connexion afin de continuer à opérer lors d'une coupure dans la salle.

**Critères d'acceptation**
- Étant donné des listes chargées en ligne, quand la connexion est perdue et que j'y navigue, alors données du dernier chargement + bannière + « Données au {heure} ».
- Étant donné un cache consulté hors ligne, quand la connexion revient, alors rafraîchissement automatique et mention de cache disparue.
- Étant donné un hors ligne, quand je tente une écriture, alors action bloquée (US-7.06) sans file d'attente locale.

**Estimation** : 3 pts
**Priorité** : COULD
**Tags** : epic-7, 5pts
---

---
**[US-7.09] Afficher le consentement RGPD au premier lancement**

Conseiller terrain, je veux être informé des données collectées et y consentir afin d' utiliser l'app en conformité et en connaissance de cause.

**Critères d'acceptation**
- Étant donné un premier lancement (ou consentement réinitialisé), quand l'écran de connexion s'affiche, alors notice RGPD (données, finalité, droits) avant tout accès.
- Étant donné la notice, quand je tape « J'accepte et je me connecte », alors consentement enregistré localement + connexion poursuivie.
- Étant donné un consentement déjà donné, quand je rouvre l'app, alors la notice n'est pas réaffichée.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-7, 3pts
---

---
**[US-7.10] Demander la permission caméra avant le premier scan QR**

Conseiller terrain, je veux que l'app m'explique le besoin caméra avant la demande système afin de comprendre l'usage et accorder la permission en connaissance de cause.

**Critères d'acceptation**
- Étant donné un premier accès au scan, quand l'écran s'ouvre, alors écran in-app « Nous avons besoin d'accéder à votre caméra… » + « Autoriser l'accès » avant la demande système.
- Étant donné « Autoriser l'accès », quand la permission est accordée, alors viseur ouvert directement aux accès suivants.
- Étant donné une permission déjà accordée, quand j'accède au scan, alors viseur immédiat sans écran explicatif.

**Estimation** : 2 pts
**Priorité** : MUST
**Tags** : epic-7, 2pts
---

---
**[US-7.11] Charger progressivement les listes longues d'inscrits et de recruteurs**

Conseiller terrain, je veux des listes rapides même avec de nombreux participants afin de rester efficace sans attendre.

**Critères d'acceptation**
- Étant donné > 50 inscrits, quand la liste s'ouvre, alors 20 premiers affichés immédiatement + skeleton pour la suite.
- Étant donné un défilement jusqu'en bas, quand le dernier item est atteint, alors 20 suivants chargés (infinite scroll).
- Étant donné tous les items chargés, quand j'atteins le bas, alors aucun indicateur superflu et compteur fidèle au total réel.

**Estimation** : 3 pts
**Priorité** : SHOULD
**Tags** : epic-7, 5pts
---

---
**[US-7.12] Gérer de façon transverse les erreurs API — toast global et écran de rechargement de liste**

Conseiller terrain, je veux un traitement cohérent des erreurs techniques (toast global et écran de liste avec réessai) afin de ne jamais rester bloqué sans comprendre, quel que soit l'écran.

**Critères d'acceptation**
- Étant donné une erreur inattendue (5xx/timeout non interceptée), quand la réponse arrive, alors toast « Une erreur est survenue. Réessayez ou contactez le support. » pendant 5 s sans bloquer la navigation.
- Étant donné un chargement de liste (événements, inscrits, recruteurs, résultats de recherche) qui échoue, quand j'ouvre/rafraîchis l'écran, alors état d'erreur dédié « Impossible de charger… » + bouton « Réessayer » relançant immédiatement avec indicateur visible.
- Étant donné une liste déjà chargée puis un rafraîchissement en échec, quand l'erreur survient, alors les dernières données restent visibles + bannière non bloquante « Données peut-être obsolètes — dernière mise à jour : [date heure] ».
- Étant donné un handler local spécifique (ex. US-5.11), quand l'erreur survient, alors il est prioritaire — un seul message par erreur, le toast global n'est pas déclenché.

**Estimation** : 3 pts
**Priorité** : MUST
**Tags** : epic-7, 5pts
---