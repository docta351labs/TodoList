# Phase 7 — Frontend: Project Scaffolding

> Tech: Vite 6, React 19, TypeScript 5 (strict), Node.js 22 LTS.

---

## Tasks

- [ ] **7.1** Scaffold Vite React-TS project:
  ```bash
  npm create vite@latest frontend -- --template react-ts
  ```
- [ ] **7.2** Install runtime dependencies:
  ```bash
  npm install react-router @tanstack/react-query @tanstack/react-query-devtools axios zod zustand
  ```
- [ ] **7.3** Install dev dependencies:
  ```bash
  npm install -D vitest @vitest/ui @testing-library/react @testing-library/user-event @testing-library/jest-dom msw playwright @playwright/test eslint @typescript-eslint/parser @typescript-eslint/eslint-plugin prettier
  ```
- [ ] **7.4** Configure `tsconfig.json`:
  - Set `"strict": true`
  - Set `"noUnusedLocals": true`, `"noUnusedParameters": true`
  - Path alias: `"@/*": ["src/*"]`
- [ ] **7.5** Configure `vite.config.ts`:
  - Add `@vitejs/plugin-react`
  - Configure Vitest test environment (`jsdom`)
  - Add `setupFiles: ['./src/tests/setup.ts']`
- [ ] **7.6** Create `src/tests/setup.ts`:
  - Import `@testing-library/jest-dom`
  - Initialize MSW server
- [ ] **7.7** Set up MSW service worker:
  ```bash
  npx msw init public/ --save
  ```
- [ ] **7.8** Install and configure Playwright:
  ```bash
  npx playwright install --with-deps
  npx playwright init
  ```
- [ ] **7.9** Add ESLint config (`eslint.config.js`) with:
  - `@typescript-eslint/recommended` rules
  - `react-hooks` plugin rules
  - No `any` type rule
- [ ] **7.10** Add Prettier config (`.prettierrc`) — 2-space indent, single quotes, trailing commas
- [ ] **7.11** Create `src/styles/tokens.css` — define CSS custom properties:
  ```css
  :root {
    --color-primary: ...;
    --color-surface: ...;
    --color-text: ...;
    --spacing-sm: 0.5rem;
    --spacing-md: 1rem;
    --spacing-lg: 1.5rem;
    --radius-md: 0.5rem;
    --font-sans: 'Inter', system-ui, sans-serif;
  }
  ```
- [ ] **7.12** Add `npm run test`, `npm run test:ui`, `npm run test:e2e` scripts to `package.json`
- [ ] **7.13** Verify `npm run build` produces zero TypeScript errors

---

## Acceptance Criteria

- `npm run build` exits with code 0
- `npm run test` (Vitest) runs and exits with code 0 (no tests yet — zero failures)
- `npm run dev` starts the dev server at `http://localhost:5173`
