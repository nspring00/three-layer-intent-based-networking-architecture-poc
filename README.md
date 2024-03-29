# "Three-layer Intent-based Networking" Architecture Proof-of-Concept 

For more info on the functionality, see the Bachelor's Thesis or the [suceeding paper on _Intent-based Management for the Distributed Computing Continuum_](https://dsg.tuwien.ac.at/team/sd/papers/SOSE_2023_A_Morichetta_Intent.pdf).

## Datasets

The datasets in the _Datasets_ directory are used to determine, which load there will be on the system at a given point in time. Further information can be found [here](Datasets/README.md).

## Prerequisites

- Docker
- Docker Hub Account (for AWS - if you want custom images)
- AWS (for AWS)
- Complete the steps given in the BT to set up the SQS queue, the Lambda function and the IAM Roles (for AWS)

## Running the Simulation locally

1. Clone the repository
2. Build via `make build`
3. Change into the `docker` directory
4. Make sure that the output files specified in the respective `docker-compose-simulate-<SIMULATION>.yml` as volumes exist
5. Adjust the simulation parameters in the same file
    - Initial intents that are specified at the start
    - Number of devices at the beginning
    - More simulation parameters e.g. jitter for data generation
6. Start only RabbitMQ via `docker-compose -f docker-compose-simulate-<SIMULATION>.yml up -d rabbitmq`
7. After a brief period, start the whole simulation via 6. Start only RabbitMQ via `docker-compose -f docker-compose-simulate-<SIMULATION>.yml up -d`

Note, that the dataset is also mapped via volume and that there exist two different datasets for the full simulation.

The Knowledge's REST API is located by default at `localhost:11000`.

## Running the simulation on AWS

By default, the images provided by me `nsp00` on Docker Hub will be used.
To change this behavior, change the image names in the respective `docker-compose-aws.yml` file, build and push the images.

1. Clone the repository
2. Deploy the AWS Lambda function via `make deploy-lambda`
3. Specify initial intents and number of devices like locally. Jitter is not supported.
4. Deploy to AWS ECS via `make aws-up`

Also note, that currently there is no way of running the same simulation setup that is possible locally on AWS.
All output values are random.

To locate and access the Knowledge's REST API, you need to find the DNS address of the load balancer.
Search for EC2 Load Balancer and find the one with the prefix `ibsm-LoadBa-`. 
When you access this address, you should be greeted by the Knowledge automatically. 
The Swagger UI is located at `<DNS>/swagger`, which can be used to test the API.
