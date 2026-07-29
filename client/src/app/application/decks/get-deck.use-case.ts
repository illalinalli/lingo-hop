import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { DeckDetails } from '../../domain/models/deck.model';
import { DeckRepository } from '../../domain/ports/deck.repository';

/** A single deck with all of its cards. */
@Injectable({ providedIn: 'root' })
export class GetDeckUseCase {
  private readonly decks = inject(DeckRepository);

  execute(deckId: string): Promise<DeckDetails> {
    return firstValueFrom(this.decks.getById(deckId));
  }
}
