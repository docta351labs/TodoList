import type { TodoItemDto, Status } from '@/types/api';
import { TodoItemStatusControl } from '../TodoItemStatusControl/TodoItemStatusControl';
import styles from './TodoItemCard.module.css';

export interface TodoItemCardProps {
  item: TodoItemDto;
  onStatusChange: (itemId: string, newStatus: Status) => void;
  onDelete: (itemId: string) => void;
  isUpdatingStatus?: boolean;
  isDeleting?: boolean;
}

export function TodoItemCard({
  item,
  onStatusChange,
  onDelete,
  isUpdatingStatus = false,
  isDeleting = false,
}: TodoItemCardProps) {
  const isDone = item.status === 'Done' || item.status === 2;

  const priorityLabel =
    item.priority === 0 || item.priority === 'Low'
      ? 'Low'
      : item.priority === 1 || item.priority === 'Medium'
        ? 'Medium'
        : 'High';

  const priorityClass =
    priorityLabel === 'Low'
      ? styles.priorityLow
      : priorityLabel === 'Medium'
        ? styles.priorityMedium
        : styles.priorityHigh;

  const formattedDueDate = item.dueDate
    ? new Date(item.dueDate).toLocaleDateString(undefined, { month: 'short', day: 'numeric' })
    : null;

  const formattedCompletedDate = item.completedAt
    ? new Date(item.completedAt).toLocaleDateString(undefined, {
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      })
    : null;

  return (
    <div className={`${styles.card} ${isDone ? styles.cardDone : ''}`}>
      <div className={styles.mainRow}>
        <div className={styles.titleArea}>
          <h4 className={`${styles.title} ${isDone ? styles.titleDone : ''}`}>{item.title}</h4>
          {item.description && <p className={styles.description}>{item.description}</p>}

          <div className={styles.badgesRow}>
            <span className={`${styles.priorityBadge} ${priorityClass}`}>
              {priorityLabel} Priority
            </span>
            {formattedDueDate && !isDone && (
              <span className={styles.metaText}>Due {formattedDueDate}</span>
            )}
            {formattedCompletedDate && isDone && (
              <span className={styles.metaText}>Completed {formattedCompletedDate}</span>
            )}
          </div>
        </div>
      </div>

      <div className={styles.controlsRow}>
        <TodoItemStatusControl
          status={item.status}
          onStatusChange={(newStatus) => onStatusChange(item.id, newStatus)}
          isLoading={isUpdatingStatus}
        />

        <button
          className={styles.deleteButton}
          onClick={() => onDelete(item.id)}
          disabled={isDeleting}
          aria-label={`Delete ${item.title}`}
        >
          Delete
        </button>
      </div>
    </div>
  );
}

export default TodoItemCard;
