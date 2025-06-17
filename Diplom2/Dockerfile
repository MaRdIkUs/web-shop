# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Diplom2/Diplom2.csproj", "Diplom2/"]
RUN dotnet restore "./Diplom2/Diplom2.csproj"
COPY . .
WORKDIR "/src/Diplom2"
RUN dotnet build "./Diplom2.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Diplom2.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY ./GeoLite2-Country.mmdb ./GeoLite2-Country.mmdb
COPY --from=publish /app/publish .
#RUN echo "10.0.1.2 my-site.com" >> /etc/hosts
ENTRYPOINT ["dotnet", "Diplom2.dll"]