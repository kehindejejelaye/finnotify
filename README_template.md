# finnotify# Project Name — One sharp sentence that says what this does and why it matters

<!-- Badges — these are generated automatically once CI/CD is wired up -->
![CI](https://github.com/YOUR_USERNAME/YOUR_REPO/actions/workflows/ci.yml/badge.svg)
![Coverage](https://img.shields.io/codecov/c/github/YOUR_USERNAME/YOUR_REPO)
![License](https://img.shields.io/github/license/YOUR_USERNAME/YOUR_REPO)
![Last Commit](https://img.shields.io/github/last-commit/YOUR_USERNAME/YOUR_REPO)

> **One paragraph.** What problem does this solve? Who has that problem? What does this system do about it?
> Write this like you're explaining it to a senior engineer at a company you want to work at.
> No buzzwords. No "leveraging cutting-edge technologies." Just the problem and the solution.

**Live demo:** https://your-project.fly.dev  
**API docs:** https://your-project.fly.dev/swagger

---

## Table of contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Tech stack](#tech-stack)
- [Getting started](#getting-started)
- [Running tests](#running-tests)
- [CI/CD pipeline](#cicd-pipeline)
- [API reference](#api-reference)
- [Design decisions](#design-decisions)
- [Roadmap](#roadmap)

---

## Overview

### The problem

Write 2–3 sentences about the real-world problem. Ground it in domain context.
Example: "Fraud detection in multi-tenant financial platforms requires per-client rule configuration
without data leakage between tenants. Most open-source solutions treat tenancy as an afterthought."

### What this system does

Describe the solution architecture at a human level — not a bullet list of features,
but a narrative of how the system works end to end.

Example: "FinNotify accepts transaction webhook events via a secured ingestion endpoint,
resolves the originating tenant, evaluates the transaction against that tenant's configured
rule set, and dispatches alerts through the appropriate channel (email or SMS).
All tenant data is strictly isolated at the query level using a tenant-aware DbContext."

### Key capabilities

- **Multi-tenancy** — tenant-aware data isolation, zero cross-tenant data leakage
- **Rule engine** — per-tenant configurable thresholds and alert conditions
- **Webhook ingestion** — idempotent event processing with replay protection
- **Observability** — structured logging, health checks, request tracing
- _(add your actual capabilities here)_

---

## Architecture

> Include your architecture diagram here as an image.
> Draw it in Excalidraw (free, exportable as PNG) or draw.io.
> It should show: entry points, services, database, external integrations, and message flow.

![Architecture diagram](docs/architecture-diagram.png)

### Layer breakdown

| Layer | Project | Responsibility |
|---|---|---|
| API | `ProjectName.Api` | HTTP routing, auth middleware, request validation |
| Application | `ProjectName.Application` | Business logic, CQRS commands/queries, use cases |
| Domain | `ProjectName.Domain` | Entities, value objects, domain events, business rules |
| Infrastructure | `ProjectName.Infrastructure` | EF Core, PostgreSQL, email, external APIs |

### Data flow

Describe the path a request takes through the system in plain English.
Example: "A webhook arrives at `POST /api/events/ingest` → validated and tenant resolved
by middleware → published as a domain event → handled by `ProcessTransactionCommand`
→ evaluated against tenant rules → alert dispatched if threshold exceeded → result logged."

---

## Tech stack

| Concern | Technology | Why |
|---|---|---|
| API framework | ASP.NET Core 8 | Primary ecosystem, strong middleware model |
| ORM | Entity Framework Core | Tenant-aware DbContext, clean migrations |
| Database | PostgreSQL | JSONB for flexible rule config, strong indexing |
| Containerisation | Docker + Docker Compose | Reproducible local environment |
| CI/CD | GitHub Actions | Native to GitHub, free for public repos |
| Hosting | Fly.io / Railway | Free tier, Docker-native, zero config TLS |
| Testing | xUnit + Testcontainers | Real DB in integration tests, no mocks for infra |
| Logging | Serilog → Seq | Structured, queryable logs |

---

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)

That's it. Everything else runs in containers.

### Local setup (one command)

```bash
git clone https://github.com/YOUR_USERNAME/YOUR_REPO.git
cd YOUR_REPO
cp .env.example .env
./scripts/setup.sh
```

`setup.sh` will:
1. Pull all Docker images
2. Start PostgreSQL and any other services
3. Run database migrations
4. Seed development data

The API will be available at `http://localhost:5000`  
Swagger UI: `http://localhost:5000/swagger`

### Environment variables

| Variable | Description | Example |
|---|---|---|
| `DATABASE_URL` | PostgreSQL connection string | `postgres://user:pass@localhost:5432/dbname` |
| `JWT_SECRET` | Secret for signing JWT tokens | `your-256-bit-secret` |
| `SMTP_HOST` | Email provider host | `smtp.sendgrid.net` |
| `TENANT_HEADER` | Header name for tenant resolution | `X-Tenant-ID` |

See `.env.example` for the full list.

---

## Running tests

```bash
# Unit tests only (fast, no Docker needed)
dotnet test tests/ProjectName.UnitTests

# Integration tests (requires Docker — spins up real PostgreSQL via Testcontainers)
dotnet test tests/ProjectName.IntegrationTests

# All tests with coverage report
dotnet test --collect:"XPlat Code Coverage"

# Architecture tests (verify Clean Architecture rules aren't violated)
dotnet test tests/ProjectName.ArchitectureTests
```

### Test strategy

- **Unit tests** cover domain logic and application handlers in isolation
- **Integration tests** use Testcontainers to run against a real PostgreSQL instance
- **Architecture tests** use NetArchTest to enforce that the domain layer has zero
  dependencies on infrastructure — this rule is machine-checked, not just a convention

---

## CI/CD pipeline

Every push triggers the following pipeline:

```
Push / PR
    │
    ├─► Build & restore
    │
    ├─► Run unit tests
    │
    ├─► Run integration tests (Testcontainers)
    │
    ├─► Code coverage report → Codecov
    │
    └─► [main branch only] Deploy to Fly.io
```

The pipeline is defined in `.github/workflows/ci.yml` and `.github/workflows/deploy.yml`.
No manual steps. No clicking. Every merge to `main` ships.

---

## API reference

Full interactive docs available at `/swagger` when the app is running.

### Authentication

All endpoints except `/health` require a Bearer JWT token:

```
Authorization: Bearer <token>
```

Obtain a token via `POST /api/auth/token`.

### Core endpoints

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/events/ingest` | Ingest a transaction webhook event |
| `GET` | `/api/tenants/{id}/rules` | Fetch alert rules for a tenant |
| `PUT` | `/api/tenants/{id}/rules` | Update alert rules for a tenant |
| `GET` | `/api/alerts` | List alerts for the authenticated tenant |
| `GET` | `/health` | Health check — returns 200 if healthy |

### Example request

```bash
curl -X POST https://your-project.fly.dev/api/events/ingest \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -H "X-Tenant-ID: tenant-abc" \
  -d '{
    "transactionId": "txn_123",
    "amount": 4500.00,
    "currency": "NGN",
    "type": "DEBIT",
    "timestamp": "2026-03-30T10:00:00Z"
  }'
```

---

## Design decisions

> This section is what separates a portfolio project from a tutorial clone.
> Every significant decision you made belongs here — with your reasoning.
> Hiring managers read this section carefully. It's a window into how you think.

### Why PostgreSQL over a document database

Tenant rule configurations are structured and relational. JSONB gives flexibility
for rule payloads without sacrificing query performance on tenant lookups.
A document DB would add operational complexity without sufficient benefit at this scale.

### Why tenant resolution happens in middleware, not in each handler

Centralising tenant resolution in middleware means no handler can accidentally
process a request without a valid tenant context. It's a security boundary enforced
at the framework level, not by developer discipline.

### Why Testcontainers instead of an in-memory database

In-memory EF Core databases don't enforce foreign key constraints, don't support
PostgreSQL-specific features (like JSONB), and create false confidence.
Testcontainers spins up a real PostgreSQL instance per test run — slower, but honest.

### _(Add your own decisions here as you build)_

---

## Roadmap

- [x] Multi-tenant webhook ingestion
- [x] Per-tenant rule engine
- [x] Email alert dispatch
- [ ] SMS alerts via Twilio
- [ ] Rate limiting per tenant
- [ ] Admin dashboard (Next.js)
- [ ] Kubernetes deployment manifests
- [ ] OpenTelemetry distributed tracing

---

## Author

**Kehinde Jejelaye** — Senior Software Engineer  
[LinkedIn](https://linkedin.com/in/kehinde-jejelaye) · [GitHub](https://github.com/kehindejejelaye) · kehindejejelaye@gmail.com