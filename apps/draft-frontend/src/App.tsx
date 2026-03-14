import { useEffect, useState } from 'react'
import { fetchDrafts, createDraft, updateDraft, deleteDraft } from './api/drafts'
import type { Draft, CreateDraftRequest, UpdateDraftRequest } from './api/drafts'
import { DraftForm } from './components/DraftForm'
import { DraftList } from './components/DraftList'
import styles from './App.module.css'

export default function App() {
  const [drafts, setDrafts] = useState<Draft[]>([])
  const [editing, setEditing] = useState<Draft | null>(null)
  const [error, setError] = useState<string | null>(null)

  async function load() {
    try {
      setDrafts(await fetchDrafts())
    } catch {
      setError('Could not reach the draft service.')
    }
  }

  useEffect(() => { load() }, [])

  async function handleSubmit(data: CreateDraftRequest | UpdateDraftRequest) {
    try {
      if (editing) {
        await updateDraft(editing.id, data as UpdateDraftRequest)
        setEditing(null)
      } else {
        await createDraft(data as CreateDraftRequest)
      }
      await load()
      setError(null)
    } catch {
      setError('Failed to save draft.')
    }
  }

  async function handleDelete(id: string) {
    if (!confirm('Delete this draft?')) return
    try {
      await deleteDraft(id)
      await load()
      setError(null)
    } catch {
      setError('Failed to delete draft.')
    }
  }

  return (
    <div className={styles.page}>
      <header className={styles.header}>
        <h1 className={styles.logo}>Happy Headlines</h1>
        <span className={styles.subtitle}>Draft editor</span>
      </header>

      <main className={styles.main}>
        <section className={styles.section}>
          <DraftForm editing={editing} onSubmit={handleSubmit} onCancel={() => setEditing(null)} />
        </section>

        {error && <p className={styles.error}>{error}</p>}

        <section className={styles.section}>
          <h2>All drafts</h2>
          <DraftList drafts={drafts} onEdit={setEditing} onDelete={handleDelete} />
        </section>
      </main>
    </div>
  )
}
