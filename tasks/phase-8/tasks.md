# Phase 8 — Frontend: Core Setup

> Establish the shared foundation: API types, HTTP client, QueryClient, and global state.  
> No feature code here — only the infrastructure that features depend on.

---

## Tasks

### Types & Validation (Zod)

- [ ] **8.1** Create `src/types/api.ts` — shared TypeScript interfaces **and** Zod schemas:
  ```ts
  // Example shape
  export const TodoItemSchema = z.object({
    id: z.string().uuid(),
    title: z.string(),
    description: z.string().nullable(),
    priority: z.enum(['Low', 'Medium', 'High']),
    status: z.enum(['Pending', 'InProgress', 'Done']),
    dueDate: z.string().nullable(),
    createdAt: z.string().datetime(),
    completedAt: z.string().datetime().nullable(),
  });
  export type TodoItemDto = z.infer<typeof TodoItemSchema>;
  ```
  - Define: `TodoItemSchema`, `TodoListSchema`, `TodoListSummarySchema`, `ProblemDetailsSchema`
  - Export inferred TypeScript types

### Axios HTTP Client

- [ ] **8.2** Create `src/api/client.ts`:
  - Axios instance with `baseURL: import.meta.env.VITE_API_BASE_URL`
  - Request interceptor: add `Authorization: Bearer mock-token` header (v1 mock)
  - Response interceptor: parse errors as `ProblemDetailsSchema`
- [ ] **8.3** Create `src/api/todoApi.ts`:
  - `getAll(): Promise<TodoListSummaryDto[]>`
  - `getById(id: string): Promise<TodoListDto>`
  - `create(dto: CreateTodoListRequest): Promise<TodoListDto>`
  - `deleteList(id: string): Promise<void>`
  - `addItem(listId: string, dto: AddTodoItemRequest): Promise<TodoItemDto>`
  - `updateItem(listId: string, itemId: string, dto: UpdateTodoItemRequest): Promise<TodoItemDto>`
  - `updateItemStatus(listId: string, itemId: string, dto: UpdateTodoItemStatusRequest): Promise<TodoItemDto>`
  - `deleteItem(listId: string, itemId: string): Promise<void>`

### TanStack Query

- [ ] **8.4** Create `src/api/queryClient.ts`:
  - `QueryClient` with default options:
    - `staleTime: 30_000`
    - `retry: 1`
    - `onError`: global error handler (logs to console in dev)

### Zustand Store

- [ ] **8.5** Create `src/store/uiStore.ts`:
  - State: `isCreateListModalOpen: boolean`
  - Actions: `openCreateListModal()`, `closeCreateListModal()`
  *(Extend as needed when UI state grows)*

### App Bootstrap

- [ ] **8.6** Update `src/main.tsx`:
  - Wrap `<App />` with `<QueryClientProvider client={queryClient}>`
  - Wrap with `<BrowserRouter>`
  - Render `<ReactQueryDevtools initialIsOpen={false} />` (dev only via `import.meta.env.DEV`)
- [ ] **8.7** Add `VITE_API_BASE_URL=http://localhost:7001/api/v1` to `.env.local`
  - Add `.env.local` to `.gitignore`

---

## Notes

- All API response shapes **must** be validated with Zod before entering component state (per AGENTS.md §4.3).
- No `any` type anywhere — use `unknown` and Zod `.parse()` / `.safeParse()`.
- The `Authorization: Bearer mock-token` header matches the mock middleware on the backend.

---

## Acceptance Criteria

- `src/api/client.ts` compiles with zero TypeScript errors
- All Zod schemas validate against sample API responses (write a quick unit test or REPL check)
