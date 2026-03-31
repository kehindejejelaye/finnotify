param(
    [string]$Name
)

# -----------------------------
# Resolve Project Name
# -----------------------------
if (-not $Name) {
    $Name = Split-Path -Leaf (Get-Location)
}

# Convert to PascalCase
$ProjectName = ($Name -split '[-_ ]+' | ForEach-Object {
    $_.Substring(0,1).ToUpper() + $_.Substring(1).ToLower()
}) -join ''

Write-Host "Using project name: $ProjectName"

# -----------------------------
# Create Base Files
# -----------------------------
New-Item README.md -ItemType File -Force | Out-Null
New-Item .env.example -ItemType File -Force | Out-Null
New-Item docker-compose.yml -ItemType File -Force | Out-Null

New-Item .github/workflows -ItemType Directory -Force | Out-Null
New-Item .github/workflows/ci.yml -ItemType File -Force | Out-Null
New-Item .github/workflows/deploy.yml -ItemType File -Force | Out-Null

New-Item docs -ItemType Directory -Force | Out-Null
New-Item docs/architecture.md -ItemType File -Force | Out-Null
New-Item docs/api-reference.md -ItemType File -Force | Out-Null

New-Item src, tests, infra/terraform, infra/k8s, scripts -ItemType Directory -Force | Out-Null

New-Item scripts/setup.sh -ItemType File -Force | Out-Null
New-Item scripts/seed.sh -ItemType File -Force | Out-Null

New-Item tests/$ProjectName.UnitTests -ItemType Directory -Force | Out-Null
New-Item tests/$ProjectName.IntegrationTests -ItemType Directory -Force | Out-Null
New-Item tests/$ProjectName.ArchitectureTests -ItemType Directory -Force | Out-Null

New-Item src/$ProjectName.Api -ItemType Directory -Force | Out-Null
New-Item src/$ProjectName.Application -ItemType Directory -Force | Out-Null
New-Item src/$ProjectName.Domain -ItemType Directory -Force | Out-Null
New-Item src/$ProjectName.Infrastructure -ItemType Directory -Force | Out-Null

# -----------------------------
# Solution
# -----------------------------
dotnet new sln -n $ProjectName

# -----------------------------
# Projects
# -----------------------------
dotnet new webapi -n "$ProjectName.Api" -o "src/$ProjectName.Api"

dotnet new classlib -n "$ProjectName.Domain" -o "src/$ProjectName.Domain"
dotnet new classlib -n "$ProjectName.Application" -o "src/$ProjectName.Application"
dotnet new classlib -n "$ProjectName.Infrastructure" -o "src/$ProjectName.Infrastructure"

dotnet new xunit -n "$ProjectName.UnitTests" -o "tests/$ProjectName.UnitTests"
dotnet new xunit -n "$ProjectName.IntegrationTests" -o "tests/$ProjectName.IntegrationTests"
dotnet new xunit -n "$ProjectName.ArchitectureTests" -o "tests/$ProjectName.ArchitectureTests"

# -----------------------------
# Add to Solution
# -----------------------------
dotnet sln add "src/$ProjectName.Api/$ProjectName.Api.csproj"
dotnet sln add "src/$ProjectName.Application/$ProjectName.Application.csproj"
dotnet sln add "src/$ProjectName.Domain/$ProjectName.Domain.csproj"
dotnet sln add "src/$ProjectName.Infrastructure/$ProjectName.Infrastructure.csproj"

dotnet sln add "tests/$ProjectName.UnitTests/$ProjectName.UnitTests.csproj"
dotnet sln add "tests/$ProjectName.IntegrationTests/$ProjectName.IntegrationTests.csproj"
dotnet sln add "tests/$ProjectName.ArchitectureTests/$ProjectName.ArchitectureTests.csproj"

# -----------------------------
# References
# -----------------------------
dotnet add "src/$ProjectName.Application" reference "src/$ProjectName.Domain"

dotnet add "src/$ProjectName.Infrastructure" reference "src/$ProjectName.Application"
dotnet add "src/$ProjectName.Infrastructure" reference "src/$ProjectName.Domain"

dotnet add "src/$ProjectName.Api" reference "src/$ProjectName.Application"
dotnet add "src/$ProjectName.Api" reference "src/$ProjectName.Infrastructure"

dotnet add "tests/$ProjectName.UnitTests" reference "src/$ProjectName.Application"
dotnet add "tests/$ProjectName.UnitTests" reference "src/$ProjectName.Domain"

dotnet add "tests/$ProjectName.IntegrationTests" reference "src/$ProjectName.Api"

dotnet add "tests/$ProjectName.ArchitectureTests" reference "src/$ProjectName.Application"
dotnet add "tests/$ProjectName.ArchitectureTests" reference "src/$ProjectName.Domain"

# -----------------------------
# Build
# -----------------------------
dotnet build

Write-Host "✅ Setup complete for $ProjectName"