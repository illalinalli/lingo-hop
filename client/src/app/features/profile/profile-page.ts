import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { LoadLearnerProfileUseCase } from '../../application/learner/load-learner-profile.use-case';
import { UpdateDailyGoalUseCase } from '../../application/learner/update-daily-goal.use-case';
import { ToastService } from '../../core/notifications/toast.service';
import { LearnerStore } from '../../core/state/learner.store';
import { TelegramService } from '../../core/telegram/telegram.service';
import { ProgressBar } from '../../shared/ui/progress-bar/progress-bar';
import { Spinner } from '../../shared/ui/spinner/spinner';
import { ShellStore } from '../shell/state/shell.store';

/** Goal choices offered as chips. */
const GOAL_CHOICES = [5, 10, 20, 30] as const;

/** Stats and the one setting the app has: how many cards a day count as done. */
@Component({
  selector: 'lh-profile-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ProgressBar, Spinner],
  templateUrl: './profile-page.html',
  styleUrl: './profile-page.scss',
})
export default class ProfilePage {
  private readonly loadProfile = inject(LoadLearnerProfileUseCase);
  private readonly updateGoal = inject(UpdateDailyGoalUseCase);
  private readonly telegram = inject(TelegramService);
  private readonly toasts = inject(ToastService);

  protected readonly learner = inject(LearnerStore);

  protected readonly goalChoices = GOAL_CHOICES;
  protected readonly isSaving = signal(false);
  protected readonly isLoading = signal(true);

  protected readonly cardsTodayLabel = computed(() => {
    const profile = this.learner.profile();
    if (!profile) {
      return '';
    }
    return `${profile.cardsReviewedToday} of ${profile.dailyGoalCards}`;
  });

  constructor() {
    inject(ShellStore).setChrome('Your progress', false);
    void this.load();
  }

  protected async load(): Promise<void> {
    this.isLoading.set(true);
    try {
      await this.loadProfile.execute();
    } catch {
      // Reported by the HTTP error interceptor.
    } finally {
      this.isLoading.set(false);
    }
  }

  protected async chooseGoal(cardsPerDay: number): Promise<void> {
    if (this.isSaving() || this.learner.profile()?.dailyGoalCards === cardsPerDay) {
      return;
    }

    this.telegram.tap();
    this.isSaving.set(true);
    try {
      await this.updateGoal.execute(cardsPerDay);
      this.toasts.success(`Daily goal set to ${cardsPerDay} cards`);
    } catch {
      // Reported by the HTTP error interceptor.
    } finally {
      this.isSaving.set(false);
    }
  }
}
