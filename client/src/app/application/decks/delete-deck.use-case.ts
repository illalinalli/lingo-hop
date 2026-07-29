import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { DeckRepository } from '../../domain/ports/deck.repository';

/** Deletes a deck with its cards and study history. */
@Injectable({ providedIn: 'root' })
export class DeleteDeckUseCase {
  private readonly decks = inject(DeckRepository);

  execute(deckId: string): Promise<void> {
    return firstValueFrom(this.decks.remove(deckId));
  }
}
