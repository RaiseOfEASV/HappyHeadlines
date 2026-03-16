import { BrowserRouter, Routes, Route, Link } from 'react-router-dom'
import { Home } from './pages/Home'
import { ArticleDetail } from './pages/ArticleDetail'
import { Subscribe } from './pages/Subscribe'

function App() {
  return (
    <BrowserRouter>
      <div style={{
        minHeight: '100vh',
        backgroundColor: '#f5f5f5'
      }}>
        <nav style={{
          backgroundColor: '#fff',
          borderBottom: '2px solid #28a745',
          padding: '1rem 2rem',
          marginBottom: '2rem',
          boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
        }}>
          <div style={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            maxWidth: '1200px',
            margin: '0 auto'
          }}>
            <Link
              to="/"
              style={{
                fontSize: '1.5rem',
                fontWeight: 'bold',
                color: '#28a745',
                textDecoration: 'none'
              }}
            >
              HappyHeadlines
            </Link>
            <div style={{ display: 'flex', gap: '1.5rem' }}>
              <Link
                to="/"
                style={{
                  color: '#333',
                  textDecoration: 'none',
                  fontWeight: '500'
                }}
              >
                News
              </Link>
              <Link
                to="/subscribe"
                style={{
                  color: '#333',
                  textDecoration: 'none',
                  fontWeight: '500'
                }}
              >
                Newsletter
              </Link>
            </div>
          </div>
        </nav>

        <main style={{
          maxWidth: '1200px',
          margin: '0 auto',
          padding: '0 2rem 2rem'
        }}>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/article/:id" element={<ArticleDetail />} />
            <Route path="/subscribe" element={<Subscribe />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  )
}

export default App
