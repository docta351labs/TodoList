Created At: 2026-08-02T13:36:24Z
Completed At: 2026-08-02T13:38:30Z
File Path: `file:///c:/Users/pablo/OneDrive/351Labs/NetProjects/TodoListGitHub/TodoList/tasks/phase-11/tasks.md`

# Phase 11 — Observability & Non-Functional Requirements

---

## Tasks

### Logging (Serilog + Seq)

- [x] **11.1** Run the app (`dotnet run`) and verify Serilog writes structured JSON logs to the console
- [x] **11.2** Open Seq UI at `http://localhost:5341` — confirm logs appear for:
  - Each incoming HTTP request (method, path, status code, duration)
  - Each MediatR command/query dispatched
  - Any domain exceptions raised
- [x] **11.3** Ensure `traceId` (correlation ID) appears in both the Seq log entry and the `ProblemDetails` response body

### Tracing (OpenTelemetry)

- [x] **11.4** Verify OpenTelemetry console exporter emits spans for each request:
  - Span: HTTP request → MediatR handler → EF Core query → HTTP response
- [x] **11.5** *(Optional for v1)* Configure OTLP exporter if a local Jaeger/Tempo collector is available

### Performance

- [x] **11.6** Manually verify `GET /api/v1/todolists` responds in < 200ms (p95) under local conditions:
  - Ensure `AsNoTracking()` is applied for all read queries

### Accessibility

- [x] **11.7** Run browser accessibility audit (Chrome DevTools → Lighthouse → Accessibility) on:
  - Dashboard page
  - List detail page
- [x] **11.8** Resolve any WCAG 2.1 AA violations found (minimum: no "critical" or "serious" issues):
  - Ensure all interactive elements have accessible labels (`aria-label`, `aria-labelledby`, or visible text)
  - Ensure color contrast meets 4.5:1 ratio for normal text

### CORS Verification

- [x] **11.9** Confirm CORS policy only allows `http://localhost:5173` (no wildcard):
  - Make a request from a different origin in browser devtools → expect blocked by CORS

---

## Acceptance Criteria

- Seq shows structured logs at `http://localhost:5341` ✓
- All requests include `traceId` in logs and error responses ✓
- No Lighthouse accessibility violations at "critical" or "serious" level ✓
- CORS rejects requests from unauthorized origins ✓
