import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { Card, CardDraft } from '../../domain/models/deck.model';
import { DeckRepository } from '../../domain/ports/deck.repository';

/** Edits a card's text. Mastery counters are kept. */
@Injectable({ providedIn: 'root' })
export class UpdateCardUseCase {
  private readonly decks = inject(DeckRepository);

  execute(deckId: string, cardId: string, draft: CardDraft): Promise<Card> {
    return firstValueFrom(this.decks.updateCard(deckId, cardId, draft));
  }
}
