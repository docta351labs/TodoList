import apiClient from './client';
import {
  TodoListSummarySchema,
  type TodoListSummaryDto,
  TodoListSchema,
  type TodoListDto,
  TodoItemSchema,
  type TodoItemDto,
  type CreateTodoListRequest,
  type AddTodoItemRequest,
  type UpdateTodoItemRequest,
  type UpdateTodoItemStatusRequest,
} from '@/types/api';
import { z } from 'zod';

export const todoApi = {
  async getAll(): Promise<TodoListSummaryDto[]> {
    const response = await apiClient.get('/todolists');
    return z.array(TodoListSummarySchema).parse(response.data);
  },

  async getById(id: string): Promise<TodoListDto> {
    const response = await apiClient.get(`/todolists/${id}`);
    return TodoListSchema.parse(response.data);
  },

  async create(dto: CreateTodoListRequest): Promise<TodoListDto> {
    const response = await apiClient.post('/todolists', dto);
    return TodoListSchema.parse(response.data);
  },

  async deleteList(id: string): Promise<void> {
    await apiClient.delete(`/todolists/${id}`);
  },

  async addItem(listId: string, dto: AddTodoItemRequest): Promise<TodoItemDto> {
    const response = await apiClient.post(`/todolists/${listId}/items`, dto);
    return TodoItemSchema.parse(response.data);
  },

  async updateItem(listId: string, itemId: string, dto: UpdateTodoItemRequest): Promise<TodoItemDto> {
    const response = await apiClient.put(`/todolists/${listId}/items/${itemId}`, dto);
    return TodoItemSchema.parse(response.data);
  },

  async updateItemStatus(
    listId: string,
    itemId: string,
    dto: UpdateTodoItemStatusRequest,
  ): Promise<TodoItemDto> {
    const response = await apiClient.patch(`/todolists/${listId}/items/${itemId}/status`, dto);
    return TodoItemSchema.parse(response.data);
  },

  async deleteItem(listId: string, itemId: string): Promise<void> {
    await apiClient.delete(`/todolists/${listId}/items/${itemId}`);
  },
};

export default todoApi;
