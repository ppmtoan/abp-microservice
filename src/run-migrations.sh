#!/bin/bash

# Tasky Database Migration Script
# Run this script to create databases and apply all migrations

echo "========================================="
echo "Tasky Database Migration"
echo "========================================="
echo ""

# Check PostgreSQL connection
echo "1. Checking PostgreSQL connection..."
if psql -h localhost -U postgres -c "SELECT version();" > /dev/null 2>&1; then
    echo "   ✓ PostgreSQL is running"
else
    echo "   ✗ Cannot connect to PostgreSQL"
    echo "   Make sure PostgreSQL is running: brew services start postgresql@14"
    exit 1
fi

echo ""
echo "2. Creating databases if they don't exist..."

# Create databases
for db in Tasky_Administration Tasky_Identity Tasky_Projects Tasky_SaaS Tasky_Gateway; do
    psql -h localhost -U postgres -tc "SELECT 1 FROM pg_database WHERE datname = '$db'" | grep -q 1
    if [ $? -eq 0 ]; then
        echo "   - $db already exists"
    else
        psql -h localhost -U postgres -c "CREATE DATABASE \"$db\";"
        echo "   ✓ Created database: $db"
    fi
done

echo ""
echo "3. Running database migrations..."
cd shared/Tasky.DbMigrator
dotnet run

echo ""
echo "========================================="
echo "Migration completed!"
echo "========================================="
