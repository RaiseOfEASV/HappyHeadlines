FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy solution and project files
COPY apps/publisher-service/PublisherService.sln apps/publisher-service/
COPY apps/publisher-service/src/Domain/Domain.csproj apps/publisher-service/src/Domain/
COPY apps/publisher-service/src/Options/Options.csproj apps/publisher-service/src/Options/
COPY apps/publisher-service/src/Publisher.Data/Publisher.Data.csproj apps/publisher-service/src/Publisher.Data/
COPY apps/publisher-service/src/Publisher.Services/Publisher.Services.csproj apps/publisher-service/src/Publisher.Services/
COPY apps/publisher-service/src/Publisher.Api/Publisher.Api.csproj apps/publisher-service/src/Publisher.Api/
COPY packages/HappyHeadlines.Monitoring/src/HappyHeadlines.Monitoring.csproj packages/HappyHeadlines.Monitoring/src/

# Restore dependencies using solution
RUN dotnet restore apps/publisher-service/PublisherService.sln

# Copy everything else
COPY apps/publisher-service/src/ apps/publisher-service/src/
COPY packages/HappyHeadlines.Monitoring/src/ packages/HappyHeadlines.Monitoring/src/

# Build and publish
WORKDIR /src/apps/publisher-service
RUN dotnet publish PublisherService.sln -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Publisher.Api.dll"]
