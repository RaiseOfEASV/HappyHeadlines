# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and all csproj files first for layer-cached restore
COPY CommentService.sln ./
COPY src/Comment.Api/Comment.Api.csproj         src/Comment.Api/
COPY src/Comment.Services/Comment.Services.csproj src/Comment.Services/
COPY src/Comment.Data/Comment.Data.csproj        src/Comment.Data/
COPY src/Domain/Domain.csproj                    src/Domain/
COPY src/Options/Options.csproj                  src/Options/

RUN dotnet restore

# Copy the rest of the source and publish
COPY src/ src/
RUN dotnet publish src/Comment.Api/Comment.Api.csproj \
    -c Release \
    -o /app/publish

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Comment.Api.dll"]