FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["conduflex_api/conduflex_api.csproj", "conduflex_api/"]
RUN dotnet restore "conduflex_api/conduflex_api.csproj"
COPY . .
WORKDIR "/src/conduflex_api"
RUN dotnet build "conduflex_api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "conduflex_api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "conduflex_api.dll"]