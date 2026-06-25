# Bonnes pratiques de tests

## Pour les tests
- L'intitulé des tests doit être en français
- Ils doivent être formaté en "Given", "When", "Then" sans commentaires supplémentaires, et un seul given / when / then par test
- Quand c'est possible, extraire au maximum les données dans des fixtures.
- Les fixtures doivent être dans le module de test
- Ajoute des tests unitaires pour la logique métier
- Ajoute des tests d'integration pour les API
- Ajoute des tests E2E pour le parcours utilisateur