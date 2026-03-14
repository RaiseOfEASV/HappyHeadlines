import type { Draft } from '../api/drafts'
import styles from './DraftList.module.css'

interface Props {
  drafts: Draft[]
  onEdit: (draft: Draft) => void
  onDelete: (id: string) => void
}

export function DraftList({ drafts, onEdit, onDelete }: Props) {
  if (drafts.length === 0) {
    return <p className={styles.empty}>No drafts yet. Create one above.</p>
  }

  return (
    <ul className={styles.list}>
      {drafts.map(draft => (
        <li key={draft.id} className={styles.item}>
          <div className={styles.itemHeader}>
            <span className={styles.itemTitle}>{draft.title}</span>
            <div className={styles.itemActions}>
              <button onClick={() => onEdit(draft)} className={styles.btnEdit}>Edit</button>
              <button onClick={() => onDelete(draft.id)} className={styles.btnDelete}>Delete</button>
            </div>
          </div>
          <p className={styles.content}>{draft.content}</p>
          <span className={styles.meta}>
            Author: {draft.authorId} &middot; Updated: {new Date(draft.updatedAt).toLocaleString()}
          </span>
        </li>
      ))}
    </ul>
  )
}
