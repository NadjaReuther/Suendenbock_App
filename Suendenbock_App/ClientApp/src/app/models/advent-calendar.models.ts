// ========================================
// ADVENT CALENDAR MODELS
// ========================================

export enum DoorType {
  Simple = 0,
  Choice = 1,
  DirectAudio = 2
}

export interface AdventDoor {
  id: number;
  dayNumber: number;
  doorType: DoorType;
  htmlContentPath?: string;
  audioPath?: string;
  emmaAudioPath?: string;
  kasimirAudioPath?: string;
}

export interface UserAdventChoice {
  id: number;
  userId: string;
  adventDoorId: number;
  choiceIndex?: number; // 0 = Emma, 1 = Kasimir
  chosenAt: Date;
}

// API Response Models
export interface GetContentResponse {
  doorType: string;
  htmlContent?: string;
  audioPath?: string;
  alreadyChosen?: boolean;
  choiceIndex?: number;
  choiceName?: string;
  emmaAudioPath?: string;
  kasimirAudioPath?: string;
}

export interface SaveChoiceRequest {
  day: number;
  choiceIndex: number; // 0 = Emma, 1 = Kasimir
}

export interface SaveChoiceResponse {
  success: boolean;
  choiceIndex: number;
  audioPath: string;
  message: string;
}

export interface IsGodResponse {
  isGod: boolean;
}

export interface OpenedDoorsResponse {
  openedDoors: number[];
}
