# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and all csproj files first for layer-cached restore
COPY apps/comment-service/CommentService.sln                         apps/comment-service/
COPY apps/comment-service/src/Comment.Api/Comment.Api.csproj         apps/comment-service/src/Comment.Api/
COPY apps/comment-service/src/Comment.Services/Comment.Services.csproj apps/comment-service/src/Comment.Services/
COPY apps/comment-service/src/Comment.Data/Comment.Data.csproj       apps/comment-service/src/Comment.Data/
COPY apps/comment-service/src/Domain/Domain.csproj                   apps/comment-service/src/Domain/
COPY apps/comment-service/src/Options/Options.csproj                 apps/comment-service/src/Options/
COPY apps/comment-service/src/Handlers/Handlers.csproj               apps/comment-service/src/Handlers/

# Copy shared package csproj files
COPY packages/MessageClient/MessageClient.csproj                                         packages/MessageClient/
COPY packages/SharedContracts/SharedContracts.csproj                                     packages/SharedContracts/
COPY packages/HappyHeadlines.Monitoring/src/HappyHeadlines.Monitoring.csproj             packages/HappyHeadlines.Monitoring/src/

RUN dotnet restore apps/comment-service/CommentService.sln

# Copy the rest of the source and publish
COPY apps/comment-service/src/ apps/comment-service/src/
COPY packages/MessageClient/   packages/MessageClient/
COPY packages/SharedContracts/ packages/SharedContracts/
COPY packages/HappyHeadlines.Monitoring/src/ packages/HappyHeadlines.Monitoring/src/

RUN dotnet publish apps/comment-service/src/Comment.Api/Comment.Api.csproj \
    -c Release \
    -o /app/publish

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Comment.Api.dll"]
