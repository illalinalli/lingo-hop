# syntax=docker/dockerfile:1
#
# Один образ на всё приложение: Angular собирается и кладётся в wwwroot API,
# поэтому мини-приложение и API живут на одном origin - без CORS и без второго хоста.
#
#   docker build -t lingohop .
#   docker run -p 8080:8080 -e ConnectionStrings__LingoHopDatabase=... -e Telegram__BotToken=... lingohop

# --- Angular -----------------------------------------------------------------
FROM node:22-alpine AS client
WORKDIR /src/client

# Зависимости отдельным слоем: пересобираются только при изменении lock-файла.
COPY client/package.json client/package-lock.json ./
RUN npm ci

COPY client/ ./
RUN npm run build

# --- API ---------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS api
WORKDIR /src

COPY NuGet.config ./
COPY server/ server/
RUN dotnet publish server/LingoHop.Api -c Release -o /app/publish

# --- Runtime -----------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=api /app/publish ./
COPY --from=client /src/client/dist/lingo-hop-web/browser ./wwwroot

ENV ASPNETCORE_ENVIRONMENT=Production
# Хостинг может переопределить порт через PORT - это читается в Program.cs.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Непривилегированный пользователь уже есть в базовом образе.
USER $APP_UID

ENTRYPOINT ["dotnet", "LingoHop.Api.dll"]
