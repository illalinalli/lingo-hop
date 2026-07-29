import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { Deck } from '../../domain/models/deck.model';
import { DeckRepository } from '../../domain/ports/deck.repository';

/** All decks of the current learner, newest first. */
@Injectable({ providedIn: 'root' })
export class ListDecksUseCase {
  private readonly decks = inject(DeckRepository);

  execute(): Promise<readonly Deck[]> {
    return firstValueFrom(this.decks.list());
  }
}
