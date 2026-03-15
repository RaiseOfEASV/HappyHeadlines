import { BrowserRouter, Routes, Route, Link } from 'react-router-dom'
import { Home } from './pages/Home'
import { CreateDraft } from './pages/CreateDraft'
import { EditDraft } from './pages/EditDraft'
import { PublishDraft } from './pages/PublishDraft'
import { Publications } from './pages/Publications'

function App() {
  return (
    <BrowserRouter>
      <div style={{
        minHeight: '100vh',
        backgroundColor: '#f5f5f5'
      }}>
        <nav style={{
          backgroundColor: '#fff',
          borderBottom: '2px solid #007bff',
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
                color: '#007bff',
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
                  fontWeight: '500',
                  transition: 'color 0.2s'
                }}
              >
                Drafts
              </Link>
              <Link
                to="/publications"
                style={{
                  color: '#333',
                  textDecoration: 'none',
                  fontWeight: '500',
                  transition: 'color 0.2s'
                }}
              >
                Publications
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
            <Route path="/create" element={<CreateDraft />} />
            <Route path="/edit/:id" element={<EditDraft />} />
            <Route path="/publish/:id" element={<PublishDraft />} />
            <Route path="/publications" element={<Publications />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  )
}

export default App
