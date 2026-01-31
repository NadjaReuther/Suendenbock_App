
import React from 'react';
import { ParchmentCard } from '../components/ParchmentCard';
import { ViewType, Character } from '../types';

interface DashboardViewProps {
  username: string;
  onNavigate: (view: ViewType) => void;
  totalUnread?: number;
  members: Character[];
  newsMentions?: number;
}

export const DashboardView: React.FC<DashboardViewProps> = ({ 
  username, 
  onNavigate, 
  totalUnread = 0, 
  members,
  newsMentions = 0
}) => {
  const currentUserChar = members.find(m => m.name === username) || members[1]; 
  const isNPC = username === 'NPC';

  return (
    <div className="min-h-screen pb-24 animate-fadeIn flex flex-col">
      <header className="bg-card-bg border-b border-parchment-border p-4 sticky top-0 z-20 shadow-sm flex items-center justify-between">
        <h1 className="font-serif font-bold text-xl text-text-dark tracking-wider uppercase text-shadow-none">Sündenbock</h1>
        <button 
          onClick={() => onNavigate('NEWS')}
          className="relative flex items-center justify-center"
        >
          <span className="material-icons text-primary text-xl">notifications</span>
          {newsMentions > 0 && (
            <div className="absolute -top-1.5 -right-1.5 w-4 h-4 bg-red-600 border-2 border-card-bg rounded-full flex items-center justify-center shadow-sm">
              <span className="text-[8px] text-white font-bold">{newsMentions}</span>
            </div>
          )}
        </button>
      </header>

      <div className="px-4 py-6 space-y-8 flex-1 overflow-y-auto">
        <section className="text-center">
          <h2 className="font-serif text-lg font-bold text-text-dark uppercase tracking-wider">
            Willkommen, {username}
          </h2>
        </section>

        {isNPC && (
          <section>
            <button 
              onClick={() => onNavigate('GOD_MODE')}
              className="w-full bg-gradient-to-r from-red-900 to-red-700 text-white p-4 rounded-xl shadow-2xl flex items-center justify-between border-2 border-primary/50 group active:scale-95 transition-all"
            >
              <div className="flex items-center space-x-3">
                <span className="material-icons text-primary text-3xl">visibility</span>
                <div className="text-left">
                  <p className="font-serif text-sm font-bold uppercase tracking-[0.2em]">Göttermodus</p>
                  <p className="text-[9px] uppercase tracking-widest opacity-70">Alle Briefe offenbaren</p>
                </div>
              </div>
              <span className="material-icons group-hover:translate-x-1 transition-transform">chevron_right</span>
            </button>
          </section>
        )}

        {/* NPC user doesn't need this overview card */}
        {!isNPC && (
          <section>
            <ParchmentCard className="p-3">
              <div className="flex items-center space-x-4">
                <div className="relative w-24 h-24 flex-shrink-0">
                  <img 
                    src={currentUserChar.imageUrl} 
                    alt={currentUserChar.name} 
                    className="w-full h-full object-cover rounded border border-primary/30 shadow-sm"
                  />
                  <div className="absolute -top-1 -right-1">
                    <span className={`w-3 h-3 rounded-full border border-white shadow-sm ${
                      currentUserChar.status === 'active' ? 'bg-green-500' : 'bg-red-500'
                    }`}></span>
                  </div>
                </div>
                <div className="flex-1 flex flex-col justify-center">
                  <p className="text-[10px] font-serif font-bold text-gray-400 uppercase tracking-widest mb-2">
                    {currentUserChar.class} • Stufe {currentUserChar.level}
                  </p>
                  {currentUserChar.wikiUrl ? (
                    <a 
                      href={currentUserChar.wikiUrl} 
                      target="_blank" 
                      rel="noopener noreferrer"
                      className="inline-flex items-center justify-center space-x-2 py-3 px-3 bg-primary text-white rounded font-serif font-bold uppercase tracking-widest text-xs transition-transform active:scale-95 shadow-md"
                    >
                      <span className="material-icons text-sm">auto_stories</span>
                      <span>Wiki öffnen</span>
                    </a>
                  ) : (
                    <div className="py-3 px-3 bg-gray-100 text-gray-400 rounded font-serif font-bold uppercase tracking-widest text-xs text-center border border-dashed border-gray-300">
                      Kein Wiki-Eintrag
                    </div>
                  )}
                </div>
              </div>
            </ParchmentCard>
          </section>
        )}

        <section>
          <h2 className="font-serif text-sm font-bold text-text-dark mb-4 uppercase tracking-wider px-1">Schnellzugriff</h2>
          <div className="grid grid-cols-2 gap-4">
            <button 
              onClick={() => onNavigate('DICE_TRAY')}
              className="bg-primary text-white py-10 rounded-xl shadow-xl flex flex-col items-center justify-center space-y-3 active:scale-95 transition-transform"
            >
              <span className="material-icons text-4xl">casino</span>
              <span className="text-xs font-serif font-bold uppercase tracking-widest">Würfeln</span>
            </button>
            <button className="bg-primary text-white py-10 rounded-xl shadow-xl flex flex-col items-center justify-center space-y-3 active:scale-95 transition-transform">
              <span className="material-icons text-4xl">assignment</span>
              <span className="text-xs font-serif font-bold uppercase tracking-widest">Quests</span>
            </button>
            <button className="bg-primary text-white py-10 rounded-xl shadow-xl flex flex-col items-center justify-center space-y-3 active:scale-95 transition-transform">
              <span className="material-icons text-4xl">search</span>
              <span className="text-xs font-serif font-bold uppercase tracking-widest">Suche</span>
            </button>
            <button 
              onClick={() => onNavigate('GUILD')}
              className="bg-primary text-white py-10 rounded-xl shadow-xl flex flex-col items-center justify-center space-y-3 active:scale-95 transition-transform"
            >
              <span className="material-icons text-4xl">group</span>
              <span className="text-[10px] font-serif font-bold uppercase tracking-widest">Gilde</span>
            </button>
          </div>
        </section>
      </div>

      <nav className="fixed bottom-0 left-0 right-0 bg-card-bg border-t border-parchment-border flex justify-around items-center py-3 shadow-lg z-30">
        <button onClick={() => onNavigate('DASHBOARD')} className="flex flex-col items-center text-accent-blue">
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
            className="bg-primary text-white w-14 h-14 rounded-full shadow-xl flex items-center justify-center border-4 border-background-light active:scale-90 transition-transform"
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
        <button onClick={() => onNavigate('PROFILE')} className="flex flex-col items-center text-gray-400">
          <span className="material-icons">person</span>
          <span className="text-[10px] font-serif uppercase">Profil</span>
        </button>
      </nav>
    </div>
  );
};
