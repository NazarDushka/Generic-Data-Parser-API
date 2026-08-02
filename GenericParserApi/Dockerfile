# syntax=docker/dockerfile:1
 
# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
 
# Copy csproj files first, preserving folder structure, for better layer caching
COPY GenericParserApi/GenericParserApi.csproj GenericParserApi/GenericParserApi.csproj
COPY ParserTests/ParserTests.csproj ParserTests/ParserTests.csproj
RUN dotnet restore GenericParserApi/GenericParserApi.csproj
 

COPY . .
RUN dotnet publish GenericParserApi/GenericParserApi.csproj -c Release -o /app/publish --no-restore
 
# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
 
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
 
COPY --from=build /app/publish .
 
ENTRYPOINT ["dotnet", "GenericParserApi.dll"]
 