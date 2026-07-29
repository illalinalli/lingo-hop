import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { Card, CardDraft, Deck, DeckDetails, DeckDraft } from '../../domain/models/deck.model';
import { DeckRepository } from '../../domain/ports/deck.repository';
import {
  CardDraftDto,
  CardDto,
  CreateDeckDto,
  DeckDetailsDto,
  DeckDto,
  UpdateDeckDto,
} from '../dto/api.dto';
import { toCard, toDeck, toDeckDetails } from '../mappers/api.mapper';
import { apiRoutes } from './api-routes';

/** HTTP adapter for {@link DeckRepository}. */
@Injectable()
export class HttpDeckRepository extends DeckRepository {
  private readonly http = inject(HttpClient);
  private readonly routes = apiRoutes();

  list(): Observable<readonly Deck[]> {
    return this.http
      .get<DeckDto[]>(this.routes.decks)
      .pipe(map((decks) => decks.map(toDeck)));
  }

  getById(deckId: string): Observable<DeckDetails> {
    return this.http.get<DeckDetailsDto>(this.routes.deck(deckId)).pipe(map(toDeckDetails));
  }

  create(draft: DeckDraft): Observable<DeckDetails> {
    const body: CreateDeckDto = {
      title: draft.title,
      icon: draft.icon ?? null,
      cards: draft.cards?.length ? draft.cards.map(toCardDraftDto) : null,
    };
    return this.http.post<DeckDetailsDto>(this.routes.decks, body).pipe(map(toDeckDetails));
  }

  rename(deckId: string, title: string, icon?: string): Observable<DeckDetails> {
    const body: UpdateDeckDto = { title, icon: icon ?? null };
    return this.http.put<DeckDetailsDto>(this.routes.deck(deckId), body).pipe(map(toDeckDetails));
  }

  remove(deckId: string): Observable<void> {
    return this.http.delete<void>(this.routes.deck(deckId));
  }

  resetProgress(deckId: string): Observable<DeckDetails> {
    return this.http
      .post<DeckDetailsDto>(this.routes.deckResetProgress(deckId), {})
      .pipe(map(toDeckDetails));
  }

  addCard(deckId: string, draft: CardDraft): Observable<Card> {
    return this.http
      .post<CardDto>(this.routes.cards(deckId), toCardDraftDto(draft))
      .pipe(map(toCard));
  }

  updateCard(deckId: string, cardId: string, draft: CardDraft): Observable<Card> {
    return this.http
      .put<CardDto>(this.routes.card(deckId, cardId), toCardDraftDto(draft))
      .pipe(map(toCard));
  }

  removeCard(deckId: string, cardId: string): Observable<void> {
    return this.http.delete<void>(this.routes.card(deckId, cardId));
  }
}

function toCardDraftDto(draft: CardDraft): CardDraftDto {
  return {
    term: draft.term,
    translation: draft.translation,
    partOfSpeech: draft.partOfSpeech ?? null,
    example: draft.example?.trim() ? draft.example.trim() : null,
  };
}
