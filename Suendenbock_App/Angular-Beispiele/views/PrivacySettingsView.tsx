
import React, { useState } from 'react';
import { ViewType } from '../types';

interface PrivacySettingsViewProps {
  onNavigate: (view: ViewType) => void;
  isOnlineVisible: boolean;
  onToggleOnlineVisibility: (visible: boolean) => void;
  totalUnread?: number;
}

export const PrivacySettingsView: React.FC<PrivacySettingsViewProps> = ({ 
  onNavigate, 
  isOnlineVisible, 
  onToggleOnlineVisibility,
  totalUnread = 0
}) => {
  const [muteDuration, setMuteDuration] = useState<string | null>(null);

  const muteOptions = [
    { id: '30m', label: '30 Minuten' },
    { id: '1h', label: '1 Stunde' },
    { id: '8h', label: '8 Stunden' },
    { id: '24h', label: '24 Stunden' },
    { id: 'forever', label: 'Dauerhaft stumm' },
  ];

  return (
    <div className="min-h-screen pb-24 animate-fadeIn flex flex-col bg-[#F7F3E8]">
      {/* Header */}
      <header className="bg-card-bg border-b border-parchment-border p-4 sticky top-0 z-20 shadow-sm flex items-center justify-between">
        <button 
          onClick={() => onNavigate('SECURITY_SETTINGS')}
          className="material-icons text-primary"
        >
          arrow_back
        </button>
        <h1 className="font-serif font-bold text-xl text-text-dark tracking-wider uppercase">Privatsphäre</h1>
        <div className="w-6"></div>
      </header>

      <div className="px-4 py-8 space-y-8 flex-1 overflow-y-auto">
        {/* Online Status Section */}
        <section className="space-y-4">
          <h3 className="font-serif text-sm font-bold text-gray-700 uppercase tracking-wider px-1 italic">Präsenz</h3>
          
          <div className="bg-[#FDFBF7] border border-[#E8DCC4] rounded-xl overflow-hidden shadow-sm">
            <div className="flex items-center justify-between p-4">
              <div className="flex items-center space-x-3">
                <span className="material-icons text-gray-400">visibility</span>
                <div className="text-left">
                  <p className="text-sm font-serif uppercase tracking-tight text-text-dark">Online-Status zeigen</p>
                  <p className="text-[9px] text-gray-400 font-sans">Anderen zeigen, wenn ihr in der App seid</p>
                </div>
              </div>
              <button 
                onClick={() => onToggleOnlineVisibility(!isOnlineVisible)}
                className={`w-12 h-6 rounded-full transition-colors relative ${isOnlineVisible ? 'bg-primary' : 'bg-gray-300'}`}
              >
                <div className={`absolute top-1 w-4 h-4 bg-white rounded-full transition-transform ${isOnlineVisible ? 'translate-x-7' : 'translate-x-1'}`}></div>
              </button>
            </div>
          </div>
        </section>

        {/* Chat Muting Section */}
        <section className="space-y-4">
          <div className="flex items-center justify-between px-1">
            <h3 className="font-serif text-sm font-bold text-gray-700 uppercase tracking-wider italic">Chat stummschalten</h3>
            {muteDuration && (
              <button 
                onClick={() => setMuteDuration(null)}
                className="text-[10px] text-primary font-bold uppercase tracking-tighter"
              >
                Aufheben
              </button>
            )}
          </div>
          
          <div className="bg-[#FDFBF7] border border-[#E8DCC4] rounded-xl overflow-hidden shadow-sm">
            {muteOptions.map((option, index) => (
              <button
                key={option.id}
                onClick={() => setMuteDuration(option.id)}
                className={`w-full flex items-center justify-between p-4 ${
                  index !== muteOptions.length - 1 ? 'border-b border-gray-100' : ''
                } active:bg-gray-50 transition-colors`}
              >
                <div className="flex items-center space-x-3">
                  <span className={`material-icons text-sm ${muteDuration === option.id ? 'text-primary' : 'text-gray-300'}`}>
                    {muteDuration === option.id ? 'notifications_off' : 'notifications_none'}
                  </span>
                  <span className={`text-sm font-serif uppercase tracking-tight ${
                    muteDuration === option.id ? 'text-primary font-bold' : 'text-text-dark'
                  }`}>
                    {option.label}
                  </span>
                </div>
                {muteDuration === option.id && (
                  <span className="material-icons text-primary text-sm">check_circle</span>
                )}
              </button>
            ))}
          </div>
          <p className="text-[9px] text-gray-400 font-sans italic px-2 text-center">
            Während der Stummschaltung werden keine akustischen Signale oder Push-Nachrichten für neue Briefe (Chats) gesendet.
          </p>
        </section>

        <p className="text-[10px] text-gray-400 font-serif uppercase text-center px-4 leading-relaxed mt-4">
          Die Gilde respektiert eure Ruhezeiten. Änderungen werden sofort wirksam.
        </p>
      </div>

      {/* Bottom Navigation */}
      <nav className="fixed bottom-0 left-0 right-0 bg-[#FDFBF7] border-t border-[#E8DCC4] flex justify-around items-center py-3 shadow-lg z-30">
        <button onClick={() => onNavigate('DASHBOARD')} className="flex flex-col items-center text-gray-400">
          <span className="material-icons">home</span>
          <span className="text-[10px] font-serif uppercase">Home</span>
        </button>
        <button onClick={() => onNavigate('GUILD')} className="flex flex-col items-center text-gray-400">
          <span className="material-icons">group</span>
          <span className="text-[10px] font-serif uppercase">Gilde</span>
        </button>
        <div className="relative -top-6">
          <button 
            onClick={() => onNavigate('CHAT_LIST')}
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
