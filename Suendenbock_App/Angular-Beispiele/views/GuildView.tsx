
import React from 'react';
import { ParchmentCard } from '../components/ParchmentCard';
import { ViewType } from '../types';

interface GuildViewProps {
  onNavigate: (view: ViewType) => void;
  totalUnread?: number;
}

const GUILD_MEMBERS = [
  { name: 'Josefin Renata II von Salzburg', img: 'https://i.pravatar.cc/150?u=josefin', highlight: false },
  { name: 'Jewa Brand', img: 'https://suna.home64.de/images/characters/jewa_brand_20251116183650.jpeg', highlight: false },
  { name: 'Jeremias Vojtec', img: 'https://i.pravatar.cc/150?u=jeremias', highlight: false },
  { name: 'Emma Landter', img: 'https://i.pravatar.cc/150?u=emma', highlight: true },
  { name: 'Salome von Wellern', img: 'https://i.pravatar.cc/150?u=salome', highlight: false },
  { name: 'Gabriel-Dorian von den Guten', img: 'https://i.pravatar.cc/150?u=gabriel', highlight: false },
  { name: 'Albert Dromee', img: 'https://i.pravatar.cc/150?u=albert', highlight: false },
  { name: 'Adrijan Tamas Kovasz', img: 'https://i.pravatar.cc/150?u=adrijan', highlight: false },
];

export const GuildView: React.FC<GuildViewProps> = ({ onNavigate, totalUnread = 0 }) => {
  return (
    <div className="min-h-screen pb-24 animate-fadeIn flex flex-col bg-[#F7F3E8]">
      {/* Top Banner / Header from Image */}
      <div className="bg-[#FDFBF7] p-6 border-b border-gray-200 flex items-center space-x-6">
        <button 
          onClick={() => onNavigate('DASHBOARD')}
          className="material-icons text-primary"
        >
          arrow_back
        </button>
        <div className="w-16 h-16 rounded-full border-4 border-[#D4AF37] overflow-hidden shadow-lg">
          <img 
            src="https://images.unsplash.com/photo-1534067783941-51c9c23ecefd?q=80&w=200&auto=format&fit=crop" 
            alt="Wolkenbruch" 
            className="w-full h-full object-cover"
          />
        </div>
        <h1 className="font-serif text-2xl font-bold text-[#3E2723] tracking-wider uppercase">Wolkenbruch</h1>
      </div>

      <div className="px-4 py-6 space-y-6 flex-1 overflow-y-auto">
        
        {/* Gildendaten Card */}
        <section>
          <div className="bg-[#FDFBF7] border border-[#E8DCC4] rounded-xl p-5 shadow-sm">
            <div className="flex items-center space-x-2 border-b border-gray-200 pb-2 mb-4">
              <span className="material-icons text-primary">bar_chart</span>
              <h3 className="font-serif text-sm font-bold text-gray-700 uppercase tracking-widest">Gildendaten</h3>
            </div>
            <div className="space-y-3 text-[11px] font-serif uppercase tracking-wider text-gray-800">
              <p><span className="font-bold">Rang:</span> A-Rang</p>
              <p><span className="font-bold">Anmeldestatus:</span> Suchend mit Anmeldung</p>
              <p><span className="font-bold">Lizenzen:</span> <span className="italic text-gray-400">Keine Lizenzen</span></p>
            </div>
          </div>
        </section>

        {/* Gildenmitglieder Grid */}
        <section className="space-y-4">
          <div className="flex items-center space-x-2 px-1">
            <span className="material-icons text-primary">swords</span>
            <h3 className="font-serif text-sm font-bold text-gray-700 uppercase tracking-widest">Mitglieder (35)</h3>
          </div>
          
          <div className="grid grid-cols-4 gap-y-6 gap-x-2">
            {GUILD_MEMBERS.map((member, i) => (
              <div key={i} className="flex flex-col items-center group">
                <div className={`relative w-14 h-14 rounded-full p-0.5 mb-2 transition-transform active:scale-95 ${member.highlight ? 'ring-2 ring-primary ring-offset-2' : 'border border-gray-300'}`}>
                  <div className="w-full h-full rounded-full overflow-hidden">
                    <img src={member.img} alt={member.name} className="w-full h-full object-cover" />
                  </div>
                </div>
                <p className="text-[8px] text-center font-serif uppercase leading-tight tracking-tighter text-gray-700 max-w-[60px]">
                  {member.name}
                </p>
              </div>
            ))}
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
        <button onClick={() => onNavigate('PROFILE')} className="flex flex-col items-center text-gray-400">
          <span className="material-icons">person</span>
          <span className="text-[10px] font-serif uppercase">Profil</span>
        </button>
      </nav>
    </div>
  );
};
