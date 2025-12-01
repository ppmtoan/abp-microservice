.PHONY: help build test clean restore migrate-all start-all format check dev-setup

help: ## Show this help message
	@echo 'Usage: make [target]'
	@echo ''
	@echo 'Available targets:'
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## / {printf "  %-20s %s\n", $$1, $$2}' $(MAKEFILE_LIST)

build: ## Build all services from unified solution
	@echo "Building unified solution..."
	@dotnet build src/Tasky.sln

build-saas: ## Build SaaS service
	@echo "Building SaaS service..."
	@dotnet build src/services/saas/host/Tasky.SaaS.HttpApi.Host/Tasky.SaaS.HttpApi.Host.csproj

build-admin: ## Build Administration service
	@echo "Building Administration service..."
	@dotnet build src/services/administration/host/Tasky.Administration.HttpApi.Host/Tasky.Administration.HttpApi.Host.csproj

build-identity: ## Build Identity service
	@echo "Building Identity service..."
	@dotnet build src/services/identity/host/Tasky.IdentityService.HttpApi.Host/Tasky.IdentityService.HttpApi.Host.csproj

build-projects: ## Build Projects service
	@echo "Building Projects service..."
	@dotnet build src/services/projects/host/Tasky.Projects.HttpApi.Host/Tasky.Projects.HttpApi.Host.csproj

build-gateway: ## Build API Gateway
	@echo "Building Gateway..."
	@dotnet build src/gateway/Tasky.Gateway/Tasky.Gateway.csproj

build-authserver: ## Build Auth Server
	@echo "Building Auth Server..."
	@dotnet build src/apps/Tasky.AuthServer/Tasky.AuthServer.csproj

test: ## Run all tests from unified solution
	@echo "Running all tests..."
	@dotnet test src/Tasky.sln

clean: ## Clean build artifacts
	@echo "Cleaning..."
	@dotnet clean src/Tasky.sln
	@find src -name 'bin' -o -name 'obj' | xargs rm -rf

restore: ## Restore NuGet packages for unified solution
	@echo "Restoring packages..."
	@dotnet restore src/Tasky.sln

migrate-saas: ## Run SaaS database migrations
	@echo "Running SaaS migrations..."
	@cd src/services/saas/src/Tasky.SaaS.EntityFrameworkCore && \
		dotnet ef database update --startup-project ../../host/Tasky.SaaS.HttpApi.Host

migrate-admin: ## Run Administration database migrations
	@echo "Running Administration migrations..."
	@cd src/services/administration/src/Tasky.Administration.EntityFrameworkCore && \
		dotnet ef database update --startup-project ../../host/Tasky.Administration.HttpApi.Host

migrate-identity: ## Run Identity database migrations
	@echo "Running Identity migrations..."
	@cd src/services/identity/src/Tasky.IdentityService.EntityFrameworkCore && \
		dotnet ef database update --startup-project ../../host/Tasky.IdentityService.HttpApi.Host

migrate-projects: ## Run Projects database migrations
	@echo "Running Projects migrations..."
	@cd src/services/projects/src/Tasky.Projects.EntityFrameworkCore && \
		dotnet ef database update --startup-project ../../host/Tasky.Projects.HttpApi.Host

migrate-all: migrate-admin migrate-identity migrate-projects migrate-saas ## Run all database migrations

start-all: ## Start all services (requires tmux or multiple terminals)
	@echo "Starting all services..."
	@echo "Note: This requires PostgreSQL, Redis, and RabbitMQ running locally"
	@bash src/start-services.sh

format: ## Format code in unified solution
	@echo "Formatting code..."
	@dotnet format src/Tasky.sln

check: ## Check code compilation
	@echo "Checking compilation..."
	@dotnet build src/Tasky.sln --no-restore

dev-setup: restore build ## Setup development environment
	@echo ""
	@echo "✅ Development environment ready!"
	@echo ""
	@echo "Next steps:"
	@echo "  1. Ensure PostgreSQL is running on localhost:5432"
	@echo "  2. Ensure Redis is running on localhost:6379"
	@echo "  3. Ensure RabbitMQ is running on localhost:5672"
	@echo "  4. Run migrations: make migrate-all"
	@echo "  5. Start services: make start-all"

.DEFAULT_GOAL := help
