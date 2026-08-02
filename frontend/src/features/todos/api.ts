import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import todoApi from '@/api/todoApi';
import type {
  TodoListSummaryDto,
  TodoListDto,
  TodoItemDto,
  CreateTodoListRequest,
  AddTodoItemRequest,
  UpdateTodoItemRequest,
  UpdateTodoItemStatusRequest,
  ProblemDetails,
} from '@/types/api';

export const todoKeys = {
  all: ['todolists'] as const,
  detail: (id: string) => ['todolists', id] as const,
};

// 9.4: Fetch all todo lists summary
export function useTodoLists() {
  return useQuery<TodoListSummaryDto[], ProblemDetails>({
    queryKey: todoKeys.all,
    queryFn: () => todoApi.getAll(),
  });
}

// 9.5: Fetch single todo list by ID
export function useTodoList(listId: string) {
  return useQuery<TodoListDto, ProblemDetails>({
    queryKey: todoKeys.detail(listId),
    queryFn: () => todoApi.getById(listId),
    enabled: Boolean(listId),
  });
}

// 9.6: Create a new todo list with optimistic update
export function useCreateTodoList() {
  const queryClient = useQueryClient();

  return useMutation<
    TodoListDto,
    ProblemDetails,
    CreateTodoListRequest,
    { previousLists?: TodoListSummaryDto[] }
  >({
    mutationFn: (dto) => todoApi.create(dto),
    onMutate: async (newList) => {
      await queryClient.cancelQueries({ queryKey: todoKeys.all });

      const previousLists = queryClient.getQueryData<TodoListSummaryDto[]>(todoKeys.all);

      const optimisticList: TodoListSummaryDto = {
        id: crypto.randomUUID(),
        title: newList.title,
        ownerId: '00000000-0000-0000-0000-000000000001',
        createdAt: new Date().toISOString(),
        itemCount: 0,
      };

      if (previousLists) {
        queryClient.setQueryData<TodoListSummaryDto[]>(todoKeys.all, [
          optimisticList,
          ...previousLists,
        ]);
      }

      return { previousLists };
    },
    onError: (_err, _newList, context) => {
      if (context?.previousLists) {
        queryClient.setQueryData(todoKeys.all, context.previousLists);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: todoKeys.all });
    },
  });
}

// 9.7: Delete todo list with optimistic update
export function useDeleteTodoList() {
  const queryClient = useQueryClient();

  return useMutation<void, ProblemDetails, string, { previousLists?: TodoListSummaryDto[] }>({
    mutationFn: (listId) => todoApi.deleteList(listId),
    onMutate: async (listId) => {
      await queryClient.cancelQueries({ queryKey: todoKeys.all });

      const previousLists = queryClient.getQueryData<TodoListSummaryDto[]>(todoKeys.all);

      if (previousLists) {
        queryClient.setQueryData<TodoListSummaryDto[]>(
          todoKeys.all,
          previousLists.filter((list) => list.id !== listId),
        );
      }

      return { previousLists };
    },
    onError: (_err, _listId, context) => {
      if (context?.previousLists) {
        queryClient.setQueryData(todoKeys.all, context.previousLists);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: todoKeys.all });
    },
  });
}

// 9.8: Add todo item with optimistic update
export function useAddTodoItem(listId: string) {
  const queryClient = useQueryClient();

  return useMutation<
    TodoItemDto,
    ProblemDetails,
    AddTodoItemRequest,
    { previousList?: TodoListDto }
  >({
    mutationFn: (dto) => todoApi.addItem(listId, dto),
    onMutate: async (newItem) => {
      await queryClient.cancelQueries({ queryKey: todoKeys.detail(listId) });

      const previousList = queryClient.getQueryData<TodoListDto>(todoKeys.detail(listId));

      if (previousList) {
        const tempItem: TodoItemDto = {
          id: crypto.randomUUID(),
          title: newItem.title,
          description: newItem.description || null,
          priority: newItem.priority,
          status: 'Pending',
          dueDate: newItem.dueDate || null,
          createdAt: new Date().toISOString(),
          completedAt: null,
        };

        queryClient.setQueryData<TodoListDto>(todoKeys.detail(listId), {
          ...previousList,
          items: [...previousList.items, tempItem],
        });
      }

      return { previousList };
    },
    onError: (_err, _newItem, context) => {
      if (context?.previousList) {
        queryClient.setQueryData(todoKeys.detail(listId), context.previousList);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: todoKeys.detail(listId) });
      queryClient.invalidateQueries({ queryKey: todoKeys.all });
    },
  });
}

// 9.9: Update todo item details with optimistic update
export function useUpdateTodoItem(listId: string) {
  const queryClient = useQueryClient();

  return useMutation<
    TodoItemDto,
    ProblemDetails,
    { itemId: string; dto: UpdateTodoItemRequest },
    { previousList?: TodoListDto }
  >({
    mutationFn: ({ itemId, dto }) => todoApi.updateItem(listId, itemId, dto),
    onMutate: async ({ itemId, dto }) => {
      await queryClient.cancelQueries({ queryKey: todoKeys.detail(listId) });

      const previousList = queryClient.getQueryData<TodoListDto>(todoKeys.detail(listId));

      if (previousList) {
        queryClient.setQueryData<TodoListDto>(todoKeys.detail(listId), {
          ...previousList,
          items: previousList.items.map((item) =>
            item.id === itemId
              ? {
                  ...item,
                  title: dto.title,
                  description: dto.description || null,
                  priority: dto.priority,
                  dueDate: dto.dueDate || null,
                }
              : item,
          ),
        });
      }

      return { previousList };
    },
    onError: (_err, _vars, context) => {
      if (context?.previousList) {
        queryClient.setQueryData(todoKeys.detail(listId), context.previousList);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: todoKeys.detail(listId) });
    },
  });
}

// 9.10: Update todo item status with optimistic update
export function useUpdateTodoItemStatus(listId: string) {
  const queryClient = useQueryClient();

  return useMutation<
    TodoItemDto,
    ProblemDetails,
    { itemId: string; dto: UpdateTodoItemStatusRequest },
    { previousList?: TodoListDto }
  >({
    mutationFn: ({ itemId, dto }) => todoApi.updateItemStatus(listId, itemId, dto),
    onMutate: async ({ itemId, dto }) => {
      await queryClient.cancelQueries({ queryKey: todoKeys.detail(listId) });

      const previousList = queryClient.getQueryData<TodoListDto>(todoKeys.detail(listId));

      if (previousList) {
        queryClient.setQueryData<TodoListDto>(todoKeys.detail(listId), {
          ...previousList,
          items: previousList.items.map((item) =>
            item.id === itemId
              ? {
                  ...item,
                  status: dto.newStatus,
                  completedAt:
                    dto.newStatus === 'Done' ? new Date().toISOString() : item.completedAt,
                }
              : item,
          ),
        });
      }

      return { previousList };
    },
    onError: (_err, _vars, context) => {
      if (context?.previousList) {
        queryClient.setQueryData(todoKeys.detail(listId), context.previousList);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: todoKeys.detail(listId) });
    },
  });
}

// 9.11: Delete todo item with optimistic update
export function useDeleteTodoItem(listId: string) {
  const queryClient = useQueryClient();

  return useMutation<void, ProblemDetails, string, { previousList?: TodoListDto }>({
    mutationFn: (itemId) => todoApi.deleteItem(listId, itemId),
    onMutate: async (itemId) => {
      await queryClient.cancelQueries({ queryKey: todoKeys.detail(listId) });

      const previousList = queryClient.getQueryData<TodoListDto>(todoKeys.detail(listId));

      if (previousList) {
        queryClient.setQueryData<TodoListDto>(todoKeys.detail(listId), {
          ...previousList,
          items: previousList.items.filter((item) => item.id !== itemId),
        });
      }

      return { previousList };
    },
    onError: (_err, _itemId, context) => {
      if (context?.previousList) {
        queryClient.setQueryData(todoKeys.detail(listId), context.previousList);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: todoKeys.detail(listId) });
      queryClient.invalidateQueries({ queryKey: todoKeys.all });
    },
  });
}
