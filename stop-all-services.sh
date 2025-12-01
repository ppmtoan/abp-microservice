#!/bin/bash

# Stop all Tasky microservices

echo "🛑 Stopping all Tasky microservices..."

# Kill processes by port
lsof -ti :7600,7001,7002,7003,7004,7500 2>/dev/null | xargs kill -9 2>/dev/null

# Also kill by process name
pkill -f "dotnet.*Tasky\.(AuthServer|Administration|Identity|SaaS|Projects|Gateway)" 2>/dev/null

sleep 2

# Verify all services stopped
if lsof -ti :7600,7001,7002,7003,7004,7500 > /dev/null 2>&1; then
    echo "⚠️  Some services may still be running. Check with: lsof -i :7600,7001,7002,7003,7004,7500"
else
    echo "✅ All services stopped successfully!"
fi
