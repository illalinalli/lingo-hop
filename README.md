# LingoHop

A Telegram Mini App for learning English words with flashcards.

**Try it live: [t.me/lingohop_bot/learn](https://t.me/lingohop_bot/learn)**

The user opens the mini app from Telegram, creates decks, fills them with cards
(word → translation) and goes through an "I remember / I don't" quiz with a progress
bar, XP and a streak.

The implementation follows the `BunnyLingo.dc.html` mockup — **without the rabbit and
the shop** (those are left for the next iteration). The pet panel is replaced by a
progress panel: level, XP to the next level and the daily goal.

---

## Features

- Decks: create, rename, set an emoji, delete, reset progress.
- Cards: add, edit, delete; a word is unique within a deck.
- Lesson: cards are ordered new → weak → learned, answered with "I remember / I don't".
  A card counts as learned after 3 correct answers in a row.
- Progress: XP and level, daily goal, streak by calendar days, statistics in the profile.

---

## Stack

| Layer | Technologies |
|---|---|
| Backend | ASP.NET Core 10, Controllers, REST, EF Core 10, PostgreSQL, OpenAPI + Swagger UI |
| Frontend | Angular 21 (standalone, signals, zoneless), SCSS |
| Auth | Telegram Mini App `initData` (HMAC-SHA256), no passwords or sessions |

---

## How the project is organised

### Backend — Clean Architecture + DDD

```
Api → Application → Domain
         ↑
   Infrastructure
```

```
server/
├── LingoHop.Domain/          # business rules: User, Deck, StudySession aggregates
├── LingoHop.Application/     # use cases and DTOs
├── LingoHop.Infrastructure/  # PostgreSQL, Telegram, external adapters
└── LingoHop.Api/             # REST API and composition root
```

### Frontend — Angular, feature-based

```
domain → application → infrastructure
                ↓
             features
```

```
client/src/app/
├── domain/          # models and ports
├── application/     # use cases
├── infrastructure/  # HTTP repositories
├── core/            # config, telegram, interceptors, shared state
├── features/        # shell, home, decks, study, profile
└── shared/ui/       # presentational primitives
```

State is signals: every feature has its own store, the shared `LearnerStore` lives in
`core/`. Ports are bound to adapters in `app.config.ts`. Each feature is loaded as its
own lazy chunk.

### Authentication

There is no password. Telegram gives the mini app a signed `initData` string; the client
sends it with every request as `Authorization: tma <initData>`, and the server verifies
the HMAC-SHA256 signature and the freshness of `auth_date`. The user is created
automatically on the first `GET /api/users/me`.

For development without a bot there is `Telegram:AllowDevelopmentFallback` (in
`Development` only): requests without `initData` are attributed to a test user, and the
`X-Dev-Telegram-Id: 123` header switches between test accounts.

---

## Running locally

**1. Database.** PostgreSQL is required:

```bash
psql -U postgres -c "CREATE ROLE lingohop LOGIN PASSWORD 'lingohop';" -c "CREATE DATABASE lingohop OWNER lingohop;"
```

Or via Docker: `docker compose up -d db`. The development connection string is already
in `server/LingoHop.Api/appsettings.Development.json`.

**2. API:**

```bash
dotnet run --project server/LingoHop.Api
```

Migrations are applied on startup. API: `http://localhost:5198`, Swagger: `/swagger`.

**3. Client:**

```bash
npm --prefix client start
```

`http://localhost:4200`. The dev server proxies `/api` to `http://localhost:5198`
(`client/proxy.conf.json`), so CORS is not involved in development.

**New migration:**

```bash
dotnet ef migrations add MigrationName --project server/LingoHop.Infrastructure --startup-project server/LingoHop.Api --output-dir Persistence/Migrations
```

---

## REST API

Full description — Swagger UI: `http://localhost:5198/swagger`.

| Method | Path | Purpose |
|---|---|---|
| GET | `/api/users/me` | current user (+ registration on first run) |
| PUT | `/api/users/me/daily-goal` | change the daily goal |
| GET | `/api/decks` | list decks |
| POST | `/api/decks` | create a deck (optionally with cards) |
| GET | `/api/decks/{deckId}` | deck with its cards |
| PUT | `/api/decks/{deckId}` | rename / change emoji |
| DELETE | `/api/decks/{deckId}` | delete a deck |
| POST | `/api/decks/{deckId}/reset-progress` | reset progress for a deck |
| POST | `/api/decks/{deckId}/cards` | add a card |
| PUT | `/api/decks/{deckId}/cards/{cardId}` | edit a card |
| DELETE | `/api/decks/{deckId}/cards/{cardId}` | delete a card |
| POST | `/api/study-sessions` | start a lesson (or resume an unfinished one) |
| GET | `/api/study-sessions/{sessionId}` | read a lesson |
| POST | `/api/study-sessions/{sessionId}/grades` | "I remember / I don't" for a card |
| POST | `/api/study-sessions/{sessionId}/complete` | finish a lesson early |
| DELETE | `/api/study-sessions/{sessionId}` | cancel a lesson without a reward |
| GET | `/health`, `/health/ready` | liveness / readiness |

All lesson endpoints return `{ session, learner }`, so after every answer the client
immediately gets the current XP, streak and daily goal status — without a second request.

---

## Intentionally not implemented

- The pet rabbit, its moods, feeding and the accessory shop — as requested.
- Coins: they belong to the pet economy, so there is XP only.
- A ready-made starter dictionary — decks are created by the user.
- Automated tests. The domain is ready for them (pure classes, time and randomness are
  injected), but there are no test projects in the solution yet.
