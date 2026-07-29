import { Observable } from 'rxjs';
import { Card, CardDraft, Deck, DeckDetails, DeckDraft } from '../models/deck.model';

/** Port for decks and the cards inside them. */
export abstract class DeckRepository {
  abstract list(): Observable<readonly Deck[]>;

  abstract getById(deckId: string): Observable<DeckDetails>;

  abstract create(draft: DeckDraft): Observable<DeckDetails>;

  abstract rename(deckId: string, title: string, icon?: string): Observable<DeckDetails>;

  abstract remove(deckId: string): Observable<void>;

  abstract resetProgress(deckId: string): Observable<DeckDetails>;

  abstract addCard(deckId: string, draft: CardDraft): Observable<Card>;

  abstract updateCard(deckId: string, cardId: string, draft: CardDraft): Observable<Card>;

  abstract removeCard(deckId: string, cardId: string): Observable<void>;
}
