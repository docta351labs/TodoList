import { useState } from 'react';
import { useParams, useNavigate } from 'react-router';
import {
  useTodoList,
  useAddTodoItem,
  useUpdateTodoItemStatus,
  useDeleteTodoItem,
} from '@/features/todos';
import { TodoItemCard } from '@/features/todos/components/TodoItemCard/TodoItemCard';
import { AddTodoItemForm } from '@/features/todos/components/AddTodoItemForm/AddTodoItemForm';
import { LoadingSpinner } from '@/components/LoadingSpinner/LoadingSpinner';
import { Toast } from '@/components/Toast/Toast';
import type { Status, AddTodoItemRequest, ProblemDetails } from '@/types/api';
import styles from './ListDetailPage.module.css';

type FilterType = 'All' | 'Pending' | 'InProgress' | 'Done';

export function ListDetailPage() {
  const { id } = useParams<{ id: string }>();
  const listId = id || '';
  const navigate = useNavigate();

  const { data: todoList, isLoading, isError, error } = useTodoList(listId);
  const addItemMutation = useAddTodoItem(listId);
  const updateStatusMutation = useUpdateTodoItemStatus(listId);
  const deleteItemMutation = useDeleteTodoItem(listId);

  const [activeFilter, setActiveFilter] = useState<FilterType>('All');
  const [toastError, setToastError] = useState<{ message: string; traceId?: string } | null>(null);

  const handleAddItem = async (dto: AddTodoItemRequest) => {
    try {
      await addItemMutation.mutateAsync(dto);
    } catch (err: unknown) {
      const problem = err as ProblemDetails;
      setToastError({
        message: problem?.title || 'Failed to add todo item.',
        traceId: problem?.traceId,
      });
    }
  };

  const handleStatusChange = async (itemId: string, newStatus: Status) => {
    try {
      await updateStatusMutation.mutateAsync({ itemId, dto: { newStatus } });
    } catch (err: unknown) {
      const problem = err as ProblemDetails;
      setToastError({
        message: problem?.title || 'Failed to update item status.',
        traceId: problem?.traceId,
      });
    }
  };

  const handleDeleteItem = async (itemId: string) => {
    try {
      await deleteItemMutation.mutateAsync(itemId);
    } catch (err: unknown) {
      const problem = err as ProblemDetails;
      setToastError({
        message: problem?.title || 'Failed to delete todo item.',
        traceId: problem?.traceId,
      });
    }
  };

  if (isLoading) {
    return <LoadingSpinner message="Loading list details..." />;
  }

  if (isError || !todoList) {
    return (
      <div className={styles.container}>
        <div className={styles.navigation}>
          <button className={styles.backButton} onClick={() => navigate('/')}>
            ← Back to Dashboard
          </button>
        </div>
        <div className={styles.emptyState}>
          <h2>List Not Found</h2>
          <p>{error?.title || 'The requested todo list could not be retrieved.'}</p>
        </div>
      </div>
    );
  }

  const filteredItems = todoList.items.filter((item) => {
    if (activeFilter === 'All') return true;
    if (activeFilter === 'Pending') return item.status === 'Pending' || item.status === 0;
    if (activeFilter === 'InProgress') return item.status === 'InProgress' || item.status === 1;
    if (activeFilter === 'Done') return item.status === 'Done' || item.status === 2;
    return true;
  });

  return (
    <div className={styles.container}>
      <nav className={styles.navigation}>
        <button className={styles.backButton} onClick={() => navigate('/')}>
          <svg
            width="16"
            height="16"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
          >
            <line x1="19" y1="12" x2="5" y2="12" />
            <polyline points="12 19 5 12 12 5" />
          </svg>
          Back to Dashboard
        </button>
      </nav>

      <header className={styles.header}>
        <h1 className={styles.title}>{todoList.title}</h1>
        <p className={styles.meta}>
          {todoList.items.length} {todoList.items.length === 1 ? 'item' : 'items'} total
        </p>
      </header>

      <section className={styles.formSection}>
        <AddTodoItemForm onSubmit={handleAddItem} isLoading={addItemMutation.isPending} />
      </section>

      <section>
        <div className={styles.itemsHeader}>
          <h2 className={styles.itemsTitle}>Tasks ({filteredItems.length})</h2>

          <div className={styles.filterGroup}>
            {(['All', 'Pending', 'InProgress', 'Done'] as FilterType[]).map((filter) => (
              <button
                key={filter}
                className={`${styles.filterTab} ${activeFilter === filter ? styles.filterTabActive : ''}`}
                onClick={() => setActiveFilter(filter)}
              >
                {filter}
              </button>
            ))}
          </div>
        </div>

        {filteredItems.length === 0 ? (
          <div className={styles.emptyState}>
            <p>No items found in this category.</p>
          </div>
        ) : (
          <div className={styles.itemsList}>
            {filteredItems.map((item) => (
              <TodoItemCard
                key={item.id}
                item={item}
                onStatusChange={handleStatusChange}
                onDelete={handleDeleteItem}
                isUpdatingStatus={updateStatusMutation.isPending}
                isDeleting={deleteItemMutation.isPending}
              />
            ))}
          </div>
        )}
      </section>

      {toastError && (
        <Toast
          message={toastError.message}
          traceId={toastError.traceId}
          onClose={() => setToastError(null)}
        />
      )}
    </div>
  );
}

export default ListDetailPage;
