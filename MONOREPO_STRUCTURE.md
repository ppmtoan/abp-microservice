# Tasky Monorepo Structure

## Overview

Tasky has been restructured as a true monorepo with a unified solution file (`Tasky.sln`) containing all microservices, shared libraries, and applications.

## Key Changes

### ✅ Unified Solution
- **Single solution file**: `src/Tasky.sln` contains all 61 projects
- **Removed**: Individual service solution files (`.Administration.sln`, `.IdentityService.sln`, etc.)
- **Benefit**: Simplified project management, atomic changes across services

### ✅ Removed .NET Aspire Integration
- Deleted `Tasky.AppHost` project
- Deleted `Tasky.ServiceDefaults` project
- Replaced Aspire packages with standard .NET packages in `Tasky.Hosting.Shared`
- Updated `DbMigrator` to use standard .NET APIs

### ✅ Removed Docker Integration
- Deleted all `Dockerfile` files
- Deleted `docker-compose.yml` and `docker-compose.dev.yml`
- Deleted service-specific docker-compose files
- **Focus**: Local development with natively installed infrastructure

### ✅ Updated Build Tooling
- **Makefile**: All targets reference unified solution
- **package.json**: Scripts use direct project paths instead of service solutions
- **dev-setup.sh**: Updated for local infrastructure setup
- **CI/CD**: Removed Docker image building, kept test infrastructure

## Project Structure

```
src/
├── Tasky.sln                    # ⭐ UNIFIED SOLUTION (61 projects)
│
├── apps/                         # Application hosts
│   ├── Tasky.AuthServer/        # OpenIddict authentication server
│   ├── Tasky.WebApp/            # Blazor WebAssembly app
│   └── angular/                 # Angular SPA
│
├── gateway/
│   └── Tasky.Gateway/           # YARP API gateway
│
├── services/                     # Microservices (no individual .sln files)
│   ├── administration/          # Audit, Features, Permissions, Settings
│   │   ├── host/                # API host
│   │   ├── src/                 # Domain, Application, EF Core, Contracts
│   │   └── test/                # Unit and integration tests
│   │
│   ├── identity/                # Users, Roles, Authentication
│   ├── projects/                # Core business domain
│   └── saas/                    # Multi-tenancy, Subscriptions
│
└── shared/                       # Cross-cutting concerns
    ├── Tasky.DbMigrator/        # Database migration tool
    ├── Tasky.Hosting.Shared/    # Common hosting setup (no Aspire)
    ├── Tasky.Microservice.Shared/
    └── Tasky.Shared/
```

## Development Workflow

### Prerequisites

Install infrastructure services locally:

```bash
# macOS (using Homebrew)
brew install postgresql@16 redis rabbitmq
brew services start postgresql@16
brew services start redis
brew services start rabbitmq

# Windows (using Chocolatey)
choco install postgresql redis rabbitmq
```

### Build Commands

```bash
# Build unified solution
make build
# or
npm run build:all
# or
dotnet build src/Tasky.sln

# Build specific service
make build-saas
make build-gateway
make build-authserver
```

### Running Migrations

```bash
# All services
make migrate-all

# Individual services
make migrate-saas
make migrate-admin
make migrate-identity
make migrate-projects
```

### Starting Services

```bash
# All services (requires tmux)
make start-all

# Individual services
cd src/apps/Tasky.AuthServer && dotnet run
cd src/gateway/Tasky.Gateway && dotnet run
cd src/services/saas/host/Tasky.SaaS.HttpApi.Host && dotnet run
```

## VS Code Integration

### Launch Configurations
- Individual service debugging
- "All Services" compound configuration for debugging all services simultaneously

### Tasks
- Build unified solution
- Build individual services
- Test, Clean, Restore, Format

### Extensions
Recommended extensions are listed in `.vscode/extensions.json`

## CI/CD Pipeline

GitHub Actions workflow (`.github/workflows/ci-cd.yml`):

1. ✅ **Build and Test**: Compile unified solution, run all tests
2. ✅ **Code Quality**: Format verification
3. ✅ **Database Migrations**: Test migrations with PostgreSQL service
4. ❌ **Docker Images**: Removed (no longer building containers)

## Benefits of This Structure

### 1. Simplified Project Management
- One solution to rule them all
- No confusion about which .sln file to open
- Single source of truth for dependencies

### 2. Faster Development
- Build entire system: `dotnet build src/Tasky.sln`
- IntelliSense works across all projects
- Easy refactoring across service boundaries

### 3. Atomic Commits
- Change shared library and all consumers in one commit
- No need to coordinate package versions
- Clear impact analysis in pull requests

### 4. Consistent Tooling
- One Makefile with all targets
- One package.json with unified scripts
- One CI/CD pipeline

### 5. Better Testing
- Run all tests: `dotnet test src/Tasky.sln`
- Integration tests across services easier to write
- Single test report

## Migration from Old Structure

### What Was Removed

**Solution Files**:
- `src/services/saas/Tasky.SaaS.sln` ❌
- `src/services/administration/Tasky.Administration.sln` ❌
- `src/services/identity/Tasky.IdentityService.sln` ❌
- `src/services/projects/Tasky.Projects.sln` ❌

**Aspire Projects**:
- `src/apps/Tasky.AppHost/` ❌
- `src/shared/Tasky.ServiceDefaults/` ❌

**Docker Files**:
- All `Dockerfile` files ❌
- All `docker-compose*.yml` files ❌

**Aspire Packages** (replaced with standard):
- `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL` → `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Aspire.RabbitMQ.Client` → `RabbitMQ.Client`
- `Aspire.StackExchange.Redis` → `StackExchange.Redis`
- `Aspire.Seq` → Removed

### What Changed

**Tasky.sln**: Now the single source of truth (was one of many)

**Makefile**: 
- Old: `make build-saas` built `services/saas/Tasky.SaaS.sln`
- New: `make build-saas` builds specific project from unified solution

**package.json**:
- Old: Referenced service-specific solution files
- New: References specific project paths

**CI/CD**:
- Old: Built Docker images for deployment
- New: Focuses on build, test, and migration validation

## Troubleshooting

### "Solution file not found"
- Always use `src/Tasky.sln`
- Service-specific .sln files have been removed

### "Cannot find Aspire references"
- Aspire has been completely removed
- Use standard .NET packages

### "Docker commands not working"
- Docker integration has been removed
- Install PostgreSQL, Redis, RabbitMQ natively

### Build Errors
```bash
# Clean and rebuild
make clean
make restore
make build
```

## Future Considerations

### What We Kept
- ✅ Microservice architecture (logical separation)
- ✅ Independent databases per service
- ✅ API Gateway pattern
- ✅ Domain-Driven Design patterns
- ✅ All ABP Framework features

### What We Changed
- 🔄 From multiple solutions → Single unified solution
- 🔄 From Docker-first → Native infrastructure
- 🔄 From Aspire → Standard .NET

### What We Can Add Later
- 🎯 Docker support (optional, for deployment)
- 🎯 Kubernetes manifests
- 🎯 Helm charts
- 🎯 Turborepo/NX for advanced build caching
- 🎯 .NET Aspire (if needed for orchestration)

## Summary

Tasky is now a clean, unified monorepo that:
- Uses a single solution file for all projects
- Supports local development without Docker
- Maintains microservice architecture principles
- Provides excellent developer experience with modern tooling
- Simplifies build, test, and deployment processes

The monorepo structure makes it easier to develop, test, and maintain the entire system while preserving the benefits of microservice architecture.
