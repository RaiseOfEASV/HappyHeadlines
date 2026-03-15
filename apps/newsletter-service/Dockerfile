FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy solution and project files
COPY apps/newsletter-service/NewsletterService.sln apps/newsletter-service/
COPY apps/newsletter-service/src/Domain/Domain.csproj apps/newsletter-service/src/Domain/
COPY apps/newsletter-service/src/Options/Options.csproj apps/newsletter-service/src/Options/
COPY apps/newsletter-service/src/Newsletter.Data/Newsletter.Data.csproj apps/newsletter-service/src/Newsletter.Data/
COPY apps/newsletter-service/src/Newsletter.Services/Newsletter.Services.csproj apps/newsletter-service/src/Newsletter.Services/
COPY apps/newsletter-service/src/Newsletter.Api/Newsletter.Api.csproj apps/newsletter-service/src/Newsletter.Api/
COPY packages/HappyHeadlines.Monitoring/src/HappyHeadlines.Monitoring.csproj packages/HappyHeadlines.Monitoring/src/

# Restore dependencies using solution
RUN dotnet restore apps/newsletter-service/NewsletterService.sln

# Copy everything else
COPY apps/newsletter-service/src/ apps/newsletter-service/src/
COPY packages/HappyHeadlines.Monitoring/src/ packages/HappyHeadlines.Monitoring/src/

# Build and publish
WORKDIR /src/apps/newsletter-service
RUN dotnet publish NewsletterService.sln -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Newsletter.Api.dll"]
