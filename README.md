# LingoHop

Telegram Mini App для изучения английских слов по карточкам.

Пользователь открывает мини-приложение из Telegram, создаёт колоды, наполняет их
карточками (слово → перевод) и проходит тестирование «помню / не помню» со шкалой
прогресса, XP и streak-ом.

Реализация соответствует макету `BunnyLingo.dc.html` — **без кролика и магазина**
(они остаются на следующую итерацию). Панель питомца заменена панелью прогресса:
уровень, XP до следующего уровня и дневная цель.

---

## Стек

| Слой | Технологии |
|---|---|
| Backend | ASP.NET Core 10, Controllers, REST, EF Core 10, PostgreSQL, OpenAPI + Swagger UI |
| Frontend | Angular 21 (standalone, signals, zoneless), SCSS |
| Auth | Telegram Mini App `initData` (HMAC-SHA256), без паролей и сессий |

## Архитектура

### Backend — Clean Architecture + DDD

Зависимости направлены строго внутрь: `Api → Application → Domain`,
`Infrastructure → Application → Domain`. У `Domain` нет ни одной ссылки на проект
или фреймворк.

```
server/
├── LingoHop.Domain/          # агрегаты, value objects, домен-события, контракты репозиториев
│   ├── Common/               #   Entity, AggregateRoot, IDomainEvent, DomainException
│   ├── Users/                #   агрегат User: TelegramUserId, StudyStreak, ExperiencePoints…
│   ├── Decks/                #   агрегат Deck (+ Card как сущность внутри), CardMastery
│   └── Study/                #   агрегат StudySession (+ SessionCard), StudyRewardPolicy
├── LingoHop.Application/     # use cases (по одному классу на сценарий), DTO, порты
│   ├── Abstractions/         #   IUnitOfWork, IClock, ICardShuffler, Security/, Events/
│   ├── Common/               #   Result / Error — ожидаемые ошибки как данные
│   ├── Users|Decks|Study/    #   UseCases/, Dtos/, мапперы, обработчики домен-событий
│   └── DependencyInjection.cs
├── LingoHop.Infrastructure/  # адаптеры портов
│   ├── Persistence/          #   DbContext, конфигурации, репозитории, UnitOfWork, Migrations
│   ├── Events/               #   диспетчер домен-событий
│   ├── Telegram/             #   проверка подписи initData
│   └── Time|Randomisation/
└── LingoHop.Api/             # composition root: контроллеры, аутентификация, OpenAPI
```

**Ключевые решения DDD**

- **Три агрегата.** `User` (личный прогресс), `Deck` (карточки и их освоенность),
  `StudySession` (очередь урока и ответы). Ссылки между агрегатами — только по `Id`.
- **Границы соблюдаются.** Конструкторы `Card` и `SessionCard` — `internal`: изменить
  карточку можно только через корень агрегата, поэтому инвариант «слово в колоде
  уникально» невозможно обойти.
- **Value objects вместо примитивов.** `DeckTitle`, `Term`, `Translation`,
  `TelegramUserId`, `CardMastery`, `StudyStreak`, `ExperiencePoints` — валидация живёт
  в одном месте и не может быть пропущена. В БД они разворачиваются в обычные колонки
  (`HasConversion` для одиночных, `ComplexProperty` для составных).
- **Домен-события вместо связей между агрегатами.** `StudySession` при оценке карточки
  публикует `CardReviewedDomainEvent`, при завершении — `StudySessionCompletedDomainEvent`.
  Обработчики обновляют `Deck` (счётчики освоенности) и `User` (XP, streak, дневная цель).
  `UnitOfWork` рассылает события **до** `SaveChanges`, поэтому всё пишется одной транзакцией.
- **Ошибки разделены по природе.** Нарушение инварианта — `DomainException` → 400 через
  `IExceptionHandler`. Ожидаемый исход (колода не найдена) — `Result`/`Error` → ProblemDetails
  с кодом вида `deck.not_found`.
- **Политики вынесены явно.** Формула награды — `StudyRewardPolicy`, порог заучивания —
  `CardMastery.StreakToLearn`, порядок карточек в уроке — `Deck.SelectCardsForStudy`
  (случайность инжектится через `ICardShuffler`, поэтому домен остаётся тестируемым).

### Frontend — Clean Architecture + Feature-based

```
client/src/app/
├── domain/                   # чистый TypeScript: модели и порты (абстрактные классы = DI-токены)
├── application/              # use cases: learner/, decks/, study/ — по классу на сценарий
├── infrastructure/           # адаптеры портов: DTO, мапперы, HTTP-репозитории
├── core/                     # кросс-срезы: config, telegram/, http/ (interceptors), state, notifications
├── features/                 # вертикальные слайсы UI
│   ├── shell/                #   каркас: header, табы, тосты, интеграция с Telegram BackButton
│   ├── home/                 #   дашборд + state/home.store.ts
│   ├── decks/                #   список, редактор, ui/deck-tile|card-form|card-row
│   ├── study/                #   урок, ui/flashcard|lesson-summary
│   └── profile/              #   статистика и дневная цель
└── shared/ui/                # презентационные примитивы (progress-bar, stat-pill, …)
```

- **Слои** (`domain` / `application` / `infrastructure`) отвечают за данные и правила,
  **фичи** — за экраны. Порты связываются с адаптерами в единственном месте — `app.config.ts`.
- `domain/` и `application/` не знают про HTTP: репозитории возвращают `Observable`
  (деталь адаптера), а use cases отдают `Promise` — с ними проще читаются signal-сторы.
- Состояние — сигналы. Стор фичи (`HomeStore`, `DeckEditorStore`, `StudyStore`) создаётся
  вместе с экраном (`providers` компонента); общий `LearnerStore` живёт в `core/`, потому
  что переживает любой отдельный экран.
- Каждая фича — свой lazy chunk (`loadComponent`), урок не тянет редактор колод.

---

## Аутентификация

Пароля нет. Telegram отдаёт мини-приложению подписанную строку `initData`; клиент
присылает её в каждом запросе как `Authorization: tma <initData>`, а сервер каждый раз
пересчитывает HMAC-SHA256 подпись секретом, производным от токена бота, и проверяет
свежесть `auth_date`. Совпадение подписи и есть доказательство личности.

Пользователь создаётся автоматически при первом `GET /api/users/me`.

**Локальная разработка без бота:** при `Telegram:AllowDevelopmentFallback = true`
(только в `Development`) запросы без `initData` приписываются тестовому пользователю.
Заголовок `X-Dev-Telegram-Id: 123` переключает между несколькими тестовыми учётками.
Вне `Development` приложение **падает при старте**, если этот флаг включён или не задан
токен бота.

---

## REST API

Полное описание — Swagger UI: `http://localhost:5198/swagger`.

| Метод | Путь | Назначение |
|---|---|---|
| GET | `/api/users/me` | текущий пользователь (+ регистрация при первом запуске) |
| PUT | `/api/users/me/daily-goal` | изменить дневную цель |
| GET | `/api/decks` | список колод |
| POST | `/api/decks` | создать колоду (можно сразу с карточками) |
| GET | `/api/decks/{deckId}` | колода с карточками |
| PUT | `/api/decks/{deckId}` | переименовать / сменить эмодзи |
| DELETE | `/api/decks/{deckId}` | удалить колоду |
| POST | `/api/decks/{deckId}/reset-progress` | сбросить прогресс по колоде |
| POST | `/api/decks/{deckId}/cards` | добавить карточку |
| PUT | `/api/decks/{deckId}/cards/{cardId}` | изменить карточку |
| DELETE | `/api/decks/{deckId}/cards/{cardId}` | удалить карточку |
| POST | `/api/study-sessions` | начать урок (или продолжить незавершённый) |
| GET | `/api/study-sessions/{sessionId}` | прочитать урок |
| POST | `/api/study-sessions/{sessionId}/grades` | «помню / не помню» для карточки |
| POST | `/api/study-sessions/{sessionId}/complete` | завершить урок досрочно |
| DELETE | `/api/study-sessions/{sessionId}` | отменить урок без награды |
| GET | `/health`, `/health/ready` | liveness / readiness |

Все эндпоинты урока возвращают `{ session, learner }`, поэтому после каждой оценки
клиент сразу получает актуальные XP, streak и статус дневной цели — без второго запроса.

### Правила подсчёта

- **XP:** 10 за каждую отвеченную карточку + 5 за каждую угаданную (`StudyRewardPolicy`).
- **Уровень:** `1 + XP / 500`, вычисляется, не хранится.
- **Карточка «заучена»** после 3 верных ответов подряд; ошибка сбрасывает серию.
- **Streak:** календарные дни (UTC) с хотя бы одним завершённым уроком.
- **Порядок карточек в уроке:** сначала новые, затем самые слабые, заученные — последними;
  внутри группы порядок случайный.

---

## Локальный запуск

### 1. База данных

Нужен PostgreSQL. Создайте роль и БД (под суперпользователем `postgres`):

```bash
psql -U postgres -c "CREATE ROLE lingohop LOGIN PASSWORD 'lingohop';" -c "CREATE DATABASE lingohop OWNER lingohop;"
```

Либо поднимите БД в Docker:

```bash
docker compose up -d db
```

Строка подключения для разработки уже прописана в
`server/LingoHop.Api/appsettings.Development.json`.

### 2. API

```bash
dotnet run --project server/LingoHop.Api
```

Миграции применяются на старте (`Database:ApplyMigrationsOnStartup: true` в
`Development`). API: `http://localhost:5198`, Swagger: `/swagger`.

### 3. Клиент

```bash
npm --prefix client start
```

`http://localhost:4200`. Dev-сервер проксирует `/api` на `http://localhost:5198`
(`client/proxy.conf.json`), поэтому CORS в разработке не задействован.

### Работа с миграциями

```bash
dotnet ef migrations add ИмяМиграции --project server/LingoHop.Infrastructure --startup-project server/LingoHop.Api --output-dir Persistence/Migrations
```

---

## Деплой

Схема одинаковая на обеих платформах: веб-сервер отдаёт статику Angular и проксирует
`/api` на Kestrel, который слушает только петлевой интерфейс. Клиент и API — на одном
origin, поэтому CORS не нужен.

**Windows Server (IIS):**

- **[docs/database-production-windows.md](docs/database-production-windows.md)** —
  PostgreSQL: установка, роли и права, `pg_hba` без `peer`, исключения антивируса,
  тюнинг с учётом ограничения `shared_buffers` на Windows, TLS, firewall, бэкапы через
  планировщик задач, миграции, чек-лист.
- **[docs/deploy-production-windows.md](docs/deploy-production-windows.md)** —
  IIS + Hosting Bundle, пул приложений, секреты в переменных пула, reverse proxy через
  URL Rewrite + ARR, HTTPS через win-acme, регистрация в @BotFather, диагностика.

**Linux (nginx + systemd):**

- **[docs/database-production-linux.md](docs/database-production-linux.md)** — PostgreSQL:
  установка из PGDG, роли и права, TLS, ufw, тюнинг, бэкапы systemd-таймером, миграции.
- **[docs/deploy-production-linux.md](docs/deploy-production-linux.md)** — systemd-юнит,
  nginx, certbot, переменные окружения, регистрация мини-приложения.

---

## Что не реализовано намеренно

- Кролик-питомец, его настроения, кормление и магазин аксессуаров — по вашему запросу.
- Монеты: это экономика питомца, поэтому вместо них только XP.
- Готовый стартовый словарь — колоды создаёт пользователь.
- Автотесты. Домен к ним готов (чистые классы, время и случайность инжектятся),
  но тестовых проектов в решении пока нет.
