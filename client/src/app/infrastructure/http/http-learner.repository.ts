import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { LearnerProfile } from '../../domain/models/learner.model';
import { LearnerRepository } from '../../domain/ports/learner.repository';
import { LearnerProfileDto, UpdateDailyGoalDto } from '../dto/api.dto';
import { toLearnerProfile } from '../mappers/api.mapper';
import { apiRoutes } from './api-routes';

/** HTTP adapter for {@link LearnerRepository}. */
@Injectable()
export class HttpLearnerRepository extends LearnerRepository {
  private readonly http = inject(HttpClient);
  private readonly routes = apiRoutes();

  getProfile(): Observable<LearnerProfile> {
    return this.http.get<LearnerProfileDto>(this.routes.me).pipe(map(toLearnerProfile));
  }

  updateDailyGoal(cardsPerDay: number): Observable<LearnerProfile> {
    const body: UpdateDailyGoalDto = { cardsPerDay };
    return this.http
      .put<LearnerProfileDto>(this.routes.dailyGoal, body)
      .pipe(map(toLearnerProfile));
  }
}
