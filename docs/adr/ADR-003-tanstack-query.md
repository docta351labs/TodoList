# ADR-003 - Use TanStack Query for Server State Management

**Date**: 2026-08-01  
**Status**: Accepted

## Context

The React frontend needs to synchronize server data (todo lists, items) with the UI. Options considered: plain `useEffect` + `useState`, Redux Toolkit Query, SWR, TanStack Query.

## Decision

Use **TanStack Query v5** (formerly React Query) as the sole source of truth for server state.

- All API calls live in `features/[feature]/api.ts` using `useQuery` and `useMutation`.
- Optimistic updates are used for mutations to provide instant UI feedback.
- Zustand is reserved for **UI-only state** (modal open/close, selected filter, etc.).

## Consequences

**Positive:**
- Built-in caching, background refetching, stale-while-revalidate.
- Eliminates boilerplate loading/error state management.
- Devtools panel for inspecting cache state during development.
- Optimistic updates + rollback are first-class features.

**Negative:**
- Learning curve for developers unfamiliar with the library.
- Requires discipline to not mix TanStack Query and Zustand for the same data.

## Key Pattern

```tsx
// features/todos/api.ts
export const useTodoList = (listId: string) =>
  useQuery({
    queryKey: ['todolists', listId],
    queryFn: () => todoApi.getById(listId),
  });

export const useAddTodoItem = (listId: string) =>
  useMutation({
    mutationFn: (dto: AddTodoItemDto) => todoApi.addItem(listId, dto),
    onMutate: async (dto) => {
      await queryClient.cancelQueries({ queryKey: ['todolists', listId] });
      const previous = queryClient.getQueryData(['todolists', listId]);
      queryClient.setQueryData(['todolists', listId], (old: TodoListDto) => ({
        ...old,
        items: [...old.items, { ...dto, id: crypto.randomUUID(), status: 'Pending' }],
      }));
      return { previous };
    },
    onError: (_err, _dto, context) => {
      queryClient.setQueryData(['todolists', listId], context?.previous);
    },
    onSettled: () => queryClient.invalidateQueries({ queryKey: ['todolists', listId] }),
  });
```

## Alternatives Rejected

| Alternative | Reason rejected |
|-------------|----------------|
| Redux Toolkit Query | More boilerplate; RTK is better suited for complex global client state |
| SWR | Less feature-rich; no mutation helpers or optimistic updates built-in |
| Plain useEffect | Error-prone; no caching or deduplication |
