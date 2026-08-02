import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { AddTodoItemForm } from './AddTodoItemForm';

describe('AddTodoItemForm Component', () => {
  it('renders form fields correctly', () => {
    render(<AddTodoItemForm onSubmit={vi.fn()} />);

    expect(screen.getByLabelText(/item title \*/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/description \(optional\)/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/priority/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/due date \(optional\)/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /add item/i })).toBeInTheDocument();
  });

  it('submits correct data when form is filled and submitted', async () => {
    const handleSubmit = vi.fn();
    render(<AddTodoItemForm onSubmit={handleSubmit} />);

    fireEvent.change(screen.getByLabelText(/item title \*/i), {
      target: { value: 'Buy milk' },
    });
    fireEvent.change(screen.getByLabelText(/description \(optional\)/i), {
      target: { value: '2% Organic' },
    });
    fireEvent.change(screen.getByLabelText(/priority/i), {
      target: { value: 'High' },
    });
    fireEvent.change(screen.getByLabelText(/due date \(optional\)/i), {
      target: { value: '2026-08-10' },
    });

    fireEvent.click(screen.getByRole('button', { name: /add item/i }));

    await waitFor(() => {
      expect(handleSubmit).toHaveBeenCalledWith({
        title: 'Buy milk',
        description: '2% Organic',
        priority: 'High',
        dueDate: '2026-08-10',
      });
    });
  });

  it('shows inline validation error when title is empty', async () => {
    const handleSubmit = vi.fn();
    render(<AddTodoItemForm onSubmit={handleSubmit} />);

    fireEvent.click(screen.getByRole('button', { name: /add item/i }));

    expect(await screen.findByText(/title is required/i)).toBeInTheDocument();
    expect(handleSubmit).not.toHaveBeenCalled();
  });

  it('shows loading state while mutation is pending', () => {
    render(<AddTodoItemForm onSubmit={vi.fn()} isLoading={true} />);

    const submitBtn = screen.getByRole('button', { name: /adding\.\.\./i });
    expect(submitBtn).toBeInTheDocument();
    expect(submitBtn).toBeDisabled();
  });
});
