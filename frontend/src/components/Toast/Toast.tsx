import { useEffect } from 'react';
import styles from './Toast.module.css';

export interface ToastProps {
  message: string;
  traceId?: string;
  onClose: () => void;
  autoDismissMs?: number;
}

export function Toast({ message, traceId, onClose, autoDismissMs = 4000 }: ToastProps) {
  useEffect(() => {
    if (autoDismissMs <= 0) return;
    const timer = setTimeout(onClose, autoDismissMs);
    return () => clearTimeout(timer);
  }, [onClose, autoDismissMs]);

  return (
    <div className={styles.toast} role="alert">
      <div className={styles.content}>
        <div className={styles.title}>Error</div>
        <div className={styles.message}>{message}</div>
        {traceId && <div className={styles.trace}>Trace ID: {traceId}</div>}
      </div>
      <button className={styles.closeButton} onClick={onClose} aria-label="Close notification">
        <svg
          width="16"
          height="16"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
        >
          <path d="M18 6L6 18M6 6l12 12" />
        </svg>
      </button>
    </div>
  );
}

export default Toast;
