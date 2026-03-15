FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy solution and project files
COPY apps/subscriber-service/SubscriberService.sln apps/subscriber-service/
COPY apps/subscriber-service/src/Domain/Domain.csproj apps/subscriber-service/src/Domain/
COPY apps/subscriber-service/src/Options/Options.csproj apps/subscriber-service/src/Options/
COPY apps/subscriber-service/src/Subscriber.Data/Subscriber.Data.csproj apps/subscriber-service/src/Subscriber.Data/
COPY apps/subscriber-service/src/Subscriber.Services/Subscriber.Services.csproj apps/subscriber-service/src/Subscriber.Services/
COPY apps/subscriber-service/src/Subscriber.Api/Subscriber.Api.csproj apps/subscriber-service/src/Subscriber.Api/
COPY packages/HappyHeadlines.Monitoring/src/HappyHeadlines.Monitoring.csproj packages/HappyHeadlines.Monitoring/src/

# Restore dependencies using solution
RUN dotnet restore apps/subscriber-service/SubscriberService.sln

# Copy everything else
COPY apps/subscriber-service/src/ apps/subscriber-service/src/
COPY packages/HappyHeadlines.Monitoring/src/ packages/HappyHeadlines.Monitoring/src/

# Build and publish
WORKDIR /src/apps/subscriber-service
RUN dotnet publish SubscriberService.sln -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Subscriber.Api.dll"]
