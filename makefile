POSTGRES-PASSWORD = PostgresPassword
JWT-TOKEN = ThisKeyHaveToBeExtremelySecureToWork
REGISTRY = registry.digitalocean.com/budgetapp-registry/


kube-secrets:
	kubectl create secret generic postgres-secret --from-literal=password=$(POSTGRES-PASSWORD)
	kubectl create secret generic budgetapi-secret --from-literal=jwt=$(JWT-TOKEN)


minikube-images:
	@eval $$(minikube docker-env) && \
    docker build -f dockerfile-migration -t budget-migration .
	@eval $$(minikube docker-env) && \
   	docker build -t budgetapi .


remote-images:
	docker build --platform linux/amd64 -f ./dockerfile-migration -t $(REGISTRY)budget-migration --push .
	docker build --platform linux/amd64 -f ./dockerfile -t $(REGISTRY)budgetapi --push .


deploy:
	# Deploy postgres
	kubectl apply -f postgres-volume-claim.yaml
	kubectl apply -f postgres-deployment.yaml
	kubectl apply -f postgres-service.yaml
	# Run migration job
	kubectl apply -f database-migrate-job.yaml
	# Deploy the app
	kubectl apply -f budgetapi-deployment.yaml
	kubectl apply -f budgetapi-service.yaml


restart:
	kubectl rollout restart deployment budgetapi


listen-postgres:
	kubectl port-forward svc/postgres 5432:5432


listen-api:
	kubectl port-forward svc/budgetapi 8765:80