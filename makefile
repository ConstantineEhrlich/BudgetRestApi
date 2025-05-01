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

build:
	docker build --platform $(PLATFORM) -f ./$(DOCKER_FILE) -t $(REGISTRY)/$(IMAGE_NAME):$(VERSION) .

run:
	docker run -p $(HOST_PORT):$(CONTAINER_PORT) $(REGISTRY)/$(IMAGE_NAME):$(VERSION)

stop:
	docker ps -q --filter ancestor=$(REGISTRY)/$(IMAGE_NAME):$(VERSION) | xargs -r docker stop

push:
	docker push $(REGISTRY)/$(IMAGE_NAME):$(VERSION)

pull:
	docker pull $(REGISTRY)/$(IMAGE_NAME):$(VERSION)

clean:
	docker rmi $(REGISTRY)/$(IMAGE_NAME):$(VERSION)



build-migration:
	docker build --platform $(PLATFORM) -f ./migration.dockerfile -t $(REGISTRY)/budget-migration:$(VERSION) .

push-migration:
	docker push $(REGISTRY)/budget-migration:$(VERSION)



build-imex:
	$(MAKE) build DOCKER_FILE="budgetimex.dockerfile" IMAGE_NAME="budgetimex"
