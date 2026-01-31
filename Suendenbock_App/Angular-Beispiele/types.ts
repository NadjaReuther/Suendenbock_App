
export interface User {
  username: string;
  email: string;
  role: string;
}

export interface Character {
  id: string;
  name: string;
  class: string;
  level: number;
  status: 'active' | 'fallen' | 'missing';
  imageUrl: string;
  wikiUrl?: string;
  isOnline?: boolean;
}

export type SuccessStatus = 'PERFECT' | 'CRITICAL' | 'SUCCESS' | 'FAILURE' | 'NONE';

export interface DiceResult {
  id: string;
  rollType: '1d100' | '1d4';
  primaryRolls: number[];
  chosenPrimary: number;
  advantage: 'NONE' | 'ADVANTAGE' | 'DISADVANTAGE';
  bonusD6: number[];
  bonusSum: number;
  finalResult: number;
  talentValue?: number;
  successStatus: SuccessStatus;
  timestamp: string;
}

export interface Message {
  id: string;
  sender: string;
  text?: string;
  imageUrl?: string;
  timestamp: string;
  isMe: boolean;
  diceResult?: DiceResult;
}

export interface ChatConversation {
  id: string;
  participant: Character;
  initiatorName: string;
  title: string;
  lastMessage: string;
  time: string;
  unread?: number;
  npcMask?: string;
}

export type ViewType = 
  | 'LOGIN' 
  | 'DASHBOARD' 
  | 'CHARACTERS' 
  | 'MAP' 
  | 'GUILD' 
  | 'PROFILE' 
  | 'NOTIFICATION_SETTINGS' 
  | 'SECURITY_SETTINGS' 
  | 'PRIVACY_SETTINGS' 
  | 'CHANGE_PASSWORD'
  | 'CHAT_LIST'
  | 'CHAT_DETAIL'
  | 'NEW_CHAT'
  | 'DICE_TRAY'
  | 'NEWS'
  | 'GOD_MODE';
