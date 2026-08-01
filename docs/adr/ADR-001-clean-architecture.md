# ADR-001 - Adopt Clean Architecture with CQRS

**Date**: 2026-08-01  
**Status**: Accepted

## Context

We need a backend architecture that is maintainable, testable, and allows independent scaling of concerns. Common alternatives considered: traditional N-tier (Controllers -> Services -> Repositories), Modular Monolith, Microservices.

## Decision

Adopt **Clean Architecture** (domain-centric layering) combined with **CQRS** (Command Query Responsibility Segregation) via **MediatR**.

Layer dependency rule: `Domain <- Application <- Infrastructure <- Api`

Commands mutate state; Queries read state. Each handler has a single responsibility (SRP).

## Consequences

**Positive:**
- Domain logic is fully isolated and unit-testable without any infrastructure.
- Adding new features = adding new handler classes (OCP).
- Clear seam for replacing infrastructure (e.g., swapping PostgreSQL for another store).

**Negative:**
- More files/boilerplate per feature compared to a thin-service approach.
- Developers unfamiliar with the pattern need onboarding.

## Alternatives Rejected

| Alternative | Reason rejected |
|-------------|----------------|
| Traditional N-tier with service classes | Service classes tend to become God objects; harder to unit-test |
| Microservices | Premature for v1 scope; operational overhead not justified |
