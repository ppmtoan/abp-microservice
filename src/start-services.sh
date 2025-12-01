#!/bin/bash

# Tasky Microservices Startup Script
# This script starts all services in separate terminal windows

echo "========================================="
echo "Starting Tasky Microservices"
echo "========================================="
echo ""

# Function to start a service in a new terminal
start_service() {
    local service_name=$1
    local service_path=$2
    local port=$3
    
    echo "Starting $service_name on port $port..."
    
    osascript -e "tell application \"Terminal\"
        do script \"cd '$PWD/$service_path' && echo 'Starting $service_name...' && dotnet run\"
    end tell" > /dev/null 2>&1
}

# Start services
start_service "AuthServer" "apps/Tasky.AuthServer" "7600"
sleep 3

start_service "Administration Service" "services/administration/host/Tasky.Administration.HttpApi.Host" "7001"
sleep 2

start_service "Identity Service" "services/identity/host/Tasky.IdentityService.HttpApi.Host" "7002"
sleep 2

start_service "SaaS Service" "services/saas/host/Tasky.SaaS.HttpApi.Host" "7003"
sleep 2

start_service "Projects Service" "services/projects/host/Tasky.Projects.HttpApi.Host" "7004"
sleep 2

start_service "Gateway" "gateway/Tasky.Gateway" "7500"
sleep 2

echo ""
echo "========================================="
echo "All services are starting!"
echo "========================================="
echo ""
echo "Services will be available at:"
echo "  - AuthServer:      https://localhost:7600"
echo "  - Administration:  https://localhost:7001"
echo "  - Identity:        https://localhost:7002"
echo "  - SaaS:            https://localhost:7003"
echo "  - Projects:        https://localhost:7004"
echo "  - Gateway:         https://localhost:7500"
echo ""
echo "To start Angular app:"
echo "  cd apps/angular && npm start"
echo ""
