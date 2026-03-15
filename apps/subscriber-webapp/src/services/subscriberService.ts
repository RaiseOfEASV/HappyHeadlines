import type { CreateSubscriberRequest, SubscriberDto } from '../types'

const BASE_URL = '/api/subscriber'

export async function subscribe(data: CreateSubscriberRequest): Promise<SubscriberDto> {
  const res = await fetch(`${BASE_URL}/subscribe`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  })
  if (!res.ok) throw new Error('Failed to subscribe')
  return res.json()
}

export async function unsubscribe(email: string): Promise<void> {
  const res = await fetch(`${BASE_URL}/unsubscribe?email=${encodeURIComponent(email)}`, {
    method: 'DELETE',
  })
  if (!res.ok) throw new Error('Failed to unsubscribe')
}
