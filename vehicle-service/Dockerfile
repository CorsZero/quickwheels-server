FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["vehicle-service.csproj", "."]
RUN dotnet restore "vehicle-service.csproj"
COPY . .
RUN dotnet build "vehicle-service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "vehicle-service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "vehicle-service.dll"]
