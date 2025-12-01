#!/bin/bash

# Start all Tasky microservices for local development
# This script starts all 6 services in the background

echo "🚀 Starting Tasky Microservices..."

# Change to project root
cd "$(dirname "$0")"

# Build all projects first to avoid concurrent build issues
echo "🔨 Building all projects..."
dotnet build src/Tasky.sln --no-restore > /dev/null 2>&1

# Clean up old log files
rm -f /tmp/authserver.log /tmp/administration.log /tmp/identity.log /tmp/saas.log /tmp/projects.log /tmp/gateway.log

# Kill any existing processes on the ports
echo "🧹 Cleaning up existing processes..."
lsof -ti :7600,7001,7002,7003,7004,7500 2>/dev/null | xargs kill -9 2>/dev/null
sleep 2

# Start AuthServer (Port 7600)
echo "▶️  Starting AuthServer on port 7600..."
cd src/apps/Tasky.AuthServer
nohup dotnet run > /tmp/authserver.log 2>&1 &
AUTH_PID=$!
cd ../../..

# Start Administration Service (Port 7001)
echo "▶️  Starting Administration Service on port 7001..."
cd src/services/administration/host/Tasky.Administration.HttpApi.Host
nohup dotnet run > /tmp/administration.log 2>&1 &
ADMIN_PID=$!
cd ../../../../..

# Start Identity Service (Port 7002)
echo "▶️  Starting Identity Service on port 7002..."
cd src/services/identity/host/Tasky.IdentityService.HttpApi.Host
nohup dotnet run > /tmp/identity.log 2>&1 &
IDENTITY_PID=$!
cd ../../../../..

# Start SaaS Service (Port 7003)
echo "▶️  Starting SaaS Service on port 7003..."
cd src/services/saas/host/Tasky.SaaS.HttpApi.Host
nohup dotnet run > /tmp/saas.log 2>&1 &
SAAS_PID=$!
cd ../../../../..

# Start Projects Service (Port 7004)
echo "▶️  Starting Projects Service on port 7004..."
cd src/services/projects/host/Tasky.Projects.HttpApi.Host
nohup dotnet run > /tmp/projects.log 2>&1 &
PROJECTS_PID=$!
cd ../../../../..

# Start Gateway (Port 7500)
echo "▶️  Starting Gateway on port 7500..."
cd src/gateway/Tasky.Gateway
nohup dotnet run > /tmp/gateway.log 2>&1 &
GATEWAY_PID=$!
cd ../../..

echo ""
echo "⏳ Waiting for services to start (60 seconds)..."
sleep 60

# Check which services are running
echo ""
echo "📊 Service Status:"
echo "===================="

check_port() {
    local port=$1
    local name=$2
    if lsof -ti :$port > /dev/null 2>&1; then
        echo "✅ $name (port $port) - RUNNING"
    else
        echo "❌ $name (port $port) - FAILED"
    fi
}

check_port 7600 "AuthServer"
check_port 7001 "Administration"
check_port 7002 "Identity"
check_port 7003 "SaaS"
check_port 7004 "Projects"
check_port 7500 "Gateway"

echo ""
echo "📝 View logs with:"
echo "   tail -f /tmp/authserver.log"
echo "   tail -f /tmp/administration.log"
echo "   tail -f /tmp/identity.log"
echo "   tail -f /tmp/saas.log"
echo "   tail -f /tmp/projects.log"
echo "   tail -f /tmp/gateway.log"
echo ""
echo "🛑 Stop all services with:"
echo "   ./stop-all-services.sh"
echo ""
