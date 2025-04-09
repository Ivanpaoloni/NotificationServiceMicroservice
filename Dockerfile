# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos los archivos
COPY ./NotificationService/ ./NotificationService/

# Nos movemos a la carpeta donde está el csproj
WORKDIR /src/NotificationService

# Restauramos dependencias y publicamos
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "NotificationService.dll"]
