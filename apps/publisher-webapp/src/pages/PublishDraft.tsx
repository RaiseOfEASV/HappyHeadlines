import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { PublicationStatus } from '../components/PublicationStatus'
import type { Draft } from '../types'
import { fetchDrafts } from '../services/draftService'
import { publishDraft } from '../services/publisherService'

export function PublishDraft() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [draft, setDraft] = useState<Draft | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [publisherId, setPublisherId] = useState('')
  const [continent, setContinent] = useState('Europe')
  const [publishing, setPublishing] = useState(false)
  const [publicationId, setPublicationId] = useState<string | null>(null)

  useEffect(() => {
    const loadDraft = async () => {
      try {
        const drafts = await fetchDrafts()
        const found = drafts.find(d => d.id === id)
        if (found) {
          setDraft(found)
        } else {
          setError('Draft not found')
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load draft')
      } finally {
        setLoading(false)
      }
    }

    loadDraft()
  }, [id])

  const handlePublish = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!draft) return

    setPublishing(true)
    setError(null)

    try {
      const publication = await publishDraft({
        draftId: draft.id,
        publisherId,
        continent
      })
      setPublicationId(publication.id)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to publish draft')
    } finally {
      setPublishing(false)
    }
  }

  if (loading) {
    return (
      <div style={{ textAlign: 'center', padding: '3rem', color: '#666' }}>
        Loading draft...
      </div>
    )
  }

  if (error && !draft) {
    return (
      <div>
        <div style={{
          padding: '1rem',
          backgroundColor: '#f8d7da',
          color: '#721c24',
          borderRadius: '4px',
          marginBottom: '1rem'
        }}>
          {error}
        </div>
        <button
          onClick={() => navigate('/')}
          style={{
            padding: '0.75rem 1.5rem',
            backgroundColor: '#6c757d',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer',
            fontSize: '1rem'
          }}
        >
          Back to Drafts
        </button>
      </div>
    )
  }

  if (publicationId) {
    return (
      <PublicationStatus
        publicationId={publicationId}
        onClose={() => navigate('/')}
      />
    )
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

      <div style={{
        border: '1px solid #ddd',
        borderRadius: '8px',
        padding: '1.5rem',
        backgroundColor: '#f9f9f9'
      }}>
        <h2 style={{ marginTop: 0, marginBottom: '1.5rem', color: '#333' }}>
          Publish Draft
        </h2>

        {draft && (
          <div style={{
            padding: '1rem',
            backgroundColor: '#e7f3ff',
            borderRadius: '4px',
            marginBottom: '1.5rem'
          }}>
            <h3 style={{ marginTop: 0, marginBottom: '0.5rem' }}>{draft.title}</h3>
            <p style={{ margin: 0, color: '#555', whiteSpace: 'pre-wrap' }}>{draft.content}</p>
            <div style={{ marginTop: '0.5rem', fontSize: '0.875rem', color: '#888' }}>
              Author: {draft.authorId}
            </div>
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

        <form onSubmit={handlePublish}>
          <div style={{ marginBottom: '1rem' }}>
            <label style={{ display: 'block', marginBottom: '0.5rem', fontWeight: 'bold', color: '#555' }}>
              Publisher ID:
            </label>
            <input
              type="text"
              value={publisherId}
              onChange={(e) => setPublisherId(e.target.value)}
              required
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

          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{ display: 'block', marginBottom: '0.5rem', fontWeight: 'bold', color: '#555' }}>
              Continent:
            </label>
            <select
              value={continent}
              onChange={(e) => setContinent(e.target.value)}
              required
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #ddd',
                borderRadius: '4px',
                fontSize: '1rem',
                boxSizing: 'border-box'
              }}
            >
              <option value="Europe">Europe</option>
              <option value="America">America</option>
              <option value="Asia">Asia</option>
              <option value="Africa">Africa</option>
              <option value="Oceania">Oceania</option>
            </select>
          </div>

          <button
            type="submit"
            disabled={publishing}
            style={{
              padding: '0.75rem 1.5rem',
              backgroundColor: publishing ? '#ccc' : '#28a745',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              cursor: publishing ? 'not-allowed' : 'pointer',
              fontSize: '1rem',
              fontWeight: 'bold'
            }}
          >
            {publishing ? 'Publishing...' : 'Publish Draft'}
          </button>
        </form>
      </div>
    </div>
  )
}
