# ========= BUILD STAGE =========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["DiffErgoSum/DiffErgoSum.csproj", "DiffErgoSum/"]
RUN dotnet restore "DiffErgoSum/DiffErgoSum.csproj"

COPY . .
WORKDIR /src/DiffErgoSum
RUN dotnet publish -c Release -o /app/publish

# ========= RUNTIME STAGE =========
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

COPY docker/entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

EXPOSE 8080
ENTRYPOINT ["/entrypoint.sh"]
