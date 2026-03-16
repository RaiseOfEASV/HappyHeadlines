FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and csproj files for layer-cached restore
COPY apps/article-service/ArticleService.sln                                              apps/article-service/
COPY apps/article-service/src/Article.Api/Article.Api.csproj                             apps/article-service/src/Article.Api/
COPY apps/article-service/src/Article.Services/Article.Services.csproj                   apps/article-service/src/Article.Services/
COPY apps/article-service/src/Article.Data/Article.Data.csproj                           apps/article-service/src/Article.Data/
COPY apps/article-service/src/Domain/Domain.csproj                                       apps/article-service/src/Domain/
COPY apps/article-service/src/models/models.csproj                                       apps/article-service/src/models/
COPY apps/article-service/src/Options/Options.csproj                                     apps/article-service/src/Options/
COPY packages/HappyHeadlines.Monitoring/src/HappyHeadlines.Monitoring.csproj             packages/HappyHeadlines.Monitoring/src/

RUN dotnet restore apps/article-service/ArticleService.sln

# Copy source
COPY apps/article-service/src/ apps/article-service/src/
COPY packages/HappyHeadlines.Monitoring/src/ packages/HappyHeadlines.Monitoring/src/

RUN dotnet publish apps/article-service/src/Article.Api/Article.Api.csproj \
    -c Release \
    -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Article.Api.dll"]
