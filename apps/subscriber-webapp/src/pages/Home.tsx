import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { fetchArticles } from '../services/articleService'
import type { Article } from '../types'

export function Home() {
  const [articles, setArticles] = useState<Article[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [continent, setContinent] = useState('Global')

  const continents = ['Global', 'Europe', 'Africa', 'Asia', 'Australia', 'NorthAmerica', 'SouthAmerica', 'Antarctica']

  useEffect(() => {
    setLoading(true)
    setError(null)
    fetchArticles(continent)
      .then(setArticles)
      .catch(() => setError('Failed to load articles'))
      .finally(() => setLoading(false))
  }, [continent])

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <h1 style={{ margin: 0, color: '#222' }}>Latest News</h1>
        <select
          value={continent}
          onChange={(e) => setContinent(e.target.value)}
          style={{ padding: '0.5rem', borderRadius: '4px', border: '1px solid #ccc', fontSize: '1rem' }}
        >
          {continents.map((c) => (
            <option key={c} value={c}>{c}</option>
          ))}
        </select>
      </div>

      {loading && <p style={{ color: '#666' }}>Loading articles...</p>}
      {error && <p style={{ color: '#dc3545' }}>{error}</p>}

      {!loading && !error && articles.length === 0 && (
        <p style={{ color: '#666' }}>No articles found.</p>
      )}

      <div style={{ display: 'grid', gap: '1.5rem' }}>
        {articles.map((article) => (
          <div
            key={article.articleId}
            style={{
              backgroundColor: '#fff',
              borderRadius: '8px',
              padding: '1.5rem',
              boxShadow: '0 1px 4px rgba(0,0,0,0.1)',
              borderLeft: '4px solid #28a745'
            }}
          >
            <h2 style={{ margin: '0 0 0.5rem', fontSize: '1.3rem' }}>
              <Link
                to={`/article/${article.articleId}`}
                style={{ color: '#222', textDecoration: 'none' }}
              >
                {article.name}
              </Link>
            </h2>
            <p style={{ color: '#555', margin: '0 0 1rem', lineHeight: '1.6' }}>
              {article.content.length > 200 ? article.content.slice(0, 200) + '…' : article.content}
            </p>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <small style={{ color: '#888' }}>
                {new Date(article.timestamp).toLocaleDateString()}
              </small>
              <Link
                to={`/article/${article.articleId}`}
                style={{
                  color: '#28a745',
                  textDecoration: 'none',
                  fontWeight: '500',
                  fontSize: '0.9rem'
                }}
              >
                Read more
              </Link>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
