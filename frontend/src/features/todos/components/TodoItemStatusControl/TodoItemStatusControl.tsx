import type { Status } from '@/types/api';
import styles from './TodoItemStatusControl.module.css';

export interface TodoItemStatusControlProps {
  status: Status;
  onStatusChange: (newStatus: Status) => void;
  isLoading?: boolean;
}

export function TodoItemStatusControl({
  status,
  onStatusChange,
  isLoading = false,
}: TodoItemStatusControlProps) {
  const isPending = status === 'Pending' || status === 0;
  const isInProgress = status === 'InProgress' || status === 1;
  const isDone = status === 'Done' || status === 2;

  if (isPending) {
    return (
      <button
        className={`${styles.actionButton} ${styles.startButton}`}
        onClick={() => onStatusChange('InProgress')}
        disabled={isLoading}
        aria-label="Start task"
      >
        <svg width="14" height="14" viewBox="0 0 24 24" fill="currentColor">
          <polygon points="5 3 19 12 5 21 5 3" />
        </svg>
        Start
      </button>
    );
  }

  if (isInProgress) {
    return (
      <button
        className={`${styles.actionButton} ${styles.completeButton}`}
        onClick={() => onStatusChange('Done')}
        disabled={isLoading}
        aria-label="Complete task"
      >
        <svg
          width="14"
          height="14"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2.5"
        >
          <polyline points="20 6 9 17 4 12" />
        </svg>
        Complete
      </button>
    );
  }

  if (isDone) {
    return (
      <span className={styles.badgeDone}>
        <svg
          width="12"
          height="12"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2.5"
        >
          <polyline points="20 6 9 17 4 12" />
        </svg>
        Completed
      </span>
    );
  }

  return null;
}

export default TodoItemStatusControl;
