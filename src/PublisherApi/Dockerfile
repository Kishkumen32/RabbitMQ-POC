FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY src/Libraries/Core/*.csproj ./src/Libraries/Core/
COPY src/Libraries/Infrastructure/Infrastructure/*.csproj ./src/Libraries/Infrastructure/Infrastructure/
COPY src/Libraries/Infrastructure/Infrastructure.AspNet/*.csproj ./src/Libraries/Infrastructure/Infrastructure.AspNet/
COPY src/Applications/PublisherApi/*.csproj ./src/Applications/PublisherApi/
RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR /source
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app ./

ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "PublisherApi.dll"]