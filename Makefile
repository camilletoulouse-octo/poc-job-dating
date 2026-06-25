# Makefile - ft-poc-parcours-candidat
# Commandes raccourcies pour le projet .NET (API + Client Blazor WebAssembly)

# ----- Variables -----
API_PROJECT         := ParcoursCandidatApi
CLIENT_PROJECT      := ParcoursCandidatClient
API_TESTS_PROJECT   := ParcoursCandidatApi.Tests
CLIENT_TESTS_PROJECT:= ParcoursCandidatClient.Tests
SOLUTION            := ft-poc-parcours-candidat.slnx
API_PORT            := 5197
CLIENT_PORT         := 5209
ADB                 := $(HOME)/Library/Android/sdk/platform-tools/adb

# ----- Aide -----
.PHONY: help
help: ## Affiche cette aide
	@echo "Commandes disponibles :"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) \
		| awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2}'

# ----- Lancement -----
.PHONY: run-api
run-api: ## Lance l'API (http://localhost:5197)
	dotnet run --project $(API_PROJECT)

.PHONY: run-client
run-client: ## Lance le client Blazor (http://localhost:5209)
	dotnet run --project $(CLIENT_PROJECT)

.PHONY: run
run: ## Lance l'API et le client en parallèle
	@echo "Démarrage de l'API et du Client en parallèle..."
	@trap 'kill 0' INT TERM EXIT; \
	dotnet run --project $(API_PROJECT) & \
	dotnet run --project $(CLIENT_PROJECT) & \
	wait

.PHONY: watch
watch: ## Lance l'API et le client en mode watch (hot reload)
	@echo "Démarrage de l'API et du Client en mode watch..."
	@trap 'kill 0' INT TERM EXIT; \
	dotnet watch --project $(API_PROJECT) & \
	dotnet watch --project $(CLIENT_PROJECT) & \
	wait

# ----- Build -----
.PHONY: build
build: ## Build toute la solution
	dotnet build $(SOLUTION)

.PHONY: restore
restore: ## Restaure les dépendances NuGet
	dotnet restore $(SOLUTION)

.PHONY: clean
clean: ## Nettoie les artefacts de build (bin/ et obj/)
	dotnet clean $(SOLUTION)

# ----- Tests -----
.PHONY: test
test: ## Lance tous les tests de la solution
	dotnet test $(SOLUTION)

.PHONY: test-api
test-api: ## Lance les tests d'intégration de l'API
	dotnet test $(API_TESTS_PROJECT)

.PHONY: test-client
test-client: ## Lance les tests unitaires + composant du client Blazor
	dotnet test $(CLIENT_TESTS_PROJECT)

# ----- Android -----
.PHONY: adb-reverse
adb-reverse: ## Configure le port forwarding ADB (émulateur → machine hôte) pour les ports 5209 et 5197
	@echo "Configuration du port forwarding ADB..."
	$(ADB) reverse tcp:$(CLIENT_PORT) tcp:$(CLIENT_PORT)
	$(ADB) reverse tcp:$(API_PORT) tcp:$(API_PORT)
	@echo "Port forwarding actif : localhost:$(CLIENT_PORT) et localhost:$(API_PORT) sur l'émulateur → machine hôte"

.PHONY: run-android
run-android: adb-reverse ## Configure le port forwarding ADB puis lance l'API et le client en parallèle
	@echo "Démarrage de l'API et du Client en parallèle..."
	@trap 'kill 0' INT TERM EXIT; \
	dotnet run --project $(API_PROJECT) & \
	dotnet run --project $(CLIENT_PROJECT) & \
	wait

# ----- Ports -----
.PHONY: kill-ports
kill-ports: ## Libère les ports utilisés par l'API (5197) et le Client (5209)
	@echo "Libération des ports $(API_PORT) et $(CLIENT_PORT)..."
	@for port in $(API_PORT) $(CLIENT_PORT); do \
		pids=$$(lsof -ti tcp:$$port); \
		if [ -n "$$pids" ]; then \
			echo "Port $$port utilisé par PID(s): $$pids - kill en cours..."; \
			kill -9 $$pids || true; \
		else \
			echo "Port $$port libre."; \
		fi; \
	done
