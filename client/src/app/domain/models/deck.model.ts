/** Grammar tags the API accepts. Keep in sync with the server's PartOfSpeech enum. */
export const PARTS_OF_SPEECH = [
  'Unspecified',
  'Noun',
  'Verb',
  'Adjective',
  'Adverb',
  'Pronoun',
  'Preposition',
  'Phrase',
  'Other',
] as const;

export type PartOfSpeech = (typeof PARTS_OF_SPEECH)[number];

/** A flashcard with its mastery counters. */
export interface Card {
  readonly id: string;
  /** Front of the card - the word being learned. */
  readonly term: string;
  /** Back of the card - the meaning. */
  readonly translation: string;
  readonly partOfSpeech: PartOfSpeech;
  readonly example?: string;

  readonly timesSeen: number;
  readonly timesKnown: number;
  readonly correctStreak: number;
  readonly isLearned: boolean;
  /** Share of correct answers, 0..1. */
  readonly accuracy: number;
  readonly lastReviewedAtUtc?: Date;
  readonly createdAtUtc: Date;
}

/** A deck tile: title, emoji and how much of it is learned. */
export interface Deck {
  readonly id: string;
  readonly title: string;
  readonly icon: string;
  readonly cardCount: number;
  readonly learnedCardCount: number;
  /** Cards the learner knew the last time they came up - what the bar counts. */
  readonly knownCardCount: number;
  /** Share of the deck's cards the learner knows, 0..1. */
  readonly completion: number;
  readonly createdAtUtc: Date;
}

/** A deck with all of its cards. */
export interface DeckDetails extends Deck {
  readonly cards: readonly Card[];
}

/** Text of a card, as entered by the learner. */
export interface CardDraft {
  readonly term: string;
  readonly translation: string;
  readonly partOfSpeech?: PartOfSpeech;
  readonly example?: string;
}

/** A deck to create, optionally with its first cards. */
export interface DeckDraft {
  readonly title: string;
  readonly icon?: string;
  readonly cards?: readonly CardDraft[];
}

/** Whether the deck has anything to study. */
export function isStudyable(deck: Deck): boolean {
  return deck.cardCount > 0;
}

/** Completion as a whole percentage, for labels and bar widths. */
export function completionPercent(deck: Deck): number {
  return Math.round(deck.completion * 100);
}
