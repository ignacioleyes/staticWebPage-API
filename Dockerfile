FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["conduflex-api/conduflex-api.csproj", "conduflex-api/"]
RUN dotnet restore "conduflex-api/conduflex-api.csproj"
COPY . .
WORKDIR "/src/conduflex-api"
RUN dotnet build "conduflex-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "conduflex-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "conduflex-api.dll"]