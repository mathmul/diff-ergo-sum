# ------------------------------------------------------------------------------
#
# DiffErgoSum Makefile
#
# > Makefile to manage DiffErgoSum development, testing, and Docker workflows
#
# ------------------------------------------------------------------------------


# ---- Environment Loading -----------------------------------------------------

include .env
export $(shell sed 's/=.*//' .env)

ifneq (,$(wildcard .env.local))
include .env.local
export $(shell sed 's/=.*//' .env.local)
endif


# ---- Makefile Settings -------------------------------------------------------

.PHONY: help up up-build build down logs test lint lint-fix bash ps refresh refresh-full
.DEFAULT_GOAL := help

# Default profile (can override with `make PROFILE=dbonly up`)
PROFILE ?= full


# ---- Help --------------------------------------------------------------------

help:  ## Show this help message
	@echo ""
	@echo "📘 DiffErgoSum Makefile — Available Commands"
	@echo ""
	@grep -h -E '^[a-zA-Z0-9_.-]+:.*?## .*$$' $(MAKEFILE_LIST) | \
        awk 'BEGIN {FS = ":.*?## "}; \
        {printf "  \033[36m%-25s\033[0m %s\n", $$1, $$2}'
	@echo ""
	@echo "💡 Tip: run \033[1mmake <command>\033[0m to execute one."
	@echo ""

# ---- Local .NET development (runs outside Docker, except postgres) -----------

serve-inmemory: ## Serve API with in-memory DB
	@echo "#\n# Serving API with in-memory DB...\n#"
	DB_DRIVER=inmemory dotnet run --project DiffErgoSum

test-inmemory: ## Run tests using in-memory DB
	@echo "#\n# Running tests using in-memory DB...\n#"
	DB_DRIVER=inmemory dotnet test

serve-sqlite: ## Serve API with SQLite
	@echo "#\n# Serving API with SQLite...\n#"
	DB_DRIVER=sqlite dotnet run --project DiffErgoSum

test-sqlite: ## Run tests using SQLite
	@echo "#\n# Running tests using SQLite...\n#"
	DB_DRIVER=sqlite dotnet test

serve-postgres: ## Serve API with Postgres (Dockerized)
	@echo "#\n# Serving API with Postgres (Dockerized)...\n#"
	docker compose --profile dbonly up -d; \
	DB_DRIVER=postgres dotnet run --project DiffErgoSum

test-postgres: ## Run tests using Postgres (Dockerized)
	@echo "#\n# Running tests using Postgres (Dockerized)...\n#"
	@set -a; if [ -f .env.test ]; then source .env.test; fi; set +a; \
	docker compose --profile dbonly up -d; \
	DB_DRIVER=postgres dotnet test

# ---- Dockerized development and testing --------------------------------------

up: ## Start Docker stack
	@echo "#\n# Starting Docker stack (profile: $(PROFILE))...\n#"
	docker compose --profile=$(PROFILE) up -d

up-build: ## Build and start Docker stack
	@echo "#\n# Building and starting Docker stack...\n#"
	docker compose --profile=$(PROFILE) up --build

build: ## Rebuild API image (no cache)
	@echo "#\n# Rebuilding API image (no cache)...\n#"
	docker compose --profile=$(PROFILE) build --no-cache api

down: ## Stop containers
	@echo "#\n# Stopping containers (profile: $(PROFILE))...\n#"
	docker compose --profile=$(PROFILE) down

logs: ## Tail logs (Ctrl+C to exit)
	@echo "#\n# Tailing logs (Ctrl+C to exit)...\n#"
	docker compose --profile=$(PROFILE) logs -f --tail=100

bash: ## Open API container shell
	@echo "#\n# Opening API container shell...\n#"
	docker compose exec api bash

list: ## List active containers
	@echo "#\n# Listing active containers...\n#"
	docker compose ps

# ---- Maintenance and Utilities --------------------------------------

test: ## Run test suite in dev container
	@echo "#\n# Running test suite in dev container...\n#"
	docker compose --profile=dev down --remove-orphans || true
	docker network prune -f
	docker compose --profile=dev run --rm api-dev dotnet restore
	docker compose --profile=dev run --rm -e DB_DRIVER=postgres api-dev dotnet test

lint: ## Check code style
	@echo "#\n# Formatting (code style check)...\n#"
	docker compose --profile=dev run --rm api-dev dotnet format --verify-no-changes

lint-fix: ## Fix code style
	@echo "#\n# Formatting (code style check)...\n#"
	docker compose --profile=dev run --rm api-dev dotnet format

refresh: ## Rebuild and restart stack (no cache)
	@echo "#\n# Rebuilding and restarting stack (no cache, profile: $(PROFILE))...\n#"
	docker compose --profile $(PROFILE) up -d --build --no-deps --force-recreate

refresh-full: ## Rebuild stack from scratch (clean volumes)
	@echo "#\n# Rebuilding stack from scratch (clean volumes)...\n#"
	docker compose --profile $(PROFILE) down -v || true
	docker compose --profile $(PROFILE) up -d --build --force-recreate

refresh-db: ## Reset Postgres database volume
	@echo "#\n# Resetting Postgres database volume...\n#"
	docker compose --profile=dbonly down -v
	docker compose --profile=dbonly up -d
