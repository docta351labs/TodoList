import { z } from 'zod';

// RFC 7807 ProblemDetails Schema
export const ProblemDetailsSchema = z.object({
  type: z.string().optional(),
  title: z.string().optional(),
  status: z.number().optional(),
  detail: z.string().optional(),
  instance: z.string().optional(),
  traceId: z.string().optional(),
  errors: z.record(z.string(), z.array(z.string())).optional(),
});

export type ProblemDetails = z.infer<typeof ProblemDetailsSchema>;

// Enums
export const PriorityEnum = {
  Low: 0,
  Medium: 1,
  High: 2,
} as const;

export const PrioritySchema = z.union([
  z.enum(['Low', 'Medium', 'High']),
  z.number().min(0).max(2),
]);

export type Priority = z.infer<typeof PrioritySchema>;

export const StatusEnum = {
  Pending: 0,
  InProgress: 1,
  Done: 2,
} as const;

export const StatusSchema = z.union([
  z.enum(['Pending', 'InProgress', 'Done']),
  z.number().min(0).max(2),
]);

export type Status = z.infer<typeof StatusSchema>;

// TodoItem Schema
export const TodoItemSchema = z.object({
  id: z.string(),
  title: z.string(),
  description: z.string().nullable().optional(),
  priority: PrioritySchema,
  status: StatusSchema,
  dueDate: z.string().nullable().optional(),
  createdAt: z.string(),
  completedAt: z.string().nullable().optional(),
});

export type TodoItemDto = z.infer<typeof TodoItemSchema>;

// TodoList Schema
export const TodoListSchema = z.object({
  id: z.string(),
  title: z.string(),
  ownerId: z.string(),
  createdAt: z.string(),
  items: z.array(TodoItemSchema),
});

export type TodoListDto = z.infer<typeof TodoListSchema>;

// TodoListSummary Schema
export const TodoListSummarySchema = z.object({
  id: z.string(),
  title: z.string(),
  ownerId: z.string(),
  createdAt: z.string(),
  itemCount: z.number(),
});

export type TodoListSummaryDto = z.infer<typeof TodoListSummarySchema>;

// Request DTO Schemas
export const CreateTodoListRequestSchema = z.object({
  title: z.string().min(1, 'Title is required').max(100, 'Title cannot exceed 100 characters'),
});

export type CreateTodoListRequest = z.infer<typeof CreateTodoListRequestSchema>;

export const AddTodoItemRequestSchema = z.object({
  title: z.string().min(1, 'Title is required').max(200, 'Title cannot exceed 200 characters'),
  description: z.string().nullable().optional(),
  priority: PrioritySchema,
  dueDate: z.string().nullable().optional(),
});

export type AddTodoItemRequest = z.infer<typeof AddTodoItemRequestSchema>;

export const UpdateTodoItemRequestSchema = z.object({
  title: z.string().min(1, 'Title is required').max(200, 'Title cannot exceed 200 characters'),
  description: z.string().nullable().optional(),
  priority: PrioritySchema,
  dueDate: z.string().nullable().optional(),
});

export type UpdateTodoItemRequest = z.infer<typeof UpdateTodoItemRequestSchema>;

export const UpdateTodoItemStatusRequestSchema = z.object({
  newStatus: StatusSchema,
});

export type UpdateTodoItemStatusRequest = z.infer<typeof UpdateTodoItemStatusRequestSchema>;
