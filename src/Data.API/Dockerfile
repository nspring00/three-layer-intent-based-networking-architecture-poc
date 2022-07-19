#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Data.API/Data.API.csproj", "src/Data.API/"]
COPY ["src/Data.Console/Data.Console.csproj", "src/Data.Console/"]
COPY ["src/Knowledge.Grpc/Knowledge.Grpc.csproj", "src/Knowledge.Grpc/"]
COPY ["src/Common.Grpc/Common.Grpc.csproj", "src/Common.Grpc/"]
COPY ["src/NetworkLayer.Grpc/NetworkLayer.Grpc.csproj", "src/NetworkLayer.Grpc/"]
COPY ["src/Common/Common.csproj", "src/Common/"]
COPY ["src/Data.Grpc/Data.Grpc.csproj", "src/Data.Grpc/"]
RUN dotnet restore "src/Data.API/Data.API.csproj"
COPY . .
WORKDIR "/src/src/Data.API"
RUN dotnet build "Data.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Data.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Data.API.dll"]