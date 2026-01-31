
import React from 'react';
import { ViewType } from '../types';

interface NewsViewProps {
  onNavigate: (view: ViewType) => void;
  totalUnread?: number;
  newsItems: Array<{id: string, title: string, date: string, content: string}>;
}

export const NewsView: React.FC<NewsViewProps> = ({ onNavigate, totalUnread = 0, newsItems }) => {
  return (
    <div className="min-h-screen pb-24 animate-fadeIn flex flex-col bg-[#F7F3E8]">
      <header className="bg-card-bg border-b border-parchment-border p-4 sticky top-0 z-20 shadow-sm flex items-center justify-between">
        <h1 className="font-serif font-bold text-xl text-text-dark tracking-wider uppercase">Neuigkeiten</h1>
        <button className="material-icons text-primary text-xl">refresh</button>
      </header>

      <div className="px-4 py-6 space-y-8 flex-1 overflow-y-auto">
        {/* News Quick Actions */}
        <section className="grid grid-cols-2 gap-4">
          <button className="bg-primary text-white py-6 rounded-xl shadow-lg flex flex-col items-center justify-center space-y-2 active:scale-95 transition-transform border border-white/20">
            <span className="material-icons text-3xl">poll</span>
            <span className="text-[10px] font-serif font-bold uppercase tracking-[0.2em]">Umfragen</span>
          </button>
          <button className="bg-primary text-white py-6 rounded-xl shadow-lg flex flex-col items-center justify-center space-y-2 active:scale-95 transition-transform border border-white/20">
            <span className="material-icons text-3xl">event</span>
            <span className="text-[10px] font-serif font-bold uppercase tracking-[0.2em]">Termine</span>
          </button>
        </section>

        {/* News Content List */}
        <section className="space-y-4">
          <h2 className="font-serif text-[10px] font-bold text-gray-500 uppercase tracking-[0.3em] px-1 border-b border-parchment-border pb-1">Aktuelle Berichte</h2>
          <div className="space-y-4">
            {newsItems.map((item) => (
              <div key={item.id} className="bg-[#FDFBF7] border border-[#E8DCC4] rounded-xl p-5 shadow-sm space-y-2 relative overflow-hidden">
                <div className="absolute top-0 right-0 w-12 h-12 bg-primary/5 rounded-bl-full pointer-events-none"></div>
                
                <div className="flex justify-between items-start relative z-10">
                  <h3 className="font-serif text-sm font-bold text-primary uppercase tracking-widest">{item.title}</h3>
                  <span className="text-[9px] text-gray-400 font-sans font-bold">{item.date}</span>
                </div>
                <p className="text-xs text-text-dark leading-relaxed font-sans relative z-10 whitespace-pre-line">{item.content}</p>
                
                <div className="pt-2 flex justify-end">
                  <button className="text-[8px] font-serif font-bold uppercase text-primary tracking-widest flex items-center">
                    Weiterlesen <span className="material-icons text-[10px] ml-1">chevron_right</span>
                  </button>
                </div>
              </div>
            ))}
          </div>
        </section>
      </div>

      <nav className="fixed bottom-0 left-0 right-0 bg-[#FDFBF7] border-t border-[#E8DCC4] flex justify-around items-center py-3 shadow-lg z-30">
        <button onClick={() => onNavigate('DASHBOARD')} className="flex flex-col items-center text-gray-400">
          <span className="material-icons">home</span>
          <span className="text-[10px] font-serif uppercase">Home</span>
        </button>
        <button onClick={() => onNavigate('NEWS')} className="flex flex-col items-center text-primary">
          <span className="material-icons">newspaper</span>
          <span className="text-[10px] font-serif uppercase">News</span>
        </button>
        <div className="relative -top-6">
          <button onClick={() => onNavigate('NEW_CHAT')} className="bg-primary text-white w-14 h-14 rounded-full shadow-xl flex items-center justify-center border-4 border-[#F7F3E8] active:scale-90 transition-transform">
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
