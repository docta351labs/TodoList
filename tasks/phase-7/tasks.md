Created At: 2026-08-02T13:03:52Z
Completed At: 2026-08-02T13:10:00Z
File Path: `file:///c:/Users/pablo/OneDrive/351Labs/NetProjects/TodoListGitHub/TodoList/tasks/phase-7/tasks.md`

# Phase 7 — Frontend: Project Scaffolding

> Tech: Vite 6, React 19, TypeScript 5 (strict), Node.js 22 LTS.

---

## Tasks

- [x] **7.1** Scaffold Vite React-TS project:
  ```bash
  npm create vite@latest frontend -- --template react-ts
  ```
- [x] **7.2** Install runtime dependencies:
  ```bash
  npm install react-router @tanstack/react-query @tanstack/react-query-devtools axios zod zustand
  ```
- [x] **7.3** Install dev dependencies:
  ```bash
  npm install -D vitest @vitest/ui @testing-library/react @testing-library/user-event @testing-library/jest-dom msw playwright @playwright/test eslint @typescript-eslint/parser @typescript-eslint/eslint-plugin prettier
  ```
- [x] **7.4** Configure `tsconfig.json`:
  - Set `"strict": true`
  - Set `"noUnusedLocals": true`, `"noUnusedParameters": true`
  - Path alias: `"@/*": ["src/*"]`
- [x] **7.5** Configure `vite.config.ts`:
  - Add `@vitejs/plugin-react`
  - Configure Vitest test environment (`jsdom`)
  - Add `setupFiles: ['./src/tests/setup.ts']`
- [x] **7.6** Create `src/tests/setup.ts`:
  - Import `@testing-library/jest-dom`
  - Initialize MSW server
- [x] **7.7** Set up MSW service worker:
  - `npx msw init public/ --save`
- [x] **7.8** Install and configure Playwright:
  - `npx playwright install --with-deps`
  - Configure `playwright.config.ts`
- [x] **7.9** Add ESLint config (`eslint.config.js`) with:
  - `@typescript-eslint/recommended` rules
  - `react-hooks` plugin rules
  - No `any` type rule
- [x] **7.10** Add Prettier config (`.prettierrc`) — 2-space indent, single quotes, trailing commas
- [x] **7.11** Create `src/styles/tokens.css` — define CSS custom properties
- [x] **7.12** Add `npm run test`, `npm run test:ui`, `npm run test:e2e` scripts to `package.json`
- [x] **7.13** Verify `npm run build` produces zero TypeScript errors

---

## Acceptance Criteria

- `npm run build` exits with code 0
- `npm run test` (Vitest) runs and exits with code 0
- `npm run dev` starts the dev server at `http://localhost:5173`
