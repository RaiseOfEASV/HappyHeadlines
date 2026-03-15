import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { DraftEditor } from '../components/DraftEditor'
import type { Draft, UpdateDraftRequest } from '../types'
import { fetchDrafts, updateDraft } from '../services/draftService'

export function EditDraft() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [draft, setDraft] = useState<Draft | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

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

  const handleSubmit = async (id: string, data: UpdateDraftRequest) => {
    await updateDraft(id, data)
    navigate('/')
  }

  if (loading) {
    return (
      <div style={{ textAlign: 'center', padding: '3rem', color: '#666' }}>
        Loading draft...
      </div>
    )
  }

  if (error || !draft) {
    return (
      <div>
        <div style={{
          padding: '1rem',
          backgroundColor: '#f8d7da',
          color: '#721c24',
          borderRadius: '4px',
          marginBottom: '1rem'
        }}>
          {error || 'Draft not found'}
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

      <DraftEditor
        draft={draft}
        onSubmit={handleSubmit}
        onCancel={() => navigate('/')}
      />
    </div>
  )
}
