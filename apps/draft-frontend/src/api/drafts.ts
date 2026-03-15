export interface Draft {
  id: string
  title: string
  content: string
  authorId: string
  createdAt: string
  updatedAt: string
}

export interface CreateDraftRequest {
  title: string
  content: string
  authorId: string
}

export interface UpdateDraftRequest {
  title: string
  content: string
}

const BASE = '/Draft'

export async function fetchDrafts(): Promise<Draft[]> {
  const res = await fetch(BASE)
  if (!res.ok) throw new Error('Failed to fetch drafts')
  return res.json()
}

export async function createDraft(data: CreateDraftRequest): Promise<Draft> {
  const res = await fetch(BASE, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  })
  if (!res.ok) throw new Error('Failed to create draft')
  return res.json()
}

export async function updateDraft(id: string, data: UpdateDraftRequest): Promise<Draft> {
  const res = await fetch(`${BASE}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  })
  if (!res.ok) throw new Error('Failed to update draft')
  return res.json()
}

export async function deleteDraft(id: string): Promise<void> {
  const res = await fetch(`${BASE}/${id}`, { method: 'DELETE' })
  if (!res.ok) throw new Error('Failed to delete draft')
}
