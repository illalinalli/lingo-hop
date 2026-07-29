import { ChangeDetectionStrategy, Component, input } from '@angular/core';

/** Centred loading indicator with an accessible label. */
@Component({
  selector: 'lh-spinner',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="wrap" role="status" [attr.aria-label]="label()">
      <span class="ring"></span>
      <span class="sr">{{ label() }}</span>
    </div>
  `,
  styles: `
    .wrap {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 40px 0;
    }

    .ring {
      width: 30px;
      height: 30px;
      border-radius: 50%;
      border: 3px solid var(--lh-line);
      border-top-color: var(--lh-green);
      animation: lh-spin 0.8s linear infinite;
    }

    .sr {
      position: absolute;
      width: 1px;
      height: 1px;
      overflow: hidden;
      clip-path: inset(50%);
      white-space: nowrap;
    }
  `,
})
export class Spinner {
  readonly label = input('Loading');
}
