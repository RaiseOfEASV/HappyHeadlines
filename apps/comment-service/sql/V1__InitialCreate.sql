CREATE TABLE Comments (
    comment_id  UUID                     NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    article_id  UUID                     NOT NULL,
    author_id   UUID                     NOT NULL,
    content     TEXT                     NOT NULL,
    created_at  TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_comments_article_id ON Comments (article_id);