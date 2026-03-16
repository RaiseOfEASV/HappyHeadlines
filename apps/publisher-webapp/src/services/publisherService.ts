import type { PublishRequest, Publication } from '../types'

const BASE_URL = '/api/publisher'

export async function publishDraft(data: PublishRequest): Promise<Publication> {
  const res = await fetch(`${BASE_URL}/publish`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  })
  if (!res.ok) throw new Error('Failed to publish draft')
  return res.json()
}

export async function getPublication(id: string): Promise<Publication> {
  const res = await fetch(`${BASE_URL}/publications/${id}`)
  if (!res.ok) throw new Error('Failed to fetch publication')
  return res.json()
}

export async function getPublications(publisherId: string): Promise<Publication[]> {
  const res = await fetch(`${BASE_URL}/publications?publisherId=${publisherId}`)
  if (!res.ok) throw new Error('Failed to fetch publications')
  return res.json()
}
