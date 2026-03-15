import type { Publication } from '../types'

interface Props {
  publications: Publication[]
  onViewStatus: (id: string) => void
}

export function PublicationList({ publications, onViewStatus }: Props) {
  if (publications.length === 0) {
    return <p style={{ color: '#666', textAlign: 'center', padding: '2rem' }}>
      No publications yet.
    </p>
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Published': return '#28a745'
      case 'Failed': return '#dc3545'
      case 'Publishing': return '#ffc107'
      default: return '#6c757d'
    }
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
      {publications.map(pub => (
        <div
          key={pub.id}
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
            <div>
              <div style={{ marginBottom: '0.5rem' }}>
                <strong>Publication ID:</strong> {pub.id}
              </div>
              <div style={{ marginBottom: '0.5rem' }}>
                <strong>Draft ID:</strong> {pub.draftId}
              </div>
              <div style={{ marginBottom: '0.5rem' }}>
                <strong>Continent:</strong> {pub.continent}
              </div>
              {pub.articleId && (
                <div style={{ marginBottom: '0.5rem' }}>
                  <strong>Article ID:</strong> {pub.articleId}
                </div>
              )}
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: '0.5rem' }}>
              <span style={{
                padding: '0.5rem 1rem',
                backgroundColor: getStatusColor(pub.status),
                color: 'white',
                borderRadius: '4px',
                fontSize: '0.875rem',
                fontWeight: 'bold'
              }}>
                {pub.status}
              </span>
              <button
                onClick={() => onViewStatus(pub.id)}
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
                View Details
              </button>
            </div>
          </div>

          {pub.errorMessage && (
            <div style={{
              padding: '1rem',
              backgroundColor: '#f8d7da',
              color: '#721c24',
              borderRadius: '4px',
              marginBottom: '1rem',
              fontSize: '0.875rem'
            }}>
              <strong>Error:</strong> {pub.errorMessage}
            </div>
          )}

          <div style={{ fontSize: '0.875rem', color: '#888' }}>
            {new Date(pub.publishInitiatedAt).toLocaleString()}
          </div>
        </div>
      ))}
    </div>
  )
}
