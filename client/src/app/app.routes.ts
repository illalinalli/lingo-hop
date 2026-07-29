import { Routes } from '@angular/router';
import { AppShell } from './features/shell/app-shell';

/**
 * Every screen lives inside the shell, so the header and tab bar are always present.
 * Pages are lazily loaded per feature: opening a lesson does not pull in the deck editor.
 */
export const routes: Routes = [
  {
    path: '',
    component: AppShell,
    children: [
      {
        path: '',
        title: 'LingoHop',
        loadComponent: () => import('./features/home/home-page'),
      },
      {
        path: 'decks',
        title: 'Your decks · LingoHop',
        loadComponent: () => import('./features/decks/deck-list-page'),
      },
      {
        // Must precede 'decks/:deckId' - without a deckId the editor is in create mode.
        path: 'decks/new',
        title: 'New deck · LingoHop',
        loadComponent: () => import('./features/decks/deck-editor-page'),
      },
      {
        path: 'decks/:deckId',
        title: 'Deck · LingoHop',
        loadComponent: () => import('./features/decks/deck-editor-page'),
      },
      {
        path: 'study/:deckId',
        title: 'Lesson · LingoHop',
        loadComponent: () => import('./features/study/study-page'),
      },
      {
        path: 'profile',
        title: 'Your progress · LingoHop',
        loadComponent: () => import('./features/profile/profile-page'),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
