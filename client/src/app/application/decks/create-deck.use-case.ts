import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { DeckDetails, DeckDraft } from '../../domain/models/deck.model';
import { DeckRepository } from '../../domain/ports/deck.repository';

/** Creates a deck, optionally with its first cards in the same request. */
@Injectable({ providedIn: 'root' })
export class CreateDeckUseCase {
  private readonly decks = inject(DeckRepository);

  execute(draft: DeckDraft): Promise<DeckDetails> {
    return firstValueFrom(this.decks.create(draft));
  }
}
