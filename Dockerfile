#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RedisTest/RedisTest.csproj", "RedisTest/"]
COPY ["RedisTest.Library/RedisTest.Library.csproj", "RedisTest.Library/"]
RUN dotnet restore "RedisTest/RedisTest.csproj"
COPY . .
WORKDIR "/src/RedisTest"
RUN dotnet build "RedisTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RedisTest.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RedisTest.dll"]