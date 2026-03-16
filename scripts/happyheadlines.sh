#!/bin/bash

# HappyHeadlines Management Script
# Unified script to manage all HappyHeadlines microservices

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Project root directory (parent of scripts/)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

# ==========================================
# Utility Functions
# ==========================================

print_header() {
    echo -e "\n${BLUE}========================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}========================================${NC}\n"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ $1${NC}"
}

command_exists() {
    command -v "$1" >/dev/null 2>&1
}

port_available() {
    ! nc -z localhost "$1" 2>/dev/null
}

check_service_http() {
    local url=$1
    local name=$2
    if curl -s --max-time 3 "$url" >/dev/null 2>&1; then
        print_success "$name is running"
        return 0
    else
        print_error "$name is not responding"
        return 1
    fi
}

check_container() {
    local container=$1
    local name=$2
    if docker ps --format '{{.Names}}' | grep -q "^${container}$"; then
        print_success "$name container is running"
        return 0
    else
        print_error "$name container is not running"
        return 1
    fi
}

# ==========================================
# Help Function
# ==========================================

show_help() {
    echo -e "${CYAN}"
    cat << 'EOF'
 _   _                         _   _                _ _ _
| | | | __ _ _ __  _ __  _   _| | | | ___  __ _  __| | (_)_ __   ___  ___
| |_| |/ _` | '_ \| '_ \| | | | |_| |/ _ \/ _` |/ _` | | | '_ \ / _ \/ __|
|  _  | (_| | |_) | |_) | |_| |  _  |  __/ (_| | (_| | | | | | |  __/\__ \
|_| |_|\__,_| .__/| .__/ \__, |_| |_|\___|\__,_|\__,_|_|_|_| |_|\___||___/
            |_|   |_|    |___/
EOF
    echo -e "${NC}"
    echo "HappyHeadlines Microservices Management Tool"
    echo ""
    echo "Usage: ./happyheadlines.sh [COMMAND]"
    echo ""
    echo "Commands:"
    echo "  check       Check system requirements"
    echo "  start       Start all services"
    echo "  stop        Stop all services"
    echo "  restart     Restart all services"
    echo "  status      Check status of all services"
    echo "  logs        Show logs from all services"
    echo "  clean       Stop services and remove volumes"
    echo "  help        Show this help message"
    echo ""
    echo "Examples:"
    echo "  ./happyheadlines.sh check"
    echo "  ./happyheadlines.sh start"
    echo "  ./happyheadlines.sh status"
    echo "  ./happyheadlines.sh logs"
    echo ""
}

# ==========================================
# Check Requirements
# ==========================================

check_requirements() {
    print_header "Checking System Requirements"

    local REQUIREMENTS_MET=true

    # Check Docker
    if command_exists docker; then
        DOCKER_VERSION=$(docker --version | cut -d ' ' -f3 | cut -d ',' -f1)
        print_success "Docker installed (version $DOCKER_VERSION)"
    else
        print_error "Docker is not installed"
        REQUIREMENTS_MET=false
    fi

    # Check Docker Compose
    if command_exists docker-compose; then
        COMPOSE_VERSION=$(docker-compose --version | cut -d ' ' -f4 | cut -d ',' -f1)
        print_success "Docker Compose installed (version $COMPOSE_VERSION)"
    elif docker compose version >/dev/null 2>&1; then
        COMPOSE_VERSION=$(docker compose version --short)
        print_success "Docker Compose (plugin) installed (version $COMPOSE_VERSION)"
    else
        print_error "Docker Compose is not installed"
        REQUIREMENTS_MET=false
    fi

    # Check .NET SDK (optional)
    if command_exists dotnet; then
        DOTNET_VERSION=$(dotnet --version)
        print_success ".NET SDK installed (version $DOTNET_VERSION) [optional]"
    else
        print_warning ".NET SDK not installed (optional, only for local development)"
    fi

    # Check Node.js (optional)
    if command_exists node; then
        NODE_VERSION=$(node --version)
        print_success "Node.js installed (version $NODE_VERSION) [optional]"
    else
        print_warning "Node.js not installed (optional, only for webapp development)"
    fi

    # Check disk space
    AVAILABLE_SPACE=$(df -h "$SCRIPT_DIR" | awk 'NR==2 {print $4}')
    print_info "Available disk space: $AVAILABLE_SPACE"

    # Check ports
    print_header "Checking Port Availability"

    if command_exists nc; then
        declare -A REQUIRED_PORTS=(
            [3000]="webapp-service"
            [3001]="website"
            [5100]="ProfanityService"
            [5200]="ArticleService"
            [5300]="PublisherService"
            [5341]="Seq"
            [5400]="NewsletterService"
            [5500]="SubscriberService"
            [5542]="CommentService"
            [5672]="RabbitMQ"
            [8080]="DraftService"
            [15672]="RabbitMQ Management"
            [9090]="Prometheus"
            [3003]="Grafana"
            [16686]="Jaeger"
        )

        for port in "${!REQUIRED_PORTS[@]}"; do
            if port_available "$port"; then
                print_success "Port $port available (${REQUIRED_PORTS[$port]})"
            else
                print_warning "Port $port is in use (${REQUIRED_PORTS[$port]})"
            fi
        done
    else
        print_warning "netcat (nc) not installed - skipping port checks"
    fi

    echo ""
    if [ "$REQUIREMENTS_MET" = false ]; then
        print_error "Required dependencies are missing!"
        echo ""
        echo "Please install:"
        echo "  Docker: https://docs.docker.com/engine/install/"
        echo "  Docker Compose: https://docs.docker.com/compose/install/"
        return 1
    else
        print_success "All requirements met!"
        return 0
    fi
}

# ==========================================
# Start Services
# ==========================================

start_services() {
    print_header "Starting HappyHeadlines Services"

    cd "$SCRIPT_DIR"

    # Clean up any orphaned containers first
    print_info "Cleaning up any orphaned containers..."
    docker stop draft-frontend 2>/dev/null || true
    docker rm draft-frontend 2>/dev/null || true

    # Clean up stale database containers that might be in 'Created' state
    for container in comment-db profanity-db draft-db publisher-db newsletter-db subscriber-db; do
        if docker ps -a --filter "name=$container" --filter "status=created" --format "{{.Names}}" | grep -q "$container"; then
            docker rm "$container" 2>/dev/null || true
        fi
    done

    print_info "Starting infrastructure services..."

    # Start Observability
    print_info "Starting Seq + Jaeger..."
    cd infra/observability
    if ! docker-compose up -d; then
        print_error "Failed to start observability services"
        return 1
    fi
    cd "$SCRIPT_DIR"
    print_success "Observability started"

    sleep 3

    # Start RabbitMQ (before metrics so services network exists when metrics starts)
    print_info "Starting RabbitMQ..."
    cd infra/messaging
    if ! docker-compose up -d; then
        print_error "Failed to start RabbitMQ"
        return 1
    fi
    cd "$SCRIPT_DIR"
    print_success "RabbitMQ started"

    sleep 5

    print_info "Starting application services..."

    # Start ArticleService
    print_info "Starting ArticleService..."
    cd apps/article-service
    if ! docker-compose up --build -d; then
        print_error "Failed to start ArticleService"
        cd "$SCRIPT_DIR"
        return 1
    fi
    cd "$SCRIPT_DIR"
    print_success "ArticleService started"

    sleep 3

    # Start DraftService (without draft-frontend)
    print_info "Starting DraftService..."
    cd apps/draft-service
    if ! docker-compose up --build -d; then
        print_error "Failed to start DraftService"
        cd "$SCRIPT_DIR"
        return 1
    fi
    cd "$SCRIPT_DIR"
    print_success "DraftService started"

    sleep 3

    # Start ProfanityService
    print_info "Starting ProfanityService..."
    cd apps/profanity-service
    if ! docker-compose up --build -d; then
        print_error "Failed to start ProfanityService"
        cd "$SCRIPT_DIR"
        return 1
    fi
    cd "$SCRIPT_DIR"
    print_success "ProfanityService started"

    sleep 3

    # Start CommentService
    print_info "Starting CommentService..."
    cd apps/comment-service
    if ! docker-compose up --build -d; then
        print_error "Failed to start CommentService"
        cd "$SCRIPT_DIR"
        return 1
    fi
    cd "$SCRIPT_DIR"
    print_success "CommentService started"

    sleep 3

    # Start PublisherService
    print_info "Starting PublisherService..."
    cd apps/publisher-service
    if ! docker-compose up --build -d; then
        print_error "Failed to start PublisherService"
        cd "$SCRIPT_DIR"
        return 1
    fi
    cd "$SCRIPT_DIR"
    print_success "PublisherService started"

    sleep 3

    # Start NewsletterService
    print_info "Starting NewsletterService..."
    cd apps/newsletter-service
    if ! docker-compose up --build -d; then
        print_error "Failed to start NewsletterService"
        cd "$SCRIPT_DIR"
        return 1
    fi
    cd "$SCRIPT_DIR"
    print_success "NewsletterService started"

    sleep 3

    # Start SubscriberService
    print_info "Starting SubscriberService..."
    cd apps/subscriber-service
    if ! docker-compose up --build -d; then
        print_error "Failed to start SubscriberService"
        cd "$SCRIPT_DIR"
        return 1
    fi
    cd "$SCRIPT_DIR"
    print_success "SubscriberService started"

    sleep 3

    # Start publisher-webapp
    print_info "Starting publisher-webapp..."
    cd apps/publisher-webapp
    if ! docker-compose up --build -d; then
        print_error "Failed to start publisher-webapp"
        cd "$SCRIPT_DIR"
        return 1
    fi
    cd "$SCRIPT_DIR"
    print_success "publisher-webapp started"

    sleep 3

    # Start website
    print_info "Starting subscriber-webapp..."
    cd apps/subscriber-webapp
    if ! docker-compose up --build -d; then
        print_error "Failed to start subscriber-webapp"
        cd "$SCRIPT_DIR"
        return 1
    fi
    cd "$SCRIPT_DIR"
    print_success "subscriber-webapp started"

    # Start Metrics (Prometheus + Grafana) — after services are up so the network exists
    print_info "Starting Prometheus + Grafana..."
    cd infra/metrics
    if ! docker-compose up -d; then
        print_error "Failed to start metrics services"
        cd "$SCRIPT_DIR"
        return 1
    fi
    cd "$SCRIPT_DIR"
    print_success "Metrics started"

    print_header "Services Started Successfully!"

    print_info "Waiting for services to be ready (15 seconds)..."
    sleep 15

    print_success "All services are running!"
    echo ""
    show_access_info
}

# ==========================================
# Stop Services
# ==========================================

stop_services() {
    print_header "Stopping HappyHeadlines Services"

    cd "$SCRIPT_DIR"

    print_info "Stopping application services..."

    cd apps/subscriber-webapp
    docker-compose down
    cd "$SCRIPT_DIR"
    print_success "subscriber-webapp stopped"

    cd apps/publisher-webapp
    docker-compose down
    cd "$SCRIPT_DIR"
    print_success "publisher-webapp stopped"

    cd apps/subscriber-service
    docker-compose down
    cd "$SCRIPT_DIR"
    print_success "SubscriberService stopped"

    cd apps/newsletter-service
    docker-compose down
    cd "$SCRIPT_DIR"
    print_success "NewsletterService stopped"

    cd apps/publisher-service
    docker-compose down
    cd "$SCRIPT_DIR"
    print_success "PublisherService stopped"

    cd apps/draft-service
    docker-compose down
    cd "$SCRIPT_DIR"
    print_success "DraftService stopped"

    cd apps/comment-service
    docker-compose down
    cd "$SCRIPT_DIR"
    print_success "CommentService stopped"

    cd apps/profanity-service
    docker-compose down
    cd "$SCRIPT_DIR"
    print_success "ProfanityService stopped"

    cd apps/article-service
    docker-compose down
    cd "$SCRIPT_DIR"
    print_success "ArticleService stopped"

    print_info "Stopping infrastructure services..."

    cd infra/metrics
    docker-compose down
    cd "$SCRIPT_DIR"
    print_success "Metrics stopped"

    cd infra/messaging
    docker-compose down
    cd "$SCRIPT_DIR"
    print_success "RabbitMQ stopped"

    cd infra/observability
    docker-compose down
    cd "$SCRIPT_DIR"
    print_success "Observability stopped"

    print_header "All Services Stopped"
    echo ""
}

# ==========================================
# Check Status
# ==========================================

check_status() {
    print_header "HappyHeadlines Status"

    cd "$SCRIPT_DIR"

    # Check if Docker is running
    if ! docker ps >/dev/null 2>&1; then
        print_error "Docker daemon is not running"
        return 1
    fi

    print_header "Infrastructure Services"
    check_container "seq" "Seq (Logs)"
    check_container "jaeger" "Jaeger (Tracing)"
    check_container "happyheadlines-rabbitmq" "RabbitMQ"
    check_container "prometheus" "Prometheus"
    check_container "grafana" "Grafana"

    print_header "Application Services"
    check_container "draft-service" "DraftService"
    check_container "profanity-api" "ProfanityService"
    check_container "comment-api-1" "CommentService"
    check_container "publisher-service" "PublisherService"
    check_container "newsletter-service" "NewsletterService"
    check_container "subscriber-service" "SubscriberService"
    check_container "publisher-webapp" "publisher-webapp"
    check_container "subscriber-webapp" "subscriber-webapp"
    check_container "articles-api-1" "ArticleService"

    print_header "Database Services"
    check_container "draft-db" "DraftService DB"
    check_container "profanity-db" "ProfanityService DB"
    check_container "comment-db" "CommentService DB"
    check_container "publisher-db" "PublisherService DB"
    check_container "newsletter-db" "NewsletterService DB"
    check_container "subscriber-db" "SubscriberService DB"
    check_container "article-db" "ArticleService DB"

    print_header "HTTP Health Checks"
    check_service_http "http://localhost:5341" "Seq"
    check_service_http "http://localhost:16686" "Jaeger"
    check_service_http "http://localhost:15672" "RabbitMQ Management"
    check_service_http "http://localhost:9090" "Prometheus"
    check_service_http "http://localhost:3003" "Grafana"
    check_service_http "http://localhost:3000" "publisher-webapp"
    check_service_http "http://localhost:3001" "subscriber-webapp"
    check_service_http "http://localhost:5500" "SubscriberService"

    print_header "Summary"
    TOTAL_CONTAINERS=$(docker ps -q | wc -l)
    print_info "Total containers running: $TOTAL_CONTAINERS"
    echo ""
}

# ==========================================
# Show Logs
# ==========================================

show_logs() {
    print_header "Service Logs"
    echo ""
    echo "Select a service to view logs:"
    echo ""
    echo "  1) DraftService"
    echo "  2) ProfanityService"
    echo "  3) CommentService"
    echo "  4) PublisherService"
    echo "  5) ArticleService"
    echo "  6) NewsletterService"
    echo "  7) SubscriberService"
    echo "  8) publisher-webapp"
    echo "  9) subscriber-webapp"
    echo "  r) RabbitMQ"
    echo "  a) All services"
    echo "  0) Cancel"
    echo ""
    read -p "Enter choice [0-9/r/a]: " choice

    case $choice in
        1)
            docker logs -f draft-service
            ;;
        2)
            docker logs -f profanity-api
            ;;
        3)
            docker logs -f comment-api-1
            ;;
        4)
            docker logs -f publisher-service
            ;;
        5)
            docker logs -f articles-api-1
            ;;
        6)
            docker logs -f newsletter-service
            ;;
        7)
            docker logs -f subscriber-service
            ;;
        8)
            docker logs -f publisher-webapp
            ;;
        9)
            docker logs -f subscriber-webapp
            ;;
        r)
            docker logs -f happyheadlines-rabbitmq
            ;;
        a)
            docker-compose -f infra/observability/docker-compose.yml logs -f &
            docker-compose -f infra/messaging/docker-compose.yml logs -f &
            docker-compose -f apps/draft-service/docker-compose.yml logs -f &
            docker-compose -f apps/profanity-service/docker-compose.yml logs -f &
            docker-compose -f apps/comment-service/docker-compose.yml logs -f &
            docker-compose -f apps/publisher-service/docker-compose.yml logs -f &
            docker-compose -f apps/article-service/docker-compose.yaml logs -f &
            docker-compose -f apps/newsletter-service/docker-compose.yml logs -f &
            docker-compose -f apps/subscriber-service/docker-compose.yml logs -f &
            docker-compose -f apps/publisher-webapp/docker-compose.yml logs -f &
            docker-compose -f apps/subscriber-webapp/docker-compose.yml logs -f
            ;;
        0)
            echo "Cancelled"
            ;;
        *)
            print_error "Invalid choice"
            ;;
    esac
}

# ==========================================
# Clean (Stop and Remove Volumes)
# ==========================================

clean_all() {
    print_header "Clean All Services and Data"
    print_warning "This will stop all services and remove all data volumes!"
    echo ""
    read -p "Are you sure? (yes/no): " confirm

    if [ "$confirm" != "yes" ]; then
        echo "Cancelled"
        return
    fi

    stop_services

    print_info "Removing volumes..."
    docker volume prune -f

    print_success "Cleanup complete!"
    echo ""
}

# ==========================================
# Show Access Information
# ==========================================

show_access_info() {
    echo -e "${GREEN}=== Access Points ===${NC}"
    echo ""
    echo "🌐 Web Applications:"
    echo "   publisher-webapp:      http://localhost:3000  (Publisher)"
    echo "   subscriber-webapp:     http://localhost:3001  (Reader)"
    echo ""
    echo "📡 API Services:"
    echo "   DraftService:          http://localhost:8080"
    echo "   PublisherService:      http://localhost:5300"
    echo "   ArticleService:        http://localhost:5200"
    echo "   NewsletterService:     http://localhost:5400"
    echo "   SubscriberService:     http://localhost:5500"
    echo "   CommentService:        http://localhost:5542"
    echo "   ProfanityService:      http://localhost:5100"
    echo ""
    echo "🔧 Infrastructure:"
    echo "   RabbitMQ Management:   http://localhost:15672 (admin/admin)"
    echo "   Seq (Logs):            http://localhost:5341"
    echo "   Jaeger (Traces):       http://localhost:16686"
    echo "   Prometheus:            http://localhost:9090"
    echo "   Grafana (Metrics):     http://localhost:3003 (admin/admin)"
    echo ""
}

# ==========================================
# Main Script Logic
# ==========================================

# Check if no arguments provided
if [ $# -eq 0 ]; then
    show_help
    exit 0
fi

# Parse command
case "$1" in
    check)
        check_requirements
        ;;
    start)
        check_requirements
        if [ $? -eq 0 ]; then
            start_services
        else
            print_error "Requirements not met. Please fix the issues above."
            exit 1
        fi
        ;;
    stop)
        stop_services
        ;;
    restart)
        stop_services
        sleep 3
        start_services
        ;;
    status)
        check_status
        ;;
    logs)
        show_logs
        ;;
    clean)
        clean_all
        ;;
    help|--help|-h)
        show_help
        ;;
    *)
        print_error "Unknown command: $1"
        echo ""
        show_help
        exit 1
        ;;
esac
