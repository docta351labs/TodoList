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

const mockInProgressItem: TodoItemDto = {
  ...mockPendingItem,
  id: '4fa85f64-5717-4562-b3fc-2c963f66afa7',
  title: 'Write report',
  status: 'InProgress',
};

const mockDoneItem: TodoItemDto = {
  ...mockPendingItem,
  id: '5fa85f64-5717-4562-b3fc-2c963f66afa8',
  title: 'Clean room',
  status: 'Done',
  completedAt: '2026-08-01T15:00:00Z',
};

describe('TodoItemCard Component', () => {
  it('renders title, priority badge, and status badge', () => {
    render(
      <TodoItemCard
        item={mockPendingItem}
        onStatusChange={vi.fn()}
        onDelete={vi.fn()}
      />,
    );

    expect(screen.getByText('Buy groceries')).toBeInTheDocument();
    expect(screen.getByText('Milk, Eggs, Bread')).toBeInTheDocument();
    expect(screen.getByText('High Priority')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /start task/i })).toBeInTheDocument();
  });

  it('calls onStatusChange with InProgress when Start button clicked', () => {
    const handleStatusChange = vi.fn();
    render(
      <TodoItemCard
        item={mockPendingItem}
        onStatusChange={handleStatusChange}
        onDelete={vi.fn()}
      />,
    );

    fireEvent.click(screen.getByRole('button', { name: /start task/i }));
    expect(handleStatusChange).toHaveBeenCalledWith(mockPendingItem.id, 'InProgress');
  });

  it('calls onStatusChange with Done when Complete button clicked', () => {
    const handleStatusChange = vi.fn();
    render(
      <TodoItemCard
        item={mockInProgressItem}
        onStatusChange={handleStatusChange}
        onDelete={vi.fn()}
      />,
    );

    fireEvent.click(screen.getByRole('button', { name: /complete task/i }));
    expect(handleStatusChange).toHaveBeenCalledWith(mockInProgressItem.id, 'Done');
  });

  it('calls onDelete when delete button clicked', () => {
    const handleDelete = vi.fn();
    render(
      <TodoItemCard
        item={mockPendingItem}
        onStatusChange={vi.fn()}
        onDelete={handleDelete}
      />,
    );

    fireEvent.click(screen.getByRole('button', { name: /delete buy groceries/i }));
    expect(handleDelete).toHaveBeenCalledWith(mockPendingItem.id);
  });

  it('does not render action buttons when status is Done', () => {
    render(
      <TodoItemCard
        item={mockDoneItem}
        onStatusChange={vi.fn()}
        onDelete={vi.fn()}
      />,
    );

    expect(screen.queryByRole('button', { name: /start task/i })).not.toBeInTheDocument();
    expect(screen.queryByRole('button', { name: /complete task/i })).not.toBeInTheDocument();
    expect(screen.getByText('Completed')).toBeInTheDocument();
  });
});
