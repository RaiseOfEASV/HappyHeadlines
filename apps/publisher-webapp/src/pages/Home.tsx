import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { DraftList } from '../components/DraftList'
import type { Draft } from '../types'
import { fetchDrafts, deleteDraft } from '../services/draftService'

export function Home() {
  const navigate = useNavigate()
  const [drafts, setDrafts] = useState<Draft[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const loadDrafts = async () => {
    try {
      setLoading(true)
      const data = await fetchDrafts()
      setDrafts(data)
      setError(null)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load drafts')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadDrafts()
  }, [])

  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this draft?')) return

    try {
      await deleteDraft(id)
      await loadDrafts()
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Failed to delete draft')
    }
  }

  const handleEdit = (draft: Draft) => {
    navigate(`/edit/${draft.id}`)
  }

  const handlePublish = (draft: Draft) => {
    navigate(`/publish/${draft.id}`)
  }

  return (
    <div>
      <div style={{
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginBottom: '2rem'
      }}>
        <h1 style={{ margin: 0, color: '#333' }}>My Drafts</h1>
        <button
          onClick={() => navigate('/create')}
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
          Create New Draft
        </button>
      </div>

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

      {loading ? (
        <div style={{ textAlign: 'center', padding: '3rem', color: '#666' }}>
          Loading drafts...
        </div>
      ) : (
        <DraftList
          drafts={drafts}
          onEdit={handleEdit}
          onDelete={handleDelete}
          onPublish={handlePublish}
        />
      )}
    </div>
  )
}
