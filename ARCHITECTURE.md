# Tasky Monorepo Architecture

## Overview

Tasky is organized as a monorepo containing multiple microservices, all sharing common libraries and development tooling. This document describes the architecture, structure, and rationale behind the monorepo organization.

## Why Monorepo?

### Advantages

1. **Unified Solution**: All services in single `Tasky.sln` file
2. **Unified Versioning**: All services share the same ABP Framework version
3. **Code Sharing**: Shared libraries are easily referenced without package management overhead
4. **Atomic Changes**: Related changes across services in single commits
5. **Consistent Tooling**: Single set of build tools, linters, and formatters
6. **Simplified CI/CD**: One pipeline for all services
7. **Better Developer Experience**: Single clone, unified build commands

### Trade-offs

- **Repository Size**: Larger codebase (mitigated with sparse checkout)
- **Build Time**: More code to build (mitigated with selective building)
- **Permissions**: Granular access control requires more setup

## Repository Structure

```
.
├── .github/
│   └── workflows/          # CI/CD pipelines
│       └── ci-cd.yml       # Main pipeline (build, test, migrate, docker)
│
├── .vscode/                # VS Code workspace settings
│   ├── extensions.json     # Recommended extensions
│   ├── launch.json         # Debug configurations (all services)
│   ├── settings.json       # Workspace settings
│   └── tasks.json          # Build tasks
│
├── scripts/                # Development automation
│   ├── dev-setup.sh        # One-command environment setup
│   └── init-databases.sql  # Database initialization
│
├── src/
│   ├── apps/               # Application hosts
│   │   ├── Tasky.AuthServer/  # OpenIddict authentication
│   │   └── Tasky.WebApp/   # Web application
│   │
│   ├── gateway/
│   │   └── Tasky.Gateway/  # YARP API gateway
│   │
│   ├── services/           # Domain microservices
│   │   ├── administration/ # Audit, Features, Permissions, Settings
│   │   │   ├── host/       # HTTP API host
│   │   │   ├── src/        # Domain, Application, EF Core, Contracts
│   │   │   └── test/       # Unit and integration tests
│   │   │
│   │   ├── identity/       # Users, Roles, Authentication
│   │   ├── projects/       # Core business domain
│   │   └── saas/           # Multi-tenancy, Subscriptions
│   │
│   └── shared/             # Cross-cutting concerns
│       ├── Tasky.DbMigrator/  # Database migration tool
│       ├── Tasky.Hosting.Shared/  # Common hosting setup
│       ├── Tasky.Microservice.Shared/  # Microservice base
│       └── Tasky.Shared/   # Common DTOs, constants
│
├── docker-compose.dev.yml  # Development infrastructure only
├── docker-compose.yml      # Full-stack deployment
├── Makefile                # CLI automation
├── package.json            # Workspace management (npm)
├── .nvmrc                  # Node version pinning
├── .editorconfig           # Code style enforcement
├── .gitattributes          # Git line ending handling
├── CONTRIBUTING.md         # Contribution guidelines
└── README.md               # Project overview
```

## Service Architecture

### Microservice Organization

Each service follows ABP's layered architecture:

```
Tasky.<ServiceName>/
├── host/
│   └── Tasky.<ServiceName>.HttpApi.Host/  # REST API host
│
├── src/
│   ├── Tasky.<ServiceName>.Domain/        # Domain entities, services
│   ├── Tasky.<ServiceName>.Domain.Shared/ # Domain constants
│   ├── Tasky.<ServiceName>.Application/   # Application services
│   ├── Tasky.<ServiceName>.Application.Contracts/  # DTOs, interfaces
│   ├── Tasky.<ServiceName>.EntityFrameworkCore/  # Data access
│   └── Tasky.<ServiceName>.HttpApi/       # API controllers
│
└── test/
    ├── Tasky.<ServiceName>.Domain.Tests/
    ├── Tasky.<ServiceName>.Application.Tests/
    └── Tasky.<ServiceName>.HttpApi.Tests/
```

### Service Boundaries

| Service | Responsibility | Database | Port |
|---------|---------------|----------|------|
| **SaaS** | Multi-tenancy, subscriptions, editions, billing | Tasky_SaaS | 7004 |
| **Administration** | Audit logs, features, permissions, settings | Tasky_Administration | 7002 |
| **Identity** | Users, roles, authentication, sessions | Tasky_Identity | 7003 |
| **Projects** | Core business domain (project management) | Tasky_Projects | 7005 |
| **Gateway** | API routing, authentication, rate limiting | - | 7001 |
| **Auth Server** | Token issuance (OpenIddict) | - | 7000 |

### Database Per Service

Each service owns its database schema:

- **Isolation**: Services cannot directly query others' databases
- **Autonomy**: Schema changes don't affect other services
- **Scalability**: Databases can scale independently
- **Technology**: Each service can use different database engines (all use PostgreSQL currently)

## Development Tooling

### Makefile Targets

```bash
make help           # Show all commands
make build          # Build all services
make test           # Run all tests
make migrate-all    # Run all migrations
make start-all      # Start all services
make clean          # Clean build artifacts
make restore        # Restore NuGet packages
make format         # Format code
make dev-setup      # Complete environment setup
```

### npm Scripts

```bash
npm run build:all       # Build all services
npm run test:all        # Run all tests
npm run migrate:all     # Run all migrations
npm run start:all       # Start all services
npm run clean           # Clean artifacts
npm run restore         # Restore packages
npm run format          # Format code
```

### Docker Compose

**Development (Infrastructure Only)**:
```bash
docker-compose -f docker-compose.dev.yml up -d
```
- PostgreSQL
- Redis
- RabbitMQ

**Production (Full Stack)**:
```bash
docker-compose up -d
```
- Infrastructure + All application services

### VS Code Integration

**Launch Configurations**:
- Individual service debugging
- "All Services" compound configuration

**Tasks**:
- Build (all/per-service)
- Test
- Clean
- Restore
- Format

**Extensions**:
- C# DevKit
- Docker
- GitLens
- Database tools
- EditorConfig

## Shared Libraries

### Tasky.Shared

Common DTOs, constants, and utilities used across all services.

**Usage**:
```xml
<ProjectReference Include="..\..\shared\Tasky.Shared\Tasky.Shared.csproj" />
```

### Tasky.Hosting.Shared

Common hosting configuration:
- Authentication setup
- Swagger configuration
- CORS policies
- Exception handling

### Tasky.Microservice.Shared

Microservice-specific utilities:
- Service discovery
- Health checks
- Logging configuration
- Distributed tracing

## Build and Deployment

### CI/CD Pipeline

GitHub Actions workflow (`.github/workflows/ci-cd.yml`):

1. **Build and Test**
   - Restore dependencies
   - Build solution
   - Run unit tests
   - Publish test results

2. **Code Quality**
   - Format verification
   - Linting (future)
   - Static analysis (future)

3. **Database Migrations**
   - Spin up test database
   - Run all migrations
   - Verify migration success

4. **Docker Images**
   - Build images for each service
   - Tag with branch/version
   - Push to container registry

### Versioning Strategy

- **Unified Version**: All services share version (e.g., 0.1.0)
- **Semantic Versioning**: MAJOR.MINOR.PATCH
- **Git Tags**: Version tags at monorepo root

### Deployment

**Docker Swarm / Kubernetes**:
```bash
# Tag: v0.1.0
docker tag tasky-saas:latest registry/tasky-saas:0.1.0
docker push registry/tasky-saas:0.1.0
```

**Helm Chart** (Future):
```bash
helm install tasky ./charts/tasky --set version=0.1.0
```

## Communication Patterns

### Synchronous (HTTP)

Services communicate via Gateway:
```
Client → Gateway → Service
```

### Asynchronous (Events)

Domain events via RabbitMQ:
```
Service A → RabbitMQ → Service B
```

**Example**:
```csharp
// SaaS Service publishes
await _distributedEventBus.PublishAsync(
    new SubscriptionCreatedEto { TenantId = subscription.TenantId }
);

// Projects Service subscribes
public class SubscriptionCreatedEventHandler 
    : IDistributedEventHandler<SubscriptionCreatedEto>
{
    public async Task HandleEventAsync(SubscriptionCreatedEto eventData)
    {
        // Initialize tenant-specific project data
    }
}
```

## Database Management

### Migrations

Each service manages its own migrations:

```bash
# Create migration
cd src/services/saas/src/Tasky.SaaS.EntityFrameworkCore
dotnet ef migrations add NewFeature \
    --startup-project ../../host/Tasky.SaaS.HttpApi.Host

# Apply migration
dotnet ef database update \
    --startup-project ../../host/Tasky.SaaS.HttpApi.Host
```

### Connection Strings

Centralized in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "SaaS": "Host=localhost;Port=5432;Database=Tasky_SaaS;...",
    "Administration": "Host=localhost;Port=5432;Database=Tasky_Administration;...",
    "Identity": "Host=localhost;Port=5432;Database=Tasky_Identity;...",
    "Projects": "Host=localhost;Port=5432;Database=Tasky_Projects;..."
  }
}
```

## Development Workflow

### 1. Initial Setup

```bash
# Clone repository
git clone <repo-url>
cd tasky

# One-command setup
./scripts/dev-setup.sh
```

### 2. Daily Development

```bash
# Start infrastructure
docker-compose -f docker-compose.dev.yml up -d

# Start services (in separate terminals or VS Code)
cd src/apps/Tasky.AuthServer && dotnet run
cd src/gateway/Tasky.Gateway && dotnet run
cd src/services/saas/host/Tasky.SaaS.HttpApi.Host && dotnet run
# ... etc
```

### 3. Making Changes

```bash
# Create feature branch
git checkout -b feature/new-subscription-logic

# Make changes in service
vim src/services/saas/src/Tasky.SaaS.Domain/Subscriptions/Subscription.cs

# Build and test
make build-saas
make test

# Create migration if needed
cd src/services/saas/src/Tasky.SaaS.EntityFrameworkCore
dotnet ef migrations add NewSubscriptionField \
    --startup-project ../../host/Tasky.SaaS.HttpApi.Host

# Apply migration
make migrate-saas

# Format code
make format

# Commit
git add .
git commit -m "feat(saas): add subscription renewal logic"
```

### 4. Testing

```bash
# Run specific service tests
dotnet test src/services/saas/test/Tasky.SaaS.Domain.Tests

# Run all tests
make test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Best Practices

### 1. Service Independence

- ✅ Each service has its own database
- ✅ No direct database access across services
- ✅ Use events for cross-service communication
- ❌ Don't share DbContext between services

### 2. Shared Code

- ✅ Share DTOs via `.Shared` projects
- ✅ Share hosting config via `Tasky.Hosting.Shared`
- ❌ Don't share domain entities
- ❌ Don't share EF Core models

### 3. Versioning

- ✅ Version APIs (e.g., `/api/v1/subscriptions`)
- ✅ Support multiple API versions during transition
- ❌ Don't break existing API contracts

### 4. Database Migrations

- ✅ Always test migrations locally first
- ✅ Keep migrations small and focused
- ✅ Write rollback scripts for production
- ❌ Don't modify existing migrations

### 5. Testing

- ✅ Write unit tests for domain logic
- ✅ Write integration tests for repositories
- ✅ Write API tests for endpoints
- ❌ Don't test framework code

## Monitoring and Observability

### Health Checks (Future)

```csharp
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

### Distributed Tracing (Future)

OpenTelemetry integration:
```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation());
```

### Logging

Structured logging with Serilog:
```csharp
Log.Information("Subscription {SubscriptionId} created for tenant {TenantId}",
    subscription.Id, subscription.TenantId);
```

## Troubleshooting

### Common Issues

**Port conflicts**:
```bash
lsof -i :7000  # Find process
kill -9 <PID>  # Kill process
```

**Database connection**:
```bash
docker ps  # Check PostgreSQL is running
docker logs tasky-postgres-dev  # View logs
```

**Migration errors**:
```bash
# Drop and recreate
docker-compose -f docker-compose.dev.yml down -v
docker-compose -f docker-compose.dev.yml up -d
make migrate-all
```

**Build errors**:
```bash
make clean
dotnet clean src/Tasky.sln
make restore
make build
```

## Future Enhancements

- [ ] **Turborepo/NX**: Advanced build caching
- [ ] **Health Checks**: Service health monitoring
- [ ] **OpenTelemetry**: Distributed tracing
- [ ] **API Versioning**: Explicit version management
- [ ] **Kubernetes Manifests**: K8s deployment
- [ ] **Helm Charts**: Package management
- [ ] **Service Mesh**: Istio/Linkerd integration
- [ ] **gRPC**: Inter-service communication
- [ ] **GraphQL**: Alternative API layer

## References

- [ABP Microservice Architecture](https://docs.abp.io/en/abp/latest/Microservice-Architecture)
- [.NET Microservices eBook](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/)
- [Monorepo Best Practices](https://monorepo.tools/)
- [Domain-Driven Design](https://docs.abp.io/en/abp/latest/Domain-Driven-Design)

---

**Last Updated**: December 2024  
**ABP Version**: 9.0.0  
**.NET Version**: 9.0
