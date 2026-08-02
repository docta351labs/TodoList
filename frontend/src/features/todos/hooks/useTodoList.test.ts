import { renderHook, waitFor } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import { useTodoList } from '../api';
import { createWrapper } from '@/tests/testUtils';

describe('useTodoList Hook', () => {
  it('returns TodoListDto with items on success', async () => {
    const listId = '7c9e6679-7425-40de-944b-e07fc1f90ae7';
    const { result } = renderHook(() => useTodoList(listId), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(result.current.data?.id).toBe(listId);
    expect(result.current.data?.title).toBe('Work Tasks');
    expect(result.current.data?.items).toHaveLength(2);
  });

  it('returns error when list not found (404)', async () => {
    const { result } = renderHook(() => useTodoList('non-existent-id'), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isError).toBe(true));
    expect(result.current.error?.status).toBe(404);
    expect(result.current.error?.title).toBe('Todo list not found');
  });
});
