import { renderHook, waitFor, act } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import { http, HttpResponse } from 'msw';
import { server } from '@/tests/server';
import { useAddTodoItem } from '../api';
import { createWrapper, createTestQueryClient } from '@/tests/testUtils';
import type { TodoListDto } from '@/types/api';

describe('useAddTodoItem Hook', () => {
  const listId = '7c9e6679-7425-40de-944b-e07fc1f90ae7';

  it('optimistically adds item to cache before API responds', async () => {
    const queryClient = createTestQueryClient();
    const wrapper = createWrapper(queryClient);

    // Populate initial query cache for list
    queryClient.setQueryData<TodoListDto>(['todolists', listId], {
      id: listId,
      title: 'Work Tasks',
      ownerId: '00000000-0000-0000-0000-000000000001',
      createdAt: '2026-08-01T12:00:00Z',
      items: [],
    });

    const { result } = renderHook(() => useAddTodoItem(listId), { wrapper });

    act(() => {
      result.current.mutate({
        title: 'New Optimistic Task',
        priority: 'High',
      });
    });

    // Verify cache updated optimistically
    await waitFor(() => {
      const cachedData = queryClient.getQueryData<TodoListDto>(['todolists', listId]);
      expect(cachedData?.items).toHaveLength(1);
      expect(cachedData?.items[0].title).toBe('New Optimistic Task');
    });
  });

  it('rolls back optimistic update when API returns error', async () => {
    server.use(
      http.post('*/todolists/:listId/items', () => {
        return HttpResponse.json(
          {
            title: 'Failed to add item',
            status: 500,
            traceId: '00-error-rollback-trace',
          },
          { status: 500 },
        );
      }),
    );

    const queryClient = createTestQueryClient();
    const wrapper = createWrapper(queryClient);

    const initialList: TodoListDto = {
      id: listId,
      title: 'Work Tasks',
      ownerId: '00000000-0000-0000-0000-000000000001',
      createdAt: '2026-08-01T12:00:00Z',
      items: [],
    };

    queryClient.setQueryData<TodoListDto>(['todolists', listId], initialList);

    const { result } = renderHook(() => useAddTodoItem(listId), { wrapper });

    act(() => {
      result.current.mutate({ title: 'Failing Item', priority: 'Low' });
    });

    await waitFor(() => expect(result.current.isError).toBe(true));

    // Verify cache rolled back to original state
    const cachedData = queryClient.getQueryData<TodoListDto>(['todolists', listId]);
    expect(cachedData?.items).toHaveLength(0);
    expect(result.current.error?.traceId).toBe('00-error-rollback-trace');
  });
});
