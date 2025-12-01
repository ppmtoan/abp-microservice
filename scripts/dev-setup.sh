#!/bin/bash
set -e

echo "🚀 Tasky Development Environment Setup"
echo "======================================"

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check prerequisites
echo -e "\n${YELLOW}Checking prerequisites...${NC}"

if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found. Please install .NET 9 SDK."
    exit 1
fi
echo "✅ .NET SDK: $(dotnet --version)"

if ! command -v psql &> /dev/null; then
    echo "⚠️  PostgreSQL client not found. Install it for database verification."
else
    echo "✅ PostgreSQL client available"
fi

# Restore NuGet packages
echo -e "\n${YELLOW}Restoring NuGet packages...${NC}"
cd src
dotnet restore Tasky.sln
cd ..
echo "✅ NuGet packages restored"

# Build solution
echo -e "\n${YELLOW}Building solution...${NC}"
cd src
dotnet build Tasky.sln --no-restore
cd ..
echo "✅ Solution built successfully"

# Run database migrations
echo -e "\n${YELLOW}Database migrations...${NC}"
echo "⚠️  Please ensure PostgreSQL, Redis, and RabbitMQ are running locally"
echo "⚠️  Update connection strings in appsettings.json if needed"
echo ""
echo "To run migrations manually:"
echo "  cd src/services/saas/src/Tasky.SaaS.EntityFrameworkCore"
echo "  dotnet ef database update --startup-project ../../host/Tasky.SaaS.HttpApi.Host"
echo ""
echo "  cd src/services/administration/src/Tasky.Administration.EntityFrameworkCore"
echo "  dotnet ef database update --startup-project ../../host/Tasky.Administration.HttpApi.Host"
echo ""
echo "  cd src/services/identity/src/Tasky.IdentityService.EntityFrameworkCore"
echo "  dotnet ef database update --startup-project ../../host/Tasky.IdentityService.HttpApi.Host"
echo ""
echo "  cd src/services/projects/src/Tasky.Projects.EntityFrameworkCore"
echo "  dotnet ef database update --startup-project ../../host/Tasky.Projects.HttpApi.Host"

# Display service URLs
echo -e "\n${GREEN}======================================"
echo "✅ Development environment is ready!"
echo "======================================${NC}"
echo ""
echo "Infrastructure Requirements:"
echo "  📊 PostgreSQL:       localhost:5432 (please install and start manually)"
echo "  🔴 Redis:            localhost:6379 (please install and start manually)"
echo "  🐰 RabbitMQ:         localhost:5672 (please install and start manually)"
echo ""
echo "To start application services:"
echo "  make start-all"
echo ""
echo "Or start individual services:"
echo "  cd src/apps/Tasky.AuthServer && dotnet run"
echo "  cd src/gateway/Tasky.Gateway && dotnet run"
echo "  cd src/services/saas/host/Tasky.SaaS.HttpApi.Host && dotnet run"
echo "  cd src/services/administration/host/Tasky.Administration.HttpApi.Host && dotnet run"
echo "  cd src/services/identity/host/Tasky.IdentityService.HttpApi.Host && dotnet run"
echo "  cd src/services/projects/host/Tasky.Projects.HttpApi.Host && dotnet run"
echo ""
