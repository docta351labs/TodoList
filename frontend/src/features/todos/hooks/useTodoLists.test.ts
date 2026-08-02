import { renderHook, waitFor } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import { http, HttpResponse } from 'msw';
import { server } from '@/tests/server';
import { useTodoLists } from '../api';
import { createWrapper } from '@/tests/testUtils';

describe('useTodoLists Hook', () => {
  it('returns list of TodoListSummaryDto when API responds with data', async () => {
    const { result } = renderHook(() => useTodoLists(), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(result.current.data).toHaveLength(1);
    expect(result.current.data?.[0].title).toBe('Work Tasks');
  });

  it('returns empty array when API responds with empty list', async () => {
    server.use(
      http.get('*/api/v1/todolists', () => {
        return HttpResponse.json([]);
      }),
    );

    const { result } = renderHook(() => useTodoLists(), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(result.current.data).toEqual([]);
  });

  it('returns error state when API returns 500', async () => {
    server.use(
      http.get('*/api/v1/todolists', () => {
        return HttpResponse.json(
          {
            title: 'Internal server error',
            status: 500,
            traceId: '00-500-trace-id',
          },
          { status: 500 },
        );
      }),
    );

    const { result } = renderHook(() => useTodoLists(), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isError).toBe(true));
    expect(result.current.error?.status).toBe(500);
  });
});
