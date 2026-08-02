Created At: 2026-08-02T13:20:44Z
Completed At: 2026-08-02T13:25:30Z
File Path: `file:///c:/Users/pablo/OneDrive/351Labs/NetProjects/TodoListGitHub/TodoList/tasks/phase-9/tasks.md`

# Phase 9 — Frontend: Features & Components

> Feature-Sliced Design (AGENTS.md §4.1).  
> TanStack Query is the single source of truth for server state — never store server data in Zustand.  
> All mutations use **optimistic updates** with rollback on error.

---

## Pages & Routing

- [x] **9.1** Configure routes in `src/App.tsx` using React Router v7:
  ```tsx
  <Routes>
    <Route path="/" element={<DashboardPage />} />
    <Route path="/lists/:id" element={<ListDetailPage />} />
  </Routes>
  ```
- [x] **9.2** Create `src/pages/DashboardPage.tsx` — displays user's todo lists
- [x] **9.3** Create `src/pages/ListDetailPage.tsx` — displays a single list with all its items

---

## Feature: `todos` — TanStack Query Hooks (`src/features/todos/api.ts`)

- [x] **9.4** `useTodoLists()` — `useQuery` → `GET /todolists`
- [x] **9.5** `useTodoList(listId: string)` — `useQuery` → `GET /todolists/:id`
- [x] **9.6** `useCreateTodoList()` — `useMutation` → `POST /todolists`
  - Optimistic: append new list to cached list
  - Rollback on error
  - Invalidate `['todolists']` on settle
- [x] **9.7** `useDeleteTodoList()` — `useMutation` → `DELETE /todolists/:id`
  - Optimistic: remove list from cached list immediately
  - Rollback on error
- [x] **9.8** `useAddTodoItem(listId: string)` — `useMutation` → `POST /items`
  - Optimistic: append item with temp ID and `status: 'Pending'`
  - Rollback on error
  - Show toast on error with `traceId` from ProblemDetails
- [x] **9.9** `useUpdateTodoItem(listId: string)` — `useMutation` → `PUT /items/:id`
- [x] **9.10** `useUpdateTodoItemStatus(listId: string)` — `useMutation` → `PATCH /items/:id/status`
  - Optimistic: update item status in cache immediately
  - Rollback on error (invalid transition → show inline error)
- [x] **9.11** `useDeleteTodoItem(listId: string)` — `useMutation` → `DELETE /items/:id`
  - Optimistic: remove item from cache immediately

---

## Feature: `todos` — Components (`src/features/todos/components/`)

### Component Rules (AGENTS.md §4.4)
- Functional components only
- Props interface named `[ComponentName]Props`
- Co-locate CSS Module (`.module.css`) with each component
- Keep each component under 200 lines

- [x] **9.12** `TodoListCard` (`TodoListCard.tsx` + `TodoListCard.module.css`)
  - Displays: list title, item count, created date
  - Navigates to `/lists/:id` on click
  - Shows delete button with confirmation
- [x] **9.13** `AddTodoListForm` (`AddTodoListForm.tsx` + `AddTodoListForm.module.css`)
  - Input: title
  - Inline validation: not empty, max 100 chars
  - Shows loading state during mutation
- [x] **9.14** `TodoItemCard` (`TodoItemCard.tsx` + `TodoItemCard.module.css`)
  - Displays: title, description, priority badge, status badge, due date, completed date
  - Renders status transition buttons (`Start` / `Complete`)
  - Renders delete button
- [x] **9.15** `AddTodoItemForm` (`AddTodoItemForm.tsx` + `AddTodoItemForm.module.css`)
  - Inputs: title (required), description (optional), priority (select), due date (optional)
  - Inline validation for all fields
- [x] **9.16** `TodoItemStatusControl` (`TodoItemStatusControl.tsx`)
  - Renders the correct action button based on current status:
    - `Pending` → "Start" button
    - `InProgress` → "Complete" button
    - `Done` → no action (read-only badge)

---

## Shared Components (`src/components/`)

- [x] **9.17** `ErrorBoundary` — catches render errors; shows retry button; logs `traceId`
- [x] **9.18** `Toast` — transient notification; auto-dismiss after 4s; shows error message + optional `traceId`
- [x] **9.19** `LoadingSpinner` — centered spinner with accessible `aria-label`

---

## Feature Barrel Export

- [x] **9.20** Create `src/features/todos/index.ts` — re-export all hooks and components

---

## Notes

- Network errors (5xx) show a `Toast` with `traceId` copied from the ProblemDetails response.
- 4xx validation errors show inline field messages (not a toast).
- `DashboardPage` uses `useTodoLists` + renders `TodoListCard` + `AddTodoListForm`.
- `ListDetailPage` uses `useTodoList(listId)` + renders `AddTodoItemForm` + list of `TodoItemCard`.

---

## Acceptance Criteria

- `npm run dev` — app loads, Dashboard shows empty state or list of todos
- Creating a list → appears immediately (optimistic update)
- Adding an item → appears immediately
- Status transition buttons work end-to-end (frontend ↔ backend)
- Deleting a list or item removes it immediately without page refresh
