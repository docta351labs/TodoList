# Phase 0 — Repository & Tooling Bootstrap

## Tasks

- [x] **0.1** Initialize solution file: `dotnet new sln -n TodoList` at `backend/`
- [x] **0.2** Verify `.editorconfig` covers all project file types (`.cs`, `.ts`, `.json`, `.md`)
- [x] **0.3** Verify `.gitignore` excludes `bin/`, `obj/`, `node_modules/`, `*.user`, `.env`, `user-secrets`
- [x] **0.4** Update `docker-compose.yml` to add the **Seq** container for local structured log viewing *(already done — verify)*

## Notes

- Docker Compose now includes: `db` (PostgreSQL 16), `seq` (Serilog log viewer), `api`, `frontend`.
- All secrets are provided via environment variables or `dotnet user-secrets` — never hardcoded.
- Seq UI is available at `http://localhost:5341`.
