
import React, { useState } from 'react';
import { ViewType } from '../types';

interface NotificationSettingsViewProps {
  onNavigate: (view: ViewType) => void;
  totalUnread?: number;
}

export const NotificationSettingsView: React.FC<NotificationSettingsViewProps> = ({ onNavigate, totalUnread = 0 }) => {
  const [settings, setSettings] = useState({
    sound: true,
    vibration: true,
    push: true,
    mentions: true
  });

  const toggleSetting = (key: keyof typeof settings) => {
    setSettings(prev => ({ ...prev, [key]: !prev[key] }));
  };

  return (
    <div className="min-h-screen pb-24 animate-fadeIn flex flex-col bg-[#F7F3E8]">
      {/* Header */}
      <header className="bg-card-bg border-b border-parchment-border p-4 sticky top-0 z-20 shadow-sm flex items-center justify-between">
        <button 
          onClick={() => onNavigate('PROFILE')}
          className="material-icons text-primary"
        >
          arrow_back
        </button>
        <h1 className="font-serif font-bold text-xl text-text-dark tracking-wider uppercase">Meldungen</h1>
        <div className="w-6"></div> {/* Spacer */}
      </header>

      <div className="px-4 py-8 space-y-6 flex-1 overflow-y-auto">
        <section className="space-y-4">
          <h3 className="font-serif text-sm font-bold text-gray-700 uppercase tracking-wider px-1 italic">Signale & Töne</h3>
          
          <div className="bg-[#FDFBF7] border border-[#E8DCC4] rounded-xl overflow-hidden shadow-sm">
            {/* Ton Toggle */}
            <div className="flex items-center justify-between p-4 border-b border-gray-100">
              <div className="flex items-center space-x-3">
                <span className="material-icons text-gray-400">volume_up</span>
                <span className="text-sm font-serif uppercase tracking-tight text-text-dark">Töne aktivieren</span>
              </div>
              <button 
                onClick={() => toggleSetting('sound')}
                className={`w-12 h-6 rounded-full transition-colors relative ${settings.sound ? 'bg-primary' : 'bg-gray-300'}`}
              >
                <div className={`absolute top-1 w-4 h-4 bg-white rounded-full transition-transform ${settings.sound ? 'translate-x-7' : 'translate-x-1'}`}></div>
              </button>
            </div>

            {/* Vibration Toggle */}
            <div className="flex items-center justify-between p-4 border-b border-gray-100">
              <div className="flex items-center space-x-3">
                <span className="material-icons text-gray-400">vibration</span>
                <span className="text-sm font-serif uppercase tracking-tight text-text-dark">Vibration</span>
              </div>
              <button 
                onClick={() => toggleSetting('vibration')}
                className={`w-12 h-6 rounded-full transition-colors relative ${settings.vibration ? 'bg-primary' : 'bg-gray-300'}`}
              >
                <div className={`absolute top-1 w-4 h-4 bg-white rounded-full transition-transform ${settings.vibration ? 'translate-x-7' : 'translate-x-1'}`}></div>
              </button>
            </div>

            {/* Push Toggle */}
            <div className="flex items-center justify-between p-4 border-b border-gray-100">
              <div className="flex items-center space-x-3">
                <span className="material-icons text-gray-400">notifications_active</span>
                <span className="text-sm font-serif uppercase tracking-tight text-text-dark">Push-Nachrichten</span>
              </div>
              <button 
                onClick={() => toggleSetting('push')}
                className={`w-12 h-6 rounded-full transition-colors relative ${settings.push ? 'bg-primary' : 'bg-gray-300'}`}
              >
                <div className={`absolute top-1 w-4 h-4 bg-white rounded-full transition-transform ${settings.push ? 'translate-x-7' : 'translate-x-1'}`}></div>
              </button>
            </div>

            {/* Erwähnungen Toggle */}
            <div className="flex items-center justify-between p-4">
              <div className="flex items-center space-x-3">
                <span className="material-icons text-gray-400">alternate_email</span>
                <span className="text-sm font-serif uppercase tracking-tight text-text-dark">Bei Erwähnungen</span>
              </div>
              <button 
                onClick={() => toggleSetting('mentions')}
                className={`w-12 h-6 rounded-full transition-colors relative ${settings.mentions ? 'bg-primary' : 'bg-gray-300'}`}
              >
                <div className={`absolute top-1 w-4 h-4 bg-white rounded-full transition-transform ${settings.mentions ? 'translate-x-7' : 'translate-x-1'}`}></div>
              </button>
            </div>
          </div>
        </section>

        <p className="text-[10px] text-gray-400 font-serif uppercase text-center px-4 leading-relaxed">
          Diese Einstellungen wirken sich auf alle Benachrichtigungen innerhalb der Wolkenbruch-Gilde aus.
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
