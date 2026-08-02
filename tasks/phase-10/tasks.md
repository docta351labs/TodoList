Created At: 2026-08-02T13:27:35Z
Completed At: 2026-08-02T13:34:40Z
File Path: `file:///c:/Users/pablo/OneDrive/351Labs/NetProjects/TodoListGitHub/TodoList/tasks/phase-10/tasks.md`

# Phase 10 — Frontend: Tests

> Framework: Vitest + React Testing Library (component tests) + Playwright (E2E smoke).  
> No snapshot tests — assert on **behavior**, not markup (AGENTS.md §4.5).  
> Test files are co-located with source: `ComponentName.test.tsx`.

---

## Component / Hook Tests (Vitest + RTL)

- [x] **10.1** `AddTodoItemForm.test.tsx`
  - `renders form fields correctly`
  - `submits correct data when form is filled and submitted`
  - `shows inline validation error when title is empty`
  - `shows loading state while mutation is pending`

- [x] **10.2** `TodoItemCard.test.tsx`
  - `renders title, priority badge, and status badge`
  - `calls useUpdateTodoItemStatus with correct args when Start button clicked`
  - `calls useUpdateTodoItemStatus with correct args when Complete button clicked`
  - `calls useDeleteTodoItem when delete button clicked`
  - `does not render action buttons when status is Done`

- [x] **10.3** `AddTodoListForm.test.tsx`
  - `submits with correct title on valid input`
  - `shows validation error when title is empty`
  - `disables submit button while mutation is in flight`

- [x] **10.4** `useTodoLists.test.ts` (hook test with MSW mock server)
  - `returns empty array on initial load`
  - `returns list of TodoListSummaryDto when API responds with data`
  - `returns error state when API returns 500`

- [x] **10.5** `useTodoList.test.ts`
  - `returns TodoListDto with items on success`
  - `returns error when list not found (404)`

- [x] **10.6** `useAddTodoItem.test.ts`
  - `optimistically adds item to cache before API responds`
  - `rolls back optimistic update when API returns error`
  - `shows toast with traceId when API returns 500`

---

## MSW Handlers (`src/tests/handlers/`)

- [x] **10.7** Create `src/tests/handlers/todoHandlers.ts` with MSW handlers for:
  - `GET /todolists` → returns mock list array
  - `POST /todolists` → returns mock created list
  - `GET /todolists/:id` → returns mock list with items
  - `DELETE /todolists/:id` → returns 204
  - `POST /todolists/:listId/items` → returns mock item
  - `PATCH /todolists/:listId/items/:itemId/status` → returns updated mock item
  - `DELETE /todolists/:listId/items/:itemId` → returns 204
- [x] **10.8** Create `src/tests/server.ts` — MSW `setupServer(...handlers)` instance

---

## E2E Test (Playwright — smoke only)

- [x] **10.9** `smoke.spec.ts` (`tests/e2e/smoke.spec.ts`):
  - Step 1: Navigate to `http://localhost:5173`
  - Step 2: Create a new list titled "Smoke Test List"
  - Step 3: Navigate into the list
  - Step 4: Add a new item titled "Test Item"
  - Step 5: Click "Start" on the item → status becomes "InProgress"
  - Step 6: Click "Complete" on the item → status becomes "Done"
  - Step 7: Delete the list
  - Assert: list no longer appears on Dashboard

---

## Notes

- Component tests must not make real HTTP requests — MSW intercepts all `axios` calls.
- Use `renderWithProviders()` helper (wraps component with `QueryClientProvider` + fresh `QueryClient`).
- Playwright E2E requires both the backend API and frontend dev server to be running.

---

## Acceptance Criteria

- `npm run test` — all 6 component/hook test suites pass
- `npm run test:e2e` — smoke test passes (requires running backend + frontend)
