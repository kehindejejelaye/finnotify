#!/usr/bin/env bash

set -e

# -----------------------------
# Resolve Project Name
# -----------------------------
RAW_NAME=$1

if [ -z "$RAW_NAME" ]; then
  RAW_NAME=$(basename "$PWD")
fi

# Convert to PascalCase
PROJECT_NAME=$(echo "$RAW_NAME" | sed -r 's/(^|-|_)([a-z])/\U\2/g')

echo "Using project name: $PROJECT_NAME"

# -----------------------------
# Create Base Files
# -----------------------------
touch README.md .env.example docker-compose.yml

mkdir -p .github/workflows
touch .github/workflows/ci.yml .github/workflows/deploy.yml

mkdir -p docs
touch docs/architecture.md docs/api-reference.md

mkdir -p src tests infra/terraform infra/k8s scripts

touch scripts/setup.sh scripts/seed.sh

mkdir -p tests/${PROJECT_NAME}.UnitTests \
         tests/${PROJECT_NAME}.IntegrationTests \
         tests/${PROJECT_NAME}.ArchitectureTests

mkdir -p src/${PROJECT_NAME}.Api \
         src/${PROJECT_NAME}.Application \
         src/${PROJECT_NAME}.Domain \
         src/${PROJECT_NAME}.Infrastructure 

# -----------------------------
# Create Solution
# -----------------------------
dotnet new sln -n ${PROJECT_NAME}

# -----------------------------
# Create Projects
# -----------------------------
dotnet new webapi -n ${PROJECT_NAME}.Api -o src/${PROJECT_NAME}.Api

dotnet new classlib -n ${PROJECT_NAME}.Domain -o src/${PROJECT_NAME}.Domain
dotnet new classlib -n ${PROJECT_NAME}.Application -o src/${PROJECT_NAME}.Application
dotnet new classlib -n ${PROJECT_NAME}.Infrastructure -o src/${PROJECT_NAME}.Infrastructure

dotnet new xunit -n ${PROJECT_NAME}.UnitTests -o tests/${PROJECT_NAME}.UnitTests
dotnet new xunit -n ${PROJECT_NAME}.IntegrationTests -o tests/${PROJECT_NAME}.IntegrationTests
dotnet new xunit -n ${PROJECT_NAME}.ArchitectureTests -o tests/${PROJECT_NAME}.ArchitectureTests

# -----------------------------
# Add to Solution
# -----------------------------
dotnet sln add src/${PROJECT_NAME}.Api/${PROJECT_NAME}.Api.csproj
dotnet sln add src/${PROJECT_NAME}.Application/${PROJECT_NAME}.Application.csproj
dotnet sln add src/${PROJECT_NAME}.Domain/${PROJECT_NAME}.Domain.csproj
dotnet sln add src/${PROJECT_NAME}.Infrastructure/${PROJECT_NAME}.Infrastructure.csproj

dotnet sln add tests/${PROJECT_NAME}.UnitTests/${PROJECT_NAME}.UnitTests.csproj
dotnet sln add tests/${PROJECT_NAME}.IntegrationTests/${PROJECT_NAME}.IntegrationTests.csproj
dotnet sln add tests/${PROJECT_NAME}.ArchitectureTests/${PROJECT_NAME}.ArchitectureTests.csproj

# -----------------------------
# Project References
# -----------------------------

# Application -> Domain
dotnet add src/${PROJECT_NAME}.Application reference src/${PROJECT_NAME}.Domain

# Infrastructure -> Application + Domain
dotnet add src/${PROJECT_NAME}.Infrastructure reference src/${PROJECT_NAME}.Application
dotnet add src/${PROJECT_NAME}.Infrastructure reference src/${PROJECT_NAME}.Domain

# API -> (excluding your specified ones)
dotnet add src/${PROJECT_NAME}.Api reference src/${PROJECT_NAME}.Application
dotnet add src/${PROJECT_NAME}.Api reference src/${PROJECT_NAME}.Infrastructure

# Tests
dotnet add tests/${PROJECT_NAME}.UnitTests reference src/${PROJECT_NAME}.Application
dotnet add tests/${PROJECT_NAME}.UnitTests reference src/${PROJECT_NAME}.Domain

dotnet add tests/${PROJECT_NAME}.IntegrationTests reference src/${PROJECT_NAME}.Api

dotnet add tests/${PROJECT_NAME}.ArchitectureTests reference src/${PROJECT_NAME}.Application
dotnet add tests/${PROJECT_NAME}.ArchitectureTests reference src/${PROJECT_NAME}.Domain

# -----------------------------
# Build
# -----------------------------
dotnet build

echo "✅ Setup complete for ${PROJECT_NAME}"