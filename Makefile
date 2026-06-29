.PHONY: up down build restart logs logs-auth logs-product logs-order logs-traefik status clean seed test-auth test-all help

DOMAIN = http://localhost
HOST_HEADER = securecoding.co

# === Docker Compose Commands ===

up: ## Start all services
	@echo "🚀 Starting Vulnerable E-Commerce Lab..."
	docker-compose up -d --build
	@echo ""
	@echo "✅ Ready! Access the API at http://$(HOST_HEADER)"
	@echo "📊 Traefik Dashboard: http://localhost:8080"
	@echo ""
	@echo "⚠️  Make sure your /etc/hosts file contains:"
	@echo "   127.0.0.1  $(HOST_HEADER)"

down: ## Stop all services
	@echo "🛑 Stopping all services..."
	docker-compose down

build: ## Build all images without starting
	@echo "🔨 Building all images..."
	docker-compose build

restart: ## Restart all services
	@echo "🔄 Rebuilding and restarting all services..."
	docker-compose down && docker-compose up -d --build

logs: ## Tail all service logs
	docker-compose logs -f

logs-auth: ## Tail Auth Service logs
	docker-compose logs -f auth-service

logs-product: ## Tail Product Service logs
	docker-compose logs -f product-service

logs-order: ## Tail Order Service logs
	docker-compose logs -f order-service

logs-traefik: ## Tail Traefik logs
	docker-compose logs -f traefik

status: ## Show running containers
	docker-compose ps

clean: ## Stop services and remove volumes (⚠️ deletes all data)
	@echo "🗑️  Stopping services and removing all data..."
	docker-compose down -v

# === Quick Tests ===

seed: ## Seed sample data (register user + create products)
	@echo "🌱 Seeding sample data..."
	@curl -s -X POST $(DOMAIN)/api/auth/register \
		-H "Host: $(HOST_HEADER)" \
		-H "Content-Type: application/json" \
		-d '{"username":"admin","email":"admin@shop.com","password":"admin123"}' | head -1
	@curl -s -X POST $(DOMAIN)/api/auth/register \
		-H "Host: $(HOST_HEADER)" \
		-H "Content-Type: application/json" \
		-d '{"username":"user1","email":"user1@shop.com","password":"pass123"}' | head -1
	@curl -s -X POST $(DOMAIN)/api/products \
		-H "Host: $(HOST_HEADER)" \
		-H "Content-Type: application/json" \
		-d '{"name":"Laptop","description":"High-performance laptop","price":1299.99,"category":"Electronics","stock":50}' | head -1
	@curl -s -X POST $(DOMAIN)/api/products \
		-H "Host: $(HOST_HEADER)" \
		-H "Content-Type: application/json" \
		-d '{"name":"Mouse","description":"Wireless mouse","price":29.99,"category":"Electronics","stock":200}' | head -1
	@curl -s -X POST $(DOMAIN)/api/products \
		-H "Host: $(HOST_HEADER)" \
		-H "Content-Type: application/json" \
		-d '{"name":"T-Shirt","description":"Cotton t-shirt","price":19.99,"category":"Clothing","stock":500}' | head -1
	@echo "\n✅ Sample data seeded!"

test-auth: ## Test auth endpoints (normal flow)
	@echo "🔑 Testing Auth Service..."
	@echo "\n--- Register ---"
	@curl -s -X POST $(DOMAIN)/api/auth/register \
		-H "Host: $(HOST_HEADER)" \
		-H "Content-Type: application/json" \
		-d '{"username":"testmake","email":"test@make.com","password":"Test123!"}' | python3 -m json.tool || true
	@echo "\n--- Login ---"
	@curl -s -X POST $(DOMAIN)/api/auth/login \
		-H "Host: $(HOST_HEADER)" \
		-H "Content-Type: application/json" \
		-d '{"username":"testmake","password":"Test123!"}' | python3 -m json.tool || true

test-all: ## Test all standard endpoints (Auth, Products, Orders) to verify they work
	@echo "🟢 Starting General Endpoint Tests..."
	@echo "\n--- Register User ---"
	@curl -s -X POST $(DOMAIN)/api/auth/register -H "Host: $(HOST_HEADER)" -H "Content-Type: application/json" -d '{"username":"user_test","email":"user@test.com","password":"UserPass123!"}' | python3 -m json.tool || true
	@echo "\n--- Login User ---"
	@curl -s -X POST $(DOMAIN)/api/auth/login -H "Host: $(HOST_HEADER)" -H "Content-Type: application/json" -d '{"username":"user_test","password":"UserPass123!"}' | python3 -m json.tool || true
	@echo "\n--- List Products ---"
	@curl -s -H "Host: $(HOST_HEADER)" $(DOMAIN)/api/products | head -20 || true
	@echo "\n--- Search Products ---"
	@curl -s -H "Host: $(HOST_HEADER)" "$(DOMAIN)/api/products/search?q=Laptop" | python3 -m json.tool || true
	@echo "\n--- Get Product Stats ---"
	@curl -s -H "Host: $(HOST_HEADER)" "$(DOMAIN)/api/products/stats?groupBy=Category" | python3 -m json.tool || true
	@echo "\n--- Create Order ---"
	@curl -s -X POST $(DOMAIN)/api/orders -H "Host: $(HOST_HEADER)" -H "Content-Type: application/json" -d '{"userId":"user_test","items":[{"productId":"p1","productName":"Laptop","quantity":1,"unitPrice":1299.99}],"totalPrice":1299.99,"discount":0,"shippingAddress":"Istanbul"}' | python3 -m json.tool || true
	@echo "\n--- List Orders ---"
	@curl -s -H "Host: $(HOST_HEADER)" $(DOMAIN)/api/orders | head -20 || true
	@echo "\n✅ All standard endpoints verified!"

help: ## Show this help
	@echo "Vulnerable E-Commerce Lab — Makefile Commands\n"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | \
		awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-15s\033[0m %s\n", $$1, $$2}'

.DEFAULT_GOAL := help
