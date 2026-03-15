import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { PublicationList } from '../components/PublicationList'
import { PublicationStatus } from '../components/PublicationStatus'
import type { Publication } from '../types'
import { getPublications } from '../services/publisherService'

export function Publications() {
  const navigate = useNavigate()
  const [publications, setPublications] = useState<Publication[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [publisherId, setPublisherId] = useState('')
  const [selectedPublicationId, setSelectedPublicationId] = useState<string | null>(null)

  const loadPublications = async (pubId: string) => {
    try {
      setLoading(true)
      const data = await getPublications(pubId)
      setPublications(data)
      setError(null)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load publications')
    } finally {
      setLoading(false)
    }
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (publisherId) {
      loadPublications(publisherId)
    }
  }

  return (
    <div>
      <div style={{ marginBottom: '2rem' }}>
        <button
          onClick={() => navigate('/')}
          style={{
            padding: '0.5rem 1rem',
            backgroundColor: '#6c757d',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer',
            fontSize: '0.875rem'
          }}
        >
          ← Back to Drafts
        </button>
      </div>

      <h1 style={{ marginBottom: '2rem', color: '#333' }}>Publications</h1>

      <form onSubmit={handleSubmit} style={{
        border: '1px solid #ddd',
        borderRadius: '8px',
        padding: '1.5rem',
        backgroundColor: '#f9f9f9',
        marginBottom: '2rem'
      }}>
        <div style={{ display: 'flex', gap: '1rem', alignItems: 'flex-end' }}>
          <div style={{ flex: 1 }}>
            <label style={{ display: 'block', marginBottom: '0.5rem', fontWeight: 'bold', color: '#555' }}>
              Publisher ID:
            </label>
            <input
              type="text"
              value={publisherId}
              onChange={(e) => setPublisherId(e.target.value)}
              required
              placeholder="Enter publisher ID to view publications"
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #ddd',
                borderRadius: '4px',
                fontSize: '1rem',
                boxSizing: 'border-box'
              }}
            />
          </div>
          <button
            type="submit"
            style={{
              padding: '0.75rem 1.5rem',
              backgroundColor: '#007bff',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              cursor: 'pointer',
              fontSize: '1rem',
              fontWeight: 'bold'
            }}
          >
            Load Publications
          </button>
        </div>
      </form>

      {error && (
        <div style={{
          padding: '1rem',
          backgroundColor: '#f8d7da',
          color: '#721c24',
          borderRadius: '4px',
          marginBottom: '1rem'
        }}>
          {error}
        </div>
      )}

      {loading && publisherId ? (
        <div style={{ textAlign: 'center', padding: '3rem', color: '#666' }}>
          Loading publications...
        </div>
      ) : publisherId && !loading ? (
        <PublicationList
          publications={publications}
          onViewStatus={setSelectedPublicationId}
        />
      ) : (
        <p style={{ textAlign: 'center', color: '#666', padding: '2rem' }}>
          Enter a publisher ID to view publications
        </p>
      )}

      {selectedPublicationId && (
        <PublicationStatus
          publicationId={selectedPublicationId}
          onClose={() => setSelectedPublicationId(null)}
        />
      )}
    </div>
  )
}
