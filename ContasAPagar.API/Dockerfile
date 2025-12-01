FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ContasAPagar.API/ContasAPagar.API.csproj", "ContasAPagar.API/"]
RUN dotnet restore "ContasAPagar.API/ContasAPagar.API.csproj"
COPY . .
WORKDIR "/src/ContasAPagar.API"
RUN dotnet build "ContasAPagar.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContasAPagar.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContasAPagar.API.dll"]

