CREATE TABLE drafts (
    id          UUID            NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    title       VARCHAR(500)    NOT NULL,
    content     TEXT            NOT NULL,
    author_id   UUID            NOT NULL,
    created_at  TIMESTAMPTZ     NOT NULL DEFAULT now(),
    updated_at  TIMESTAMPTZ     NOT NULL DEFAULT now()
);
