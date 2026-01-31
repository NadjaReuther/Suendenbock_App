
import React, { useRef, useState } from 'react';
import { ViewType, Character } from '../types';

interface ProfileViewProps {
  onNavigate: (view: ViewType) => void;
  onLogout: () => void;
  username: string; 
  stats: {
    chatCount: number;
    messageCount: number;
    rollCount: number;
  };
  totalUnread?: number;
  members: Character[];
  onUpdateImage: (newUrl: string) => void;
}

export const ProfileView: React.FC<ProfileViewProps> = ({ 
  onNavigate, 
  onLogout, 
  stats, 
  username, 
  totalUnread = 0, 
  members, 
  onUpdateImage 
}) => {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [isUpdating, setIsUpdating] = useState(false);

  // Find current user character data dynamically
  const currentUserChar = members.find(m => m.name === username) || members[1];

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setIsUpdating(true);
      const reader = new FileReader();
      reader.onload = (event) => {
        if (event.target?.result) {
          onUpdateImage(event.target.result as string);
          setIsUpdating(false);
        }
      };
      reader.readAsDataURL(file);
    }
  };

  return (
    <div className="min-h-screen pb-24 animate-fadeIn flex flex-col bg-[#F7F3E8]">
      {/* Hidden File Input */}
      <input 
        type="file" 
        ref={fileInputRef} 
        className="hidden" 
        accept="image/*" 
        onChange={handleImageChange}
      />

      {/* Header */}
      <header className="bg-card-bg border-b border-parchment-border p-4 sticky top-0 z-20 shadow-sm flex items-center justify-between">
        <button 
          onClick={() => onNavigate('DASHBOARD')}
          className="material-icons text-primary"
        >
          arrow_back
        </button>
        <h1 className="font-serif font-bold text-xl text-text-dark tracking-wider uppercase">Profil</h1>
        <button onClick={onLogout} className="material-icons text-primary text-xl">logout</button>
      </header>

      <div className="px-4 py-8 space-y-8 flex-1 overflow-y-auto">
        {/* Profile Identity */}
        <section className="flex flex-col items-center">
          <div className="relative mb-4 group cursor-pointer" onClick={() => fileInputRef.current?.click()}>
            <div className={`w-32 h-32 rounded-full border-4 border-primary overflow-hidden shadow-2xl relative transition-transform active:scale-95 ${isUpdating ? 'opacity-50' : ''}`}>
              <img 
                src={currentUserChar.imageUrl} 
                alt={currentUserChar.name} 
                className="w-full h-full object-cover"
              />
              {/* Overlay for interaction */}
              <div className="absolute inset-0 bg-black/20 opacity-0 group-hover:opacity-100 flex items-center justify-center transition-opacity">
                <span className="material-icons text-white text-3xl">photo_camera</span>
              </div>
              {isUpdating && (
                <div className="absolute inset-0 flex items-center justify-center">
                  <span className="material-icons text-primary animate-spin text-3xl">autorenew</span>
                </div>
              )}
            </div>
            {/* Small floating edit button */}
            <div className="absolute bottom-1 right-1 bg-primary w-8 h-8 rounded-full border-4 border-[#F7F3E8] flex items-center justify-center shadow-md">
              <span className="material-icons text-white text-xs">edit</span>
            </div>
          </div>
          <h2 className="font-serif text-2xl font-bold text-text-dark uppercase tracking-widest">{currentUserChar.name}</h2>
          <p className="text-xs text-primary font-serif uppercase tracking-widest font-bold">Stufe {currentUserChar.level} • {currentUserChar.class}</p>
          
          <button 
            onClick={() => fileInputRef.current?.click()}
            className="mt-4 px-4 py-1.5 bg-primary/10 border border-primary/20 rounded-full text-[10px] font-serif font-bold text-primary uppercase tracking-[0.2em] active:bg-primary/20 transition-colors"
          >
            Bild ändern
          </button>
        </section>

        {/* Stats Grid */}
        <section className="grid grid-cols-3 gap-4">
          <div className="bg-[#FDFBF7] border border-[#E8DCC4] rounded-lg p-3 text-center shadow-sm flex flex-col items-center">
            <span className="material-icons text-primary/30 text-lg mb-1">forum</span>
            <p className="text-primary font-bold text-lg font-serif">{stats.chatCount}</p>
            <p className="text-[8px] uppercase font-serif tracking-tighter text-gray-500 font-bold">Chats</p>
          </div>
          <div className="bg-[#FDFBF7] border border-[#E8DCC4] rounded-lg p-3 text-center shadow-sm flex flex-col items-center">
            <span className="material-icons text-primary/30 text-lg mb-1">history_edu</span>
            <p className="text-primary font-bold text-lg font-serif">{stats.messageCount}</p>
            <p className="text-[8px] uppercase font-serif tracking-tighter text-gray-500 font-bold">Nachrichten</p>
          </div>
          <div className="bg-[#FDFBF7] border border-[#E8DCC4] rounded-lg p-3 text-center shadow-sm flex flex-col items-center">
            <span className="material-icons text-primary/30 text-lg mb-1">casino</span>
            <p className="text-primary font-bold text-lg font-serif">{stats.rollCount}</p>
            <p className="text-[8px] uppercase font-serif tracking-tighter text-gray-500 font-bold">Würfe</p>
          </div>
        </section>

        {/* Action Menu */}
        <section className="space-y-3">
          <h3 className="font-serif text-sm font-bold text-gray-700 uppercase tracking-wider px-1">Einstellungen</h3>
          <div className="bg-[#FDFBF7] border border-[#E8DCC4] rounded-xl overflow-hidden shadow-sm">
            <button 
              onClick={() => onNavigate('NOTIFICATION_SETTINGS')}
              className="w-full flex items-center justify-between p-4 border-b border-gray-100 active:bg-gray-50"
            >
              <div className="flex items-center space-x-3">
                <span className="material-icons text-gray-400">notifications_none</span>
                <span className="text-sm font-serif uppercase tracking-tight text-text-dark">Benachrichtigungen</span>
              </div>
              <span className="material-icons text-gray-300">chevron_right</span>
            </button>
            <button 
              onClick={() => onNavigate('SECURITY_SETTINGS')}
              className="w-full flex items-center justify-between p-4 active:bg-gray-50"
            >
              <div className="flex items-center space-x-3">
                <span className="material-icons text-gray-400">security</span>
                <span className="text-sm font-serif uppercase tracking-tight text-text-dark">Sicherheit</span>
              </div>
              <span className="material-icons text-gray-300">chevron_right</span>
            </button>
          </div>
        </section>
      </div>

      {/* Bottom Navigation */}
      <nav className="fixed bottom-0 left-0 right-0 bg-[#FDFBF7] border-t border-[#E8DCC4] flex justify-around items-center py-3 shadow-lg z-30">
        <button onClick={() => onNavigate('DASHBOARD')} className="flex flex-col items-center text-gray-400">
          <span className="material-icons">home</span>
          <span className="text-[10px] font-serif uppercase">Home</span>
        </button>
        <button onClick={() => onNavigate('NEWS')} className="flex flex-col items-center text-gray-400">
          <span className="material-icons">newspaper</span>
          <span className="text-[10px] font-serif uppercase">News</span>
        </button>
        <div className="relative -top-6">
          <button 
            onClick={() => onNavigate('NEW_CHAT')}
            className="bg-primary text-white w-14 h-14 rounded-full shadow-xl flex items-center justify-center border-4 border-[#F7F3E8] active:scale-90 transition-transform"
          >
            <span className="material-icons text-2xl">add</span>
          </button>
        </div>
        <button onClick={() => onNavigate('CHAT_LIST')} className="flex flex-col items-center text-gray-400 relative">
          <span className="material-icons">chat</span>
          <span className="text-[10px] font-serif uppercase">Chat</span>
          {totalUnread > 0 && (
            <div className="absolute -top-1 -right-1 w-4 h-4 bg-primary text-white text-[9px] font-bold rounded-full flex items-center justify-center border border-white">
              {totalUnread}
            </div>
          )}
        </button>
        <button onClick={() => onNavigate('PROFILE')} className="flex flex-col items-center text-primary">
          <span className="material-icons">person</span>
          <span className="text-[10px] font-serif uppercase">Profil</span>
        </button>
      </nav>
    </div>
  );
};
