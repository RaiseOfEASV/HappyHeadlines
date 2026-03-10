CREATE TABLE [Articles] (
    [ArticleId] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    CONSTRAINT [PK_Articles] PRIMARY KEY ([ArticleId])
    );

CREATE TABLE [ArticleAuthors] (
    [Id] uniqueidentifier NOT NULL,
    [ArticleId] uniqueidentifier NOT NULL,
    [AuthorId] uniqueidentifier NOT NULL,
     CONSTRAINT [PK_ArticleAuthors] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ArticleAuthors_Articles_ArticleId] FOREIGN KEY ([ArticleId]) REFERENCES [Articles] ([ArticleId]) ON DELETE CASCADE
    );

CREATE INDEX [IX_ArticleAuthors_ArticleId] ON [ArticleAuthors] ([ArticleId]);
