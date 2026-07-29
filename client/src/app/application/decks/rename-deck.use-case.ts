import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { DeckDetails } from '../../domain/models/deck.model';
import { DeckRepository } from '../../domain/ports/deck.repository';

/** Renames a deck and/or changes its emoji. */
@Injectable({ providedIn: 'root' })
export class RenameDeckUseCase {
  private readonly decks = inject(DeckRepository);

  execute(deckId: string, title: string, icon?: string): Promise<DeckDetails> {
    return firstValueFrom(this.decks.rename(deckId, title, icon));
  }
}
