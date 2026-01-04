
/**
 * Represents the current status of a player character.
 */
export interface PlayerStatus {
  generatedPokus: number;
  healthPercent: number;
  openQuests: number;
  level: number;
  characterName: string;
}

/**
 * Represents an action item on the dashboard.
 */
export interface DashboardAction {
  id: string;
  title: string;
  subtitle: string;
  icon: string;
}

export type QuestType = 'individual' | 'group';
export type QuestStatus = 'active' | 'completed' | 'failed';

export interface Quest {
  id: string;
  title: string;
  description: string;
  type: QuestType;
  assignedTo: string[]; // Character names
  status: QuestStatus;
}

export type TrophyStatus = 'bought' | 'slain';

export interface Trophy {
  id: string;
  name: string;
  monsterType: string;
  imageUrl: string;
  description: string;
  baseEffect: string;  // Effect when bought
  slainEffect: string; // Effect when slain (better)
  status: TrophyStatus;
}

export type MarkerType = 'quest' | 'info' | 'danger' | 'settlement';

export interface MapMarker {
  id: string;
  x: number; // Percentage 0-100
  y: number; // Percentage 0-100
  label: string;
  type: MarkerType;
  questId?: string;
  description?: string;
}

/**
 * Game session state.
 */
export interface GameState {
  status: PlayerStatus;
  isReady: boolean;
  isRestConfirming: boolean;
  isCampConfirming: boolean;
  isLoading: boolean;
  lastMessage: string;
  currentView: 'dashboard' | 'battle' | 'resting' | 'victory' | 'quests' | 'trophies' | 'map';
}

/**
 * Enum for game modes (C# style enum).
 */
export enum GameMode {
  EXPLORATION = 'Exploration',
  COMBAT = 'Combat',
  QUESTING = 'Questing'
}
