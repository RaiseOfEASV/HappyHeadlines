import { useNavigate } from 'react-router-dom'
import { DraftForm } from '../components/DraftForm'
import { createDraft } from '../services/draftService'
import type { CreateDraftRequest } from '../types'

export function CreateDraft() {
  const navigate = useNavigate()

  const handleSubmit = async (data: CreateDraftRequest) => {
    await createDraft(data)
    navigate('/')
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

      <DraftForm
        onSubmit={handleSubmit}
        onCancel={() => navigate('/')}
      />
    </div>
  )
}
