#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/NetworkLayer.API/NetworkLayer.API.csproj", "src/NetworkLayer.API/"]
COPY ["src/NetworkLayer.Grpc/NetworkLayer.Grpc.csproj", "src/NetworkLayer.Grpc/"]
COPY ["src/Common/Common.csproj", "src/Common/"]
RUN dotnet restore "src/NetworkLayer.API/NetworkLayer.API.csproj"
COPY . .
WORKDIR "/src/src/NetworkLayer.API"
RUN dotnet build "NetworkLayer.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NetworkLayer.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetworkLayer.API.dll"]