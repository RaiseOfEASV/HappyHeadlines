IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310145229_InitialSchema'
)
BEGIN
    CREATE TABLE [Articles] (
        [ArticleId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [Timestamp] datetime2 NOT NULL,
        CONSTRAINT [PK_Articles] PRIMARY KEY ([ArticleId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310145229_InitialSchema'
)
BEGIN
    CREATE TABLE [ArticleAuthors] (
        [Id] uniqueidentifier NOT NULL,
        [ArticleId] uniqueidentifier NOT NULL,
        [AuthorId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ArticleAuthors] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ArticleAuthors_Articles_ArticleId] FOREIGN KEY ([ArticleId]) REFERENCES [Articles] ([ArticleId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310145229_InitialSchema'
)
BEGIN
    CREATE INDEX [IX_ArticleAuthors_ArticleId] ON [ArticleAuthors] ([ArticleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310145229_InitialSchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260310145229_InitialSchema', N'9.0.13');
END;

COMMIT;
GO

