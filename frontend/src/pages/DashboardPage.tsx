import { useState } from 'react';
import { useTodoLists, useCreateTodoList, useDeleteTodoList } from '@/features/todos';
import { TodoListCard } from '@/features/todos/components/TodoListCard/TodoListCard';
import { AddTodoListForm } from '@/features/todos/components/AddTodoListForm/AddTodoListForm';
import { LoadingSpinner } from '@/components/LoadingSpinner/LoadingSpinner';
import { Toast } from '@/components/Toast/Toast';
import { useUiStore } from '@/store/uiStore';
import type { ProblemDetails } from '@/types/api';
import styles from './DashboardPage.module.css';

export function DashboardPage() {
  const { data: lists, isLoading, isError, error } = useTodoLists();
  const createMutation = useCreateTodoList();
  const deleteMutation = useDeleteTodoList();

  const { isCreateListModalOpen, openCreateListModal, closeCreateListModal } = useUiStore();
  const [toastError, setToastError] = useState<{ message: string; traceId?: string } | null>(null);

  const handleCreateList = async (title: string) => {
    try {
      await createMutation.mutateAsync({ title });
      closeCreateListModal();
    } catch (err: unknown) {
      const problem = err as ProblemDetails;
      setToastError({
        message: problem?.title || 'Failed to create todo list.',
        traceId: problem?.traceId,
      });
    }
  };

  const handleDeleteList = async (id: string) => {
    try {
      await deleteMutation.mutateAsync(id);
    } catch (err: unknown) {
      const problem = err as ProblemDetails;
      setToastError({
        message: problem?.title || 'Failed to delete todo list.',
        traceId: problem?.traceId,
      });
    }
  };

  return (
    <div className={styles.container}>
      <header className={styles.header}>
        <div className={styles.titleGroup}>
          <h1 className={styles.title}>My Task Lists</h1>
          <p className={styles.subtitle}>Manage your todo lists and stay organized</p>
        </div>
        <button className={styles.createButton} onClick={openCreateListModal}>
          <svg
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2.5"
          >
            <path d="M12 5v14M5 12h14" />
          </svg>
          New List
        </button>
      </header>

      {isLoading && <LoadingSpinner message="Loading your todo lists..." />}

      {isError && (
        <div className={styles.emptyState}>
          <h2 className={styles.emptyTitle}>Error loading lists</h2>
          <p className={styles.emptyText}>
            {error?.title || 'Could not connect to the backend server.'}
          </p>
        </div>
      )}

      {!isLoading && !isError && lists && lists.length === 0 && (
        <div className={styles.emptyState}>
          <svg
            className={styles.emptyIcon}
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="1.5"
          >
            <path d="M9 5H7a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-2M9 5a2 2 0 0 1 2-2h2a2 2 0 0 1 2 2" />
          </svg>
          <h2 className={styles.emptyTitle}>No todo lists found</h2>
          <p className={styles.emptyText}>
            Get started by creating your first todo list to organize your tasks.
          </p>
          <button className={styles.createButton} onClick={openCreateListModal}>
            Create First List
          </button>
        </div>
      )}

      {!isLoading && !isError && lists && lists.length > 0 && (
        <div className={styles.grid}>
          {lists.map((list) => (
            <TodoListCard
              key={list.id}
              list={list}
              onDelete={handleDeleteList}
              isDeleting={deleteMutation.isPending}
            />
          ))}
        </div>
      )}

      {isCreateListModalOpen && (
        <div className={styles.modalOverlay} onClick={closeCreateListModal}>
          <div className={styles.modalContent} onClick={(e) => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h2 className={styles.modalTitle}>Create New List</h2>
            </div>
            <AddTodoListForm
              onSubmit={handleCreateList}
              onCancel={closeCreateListModal}
              isLoading={createMutation.isPending}
            />
          </div>
        </div>
      )}

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

export default DashboardPage;
