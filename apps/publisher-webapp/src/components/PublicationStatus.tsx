import { useEffect, useState } from 'react'
import type { Publication } from '../types'
import { getPublication } from '../services/publisherService'

interface Props {
  publicationId: string
  onClose: () => void
}

export function PublicationStatus({ publicationId, onClose }: Props) {
  const [publication, setPublication] = useState<Publication | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let intervalId: number | undefined

    const fetchStatus = async () => {
      try {
        const pub = await getPublication(publicationId)
        setPublication(pub)
        setLoading(false)

        if (pub.status === 'Published' || pub.status === 'Failed') {
          if (intervalId) clearInterval(intervalId)
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to fetch status')
        setLoading(false)
        if (intervalId) clearInterval(intervalId)
      }
    }

    fetchStatus()
    intervalId = window.setInterval(fetchStatus, 2000)

    return () => {
      if (intervalId) clearInterval(intervalId)
    }
  }, [publicationId])

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Published': return '#28a745'
      case 'Failed': return '#dc3545'
      case 'Publishing': return '#ffc107'
      default: return '#6c757d'
    }
  }

  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'rgba(0,0,0,0.5)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      zIndex: 1000
    }}>
      <div style={{
        backgroundColor: 'white',
        padding: '2rem',
        borderRadius: '8px',
        maxWidth: '500px',
        width: '90%'
      }}>
        <h3 style={{ marginTop: 0, marginBottom: '1.5rem' }}>
          Publication Status
        </h3>

        {loading && !publication && (
          <div style={{ textAlign: 'center', padding: '2rem' }}>
            Loading...
          </div>
        )}

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

        {publication && (
          <div>
            <div style={{ marginBottom: '1rem' }}>
              <strong>Status:</strong>
              <span style={{
                marginLeft: '0.5rem',
                padding: '0.25rem 0.75rem',
                backgroundColor: getStatusColor(publication.status),
                color: 'white',
                borderRadius: '4px',
                fontSize: '0.875rem'
              }}>
                {publication.status}
              </span>
            </div>

            <div style={{ marginBottom: '0.5rem' }}>
              <strong>Publication ID:</strong> {publication.id}
            </div>

            <div style={{ marginBottom: '0.5rem' }}>
              <strong>Draft ID:</strong> {publication.draftId}
            </div>

            <div style={{ marginBottom: '0.5rem' }}>
              <strong>Publisher:</strong> {publication.publisherId}
            </div>

            <div style={{ marginBottom: '0.5rem' }}>
              <strong>Continent:</strong> {publication.continent}
            </div>

            {publication.articleId && (
              <div style={{ marginBottom: '0.5rem' }}>
                <strong>Article ID:</strong> {publication.articleId}
              </div>
            )}

            {publication.errorMessage && (
              <div style={{
                marginTop: '1rem',
                padding: '1rem',
                backgroundColor: '#f8d7da',
                color: '#721c24',
                borderRadius: '4px'
              }}>
                <strong>Error:</strong> {publication.errorMessage}
              </div>
            )}

            <div style={{ marginTop: '0.5rem', fontSize: '0.875rem', color: '#888' }}>
              {new Date(publication.timestamp).toLocaleString()}
            </div>
          </div>
        )}

        <button
          onClick={onClose}
          style={{
            marginTop: '1.5rem',
            padding: '0.75rem 1.5rem',
            backgroundColor: '#6c757d',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer',
            fontSize: '1rem',
            width: '100%'
          }}
        >
          Close
        </button>
      </div>
    </div>
  )
}
