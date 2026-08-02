import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { AddTodoListForm } from './AddTodoListForm';

describe('AddTodoListForm Component', () => {
  it('submits with correct title on valid input', async () => {
    const handleSubmit = vi.fn();
    render(<AddTodoListForm onSubmit={handleSubmit} />);

    const input = screen.getByLabelText(/list title/i);
    fireEvent.change(input, { target: { value: 'Shopping List' } });
    fireEvent.click(screen.getByRole('button', { name: /create list/i }));

    await waitFor(() => {
      expect(handleSubmit).toHaveBeenCalledWith('Shopping List');
    });
  });

  it('shows validation error when title is empty', async () => {
    const handleSubmit = vi.fn();
    render(<AddTodoListForm onSubmit={handleSubmit} />);

    fireEvent.click(screen.getByRole('button', { name: /create list/i }));

    expect(await screen.findByText(/title is required/i)).toBeInTheDocument();
    expect(handleSubmit).not.toHaveBeenCalled();
  });

  it('disables submit button while mutation is in flight', () => {
    render(<AddTodoListForm onSubmit={vi.fn()} isLoading={true} />);

    const submitBtn = screen.getByRole('button', { name: /creating\.\.\./i });
    expect(submitBtn).toBeInTheDocument();
    expect(submitBtn).toBeDisabled();
  });
});
