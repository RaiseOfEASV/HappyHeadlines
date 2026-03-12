CREATE TABLE profanity_words (
    id    UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    word  VARCHAR(255) NOT NULL,
    CONSTRAINT uq_profanity_words_word UNIQUE (word)
);
