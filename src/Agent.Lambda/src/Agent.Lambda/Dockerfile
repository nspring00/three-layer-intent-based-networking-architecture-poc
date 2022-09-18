FROM public.ecr.aws/lambda/dotnet:6 AS base

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim as build
WORKDIR /src
COPY ["Agent.Lambda.csproj", "Agent.Lambda/"]
RUN dotnet restore "Agent.Lambda/Agent.Lambda.csproj"

WORKDIR "/src/Agent.Lambda"
COPY . .
RUN dotnet build "Agent.Lambda.csproj" --configuration Release --output /app/build

FROM build AS publish
RUN dotnet publish "Agent.Lambda.csproj" \
            --configuration Release \ 
            --runtime linux-x64 \
            --self-contained false \ 
            --output /app/publish \
            -p:PublishReadyToRun=true  

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .

