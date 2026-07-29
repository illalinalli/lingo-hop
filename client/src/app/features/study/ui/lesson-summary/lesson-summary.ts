import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { LearnerProfile } from '../../../../domain/models/learner.model';
import { StudySession } from '../../../../domain/models/study-session.model';

/** The reward sheet that slides up when a lesson finishes. */
@Component({
  selector: 'lh-lesson-summary',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @let result = session();

    <div class="backdrop" role="dialog" aria-modal="true" aria-labelledby="lh-summary-title">
      <div class="sheet">
        <div class="celebrate" aria-hidden="true">🎉</div>
        <h2 class="title" id="lh-summary-title">Lesson complete!</h2>
        <p class="subtitle">{{ subtitle() }}</p>

        <div class="rewards">
          <div class="reward xp">
            <div class="reward-value">+{{ result.experienceEarned }}</div>
            <div class="reward-label">XP gained</div>
          </div>
          <div class="reward known">
            <div class="reward-value">{{ result.knownCards }}/{{ result.answeredCards }}</div>
            <div class="reward-label">Words known</div>
          </div>
        </div>

        @let profile = learner();

        @if (profile) {
          <div class="rows">
            <div class="row">
              <span aria-hidden="true">🔥</span>
              <span>{{ profile.streak }}-day streak</span>
            </div>
            <div class="row">
              <span aria-hidden="true">⭐</span>
              <span>Level {{ profile.level }} · {{ profile.experience }} XP total</span>
            </div>
            @if (profile.dailyGoalCompleted) {
              <div class="row done">
                <span aria-hidden="true">✅</span>
                <span>Daily goal completed</span>
              </div>
            } @else {
              <div class="row">
                <span aria-hidden="true">🎯</span>
                <span>{{ profile.cardsReviewedToday }} / {{ profile.dailyGoalCards }} cards today</span>
              </div>
            }
          </div>
        }

        <div class="actions">
          @if (canStudyAgain()) {
            <button type="button" class="again" (click)="studyAgain.emit()">Study again</button>
          }
          <button type="button" class="primary" (click)="finish.emit()">Back home</button>
        </div>
      </div>
    </div>
  `,
  styleUrl: './lesson-summary.scss',
})
export class LessonSummary {
  readonly session = input.required<StudySession>();

  readonly learner = input<LearnerProfile | null>(null);

  readonly canStudyAgain = input(true);

  readonly finish = output<void>();

  readonly studyAgain = output<void>();

  protected readonly subtitle = computed(() => {
    const result = this.session();
    if (result.answeredCards === 0) {
      return 'No cards graded this time.';
    }
    if (result.knownCards === result.answeredCards) {
      return `Perfect run - you knew all ${result.answeredCards}.`;
    }
    return `You knew ${result.knownCards} of ${result.answeredCards} words.`;
  });
}
