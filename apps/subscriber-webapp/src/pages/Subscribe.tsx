import { useState } from 'react'
import { subscribe } from '../services/subscriberService'

export function Subscribe() {
  const [email, setEmail] = useState('')
  const [name, setName] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [success, setSuccess] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!email.trim()) return

    setSubmitting(true)
    setError(null)
    setSuccess(false)

    try {
      await subscribe({ email, name: name.trim() || undefined })
      setSuccess(true)
      setEmail('')
      setName('')
    } catch {
      setError('Failed to subscribe. Please try again.')
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div style={{ maxWidth: '500px', margin: '0 auto' }}>
      <div style={{
        backgroundColor: '#fff',
        borderRadius: '8px',
        padding: '2rem',
        boxShadow: '0 1px 4px rgba(0,0,0,0.1)'
      }}>
        <h1 style={{ margin: '0 0 0.5rem', color: '#222' }}>Newsletter</h1>
        <p style={{ color: '#555', marginBottom: '1.5rem' }}>
          Get the latest positive news delivered straight to your inbox every day.
        </p>

        {success ? (
          <div style={{
            backgroundColor: '#d4edda',
            border: '1px solid #c3e6cb',
            borderRadius: '6px',
            padding: '1rem',
            color: '#155724'
          }}>
            <strong>You're subscribed!</strong> Thank you for joining HappyHeadlines.
          </div>
        ) : (
          <form onSubmit={(e) => { void handleSubmit(e) }}>
            <div style={{ marginBottom: '1rem' }}>
              <label style={{ display: 'block', marginBottom: '0.25rem', fontWeight: '500' }}>
                Name (optional)
              </label>
              <input
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
                placeholder="Your name"
                style={{
                  width: '100%',
                  padding: '0.6rem',
                  borderRadius: '4px',
                  border: '1px solid #ccc',
                  fontSize: '1rem',
                  boxSizing: 'border-box'
                }}
              />
            </div>
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', marginBottom: '0.25rem', fontWeight: '500' }}>
                Email address
              </label>
              <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="you@example.com"
                required
                style={{
                  width: '100%',
                  padding: '0.6rem',
                  borderRadius: '4px',
                  border: '1px solid #ccc',
                  fontSize: '1rem',
                  boxSizing: 'border-box'
                }}
              />
            </div>
            {error && <p style={{ color: '#dc3545', margin: '0 0 1rem' }}>{error}</p>}
            <button
              type="submit"
              disabled={submitting}
              style={{
                width: '100%',
                backgroundColor: '#28a745',
                color: '#fff',
                border: 'none',
                padding: '0.75rem',
                borderRadius: '4px',
                fontSize: '1rem',
                cursor: submitting ? 'not-allowed' : 'pointer',
                opacity: submitting ? 0.7 : 1
              }}
            >
              {submitting ? 'Subscribing...' : 'Subscribe to newsletter'}
            </button>
          </form>
        )}
      </div>
    </div>
  )
}
