# Tasky Microservices - Local Development Setup

## Prerequisites

✅ **Installed and Verified:**
- .NET SDK 9.0.308
- Node.js v25.2.1
- npm 11.6.2
- PostgreSQL 14.20 (Homebrew)
- Git 2.50.1

## Database Setup

### 1. Ensure PostgreSQL is Running
```bash
# Check status
brew services list | grep postgresql

# Start if not running
brew services start postgresql@14

# Test connection
psql -h localhost -U postgres -c "SELECT version();"
```

### 2. Run Database Migrations
```bash
cd src
chmod +x run-migrations.sh
./run-migrations.sh
```

This script will:
- Create 5 databases: `Tasky_Administration`, `Tasky_Identity`, `Tasky_Projects`, `Tasky_SaaS`, `Tasky_Gateway`
- Apply all EF Core migrations
- Seed initial data (admin user, roles, permissions, OpenIddict clients)

## Running the Application

### Option 1: Start All Services (Recommended for development)
```bash
cd src
chmod +x start-services.sh
./start-services.sh
```

This will open 6 terminal windows, each running a service:
- AuthServer (https://localhost:7600)
- Administration API (https://localhost:7001)
- Identity API (https://localhost:7002)
- SaaS API (https://localhost:7003)
- Projects API (https://localhost:7004)
- API Gateway (https://localhost:7500)

### Option 2: Start Services Manually

**Terminal 1 - AuthServer:**
```bash
cd src/apps/Tasky.AuthServer
dotnet run
```

**Terminal 2 - Administration Service:**
```bash
cd src/services/administration/host/Tasky.Administration.HttpApi.Host
dotnet run
```

**Terminal 3 - Identity Service:**
```bash
cd src/services/identity/host/Tasky.IdentityService.HttpApi.Host
dotnet run
```

**Terminal 4 - SaaS Service:**
```bash
cd src/services/saas/host/Tasky.SaaS.HttpApi.Host
dotnet run
```

**Terminal 5 - Projects Service:**
```bash
cd src/services/projects/host/Tasky.Projects.HttpApi.Host
dotnet run
```

**Terminal 6 - Gateway:**
```bash
cd src/gateway/Tasky.Gateway
dotnet run
```

### Start Angular Application
**Terminal 7 - Angular:**
```bash
cd src/apps/angular
npm start
# or
ng serve
```

Access the app at: **http://localhost:4200**

## Connection Strings

All services are configured to use local PostgreSQL:

```json
{
  "ConnectionStrings": {
    "AdministrationService": "Host=localhost;Port=5432;Database=Tasky_Administration;Username=postgres;Password=postgres;",
    "IdentityService": "Host=localhost;Port=5432;Database=Tasky_Identity;Username=postgres;Password=postgres;",
    "ProjectsService": "Host=localhost;Port=5432;Database=Tasky_Projects;Username=postgres;Password=postgres;",
    "SaaSService": "Host=localhost;Port=5432;Database=Tasky_SaaS;Username=postgres;Password=postgres;"
  }
}
```

To use a different password, update `appsettings.json` in each service directory.

## Default Credentials

After running migrations, you can login with:

**Username:** `admin`  
**Password:** `1q2w3E*`

## Service Endpoints

### Backend Services:
- **AuthServer:** https://localhost:7600
- **Administration API:** https://localhost:7001/swagger
- **Identity API:** https://localhost:7002/swagger
- **SaaS API:** https://localhost:7003/swagger
- **Projects API:** https://localhost:7004/swagger
- **Gateway:** https://localhost:7500

### Frontend:
- **Angular App:** http://localhost:4200

## SaaS Features Available

The following SaaS multi-tenancy features are fully implemented:

### Backend APIs:
1. **Edition Management** - `/api/saas/editions`
2. **Subscription Management** - `/api/saas/subscriptions`
3. **Invoice Management** - `/api/saas/invoices`
4. **Host Admin Dashboard** - `/api/saas/host-admin`
5. **Tenant Admin Dashboard** - `/api/saas/tenant-admin`
6. **Tenant Provisioning** - `/api/saas/tenant-provisioning`

### Frontend UI:
Navigate to: **http://localhost:4200/saas**

1. **Editions** - Create and manage SaaS plans with pricing
2. **Subscriptions** - View and manage tenant subscriptions
3. **Invoices** - Track and manage billing invoices
4. **Host Dashboard** - Metrics and tenant management
5. **Tenant Dashboard** - View current subscription and invoices
6. **Tenant Provisioning** - 2-step wizard for new tenant creation

## Troubleshooting

### PostgreSQL Connection Issues
```bash
# Check if PostgreSQL is running
brew services list

# Restart PostgreSQL
brew services restart postgresql@14

# Check port 5432 is available
lsof -i :5432
```

### Service Won't Start
```bash
# Check if port is already in use
lsof -i :7600  # Replace with your service port

# Kill process if needed
kill -9 <PID>
```

### Database Migration Errors
```bash
# Drop and recreate databases
psql -h localhost -U postgres -c "DROP DATABASE IF EXISTS Tasky_Administration;"
psql -h localhost -U postgres -c "CREATE DATABASE Tasky_Administration;"

# Then rerun migrations
cd src/shared/Tasky.DbMigrator
dotnet run
```

### Build Errors
```bash
# Clean and rebuild
cd src
dotnet clean
dotnet build
```

### Angular Errors
```bash
cd src/apps/angular

# Clear node_modules and reinstall
rm -rf node_modules package-lock.json
npm install

# Clear Angular cache
npx ng cache clean
```

## Development Workflow

1. **Start PostgreSQL** (if not running)
2. **Run Migrations** (first time only or when schema changes)
3. **Start Backend Services** (all 6 services)
4. **Start Angular App**
5. **Navigate to http://localhost:4200**
6. **Login with admin credentials**
7. **Access SaaS features at /saas route**

## Project Structure

```
src/
├── apps/
│   ├── angular/              # Angular frontend
│   ├── Tasky.AuthServer/     # OpenIddict authentication server
│   └── Tasky.WebApp/         # Blazor app (optional)
├── gateway/
│   └── Tasky.Gateway/        # API Gateway (Ocelot)
├── services/
│   ├── administration/       # Administration microservice
│   ├── identity/             # Identity microservice
│   ├── projects/             # Projects microservice
│   └── saas/                 # SaaS microservice (NEW)
├── shared/
│   └── Tasky.DbMigrator/     # Database migration tool
└── scripts/
    ├── run-migrations.sh     # Database setup script
    └── start-services.sh     # Service startup script
```

## Next Steps

1. ✅ Remove Aspire/Docker dependencies - **DONE**
2. ✅ Configure local PostgreSQL - **DONE**
3. ✅ Add connection strings - **DONE**
4. ✅ Verify builds - **DONE**
5. ✅ Run migrations - **DONE**
6. 🔄 Start services
7. 🔄 Test SaaS features

Happy coding! 🚀
