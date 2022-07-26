#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Agent.API/Agent.API.csproj", "src/Agent.API/"]
COPY ["src/Agent.Core/Agent.Core.csproj", "src/Agent.Core/"]
COPY ["src/Knowledge.Grpc/Knowledge.Grpc.csproj", "src/Knowledge.Grpc/"]
COPY ["src/Common.Grpc/Common.Grpc.csproj", "src/Common.Grpc/"]
COPY ["src/NetworkLayer.Grpc/NetworkLayer.Grpc.csproj", "src/NetworkLayer.Grpc/"]
COPY ["src/Common/Common.csproj", "src/Common/"]
COPY ["src/Data.Grpc/Data.Grpc.csproj", "src/Data.Grpc/"]
COPY ["src/Agent.API.Contracts/Agent.API.Contracts.csproj", "src/Agent.API.Contracts/"]
COPY ["src/Common.Web/Common.Web.csproj", "src/Common.Web/"]
RUN dotnet restore "src/Agent.API/Agent.API.csproj"
COPY . .
WORKDIR "/src/src/Agent.API"
RUN dotnet build "Agent.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Agent.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Agent.API.dll"]