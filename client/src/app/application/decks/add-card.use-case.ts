import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { Card, CardDraft } from '../../domain/models/deck.model';
import { DeckRepository } from '../../domain/ports/deck.repository';

/** Adds a card to a deck. The API rejects a word that is already in the deck. */
@Injectable({ providedIn: 'root' })
export class AddCardUseCase {
  private readonly decks = inject(DeckRepository);

  execute(deckId: string, draft: CardDraft): Promise<Card> {
    return firstValueFrom(this.decks.addCard(deckId, draft));
  }
}
