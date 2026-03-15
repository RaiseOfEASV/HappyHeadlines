CREATE TABLE publications (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    draft_id UUID NOT NULL,
    article_id UUID NULL,
    publisher_id UUID NOT NULL,
    status VARCHAR(50) NOT NULL,  -- 'Draft', 'Publishing', 'Published', 'Failed'
    title VARCHAR(500) NOT NULL,
    content TEXT NOT NULL,
    continent VARCHAR(50) NULL,
    publish_initiated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    publish_completed_at TIMESTAMPTZ NULL,
    error_message TEXT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE INDEX idx_publications_draft_id ON publications(draft_id);
CREATE INDEX idx_publications_status ON publications(status);
CREATE INDEX idx_publications_publisher_id ON publications(publisher_id);
CREATE INDEX idx_publications_created_at ON publications(created_at DESC);
