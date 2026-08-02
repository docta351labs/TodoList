import { useState, type FormEvent } from 'react';
import { CreateTodoListRequestSchema } from '@/types/api';
import styles from './AddTodoListForm.module.css';

export interface AddTodoListFormProps {
  onSubmit: (title: string) => Promise<void> | void;
  onCancel?: () => void;
  isLoading?: boolean;
}

export function AddTodoListForm({ onSubmit, onCancel, isLoading = false }: AddTodoListFormProps) {
  const [title, setTitle] = useState('');
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    const result = CreateTodoListRequestSchema.safeParse({ title: title.trim() });
    if (!result.success) {
      const issue = result.error.issues[0];
      setError(issue?.message || 'Invalid title');
      return;
    }

    setError(null);
    try {
      await onSubmit(result.data.title);
      setTitle('');
    } catch (err) {
      // Handled by mutation / parent component
    }
  };

  return (
    <form className={styles.form} onSubmit={handleSubmit} noValidate>
      <div className={styles.field}>
        <label className={styles.label} htmlFor="todo-list-title">
          List Title
        </label>
        <input
          id="todo-list-title"
          type="text"
          className={`${styles.input} ${error ? styles.inputError : ''}`}
          placeholder="e.g. Work Tasks, Shopping List..."
          value={title}
          onChange={(e) => {
            setTitle(e.target.value);
            if (error) setError(null);
          }}
          disabled={isLoading}
          autoFocus
        />
        {error && <span className={styles.errorText}>{error}</span>}
      </div>

      <div className={styles.actions}>
        {onCancel && (
          <button type="button" className={styles.cancelButton} onClick={onCancel} disabled={isLoading}>
            Cancel
          </button>
        )}
        <button type="submit" className={styles.submitButton} disabled={isLoading}>
          {isLoading ? 'Creating...' : 'Create List'}
        </button>
      </div>
    </form>
  );
}

export default AddTodoListForm;
