import { useState, type MouseEvent } from 'react';
import { useNavigate } from 'react-router';
import type { TodoListSummaryDto } from '@/types/api';
import styles from './TodoListCard.module.css';

export interface TodoListCardProps {
  list: TodoListSummaryDto;
  onDelete: (id: string) => void;
  isDeleting?: boolean;
}

export function TodoListCard({ list, onDelete, isDeleting = false }: TodoListCardProps) {
  const navigate = useNavigate();
  const [showConfirm, setShowConfirm] = useState(false);

  const handleCardClick = () => {
    if (!showConfirm) {
      navigate(`/lists/${list.id}`);
    }
  };

  const handleDeleteClick = (e: MouseEvent) => {
    e.stopPropagation();
    setShowConfirm(true);
  };

  const handleConfirmDelete = (e: MouseEvent) => {
    e.stopPropagation();
    onDelete(list.id);
  };

  const handleCancelDelete = (e: MouseEvent) => {
    e.stopPropagation();
    setShowConfirm(false);
  };

  const formattedDate = new Date(list.createdAt).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });

  return (
    <div className={styles.card} onClick={handleCardClick} role="button" tabIndex={0}>
      <div className={styles.header}>
        <h3 className={styles.title}>{list.title}</h3>
        {showConfirm ? (
          <div className={styles.confirmDialog} onClick={(e) => e.stopPropagation()}>
            <button className={styles.confirmYes} onClick={handleConfirmDelete} disabled={isDeleting}>
              Confirm
            </button>
            <button className={styles.confirmNo} onClick={handleCancelDelete}>
              Cancel
            </button>
          </div>
        ) : (
          <button
            className={styles.deleteButton}
            onClick={handleDeleteClick}
            aria-label={`Delete ${list.title}`}
            title="Delete list"
          >
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M3 6h18M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
            </svg>
          </button>
        )}
      </div>

      <div className={styles.footer}>
        <span className={styles.itemCountBadge}>
          {list.itemCount} {list.itemCount === 1 ? 'item' : 'items'}
        </span>
        <span>Created {formattedDate}</span>
      </div>
    </div>
  );
}

export default TodoListCard;
