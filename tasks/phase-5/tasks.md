# Phase 5 — API Layer (`TodoList.Api`)

> Minimal API style with `RouteGroupBuilder` extension methods (ADR-005).  
> JWT authentication is **mocked in v1** — all endpoints accept requests as the fixed mock user.

---

## Tasks

### Program.cs & Host Configuration

- [ ] **5.1** Configure `WebApplication` builder in `Program.cs`:
  - Call `builder.Services.AddInfrastructure(builder.Configuration)`
  - Register MediatR (`AddMediatR`) scanning `Application` assembly
  - Register FluentValidation (`AddValidatorsFromAssembly`) scanning `Application` assembly
  - Register `ValidationBehavior` and `UnitOfWorkBehavior` as pipeline behaviors
- [ ] **5.2** Configure **Serilog** (reads from `appsettings.json`):
  - Sinks: Console (with structured output) + Seq (`http://localhost:5341`)
  - Enrich with: `FromLogContext`, `WithMachineName`, `WithThreadId`
  - Minimum level: `Information` (override `Microsoft` to `Warning`)
- [ ] **5.3** Configure **OpenTelemetry**:
  - ASP.NET Core instrumentation
  - HTTP client instrumentation
  - Console exporter (dev only)
- [ ] **5.4** Configure **OpenAPI / Scalar**:
  - `builder.Services.AddOpenApi()`
  - Map `/scalar` endpoint (no auth required)
- [ ] **5.5** Configure **health checks**:
  - `builder.Services.AddHealthChecks().AddNpgSql(connectionString)`
  - Map `GET /health` (no auth required)
- [ ] **5.6** Configure **CORS**:
  - Allow only `http://localhost:5173` (dev frontend origin)
  - No wildcard `*` in any environment
- [ ] **5.7** Configure **mock authentication middleware** (v1):
  - All requests are treated as authenticated with `OwnerId = fixed Guid`
  - Use a custom middleware or bypass policy that sets `ClaimsPrincipal` automatically
  - Comment clearly: `// TODO: Replace with JWT Bearer auth when Identity Provider is integrated`
- [ ] **5.8** Configure **global exception middleware** returning RFC 7807 `ProblemDetails`:
  - `DomainException` → 422 Unprocessable Entity
  - `NotFoundException` → 404 Not Found
  - `ValidationException` (FluentValidation) → 400 Bad Request with errors dict
  - Unhandled exceptions → 500 Internal Server Error
  - Always include `traceId` in response

### `appsettings.json` / Secrets

- [ ] **5.9** Set up `appsettings.json` with non-secret config (Serilog config, Seq URL, CORS origin)
- [ ] **5.10** Set up `dotnet user-secrets` for `ConnectionStrings:DefaultConnection`
  - *(Jwt:Secret placeholder kept for future use but not enforced in v1)*

### Endpoints — TodoLists (`TodoListEndpoints.cs`)

- [ ] **5.11** `GET    /api/v1/todolists` → send `GetTodoListsQuery` → `200 OK`
- [ ] **5.12** `POST   /api/v1/todolists` → send `CreateTodoListCommand` → `201 CreatedAtRoute("GetTodoListById", ...)`
- [ ] **5.13** `GET    /api/v1/todolists/{id:guid}` → send `GetTodoListByIdQuery` → `200 OK` or `404`
- [ ] **5.14** `DELETE /api/v1/todolists/{id:guid}` → send `DeleteTodoListCommand` → `204 No Content`

### Endpoints — TodoItems (`TodoItemEndpoints.cs`)

- [ ] **5.15** `POST   /api/v1/todolists/{listId:guid}/items` → `AddTodoItemCommand` → `201`
- [ ] **5.16** `PUT    /api/v1/todolists/{listId:guid}/items/{itemId:guid}` → `UpdateTodoItemCommand` → `200`
- [ ] **5.17** `PATCH  /api/v1/todolists/{listId:guid}/items/{itemId:guid}/status` → `UpdateTodoItemStatusCommand` → `200`
- [ ] **5.18** `DELETE /api/v1/todolists/{listId:guid}/items/{itemId:guid}` → `DeleteTodoItemCommand` → `204`

### Wiring in `Program.cs`

- [ ] **5.19** Map route groups:
  ```csharp
  var api = app.MapGroup("/api/v1");
  api.MapGroup("/todolists").MapTodoLists();
  ```
  *(No `.RequireAuthorization()` in v1 — applied globally via mock middleware)*

---

## Notes

- Mock auth middleware sets `HttpContext.User` with `ClaimsPrincipal` containing the fixed `OwnerId` claim.
- All responses use `Results.*` helper methods (not `IActionResult`).
- `ISender` (MediatR) is injected directly into endpoint handler delegates.
- `OwnerId` is extracted from `HttpContext.User` in endpoint handlers and passed to commands/queries.

---

## Acceptance Criteria

- `dotnet run` starts without errors
- `GET https://localhost:7001/health` → `200 Healthy`
- `GET https://localhost:7001/scalar` → OpenAPI UI loads
- All 8 endpoints return correct status codes with Postman/curl
- Seq at `http://localhost:5341` shows structured logs for each request
