import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { StudyRepository } from '../../domain/ports/study.repository';

/** Drops an unfinished lesson without a reward, so the next start builds a fresh queue. */
@Injectable({ providedIn: 'root' })
export class AbandonLessonUseCase {
  private readonly study = inject(StudyRepository);

  execute(sessionId: string): Promise<void> {
    return firstValueFrom(this.study.abandon(sessionId));
  }
}
