build:
	docker compose -f docker/docker-compose.yml build
push:
	docker compose -f docker/docker-compose.yml push
up:
	docker compose -f docker/docker-compose.yml up
down:
	docker compose -f docker/docker-compose.yml down	
aws-up:
	docker --context myecscontext compose -f docker/docker-compose-aws.yml up
aws-down:
	docker --context myecscontext compose -f docker/docker-compose-aws.yml down
deploy-lambda:
	cd src/Agent.Lambda/src/Agent.Lambda && dotnet lambda deploy-function
aws-convert:
	docker --context myecscontext compose -f docker/docker-compose-aws.yml convert > docker-aws-convert.yml
