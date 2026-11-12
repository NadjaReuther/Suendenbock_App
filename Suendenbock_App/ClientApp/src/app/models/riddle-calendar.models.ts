// Rätsel-Kalender Models

export interface RiddleDoor {
  id: number;
  dayNumber: number;
  imagePath: string;
  description?: string;
}

export interface UserRiddleProgress {
  riddleDoorId: number;
  hintsUsed: number;
  attemptCount: number;
  isSolved: boolean;
  firstOpenedAt: Date;
  solvedAt?: Date;
}

export interface HintResponse {
  hint: string;
  hintsUsed: number;
}

export interface CheckSolutionResponse {
  correct: boolean;
  attemptCount: number;
  hintsUsed: number;
  isSolved: boolean;
}
