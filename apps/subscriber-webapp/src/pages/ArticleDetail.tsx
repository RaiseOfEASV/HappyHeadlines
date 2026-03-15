import { useEffect, useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { fetchArticle } from '../services/articleService'
import { fetchComments, createComment } from '../services/commentService'
import type { Article, Comment } from '../types'

export function ArticleDetail() {
  const { id } = useParams<{ id: string }>()
  const [article, setArticle] = useState<Article | null>(null)
  const [comments, setComments] = useState<Comment[]>([])
  const [loadingArticle, setLoadingArticle] = useState(true)
  const [loadingComments, setLoadingComments] = useState(true)
  const [articleError, setArticleError] = useState<string | null>(null)
  const [commentContent, setCommentContent] = useState('')
  const [submitting, setSubmitting] = useState(false)

  const getOrCreateAuthorId = () => {
    const key = 'hh_author_id'
    let id = localStorage.getItem(key)
    if (!id) {
      id = crypto.randomUUID()
      localStorage.setItem(key, id)
    }
    return id
  }
  const [submitError, setSubmitError] = useState<string | null>(null)
  const [submitSuccess, setSubmitSuccess] = useState(false)

  useEffect(() => {
    if (!id) return
    fetchArticle(id)
      .then(setArticle)
      .catch(() => setArticleError('Failed to load article'))
      .finally(() => setLoadingArticle(false))

    fetchComments(id)
      .then(setComments)
      .catch(() => {})
      .finally(() => setLoadingComments(false))
  }, [id])

  const handleCommentSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!id || !commentContent.trim()) return

    setSubmitting(true)
    setSubmitError(null)
    setSubmitSuccess(false)

    try {
      const newComment = await createComment({
        articleId: id,
        authorId: getOrCreateAuthorId(),
        content: commentContent,
      })
      setComments((prev) => [...prev, newComment])
      setCommentContent('')
      setSubmitSuccess(true)
    } catch {
      setSubmitError('Failed to post comment')
    } finally {
      setSubmitting(false)
    }
  }

  if (loadingArticle) return <p style={{ color: '#666' }}>Loading article...</p>
  if (articleError) return <p style={{ color: '#dc3545' }}>{articleError}</p>
  if (!article) return <p style={{ color: '#666' }}>Article not found.</p>

  return (
    <div>
      <Link to="/" style={{ color: '#28a745', textDecoration: 'none', fontSize: '0.9rem' }}>
        &larr; Back to news
      </Link>

      <article style={{
        backgroundColor: '#fff',
        borderRadius: '8px',
        padding: '2rem',
        marginTop: '1rem',
        boxShadow: '0 1px 4px rgba(0,0,0,0.1)'
      }}>
        <h1 style={{ margin: '0 0 0.5rem', color: '#222' }}>{article.name}</h1>
        <small style={{ color: '#888', display: 'block', marginBottom: '1.5rem' }}>
          {new Date(article.timestamp).toLocaleDateString()}
        </small>
        <p style={{ color: '#333', lineHeight: '1.8', whiteSpace: 'pre-wrap' }}>{article.content}</p>
      </article>

      <section style={{ marginTop: '2rem' }}>
        <h2 style={{ color: '#222', marginBottom: '1rem' }}>
          Comments {!loadingComments && `(${comments.length})`}
        </h2>

        {loadingComments && <p style={{ color: '#666' }}>Loading comments...</p>}

        {!loadingComments && comments.length === 0 && (
          <p style={{ color: '#666' }}>No comments yet. Be the first!</p>
        )}

        <div style={{ display: 'grid', gap: '1rem', marginBottom: '2rem' }}>
          {comments.map((comment) => (
            <div
              key={comment.commentId}
              style={{
                backgroundColor: '#fff',
                borderRadius: '6px',
                padding: '1rem',
                boxShadow: '0 1px 3px rgba(0,0,0,0.08)'
              }}
            >
              <p style={{ margin: '0 0 0.5rem', color: '#333' }}>{comment.content}</p>
              <small style={{ color: '#888' }}>
                {comment.authorId} &middot; {new Date(comment.createdAt).toLocaleDateString()}
              </small>
            </div>
          ))}
        </div>

        <div style={{
          backgroundColor: '#fff',
          borderRadius: '8px',
          padding: '1.5rem',
          boxShadow: '0 1px 4px rgba(0,0,0,0.1)'
        }}>
          <h3 style={{ margin: '0 0 1rem', color: '#222' }}>Leave a comment</h3>
          <form onSubmit={(e) => { void handleCommentSubmit(e) }}>
            <div style={{ marginBottom: '1rem' }}>
              <label style={{ display: 'block', marginBottom: '0.25rem', fontWeight: '500' }}>
                Comment
              </label>
              <textarea
                value={commentContent}
                onChange={(e) => setCommentContent(e.target.value)}
                placeholder="Write your comment..."
                required
                rows={4}
                style={{
                  width: '100%',
                  padding: '0.5rem',
                  borderRadius: '4px',
                  border: '1px solid #ccc',
                  fontSize: '1rem',
                  resize: 'vertical',
                  boxSizing: 'border-box'
                }}
              />
            </div>
            {submitError && <p style={{ color: '#dc3545', margin: '0 0 0.5rem' }}>{submitError}</p>}
            {submitSuccess && <p style={{ color: '#28a745', margin: '0 0 0.5rem' }}>Comment posted!</p>}
            <button
              type="submit"
              disabled={submitting}
              style={{
                backgroundColor: '#28a745',
                color: '#fff',
                border: 'none',
                padding: '0.6rem 1.5rem',
                borderRadius: '4px',
                fontSize: '1rem',
                cursor: submitting ? 'not-allowed' : 'pointer',
                opacity: submitting ? 0.7 : 1
              }}
            >
              {submitting ? 'Posting...' : 'Post comment'}
            </button>
          </form>
        </div>
      </section>
    </div>
  )
}
