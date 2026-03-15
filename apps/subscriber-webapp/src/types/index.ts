export interface Article {
  articleId: string
  name: string
  content: string
  timestamp: string
  authorIds: string[]
}

export interface Comment {
  commentId: string
  articleId: string
  authorId: string
  content: string
  createdAt: string
}

export interface CreateCommentRequest {
  articleId: string
  authorId: string
  content: string
}

export interface CreateSubscriberRequest {
  email: string
  name?: string
}

export interface SubscriberDto {
  subscriberId: string
  email: string
  name?: string
  isActive: boolean
  subscribedAt: string
}
