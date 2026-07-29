import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { StudySessionState } from '../../domain/models/study-session.model';
import { StudyRepository } from '../../domain/ports/study.repository';
import { GradeCardDto, StartStudySessionDto, StudySessionStateDto } from '../dto/api.dto';
import { toStudySessionState } from '../mappers/api.mapper';
import { apiRoutes } from './api-routes';

/** HTTP adapter for {@link StudyRepository}. */
@Injectable()
export class HttpStudyRepository extends StudyRepository {
  private readonly http = inject(HttpClient);
  private readonly routes = apiRoutes();

  start(deckId: string, cardLimit?: number): Observable<StudySessionState> {
    const body: StartStudySessionDto = { deckId, cardLimit: cardLimit ?? null };
    return this.http
      .post<StudySessionStateDto>(this.routes.studySessions, body)
      .pipe(map(toStudySessionState));
  }

  getById(sessionId: string): Observable<StudySessionState> {
    return this.http
      .get<StudySessionStateDto>(this.routes.studySession(sessionId))
      .pipe(map(toStudySessionState));
  }

  grade(sessionId: string, cardId: string, known: boolean): Observable<StudySessionState> {
    const body: GradeCardDto = { cardId, known };
    return this.http
      .post<StudySessionStateDto>(this.routes.studySessionGrades(sessionId), body)
      .pipe(map(toStudySessionState));
  }

  complete(sessionId: string): Observable<StudySessionState> {
    return this.http
      .post<StudySessionStateDto>(this.routes.studySessionComplete(sessionId), {})
      .pipe(map(toStudySessionState));
  }

  abandon(sessionId: string): Observable<void> {
    return this.http.delete<void>(this.routes.studySession(sessionId));
  }
}
