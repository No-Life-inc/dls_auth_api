# Use the ASP.NET base image for the runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5012
EXPOSE 7266

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DLS_Backend.csproj", "./"]
RUN dotnet restore "DLS_Backend.csproj"
COPY . .
RUN dotnet build "DLS_Backend.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "DLS_Backend.csproj" -c Release -o /app/publish

# Final stage/image for running the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DLS_Backend.dll"]
