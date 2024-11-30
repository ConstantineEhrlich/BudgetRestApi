BUDGETDB-PASSWORD = PostgresPassword
AUTHDB-PASSWORD = MongoDbPassword
JWT-TOKEN = ThisKeyHaveToBeExtremelySecureToWork
VERSION ?= latest
PLATFORM ?= linux/amd64
REGISTRY = docker-registry.home.arpa/mybudget
IMAGE_NAME = budgetapi
DOCKER_FILE ?= budgetapi.dockerfile
CONTAINER_PORT ?= 80
HOST_PORT ?= 5005

config:
	@echo "Registry:     $(REGISTRY)"
	@echo "Image:        $(IMAGE_NAME)"
	@echo "Version:      $(VERSION)"
	@echo "Platform:     $(PLATFORM)"
	@echo "Docker file:  $(DOCKER_FILE)"
	@echo "Ports:        Host $(HOST_PORT) -> Container $(CONTAINER_PORT)"

build-migration:
	docker build --platform $(PLATFORM) -f ./migration.dockerfile -t $(REGISTRY)/budget-migration:$(VERSION) .

push-migration:
	docker push $(REGISTRY)/budget-migration:$(VERSION)

build:
	docker build --platform $(PLATFORM) -f ./$(DOCKER_FILE) -t $(REGISTRY)/$(IMAGE_NAME):$(VERSION) .

run:
	docker run -p $(HOST_PORT):$(CONTAINER_PORT) $(REGISTRY)/$(IMAGE_NAME):$(VERSION)

stop:
	docker ps -q --filter ancestor=$(REGISTRY)/$(IMAGE_NAME):$(VERSION) | xargs -r docker stop

push:
	docker push $(REGISTRY)/$(IMAGE_NAME):$(VERSION)

remote-images:
	docker build --platform linux/amd64 -f ./migration.dockerfile -t $(REGISTRY)budget-migration --push .
	docker build --platform linux/amd64 -f ./budgetapi.dockerfile -t $(REGISTRY)budgetapi --push .

deploy:
	# Deploy budgetapi database
	kubectl apply -f ./BudgetModel/postgres-claim.yaml
	kubectl apply -f ./BudgetModel/budgetdb-deployment.yaml
	kubectl apply -f ./BudgetModel/budgetdb-service.yaml
	# Run migration job
	kubectl apply -f ./BudgetModel/budgetdb-migration-job.yaml
	# Deploy the app
	kubectl apply -f ./BudgetWebApi/budgetapi-deployment.yaml
	kubectl apply -f ./BudgetWebApi/budgetapi-service.yaml
	

restart:
	kubectl rollout restart deployment budgetapi


listen-postgres:
	kubectl port-forward svc/postgres 5432:5432


listen-api:
	kubectl port-forward svc/budgetapi 8765:80

	
delete-cluster:
	kubectl delete deployments --all --namespace default
	kubectl delete services --all --namespace default
	kubectl delete jobs --all --namespace default
	