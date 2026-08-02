import { describe, it, expect } from 'vitest';
import {
  TodoListSummarySchema,
  TodoListSchema,
  TodoItemSchema,
  ProblemDetailsSchema,
} from '@/types/api';

describe('API Zod Schemas Validation', () => {
  it('validates TodoItemSchema against a valid todo item response', () => {
    const sampleItem = {
      id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
      title: 'Buy groceries',
      description: 'Milk, Eggs, Bread',
      priority: 'High',
      status: 'Pending',
      dueDate: '2026-08-05',
      createdAt: '2026-08-02T10:00:00Z',
      completedAt: null,
    };

    const parsed = TodoItemSchema.safeParse(sampleItem);
    if (!parsed.success) console.log('TodoItem error:', parsed.error);
    expect(parsed.success).toBe(true);
  });

  it('validates TodoListSummarySchema against a list summary response', () => {
    const sampleSummary = {
      id: '7c9e6679-7425-40de-944b-e07fc1f90ae7',
      title: 'Personal Tasks',
      ownerId: '7c9e6679-7425-40de-944b-e07fc1f90ae7',
      createdAt: '2026-08-01T12:00:00Z',
      itemCount: 5,
    };

    const parsed = TodoListSummarySchema.safeParse(sampleSummary);
    if (!parsed.success) console.log('TodoListSummary error:', parsed.error);
    expect(parsed.success).toBe(true);
  });

  it('validates TodoListSchema with nested items', () => {
    const sampleList = {
      id: '7c9e6679-7425-40de-944b-e07fc1f90ae7',
      title: 'Work Tasks',
      ownerId: '7c9e6679-7425-40de-944b-e07fc1f90ae7',
      createdAt: '2026-08-01T12:00:00Z',
      items: [
        {
          id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
          title: 'Finish report',
          description: null,
          priority: 'Medium',
          status: 'InProgress',
          dueDate: null,
          createdAt: '2026-08-01T13:00:00Z',
          completedAt: null,
        },
      ],
    };

    const parsed = TodoListSchema.safeParse(sampleList);
    if (!parsed.success) console.log('TodoList error:', parsed.error);
    expect(parsed.success).toBe(true);
  });

  it('validates RFC 7807 ProblemDetails error response', () => {
    const sampleError = {
      type: 'https://tools.ietf.org/html/rfc7231#section-6.5.1',
      title: 'One or more validation errors occurred.',
      status: 400,
      traceId: '00-abc123-def456-00',
      errors: {
        Title: ['Title must not be empty.'],
      },
    };

    const parsed = ProblemDetailsSchema.safeParse(sampleError);
    if (!parsed.success) console.log('ProblemDetails error:', parsed.error);
    expect(parsed.success).toBe(true);
  });
});
