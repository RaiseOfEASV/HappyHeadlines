export interface Draft {
  id: string
  title: string
  content: string
  authorId: string
  createdAt: string
  updatedAt: string
}

export interface CreateDraftRequest {
  title: string
  content: string
  authorId: string
}

export interface UpdateDraftRequest {
  title: string
  content: string
}

export interface PublishRequest {
  draftId: string
  publisherId: string
  continent: string
}

export interface Publication {
  id: string
  draftId: string
  publisherId: string
  continent: string
  status: 'Publishing' | 'Published' | 'Failed'
  articleId?: string
  errorMessage?: string
  timestamp: string
}
