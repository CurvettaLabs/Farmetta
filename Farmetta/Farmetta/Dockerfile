﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Farmetta/Farmetta/Farmetta.csproj", "Farmetta/Farmetta/"]
COPY ["Farmetta/Farmetta.Client/Farmetta.Client.csproj", "Farmetta/Farmetta.Client/"]
RUN dotnet restore "Farmetta/Farmetta/Farmetta.csproj"
COPY . .
WORKDIR "/src/Farmetta/Farmetta"
RUN dotnet build "Farmetta.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Farmetta.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root
ENTRYPOINT ["dotnet", "Farmetta.dll"]
