# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /eShopOnWebMicroServices/ApiGateway
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_ENVIRONMENT=Production

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /eShopOnWebMicroServices/ApiGateway

# Copy project files and restore dependencies
COPY ApiGateway.csproj .
RUN dotnet restore

# Copy the rest of the files and build the project
COPY . .
RUN dotnet build "ApiGateway.csproj" -c Release -o /eShopOnWebMicroServices/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /eShopOnWebMicroServices/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /eShopOnWebMicroServices/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
