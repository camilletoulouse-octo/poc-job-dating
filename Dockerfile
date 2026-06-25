# syntax=docker/dockerfile:1
#
# Image Docker pour Railway : un seul service héberge à la fois l'API
# (`ParcoursCandidatApi`) et le client Blazor WebAssembly (`ParcoursCandidatClient`).
# Le `dotnet publish` depuis la racine du repo embarque automatiquement le `wwwroot`
# du client (via le `ProjectReference`) dans `out/wwwroot/_framework/`.

# ---------- Étape de build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json ./
COPY ParcoursCandidatApi/ParcoursCandidatApi.csproj ParcoursCandidatApi/
COPY ParcoursCandidatClient/ParcoursCandidatClient.csproj ParcoursCandidatClient/
RUN dotnet restore ParcoursCandidatApi/ParcoursCandidatApi.csproj

COPY ParcoursCandidatApi/ ParcoursCandidatApi/
COPY ParcoursCandidatClient/ ParcoursCandidatClient/
RUN dotnet publish ParcoursCandidatApi/ParcoursCandidatApi.csproj -c Release -o /app/out --no-restore

# ---------- Étape de runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./out

# Railway proxifie le trafic public vers le port 8080 du conteneur ("Custom port").
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["./out/ParcoursCandidatApi"]
