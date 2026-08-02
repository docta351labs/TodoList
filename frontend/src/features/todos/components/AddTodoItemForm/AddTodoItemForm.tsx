import { useState, type FormEvent } from 'react';
import type { Priority, AddTodoItemRequest } from '@/types/api';
import { AddTodoItemRequestSchema } from '@/types/api';
import styles from './AddTodoItemForm.module.css';

export interface AddTodoItemFormProps {
  onSubmit: (dto: AddTodoItemRequest) => Promise<void> | void;
  isLoading?: boolean;
}

export function AddTodoItemForm({ onSubmit, isLoading = false }: AddTodoItemFormProps) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [priority, setPriority] = useState<Priority>('Medium');
  const [dueDate, setDueDate] = useState('');
  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    const payload = {
      title: title.trim(),
      description: description.trim() || null,
      priority,
      dueDate: dueDate || null,
    };

    const result = AddTodoItemRequestSchema.safeParse(payload);
    if (!result.success) {
      const fieldErrors: Record<string, string> = {};
      result.error.issues.forEach((issue) => {
        const fieldName = issue.path[0] as string;
        if (fieldName) {
          fieldErrors[fieldName] = issue.message;
        }
      });
      setErrors(fieldErrors);
      return;
    }

    setErrors({});
    try {
      await onSubmit(result.data);
      setTitle('');
      setDescription('');
      setPriority('Medium');
      setDueDate('');
    } catch (err) {
      // Handled upstream
    }
  };

  return (
    <form className={styles.form} onSubmit={handleSubmit} noValidate>
      <div className={styles.field}>
        <label className={styles.label} htmlFor="item-title">
          Item Title *
        </label>
        <input
          id="item-title"
          type="text"
          className={`${styles.input} ${errors.title ? styles.inputError : ''}`}
          placeholder="e.g. Buy milk, Finish report..."
          value={title}
          onChange={(e) => {
            setTitle(e.target.value);
            if (errors.title) setErrors((prev) => ({ ...prev, title: '' }));
          }}
          disabled={isLoading}
        />
        {errors.title && <span className={styles.errorText}>{errors.title}</span>}
      </div>

      <div className={styles.field}>
        <label className={styles.label} htmlFor="item-desc">
          Description (Optional)
        </label>
        <textarea
          id="item-desc"
          className={styles.textarea}
          placeholder="Add extra context or steps..."
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          disabled={isLoading}
          rows={2}
        />
      </div>

      <div className={styles.row}>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="item-priority">
            Priority
          </label>
          <select
            id="item-priority"
            className={styles.select}
            value={priority}
            onChange={(e) => setPriority(e.target.value as Priority)}
            disabled={isLoading}
          >
            <option value="Low">Low</option>
            <option value="Medium">Medium</option>
            <option value="High">High</option>
          </select>
        </div>

        <div className={styles.field}>
          <label className={styles.label} htmlFor="item-duedate">
            Due Date (Optional)
          </label>
          <input
            id="item-duedate"
            type="date"
            className={styles.input}
            value={dueDate}
            onChange={(e) => setDueDate(e.target.value)}
            disabled={isLoading}
          />
        </div>
      </div>

      <div className={styles.actions}>
        <button type="submit" className={styles.submitButton} disabled={isLoading}>
          {isLoading ? 'Adding...' : 'Add Item'}
        </button>
      </div>
    </form>
  );
}

export default AddTodoItemForm;
