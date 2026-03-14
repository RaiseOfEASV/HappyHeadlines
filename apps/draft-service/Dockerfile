# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and all csproj files first for layer-cached restore
COPY apps/draft-service/DraftService.sln                                    apps/draft-service/
COPY apps/draft-service/src/Draft.Api/Draft.Api.csproj                      apps/draft-service/src/Draft.Api/
COPY apps/draft-service/src/Draft.Services/Draft.Services.csproj            apps/draft-service/src/Draft.Services/
COPY apps/draft-service/src/Draft.Data/Draft.Data.csproj                    apps/draft-service/src/Draft.Data/
COPY apps/draft-service/src/Domain/Domain.csproj                            apps/draft-service/src/Domain/
COPY apps/draft-service/src/Options/Options.csproj                          apps/draft-service/src/Options/
COPY packages/HappyHeadlines.Monitoring/src/HappyHeadlines.Monitoring.csproj    packages/HappyHeadlines.Monitoring/src/

RUN dotnet restore apps/draft-service/DraftService.sln

# Copy the rest of the source and publish
COPY apps/draft-service/src/    apps/draft-service/src/
COPY packages/HappyHeadlines.Monitoring/src/  packages/HappyHeadlines.Monitoring/src/

RUN dotnet publish apps/draft-service/src/Draft.Api/Draft.Api.csproj \
    -c Release \
    -o /app/publish

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Draft.Api.dll"]
