# Usado para depuração no Visual Studio (modo rápido)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Fase de compilação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Notes.API/Notes.API.csproj", "Notes.API/"]
RUN dotnet restore "Notes.API/Notes.API.csproj"
COPY . . 
WORKDIR "/src/Notes.API"
RUN dotnet build "Notes.API.csproj" -c ${BUILD_CONFIGURATION} -o /app/build

# Fase de publicação
FROM build AS publish
RUN dotnet publish "Notes.API.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false

# Fase final para produção
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 
ENTRYPOINT ["dotnet", "Notes.API.dll"]