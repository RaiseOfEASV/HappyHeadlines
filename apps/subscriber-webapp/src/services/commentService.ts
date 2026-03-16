import type { Comment, CreateCommentRequest } from '../types'

const BASE_URL = '/api/comment'

export async function fetchComments(articleId: string): Promise<Comment[]> {
  const res = await fetch(`${BASE_URL}/article/${articleId}`)
  if (!res.ok) throw new Error('Failed to fetch comments')
  return res.json()
}

export async function createComment(data: CreateCommentRequest): Promise<Comment> {
  const res = await fetch(BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      ArticleId: data.articleId,
      AuthorId: data.authorId,
      Content: data.content,
    }),
  })
  if (!res.ok) throw new Error('Failed to create comment')
  return res.json()
}
