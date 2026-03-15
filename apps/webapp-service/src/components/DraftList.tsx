import type { Draft } from '../types'

interface Props {
  drafts: Draft[]
  onEdit: (draft: Draft) => void
  onDelete: (id: string) => void
  onPublish: (draft: Draft) => void
}

export function DraftList({ drafts, onEdit, onDelete, onPublish }: Props) {
  if (drafts.length === 0) {
    return <p style={{ color: '#666', textAlign: 'center', padding: '2rem' }}>
      No drafts yet. Create one to get started.
    </p>
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
      {drafts.map(draft => (
        <div
          key={draft.id}
          style={{
            border: '1px solid #ddd',
            borderRadius: '8px',
            padding: '1.5rem',
            backgroundColor: '#fff'
          }}
        >
          <div style={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'flex-start',
            marginBottom: '1rem'
          }}>
            <h3 style={{ margin: 0, fontSize: '1.25rem', color: '#333' }}>
              {draft.title}
            </h3>
            <div style={{ display: 'flex', gap: '0.5rem' }}>
              <button
                onClick={() => onEdit(draft)}
                style={{
                  padding: '0.5rem 1rem',
                  backgroundColor: '#007bff',
                  color: 'white',
                  border: 'none',
                  borderRadius: '4px',
                  cursor: 'pointer',
                  fontSize: '0.875rem'
                }}
              >
                Edit
              </button>
              <button
                onClick={() => onPublish(draft)}
                style={{
                  padding: '0.5rem 1rem',
                  backgroundColor: '#28a745',
                  color: 'white',
                  border: 'none',
                  borderRadius: '4px',
                  cursor: 'pointer',
                  fontSize: '0.875rem'
                }}
              >
                Publish
              </button>
              <button
                onClick={() => onDelete(draft.id)}
                style={{
                  padding: '0.5rem 1rem',
                  backgroundColor: '#dc3545',
                  color: 'white',
                  border: 'none',
                  borderRadius: '4px',
                  cursor: 'pointer',
                  fontSize: '0.875rem'
                }}
              >
                Delete
              </button>
            </div>
          </div>
          <p style={{
            color: '#555',
            lineHeight: '1.6',
            marginBottom: '1rem',
            whiteSpace: 'pre-wrap'
          }}>
            {draft.content}
          </p>
          <div style={{ fontSize: '0.875rem', color: '#888' }}>
            Author: {draft.authorId} &middot; Updated: {new Date(draft.updatedAt).toLocaleString()}
          </div>
        </div>
      ))}
    </div>
  )
}
