import { useState, useEffect } from 'react'
import type { Draft, CreateDraftRequest, UpdateDraftRequest } from '../api/drafts'
import styles from './DraftForm.module.css'

interface Props {
  editing: Draft | null
  onSubmit: (data: CreateDraftRequest | UpdateDraftRequest) => void
  onCancel: () => void
}

export function DraftForm({ editing, onSubmit, onCancel }: Props) {
  const [title, setTitle] = useState('')
  const [content, setContent] = useState('')
  const [authorId, setAuthorId] = useState('')

  useEffect(() => {
    if (editing) {
      setTitle(editing.title)
      setContent(editing.content)
      setAuthorId(editing.authorId)
    } else {
      setTitle('')
      setContent('')
      setAuthorId('')
    }
  }, [editing])

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    if (editing) {
      onSubmit({ title, content } satisfies UpdateDraftRequest)
    } else {
      onSubmit({ title, content, authorId } satisfies CreateDraftRequest)
    }
  }

  return (
    <form onSubmit={handleSubmit} className={styles.form}>
      <h2>{editing ? 'Edit draft' : 'New draft'}</h2>

      <label className={styles.label}>Title
        <input
          className={styles.input}
          value={title}
          onChange={e => setTitle(e.target.value)}
          required
        />
      </label>

      {!editing && (
        <label className={styles.label}>Author ID
          <input
            className={styles.input}
            value={authorId}
            onChange={e => setAuthorId(e.target.value)}
            placeholder="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
            required
          />
        </label>
      )}

      <label className={styles.label}>Content
        <textarea
          className={styles.textarea}
          value={content}
          onChange={e => setContent(e.target.value)}
          required
        />
      </label>

      <div className={styles.actions}>
        <button type="submit" className={styles.btnPrimary}>
          {editing ? 'Save changes' : 'Create'}
        </button>
        {editing && (
          <button type="button" onClick={onCancel} className={styles.btnSecondary}>
            Cancel
          </button>
        )}
      </div>
    </form>
  )
}
