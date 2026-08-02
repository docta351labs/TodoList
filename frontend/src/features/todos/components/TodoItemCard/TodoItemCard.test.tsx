import { render, screen, fireEvent } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { TodoItemCard } from './TodoItemCard';
import type { TodoItemDto } from '@/types/api';

const mockPendingItem: TodoItemDto = {
  id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
  title: 'Buy groceries',
  description: 'Milk, Eggs, Bread',
  priority: 'High',
  status: 'Pending',
  dueDate: '2026-08-05',
  createdAt: '2026-08-01T12:00:00Z',
  completedAt: null,
};

describe('TodoItemCard Component', () => {
  it('renders item title, description, priority, and status control', () => {
    const handleStatusChange = vi.fn();
    const handleDelete = vi.fn();

    render(
      <TodoItemCard
        item={mockPendingItem}
        onStatusChange={handleStatusChange}
        onDelete={handleDelete}
      />,
    );

    expect(screen.getByText('Buy groceries')).toBeInTheDocument();
    expect(screen.getByText('Milk, Eggs, Bread')).toBeInTheDocument();
    expect(screen.getByText('High Priority')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /start task/i })).toBeInTheDocument();
  });

  it('triggers onStatusChange callback when Start action button is clicked', () => {
    const handleStatusChange = vi.fn();
    const handleDelete = vi.fn();

    render(
      <TodoItemCard
        item={mockPendingItem}
        onStatusChange={handleStatusChange}
        onDelete={handleDelete}
      />,
    );

    const startBtn = screen.getByRole('button', { name: /start task/i });
    fireEvent.click(startBtn);

    expect(handleStatusChange).toHaveBeenCalledWith(mockPendingItem.id, 'InProgress');
  });

  it('triggers onDelete callback when Delete button is clicked', () => {
    const handleStatusChange = vi.fn();
    const handleDelete = vi.fn();

    render(
      <TodoItemCard
        item={mockPendingItem}
        onStatusChange={handleStatusChange}
        onDelete={handleDelete}
      />,
    );

    const deleteBtn = screen.getByRole('button', { name: /delete buy groceries/i });
    fireEvent.click(deleteBtn);

    expect(handleDelete).toHaveBeenCalledWith(mockPendingItem.id);
  });
});
