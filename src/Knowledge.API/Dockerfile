#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Knowledge.API/Knowledge.API.csproj", "src/Knowledge.API/"]
COPY ["src/Knowledge.Grpc/Knowledge.Grpc.csproj", "src/Knowledge.Grpc/"]
COPY ["src/Knowledge.API.Contracts/Knowledge.API.Contracts.csproj", "src/Knowledge.API.Contracts/"]
COPY ["src/Common/Common.csproj", "src/Common/"]
COPY ["src/Common.Web/Common.Web.csproj", "src/Common.Web/"]
RUN dotnet restore "src/Knowledge.API/Knowledge.API.csproj"
COPY . .
WORKDIR "/src/src/Knowledge.API"
RUN dotnet build "Knowledge.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Knowledge.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Knowledge.API.dll"]