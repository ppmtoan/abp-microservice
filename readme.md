# Tasky - ABP Microservice Monorepo

A modern microservice architecture built with ABP Framework, implementing Domain-Driven Design (DDD) principles, CQRS patterns, and industry best practices.

## 🏗️ Architecture Overview

This is a monorepo-based microservice solution with the following services:

- **SaaS Service**: Multi-tenancy management with subscriptions, editions, and billing
- **Identity Service**: Authentication, authorization, and user management
- **Administration Service**: Audit logging, feature management, permissions, and settings
- **Projects Service**: Core business domain for project management
- **Gateway**: YARP-based API gateway with authentication and routing
- **Auth Server**: OpenIddict-based authentication server

### Technology Stack

- **Framework**: ABP Framework 9.0.0
- **Runtime**: .NET 9.0
- **Database**: PostgreSQL 16 with EF Core 9.0.0
- **Cache**: Redis 7
- **Message Broker**: RabbitMQ 3
- **API Gateway**: YARP (Yet Another Reverse Proxy)
- **Authentication**: OpenIddict

## 🚀 Quick Start

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL 16](https://www.postgresql.org/download/)
- [Redis 7](https://redis.io/download)
- [RabbitMQ 3](https://www.rabbitmq.com/download.html)
- [Node.js 20.x](https://nodejs.org/) (optional, for npm scripts)

### Setup Steps

#### 1. Install Infrastructure

Install and start PostgreSQL, Redis, and RabbitMQ on your local machine.

**macOS (using Homebrew):**
```bash
brew install postgresql@16 redis rabbitmq
brew services start postgresql@16
brew services start redis
brew services start rabbitmq
```

**Windows (using Chocolatey):**
```bash
choco install postgresql redis rabbitmq
```

#### 2. Restore and Build

```bash
make restore
make build
```

#### 3. Run Migrations

```bash
make migrate-all
```

#### 4. Start Services

```bash
make start-all
```

Or use the automated setup script:
```bash
./scripts/dev-setup.sh
```

## 📁 Project Structure

```
├── .github/workflows/          # CI/CD pipelines
├── scripts/                    # Development scripts
│   └── dev-setup.sh           # Environment setup
├── src/                        # Unified monorepo source
│   ├── apps/                  # Application hosts
│   │   ├── Tasky.AuthServer/  # Authentication server
│   │   └── Tasky.WebApp/      # Web application
│   ├── gateway/
│   │   └── Tasky.Gateway/     # API gateway with YARP
│   ├── services/              # Microservices
│   │   ├── administration/    # Administration service
│   │   ├── identity/          # Identity service
│   │   ├── projects/          # Projects service
│   │   └── saas/              # SaaS service
│   └── shared/                # Shared libraries
│       ├── Tasky.DbMigrator/
│       ├── Tasky.Hosting.Shared/
│       ├── Tasky.Microservice.Shared/
│       ├── Tasky.Microservice.Shared/
│       └── Tasky.Shared/
├── Makefile                   # Build automation
├── package.json               # Workspace management
├── .editorconfig              # Code style
└── .nvmrc                     # Node version

```

## 🛠️ Development Workflow

### Using Makefile

```bash
# Show all available commands
make help

# Build all services
make build

# Build specific service
make build-saas
make build-administration
make build-identity
make build-projects

# Run tests
make test

# Clean build artifacts
make clean

# Restore dependencies
make restore

# Run migrations
make migrate-all
make migrate-saas
make migrate-administration
make migrate-identity
make migrate-projects

# Start all services
make start-all

# Format code
make format

# Development setup
make dev-setup
```

### Using npm Scripts

```bash
# Build all services
npm run build:all

# Build specific service
npm run build:saas

# Run tests
npm run test:all

# Migrate databases
npm run migrate:all

# Start all services
npm run start:all

# Format code
npm run format
```

## 🏗️ Architecture Patterns

### Domain-Driven Design (DDD)

The solution implements DDD tactical patterns:

#### Value Objects
- **Money**: Currency amount with currency code
- **DateRange**: Time period with validation (StartDate, EndDate)
- **FeatureLimits**: SaaS edition feature constraints
- **InvoiceNumber**: Strongly-typed invoice identifier

#### Domain Services
- Encapsulate complex business logic
- Coordinate between multiple aggregates

#### Specifications
- Reusable query specifications
- Business rule definitions

#### Domain Events
- Inter-service communication
- Eventual consistency

### Input Validation

Custom validation attributes for business rules:

```csharp
[GreaterThan(nameof(StartDate))]
public DateTime EndDate { get; set; }

[ValidFeatureLimits]
public Dictionary<string, object> FeatureLimits { get; set; }

[ValidDateRange(MinDaysFromNow = 1)]
public DateRangeDto SubscriptionPeriod { get; set; }
```

## 🗄️ Database Schema

### Services and Databases

| Service | Database | Tables |
|---------|----------|---------|
| SaaS | Tasky_SaaS | 6 tables (Editions, Subscriptions, Invoices, Tenants, TenantConnectionStrings, BlobContainers) |
| Administration | Tasky_Administration | Audit logs, Features, Permissions, Settings |
| Identity | Tasky_Identity | Users, Roles, Claims, Sessions, OpenIddict |
| Projects | Tasky_Projects | Project entities |

### Connection Strings

```json
{
  "ConnectionStrings": {
    "SaaS": "Host=localhost;Port=5432;Database=Tasky_SaaS;Username=postgres;Password=postgres",
    "Administration": "Host=localhost;Port=5432;Database=Tasky_Administration;Username=postgres;Password=postgres",
    "Identity": "Host=localhost;Port=5432;Database=Tasky_Identity;Username=postgres;Password=postgres",
    "Projects": "Host=localhost;Port=5432;Database=Tasky_Projects;Username=postgres;Password=postgres"
  }
}
```

## 🔗 Service URLs

### Development Environment

| Service | URL | Description |
|---------|-----|-------------|
| Auth Server | http://localhost:7000 | OpenIddict authentication |
| Gateway | http://localhost:7001 | API gateway |
| Administration API | http://localhost:7002 | Administration service |
| Identity API | http://localhost:7003 | Identity service |
| SaaS API | http://localhost:7004 | SaaS service |
| Projects API | http://localhost:7005 | Projects service |
| PostgreSQL | localhost:5432 | Database server |
| Redis | localhost:6379 | Cache server |
| RabbitMQ | localhost:5672 | Message broker |
| RabbitMQ UI | http://localhost:15672 | Management UI (admin/admin) |

## 🧪 Testing

```bash
# Run all tests
make test
# or
npm run test:all

# Run tests with dotnet
dotnet test src/Tasky.sln
```

## 📝 Code Quality

### EditorConfig

The project includes `.editorconfig` for consistent coding style across teams and IDEs.

### Code Formatting

```bash
# Format all code
make format
# or
npm run format

# Check format without changes
dotnet format src/Tasky.sln --verify-no-changes
```

## 🔄 CI/CD

GitHub Actions workflow includes:

1. **Build and Test**: Compile solution and run unit tests
2. **Code Quality**: Format verification
3. **Database Migrations**: Test migrations in isolated environment
4. **Docker Images**: Build and push images to container registry

### Required Secrets

- `CONTAINER_REGISTRY`: Registry URL
- `REGISTRY_USERNAME`: Registry username
- `REGISTRY_PASSWORD`: Registry password

## 📚 Documentation

- [ABP Framework Documentation](https://docs.abp.io/en/abp/latest/)
- [.NET Microservices Architecture](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/)
- [Domain-Driven Design](https://docs.abp.io/en/abp/latest/Domain-Driven-Design)
- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [OpenIddict Documentation](https://documentation.openiddict.com/)

## 🎯 Features

- ✅ **Microservice Architecture**: Independent, scalable services
- ✅ **Unified Monorepo**: Single solution file for all services
- ✅ **Domain-Driven Design**: Value Objects, Domain Services, Specifications
- ✅ **Input Validation Layer**: Custom validation attributes
- ✅ **Multi-tenancy**: Full SaaS support with subscriptions
- ✅ **API Gateway**: YARP-based routing and authentication
- ✅ **Authentication**: OpenIddict with JWT tokens
- ✅ **Database Migrations**: EF Core with PostgreSQL
- ✅ **CI/CD Pipeline**: GitHub Actions with automated tests
- ✅ **Monorepo Tooling**: Makefile, npm scripts, development scripts
- ✅ **Code Quality**: EditorConfig, format checking

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

### Development Guidelines

1. Follow the existing code style (enforced by `.editorconfig`)
2. Write unit tests for new features
3. Update documentation as needed
4. Ensure all tests pass before submitting PR
5. Use conventional commit messages

## 📄 License

This project is licensed under the [MIT License](LICENSE.md).

## 🙏 Acknowledgments

- [ABP Framework](https://abp.io/)
- [eShopOnAbp](https://github.com/abpframework/eShopOnAbp)
- Original template by [Anto Subash](https://antosubash.com)

## 📋 Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history and updates.

## 🐛 Troubleshooting

### Port Already in Use

```bash
# Find process using port
lsof -i :7000

# Kill process
kill -9 <PID>
```

### Database Connection Issues

```bash
# Check PostgreSQL is running
psql -U postgres -h localhost -c "SELECT version();"

# Check PostgreSQL service status (macOS)
brew services list | grep postgresql

# Restart PostgreSQL
brew services restart postgresql@16
```

### Migration Errors

```bash
# Drop and recreate databases manually
psql -U postgres -h localhost
DROP DATABASE "Tasky_SaaS";
DROP DATABASE "Tasky_Administration";
DROP DATABASE "Tasky_Identity";
DROP DATABASE "Tasky_Projects";

# Create databases
CREATE DATABASE "Tasky_SaaS";
CREATE DATABASE "Tasky_Administration";
CREATE DATABASE "Tasky_Identity";
CREATE DATABASE "Tasky_Projects";

# Run migrations
make migrate-all
```

### Build Errors

```bash
# Clean and rebuild
make clean
dotnet clean src/Tasky.sln
make restore
make build
```

## 📞 Support

- Create an [Issue](https://github.com/yourusername/tasky/issues) for bug reports
- Start a [Discussion](https://github.com/yourusername/tasky/discussions) for questions
- Check [Documentation](docs/) for guides

## 🗺️ Roadmap

- [ ] Health checks for all services
- [ ] Distributed tracing with OpenTelemetry
- [ ] API versioning
- [ ] GraphQL support
- [ ] gRPC inter-service communication
- [ ] Kubernetes deployment manifests
- [ ] Helm charts
- [ ] Service mesh (Istio/Linkerd)
- [ ] Blazor WebAssembly UI
- [ ] Angular frontend improvements

## 📊 Project Status

| Component | Status |
|-----------|--------|
| SaaS Service | ✅ Stable |
| Administration Service | ✅ Stable |
| Identity Service | ✅ Stable |
| Projects Service | ✅ Stable |
| API Gateway | ✅ Stable |
| Auth Server | ✅ Stable |
| Docker Support | ✅ Complete |
| CI/CD Pipeline | ✅ Complete |
| Documentation | ✅ Complete |
| Unit Tests | 🚧 In Progress |
| Integration Tests | ⏳ Planned |
| E2E Tests | ⏳ Planned |

---

**Built with ❤️ using ABP Framework and .NET 9**
