import type { Draft, CreateDraftRequest, UpdateDraftRequest } from '../types'

const BASE_URL = '/api/draft'

export async function fetchDrafts(): Promise<Draft[]> {
  const res = await fetch(BASE_URL)
  if (!res.ok) throw new Error('Failed to fetch drafts')
  return res.json()
}

export async function fetchDraft(id: string): Promise<Draft> {
  const res = await fetch(`${BASE_URL}/${id}`)
  if (!res.ok) throw new Error('Failed to fetch draft')
  return res.json()
}

export async function createDraft(data: CreateDraftRequest): Promise<Draft> {
  const res = await fetch(BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  })
  if (!res.ok) throw new Error('Failed to create draft')
  return res.json()
}

export async function updateDraft(id: string, data: UpdateDraftRequest): Promise<Draft> {
  const res = await fetch(`${BASE_URL}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  })
  if (!res.ok) throw new Error('Failed to update draft')
  return res.json()
}

export async function deleteDraft(id: string): Promise<void> {
  const res = await fetch(`${BASE_URL}/${id}`, { method: 'DELETE' })
  if (!res.ok) throw new Error('Failed to delete draft')
}
