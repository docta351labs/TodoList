import { http, HttpResponse } from 'msw';
import type { TodoListSummaryDto, TodoListDto, TodoItemDto, Priority, Status } from '@/types/api';

export const mockListsSummary: TodoListSummaryDto[] = [
  {
    id: '7c9e6679-7425-40de-944b-e07fc1f90ae7',
    title: 'Work Tasks',
    ownerId: '00000000-0000-0000-0000-000000000001',
    createdAt: '2026-08-01T12:00:00Z',
    itemCount: 2,
  },
];

export const mockSingleList: TodoListDto = {
  id: '7c9e6679-7425-40de-944b-e07fc1f90ae7',
  title: 'Work Tasks',
  ownerId: '00000000-0000-0000-0000-000000000001',
  createdAt: '2026-08-01T12:00:00Z',
  items: [
    {
      id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
      title: 'Finish report',
      description: 'Quarterly review',
      priority: 'High',
      status: 'Pending',
      dueDate: '2026-08-10',
      createdAt: '2026-08-01T13:00:00Z',
      completedAt: null,
    },
    {
      id: '4fa85f64-5717-4562-b3fc-2c963f66afa7',
      title: 'Buy coffee',
      description: null,
      priority: 'Low',
      status: 'Done',
      dueDate: null,
      createdAt: '2026-08-01T14:00:00Z',
      completedAt: '2026-08-01T15:00:00Z',
    },
  ],
};

export const todoHandlers = [
  // GET /todolists
  http.get('*/api/v1/todolists', () => {
    return HttpResponse.json(mockListsSummary);
  }),

  // POST /todolists
  http.post('*/api/v1/todolists', async ({ request }) => {
    const body = (await request.json()) as { title: string };
    const createdList: TodoListDto = {
      id: crypto.randomUUID(),
      title: body.title,
      ownerId: '00000000-0000-0000-0000-000000000001',
      createdAt: new Date().toISOString(),
      items: [],
    };
    return HttpResponse.json(createdList, { status: 201 });
  }),

  // GET /todolists/:id
  http.get('*/api/v1/todolists/:id', ({ params }) => {
    const { id } = params;
    if (id === 'non-existent-id') {
      return HttpResponse.json(
        {
          type: 'https://tools.ietf.org/html/rfc7231#section-6.5.4',
          title: 'Todo list not found',
          status: 404,
          traceId: '00-test-trace-id-404',
        },
        { status: 404 },
      );
    }
    return HttpResponse.json({ ...mockSingleList, id: String(id) });
  }),

  // DELETE /todolists/:id
  http.delete('*/api/v1/todolists/:id', () => {
    return new HttpResponse(null, { status: 204 });
  }),

  // POST /todolists/:listId/items
  http.post('*/api/v1/todolists/:listId/items', async ({ request }) => {
    const body = (await request.json()) as {
      title: string;
      description?: string | null;
      priority: Priority;
      dueDate?: string | null;
    };
    const createdItem: TodoItemDto = {
      id: crypto.randomUUID(),
      title: body.title,
      description: body.description || null,
      priority: body.priority,
      status: 'Pending',
      dueDate: body.dueDate || null,
      createdAt: new Date().toISOString(),
      completedAt: null,
    };
    return HttpResponse.json(createdItem, { status: 201 });
  }),

  // PUT /todolists/:listId/items/:itemId
  http.put('*/api/v1/todolists/:listId/items/:itemId', async ({ request, params }) => {
    const body = (await request.json()) as {
      title: string;
      description?: string | null;
      priority: Priority;
      dueDate?: string | null;
    };
    const updatedItem: TodoItemDto = {
      id: String(params.itemId),
      title: body.title,
      description: body.description || null,
      priority: body.priority,
      status: 'Pending',
      dueDate: body.dueDate || null,
      createdAt: new Date().toISOString(),
      completedAt: null,
    };
    return HttpResponse.json(updatedItem, { status: 200 });
  }),

  // PATCH /todolists/:listId/items/:itemId/status
  http.patch('*/api/v1/todolists/:listId/items/:itemId/status', async ({ request, params }) => {
    const body = (await request.json()) as { newStatus: Status };
    const updatedItem: TodoItemDto = {
      id: String(params.itemId),
      title: 'Updated Item',
      description: null,
      priority: 'Medium',
      status: body.newStatus,
      dueDate: null,
      createdAt: new Date().toISOString(),
      completedAt: body.newStatus === 'Done' ? new Date().toISOString() : null,
    };
    return HttpResponse.json(updatedItem, { status: 200 });
  }),

  // DELETE /todolists/:listId/items/:itemId
  http.delete('*/api/v1/todolists/:listId/items/:itemId', () => {
    return new HttpResponse(null, { status: 204 });
  }),
];
