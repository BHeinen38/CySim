# Build CySim
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app
COPY . ./
RUN dotnet restore 
RUN dotnet publish CySim.csproj -c Debug -o out


# Migrate data using ef
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS migrate
WORKDIR /app
COPY . ./
RUN rm -f Migrations/* 
RUN dotnet restore 
RUN dotnet tool install --global dotnet-ef --version 6.0.5
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet ef migrations add DockerInit

# Running web server
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS run
WORKDIR /app
COPY --from=build /app/out ./
