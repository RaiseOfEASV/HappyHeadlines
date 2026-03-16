workspace "HappyHeadlines" "Distributed microservices platform for article management, comments, subscriptions, and newsletters" {

    model {

        # ── People ──────────────────────────────────────────────
        publisher = person "Publisher" "Creates and publishes news articles via the publisher web app"
        subscriber = person "Subscriber" "Reads articles, posts comments, and manages newsletter preferences"
        newsletterRecipient = person "Newsletter Recipient" "Receives the daily email digest"

        # ── External Systems ─────────────────────────────────────
        smtp = softwareSystem "SMTP Server" "Sends newsletter emails to subscribers" "External"
        seq = softwareSystem "Seq" "Structured log aggregation and search" "External,Observability"
        jaeger = softwareSystem "Jaeger" "Distributed tracing via OpenTelemetry OTLP" "External,Observability"
        prometheus = softwareSystem "Prometheus" "Metrics scraping and storage" "External,Observability"
        grafana = softwareSystem "Grafana" "Metrics dashboards and alerting" "External,Observability"

        # ── HappyHeadlines Platform ──────────────────────────────
        happyheadlines = softwareSystem "HappyHeadlines Platform" "Distributed microservices platform for article management, comments, subscriptions, and newsletters" {

            # ── Frontend ─────────────────────────────────────────
            publisherWebapp = container "Publisher WebApp" "Draft management, publishing workflow, and publication status tracking" "React 18 / TypeScript / Nginx" "WebApp" {
                tags "Frontend"
            }

            subscriberWebapp = container "Subscriber WebApp" "Article browsing, comment management, and subscription management" "React 18 / TypeScript / Nginx" "WebApp" {
                tags "Frontend"
            }

            # ── Backend Services ──────────────────────────────────
            draftService = container "Draft Service" "CRUD operations for news article drafts" ".NET 9 / ASP.NET Core / EF Core | :8080" "Service" {
                draftController = component "DraftController" "REST endpoints: GET/POST/PUT/DELETE /Draft and /Draft/{id}" "ASP.NET Core Controller"
                draftAppService = component "DraftService" "Business logic for creating, updating, and deleting drafts" "Application Service"
                draftRepository = component "DraftRepository" "Reads and writes drafts via EF Core" "Repository"

                draftController -> draftAppService "Delegates to"
                draftAppService -> draftRepository "Persists and queries"
            }

            publisherService = container "Publisher Service" "Publishes drafts as articles; tracks publication status; emits ArticlePublishedEvent" ".NET 9 / ASP.NET Core / MassTransit | :5300" "Service" {
                publisherController = component "PublisherController" "REST endpoints: POST /publish, GET /publications, GET /publications/{id}, PATCH /publications/{id}, POST /publications/{id}/retry" "ASP.NET Core Controller"
                publisherAppService = component "PublisherService" "Orchestrates publish workflow: fetch draft → create article → record publication → emit event" "Application Service"
                publishArticleConsumer = component "PublishArticleCommandConsumer" "Consumes PublishArticleCommand from RabbitMQ for async publishing" "MassTransit Consumer"
                draftClient = component "DraftServiceClient" "HTTP client for Draft Service (30s timeout)" "HTTP Client"
                articleClient = component "ArticleServiceClient" "HTTP client for Article Service (30s timeout)" "HTTP Client"
                articleEventPublisher = component "ArticleEventPublisher" "Publishes ArticlePublishedEvent to RabbitMQ" "MassTransit Publisher"
                publicationRepository = component "PublicationRepository" "Reads and writes publication records via EF Core" "Repository"

                publisherController -> publisherAppService "Delegates to"
                publishArticleConsumer -> publisherAppService "Delegates to"
                publisherAppService -> draftClient "Fetch draft content"
                publisherAppService -> articleClient "Create article in article service"
                publisherAppService -> publicationRepository "Persist publication record"
                publisherAppService -> articleEventPublisher "Emit event after successful publish"
            }

            articleService = container "Article Service" "Multi-region article storage and retrieval; scaled to 3 instances behind nginx" ".NET 9 / ASP.NET Core / EF Core / Nginx | :5200" "Service" {
                tags "Scaled"
                articleController = component "ArticleController" "REST endpoints: GET /api/articles/{continent}, GET /api/articles/{id}, POST /api/articles, POST /api/articles/{continent}, PUT /api/articles/{id}, DELETE /api/articles/{id}" "ASP.NET Core Controller"
                articleAppService = component "ArticleService" "Business logic for article operations with continent-based routing" "Application Service"
                articleRepository = component "ArticleRepository" "Reads and writes articles using multi-region EF Core context routing" "Repository"
                articleCacheService = component "ArticleCacheService" "Caches article responses in Redis" "Cache Service"

                articleController -> articleAppService "Delegates to"
                articleAppService -> articleRepository "Persists and queries"
                articleAppService -> articleCacheService "Caches responses"
            }

            commentService = container "Comment Service" "Article comments with profanity filtering; scaled to 2 instances behind nginx" ".NET 9 / ASP.NET Core / EF Core / Polly | :5542" "Service" {
                tags "Scaled"
                commentController = component "CommentController" "REST endpoints: GET /api/comments/article/{articleId}, POST /api/comments, DELETE /api/comments/{commentId}" "ASP.NET Core Controller"
                commentAppService = component "CommentService" "Business logic for comment CRUD; calls profanity filter on create" "Application Service"
                profanityClient = component "ProfanityServiceClient" "HTTP client to Profanity Service protected by Polly Circuit Breaker (50% failure threshold, 30s break). Fallback: save comment unfiltered" "HTTP Client + Polly"
                commentRepository = component "CommentRepository" "Reads and writes comments via EF Core" "Repository"
                commentCacheService = component "CommentCacheService" "Caches GET /api/comments/article/{id} responses in Redis" "Cache Service"

                commentController -> commentAppService "Delegates to"
                commentAppService -> profanityClient "Filter text on POST (5s timeout)"
                commentAppService -> commentRepository "Persist and query comments"
                commentAppService -> commentCacheService "Cache reads"
            }

            profanityService = container "Profanity Service" "Filters profane words from submitted text" ".NET 9 / ASP.NET Core / EF Core | :5100" "Service" {
                profanityController = component "ProfanityController" "REST endpoints: POST /profanity/filter, GET /profanity/words, POST /profanity/words" "ASP.NET Core Controller"
                profanityAppService = component "ProfanityService" "Replaces matched profane words with asterisks" "Application Service"
                profanityRepository = component "ProfanityRepository" "Reads profane word list via EF Core" "Repository"

                profanityController -> profanityAppService "Delegates to"
                profanityAppService -> profanityRepository "Reads word list"
            }

            subscriberService = container "Subscriber Service" "Manages email subscriptions and preferences; emits NewSubscriberEvent" ".NET 9 / ASP.NET Core / MassTransit | :5500" "Service" {
                subscriberController = component "SubscriberController" "REST endpoints: POST /subscribe, DELETE /unsubscribe, PUT /preferences, GET /?email=" "ASP.NET Core Controller"
                subscriberAppService = component "SubscriberService" "Business logic for subscribe/unsubscribe/preferences" "Application Service"
                subscriberEventPublisher = component "SubscriberEventPublisher" "Publishes NewSubscriberEvent to RabbitMQ on new subscription" "MassTransit Publisher"
                subscriberRepository = component "SubscriberRepository" "Reads and writes subscriber records via EF Core" "Repository"

                subscriberController -> subscriberAppService "Delegates to"
                subscriberAppService -> subscriberRepository "Persist and query subscribers"
                subscriberAppService -> subscriberEventPublisher "Emit event on new subscription"
            }

            newsletterService = container "Newsletter Service" "Compiles and sends daily email digest; reacts to published articles and new subscribers" ".NET 9 / ASP.NET Core / MassTransit / Background Worker | :5400" "Service" {
                newsletterController = component "NewsletterController" "REST endpoints: POST /api/newsletter/send/daily-digest, GET /api/newsletter/history" "ASP.NET Core Controller"
                newsletterAppService = component "NewsletterService" "Orchestrates digest: fetch articles → compose email → send → record history" "Application Service"
                dailyScheduler = component "DailyNewsletterScheduler" "Hosted background service; triggers digest send daily at 09:00 UTC" "Background Service"
                articlePublishedConsumer = component "ArticlePublishedEventConsumer" "Consumes ArticlePublishedEvent; caches article for next digest" "MassTransit Consumer"
                newSubscriberConsumer = component "NewSubscriberEventConsumer" "Consumes NewSubscriberEvent; adds subscriber to local mailing list" "MassTransit Consumer"
                newsletterArticleClient = component "ArticleServiceClient" "HTTP client for Article Service; fetches articles for digest (30s timeout)" "HTTP Client"
                emailSender = component "EmailSender" "Composes and sends newsletter emails via SMTP" "SMTP Client"
                newsletterRepository = component "NewsletterRepository" "Reads and writes newsletter send history via EF Core" "Repository"
                newsletterCacheService = component "NewsletterCacheService" "Caches subscriber list and job state in Redis" "Cache Service"

                newsletterController -> newsletterAppService "Trigger manual send"
                dailyScheduler -> newsletterAppService "Trigger scheduled send"
                articlePublishedConsumer -> newsletterCacheService "Store article for next digest"
                newSubscriberConsumer -> newsletterRepository "Persist new subscriber"
                newsletterAppService -> newsletterArticleClient "Fetch articles for digest"
                newsletterAppService -> emailSender "Send to each subscriber"
                newsletterAppService -> newsletterRepository "Record send history"
                newsletterAppService -> newsletterCacheService "Read/write job state"
            }

            # ── Data Stores ───────────────────────────────────────
            articleDb = container "Article DB" "Stores articles across 8 continental databases: Global, Europe, Africa, Asia, Australia, SouthAmerica, NorthAmerica, Antarctica" "SQL Server 2022 | :1433" "Database"
            commentDb = container "Comment DB" "Stores article comments" "PostgreSQL 17 | :5435" "Database"
            draftDb = container "Draft DB" "Stores draft articles" "PostgreSQL 17 | :5434" "Database"
            profanityDb = container "Profanity DB" "Stores the profane word list" "PostgreSQL 17 | :5433" "Database"
            publisherDb = container "Publisher DB" "Stores publication records and status" "PostgreSQL 17 | :5436" "Database"
            subscriberDb = container "Subscriber DB" "Stores subscriber emails and preferences" "PostgreSQL 17 | :5438" "Database"
            newsletterDb = container "Newsletter DB" "Stores newsletter send history and cached subscriber list" "PostgreSQL 17 | :5437" "Database"

            # ── Caches ────────────────────────────────────────────
            articleRedis = container "Article Cache" "Caches article API responses" "Redis 7 | :6379" "Cache"
            commentRedis = container "Comment Cache" "Caches comment API responses" "Redis 7 | :6380" "Cache"
            newsletterRedis = container "Newsletter Cache" "Caches job state and subscriber list" "Redis 7 | :6381" "Cache"

            # ── Message Broker ────────────────────────────────────
            rabbitmq = container "RabbitMQ" "Async event bus for inter-service communication via MassTransit" "RabbitMQ 3 | AMQP :5672" "MessageBroker"

            # ── Container-level relationships ─────────────────────

            # Publisher flow
            publisherWebapp -> draftService "CRUD drafts via nginx proxy" "HTTP"
            publisherWebapp -> publisherService "Trigger publish; check publication status" "HTTP"

            # Publishing pipeline
            publisherService -> draftService "GET /Draft/{id}" "HTTP :8080"
            publisherService -> articleService "POST /api/articles/{continent}" "HTTP :80"
            publisherService -> rabbitmq "Publishes ArticlePublishedEvent" "AMQP"
            publisherService -> publisherDb "Reads/writes publication records" "EF Core"

            # Draft service persistence
            draftService -> draftDb "Reads/writes drafts" "EF Core"

            # Article service
            articleService -> articleDb "Reads/writes articles (multi-region routing)" "EF Core"
            articleService -> articleRedis "Caches article responses" "Redis"

            # Comment flow
            commentService -> profanityService "POST /profanity/filter [Circuit Breaker, 5s]" "HTTP :8080"
            commentService -> commentDb "Reads/writes comments" "EF Core"
            commentService -> commentRedis "Caches comment responses" "Redis"
            profanityService -> profanityDb "Reads profane word list" "EF Core"

            # Subscriber flow
            subscriberService -> subscriberDb "Reads/writes subscribers" "EF Core"
            subscriberService -> rabbitmq "Publishes NewSubscriberEvent" "AMQP"

            # Newsletter flow
            newsletterService -> rabbitmq "Consumes ArticlePublishedEvent + NewSubscriberEvent" "AMQP"
            newsletterService -> articleService "GET /api/articles/{continent} for daily digest" "HTTP :80"
            newsletterService -> newsletterDb "Reads/writes newsletter history" "EF Core"
            newsletterService -> newsletterRedis "Caches job state and subscriber list" "Redis"

            # Subscriber webapp
            subscriberWebapp -> articleService "GET articles" "HTTP via nginx"
            subscriberWebapp -> commentService "GET/POST comments" "HTTP via nginx"
            subscriberWebapp -> subscriberService "Subscribe / manage preferences" "HTTP via nginx"

            # Component → DB/Cache cross-container wiring
            draftRepository -> draftDb "SQL queries" "EF Core"
            publicationRepository -> publisherDb "SQL queries" "EF Core"
            draftClient -> draftService "GET /Draft/{id}" "HTTP"
            articleClient -> articleService "POST /api/articles/{continent}" "HTTP"
            articleEventPublisher -> rabbitmq "ArticlePublishedEvent" "AMQP"
            articleRepository -> articleDb "SQL queries (multi-region)" "EF Core"
            articleCacheService -> articleRedis "GET/SET" "Redis"
            commentRepository -> commentDb "SQL queries" "EF Core"
            commentCacheService -> commentRedis "GET/SET" "Redis"
            profanityClient -> profanityService "POST /profanity/filter" "HTTP"
            profanityRepository -> profanityDb "SQL queries" "EF Core"
            subscriberRepository -> subscriberDb "SQL queries" "EF Core"
            subscriberEventPublisher -> rabbitmq "NewSubscriberEvent" "AMQP"
            newsletterArticleClient -> articleService "GET /api/articles/{continent}" "HTTP"
            newsletterRepository -> newsletterDb "SQL queries" "EF Core"
            newsletterCacheService -> newsletterRedis "GET/SET" "Redis"
            articlePublishedConsumer -> rabbitmq "Consumes ArticlePublishedEvent" "AMQP"
            newSubscriberConsumer -> rabbitmq "Consumes NewSubscriberEvent" "AMQP"
        }

        # ── System-level relationships ────────────────────────────
        publisher -> happyheadlines "Manages drafts and publishes articles" "HTTPS"
        subscriber -> happyheadlines "Browses articles, posts comments, manages subscription" "HTTPS"
        happyheadlines -> newsletterRecipient "Sends daily digest email" "SMTP"
        happyheadlines -> smtp "Sends emails via" "SMTP"
        happyheadlines -> seq "Ships structured logs to" "HTTP"
        happyheadlines -> jaeger "Sends distributed traces to" "OTLP gRPC/HTTP"
        happyheadlines -> prometheus "Exposes /metrics scraped by" "HTTP"
        prometheus -> grafana "Feeds metrics to" "HTTP"
        newsletterService -> smtp "Sends newsletter emails via" "SMTP"

        publisher -> publisherWebapp "Uses" "HTTPS"
        subscriber -> subscriberWebapp "Uses" "HTTPS"
    }

    views {

        # ── Level 1: System Context ───────────────────────────────
        systemContext happyheadlines "SystemContext" "HappyHeadlines — Level 1: System Context" {
            include *
            autoLayout lr
        }

        # ── Level 2: Containers ───────────────────────────────────
        container happyheadlines "Containers" "HappyHeadlines — Level 2: Containers" {
            include *
            autoLayout lr
        }

        # ── Level 3: Components — Publisher Service ───────────────
        component publisherService "Components_PublisherService" "HappyHeadlines — Level 3: Components — Publisher Service" {
            include *
            autoLayout lr
        }

        # ── Level 3: Components — Comment Service ─────────────────
        component commentService "Components_CommentService" "HappyHeadlines — Level 3: Components — Comment Service" {
            include *
            autoLayout lr
        }

        # ── Level 3: Components — Newsletter Service ──────────────
        component newsletterService "Components_NewsletterService" "HappyHeadlines — Level 3: Components — Newsletter Service" {
            include *
            autoLayout lr
        }

        # ── Level 3: Components — Draft Service ───────────────────
        component draftService "Components_DraftService" "HappyHeadlines — Level 3: Components — Draft Service" {
            include *
            autoLayout lr
        }

        # ── Level 3: Components — Article Service ─────────────────
        component articleService "Components_ArticleService" "HappyHeadlines — Level 3: Components — Article Service" {
            include *
            autoLayout lr
        }

        # ── Level 3: Components — Subscriber Service ──────────────
        component subscriberService "Components_SubscriberService" "HappyHeadlines — Level 3: Components — Subscriber Service" {
            include *
            autoLayout lr
        }

        # ── Styles ────────────────────────────────────────────────
        styles {
            element "Person" {
                shape Person
                background #08427B
                color #ffffff
                fontSize 22
            }
            element "WebApp" {
                shape WebBrowser
                background #1168BD
                color #ffffff
            }
            element "Service" {
                shape RoundedBox
                background #1168BD
                color #ffffff
            }
            element "Database" {
                shape Cylinder
                background #336791
                color #ffffff
            }
            element "Cache" {
                shape Cylinder
                background #D82C20
                color #ffffff
            }
            element "MessageBroker" {
                shape Pipe
                background #FF6600
                color #ffffff
            }
            element "External" {
                background #999999
                color #ffffff
            }
            element "Observability" {
                background #6B4F9E
                color #ffffff
            }
            element "Scaled" {
                border dashed
            }
            relationship "AMQP" {
                style dashed
                color #FF6600
            }
            relationship "Redis" {
                style dashed
                color #D82C20
            }
            relationship "EF Core" {
                style dashed
            }
        }
    }
}