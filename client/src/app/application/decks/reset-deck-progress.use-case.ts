import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { DeckDetails } from '../../domain/models/deck.model';
import { DeckRepository } from '../../domain/ports/deck.repository';

/** Clears every card's mastery counters so the deck can be learned from scratch. */
@Injectable({ providedIn: 'root' })
export class ResetDeckProgressUseCase {
  private readonly decks = inject(DeckRepository);

  execute(deckId: string): Promise<DeckDetails> {
    return firstValueFrom(this.decks.resetProgress(deckId));
  }
}
