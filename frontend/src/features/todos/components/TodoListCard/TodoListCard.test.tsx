import { render, screen, fireEvent } from '@testing-library/react';
import { BrowserRouter } from 'react-router';
import { describe, it, expect, vi } from 'vitest';
import { TodoListCard } from './TodoListCard';
import type { TodoListSummaryDto } from '@/types/api';

const mockList: TodoListSummaryDto = {
  id: '7c9e6679-7425-40de-944b-e07fc1f90ae7',
  title: 'Work Tasks',
  ownerId: '00000000-0000-0000-0000-000000000001',
  createdAt: '2026-08-01T12:00:00Z',
  itemCount: 3,
};

describe('TodoListCard Component', () => {
  it('renders list title and item count', () => {
    const handleDelete = vi.fn();
    render(
      <BrowserRouter>
        <TodoListCard list={mockList} onDelete={handleDelete} />
      </BrowserRouter>,
    );

    expect(screen.getByText('Work Tasks')).toBeInTheDocument();
    expect(screen.getByText('3 items')).toBeInTheDocument();
  });

  it('triggers delete confirmation when delete button is clicked', () => {
    const handleDelete = vi.fn();
    render(
      <BrowserRouter>
        <TodoListCard list={mockList} onDelete={handleDelete} />
      </BrowserRouter>,
    );

    const deleteBtn = screen.getByRole('button', { name: /delete work tasks/i });
    fireEvent.click(deleteBtn);

    expect(screen.getByText('Confirm')).toBeInTheDocument();
    const confirmBtn = screen.getByText('Confirm');
    fireEvent.click(confirmBtn);

    expect(handleDelete).toHaveBeenCalledWith(mockList.id);
  });
});
