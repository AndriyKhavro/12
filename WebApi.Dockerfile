#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RedisTest.WebApi/RedisTest.WebApi.csproj", "RedisTest.WebApi/"]
COPY ["RedisTest.Library/RedisTest.Library.csproj", "RedisTest.Library/"]
RUN dotnet restore "RedisTest.WebApi/RedisTest.WebApi.csproj"
COPY . .
WORKDIR "/src/RedisTest.WebApi"
RUN dotnet build "RedisTest.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RedisTest.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RedisTest.WebApi.dll"]