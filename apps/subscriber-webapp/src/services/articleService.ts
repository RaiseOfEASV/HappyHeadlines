import type { Article } from '../types'

const BASE_URL = '/api/articles'

export async function fetchArticles(continent: string = 'Global'): Promise<Article[]> {
  const res = await fetch(`${BASE_URL}/${continent}`)
  if (!res.ok) throw new Error('Failed to fetch articles')
  return res.json()
}

export async function fetchArticle(id: string): Promise<Article> {
  const res = await fetch(`${BASE_URL}/${id}`)
  if (!res.ok) throw new Error('Failed to fetch article')
  return res.json()
}
