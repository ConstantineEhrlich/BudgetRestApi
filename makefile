BUDGETDB-PASSWORD = PostgresPassword
AUTHDB-PASSWORD = MongoDbPassword
JWT-TOKEN = ThisKeyHaveToBeExtremelySecureToWork
REGISTRY = registry.digitalocean.com/budgetapp-registry/


kube-secrets:
	kubectl delete secrets --all --namespace default
	kubectl create secret generic budgetdb --from-literal=password=$(BUDGETDB-PASSWORD)
	kubectl create secret generic authdb --from-literal=password=$(AUTHDB-PASSWORD)	
	kubectl create secret generic jwt --from-literal=token=$(JWT-TOKEN)
	


minikube-images:
	@eval $$(minikube docker-env) && \
    docker build -f ./migration.dockerfile -t $(REGISTRY)budget-migration .
	@eval $$(minikube docker-env) && \
   	docker build -f ./budgetapi.dockerfile -t $(REGISTRY)budgetapi .


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
	