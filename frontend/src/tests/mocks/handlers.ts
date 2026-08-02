import { http, HttpResponse } from 'msw';

export const handlers = [
  http.get('*/api/v1/health', () => {
    return HttpResponse.json({ status: 'Healthy' });
  }),

  http.get('*/api/v1/todolists', () => {
    return HttpResponse.json([
      {
        id: '7c9e6679-7425-40de-944b-e07fc1f90ae7',
        title: 'Work Tasks',
        ownerId: '00000000-0000-0000-0000-000000000001',
        createdAt: new Date().toISOString(),
        itemCount: 2,
      },
    ]);
  }),

  http.get('*/api/v1/todolists/:id', ({ params }) => {
    return HttpResponse.json({
      id: params.id,
      title: 'Work Tasks',
      ownerId: '00000000-0000-0000-0000-000000000001',
      createdAt: new Date().toISOString(),
      items: [
        {
          id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
          title: 'Finish report',
          description: 'Quarterly review',
          priority: 'High',
          status: 'Pending',
          dueDate: '2026-08-10',
          createdAt: new Date().toISOString(),
          completedAt: null,
        },
      ],
    });
  }),
];
