# Ajout de fonctionnalité
- Je veux que tu n'ajoutes que les fonctionnalités que j'ai explicitement demandées
- Prends exemple sur le code existant pour garder une cohérence dans le style de code
- Tout nouveau service doit être mocké
- Ajoute un fichier json avec des données en dure pour cette feature

# Vérification du code
- Je ne veux pas que tu vérifies toi même le code que tu as écrit. Je préfère que tu lances une analyse Sonar par exemple

# Bonnes pratiques de code
- Utilise des noms avec des majuscules pour les enums (ex: UiStatus.LOADING)
- Utilise toujours des doubles guillemets pour les strings
- Créer des interfaces pour les repositories
- Pas de code mort (méthodes ou variables non utilisées)

### À ne pas faire
- ❌ Imports relatifs (`../../../`)
- ❌ Classes trop grandes (>200 lignes)
- ❌ Ne pas lancer de commande pour lancer l'app après les modifications
- ❌ Ne pas modifier les fichiers générés
