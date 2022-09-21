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