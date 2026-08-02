import styles from './LoadingSpinner.module.css';

export interface LoadingSpinnerProps {
  message?: string;
}

export function LoadingSpinner({ message = 'Loading...' }: LoadingSpinnerProps) {
  return (
    <div className={styles.container} role="status" aria-live="polite">
      <svg
        className={styles.spinner}
        viewBox="0 0 24 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
        aria-hidden="true"
      >
        <circle
          cx="12"
          cy="12"
          r="10"
          stroke="currentColor"
          strokeWidth="3"
          strokeDasharray="31.415 31.415"
          opacity="0.25"
        />
        <path
          d="M12 2C6.47715 2 2 6.47715 2 12C2 14.3409 2.80521 16.4934 4.15392 18.2"
          stroke="currentColor"
          strokeWidth="3"
          strokeLinecap="round"
        />
      </svg>
      {message && <span className={styles.label}>{message}</span>}
    </div>
  );
}

export default LoadingSpinner;
