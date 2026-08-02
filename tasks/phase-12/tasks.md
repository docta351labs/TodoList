Created At: 2026-08-02T13:41:36Z
Completed At: 2026-08-02T13:45:10Z
File Path: `file:///c:/Users/pablo/OneDrive/351Labs/NetProjects/TodoListGitHub/TodoList/tasks/phase-12/tasks.md`

# Phase 12 — CI / Final Polish

> This phase ensures the project is clean, documented, and ready for a PR or team handoff.

---

## Tasks

### Build Verification

- [x] **12.1** Run `dotnet build` — must exit with **zero warnings** and **zero errors**
- [x] **12.2** Run `npm run build` — must exit with **zero TypeScript errors** and **zero Vite errors**

### Test Suite — All Green

- [x] **12.3** Run `dotnet test` (all backend suites) — all tests green
- [x] **12.4** Run `npm run test` (Vitest) — all component/hook tests green
- [x] **12.5** Run `npm run test:e2e` (Playwright) — smoke test passes

### Documentation

- [x] **12.6** Update `README.md` if any setup steps changed:
  - Docker Compose now includes Seq — add `http://localhost:5341` to Quick Start
  - Note that JWT auth is mocked in v1 (no token needed)
- [x] **12.7** Verify OpenAPI spec is current:
  - Start the API and navigate to `http://localhost:7001/scalar`
  - All 8 endpoints should be visible with correct request/response schemas

### Code Quality

- [x] **12.8** Run ESLint: `npm run lint` — zero errors
- [x] **12.9** Run Prettier: `npm run format:check` — zero formatting violations
- [x] **12.10** Ensure no `// TODO` comments remain except the intentional mock-auth swap note in `Program.cs`

### Git Hygiene

- [x] **12.11** Ensure `.gitignore` excludes:
  - `*.user`, `bin/`, `obj/`, `.vs/`
  - `node_modules/`, `dist/`
  - `.env.local`, `appsettings.*.json` (secrets)
  - `playwright-report/`, `test-results/`
- [x] **12.12** Review commit history — all commits follow Conventional Commits format:
  ```
  feat(domain): add TodoItem entity
  test(api): add integration test for DELETE /todolists/{id}
  fix(infra): correct snake_case mapping for created_at column
  ```
- [x] **12.13** Confirm PR checklist (AGENTS.md §5) is satisfied:
  - [x] All existing tests pass
  - [x] New feature has corresponding unit tests
  - [x] No new compiler warnings
  - [x] OpenAPI spec regenerated if endpoints changed
  - [x] Migration added if schema changed

---

## Acceptance Criteria

- `dotnet test` → all green ✓
- `npm run test` → all green ✓
- `npm run build` → zero errors ✓
- `dotnet build` → zero warnings ✓
- `README.md` is up to date ✓
