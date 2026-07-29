import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { DeckRepository } from '../../domain/ports/deck.repository';

/** Removes a card from a deck. */
@Injectable({ providedIn: 'root' })
export class DeleteCardUseCase {
  private readonly decks = inject(DeckRepository);

  execute(deckId: string, cardId: string): Promise<void> {
    return firstValueFrom(this.decks.removeCard(deckId, cardId));
  }
}
