# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and all csproj files first for layer-cached restore
COPY apps/profanity-service/ProfanityService.sln                                      apps/profanity-service/
COPY apps/profanity-service/src/Profanity.Api/Profanity.Api.csproj                    apps/profanity-service/src/Profanity.Api/
COPY apps/profanity-service/src/Profanity.Services/Profanity.Services.csproj          apps/profanity-service/src/Profanity.Services/
COPY apps/profanity-service/src/Profanity.Data/Profanity.Data.csproj                  apps/profanity-service/src/Profanity.Data/
COPY apps/profanity-service/src/Domain/Domain.csproj                                  apps/profanity-service/src/Domain/
COPY apps/profanity-service/src/Options/Options.csproj                                apps/profanity-service/src/Options/
COPY packages/HappyHeadlines.Logging/src/HappyHeadlines.Logging.csproj               packages/HappyHeadlines.Logging/src/

RUN dotnet restore apps/profanity-service/ProfanityService.sln

# Copy the rest of the source and publish
COPY apps/profanity-service/src/    apps/profanity-service/src/
COPY packages/HappyHeadlines.Logging/src/  packages/HappyHeadlines.Logging/src/

RUN dotnet publish apps/profanity-service/src/Profanity.Api/Profanity.Api.csproj \
    -c Release \
    -o /app/publish

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Profanity.Api.dll"]
