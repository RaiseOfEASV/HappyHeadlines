CREATE TABLE subscribers (
    subscriber_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) NOT NULL UNIQUE,
    name VARCHAR(255),
    is_active BOOLEAN NOT NULL DEFAULT true,
    preferences JSONB NOT NULL DEFAULT '{}',
    subscribed_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    unsubscribed_at TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_subscribers_email ON subscribers(email);
CREATE INDEX idx_subscribers_is_active ON subscribers(is_active);

CREATE TABLE newsletter_history (
    newsletter_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    newsletter_type VARCHAR(50) NOT NULL,  -- 'breaking_news' or 'daily_digest'
    subject VARCHAR(500) NOT NULL,
    content_html TEXT NOT NULL,
    article_ids JSONB NOT NULL,
    sent_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    recipient_count INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_newsletter_history_type ON newsletter_history(newsletter_type);
CREATE INDEX idx_newsletter_history_sent_at ON newsletter_history(sent_at DESC);

CREATE TABLE newsletter_delivery (
    delivery_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    newsletter_id UUID NOT NULL REFERENCES newsletter_history(newsletter_id),
    subscriber_id UUID NOT NULL REFERENCES subscribers(subscriber_id),
    email VARCHAR(255) NOT NULL,
    status VARCHAR(50) NOT NULL,  -- 'queued', 'sent', 'failed', 'bounced'
    sent_at TIMESTAMPTZ NULL,
    opened_at TIMESTAMPTZ NULL,
    error_message TEXT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_newsletter_delivery_newsletter_id ON newsletter_delivery(newsletter_id);
CREATE INDEX idx_newsletter_delivery_subscriber_id ON newsletter_delivery(subscriber_id);
CREATE INDEX idx_newsletter_delivery_status ON newsletter_delivery(status);
