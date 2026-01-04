
import React from 'react';
import { Trophy, TrophyStatus } from '../types';

interface TrophyViewProps {
  onBack: () => void;
  trophies: Trophy[];
  equippedIds: string[];
  onEquip: (id: string) => void;
  onToggleStatus: (id: string) => void;
}

const TrophyView: React.FC<TrophyViewProps> = ({ onBack, trophies, equippedIds, onEquip, onToggleStatus }) => {
  const equippedTrophies = trophies.filter(t => equippedIds.includes(t.id));
  const inventoryTrophies = trophies.filter(t => !equippedIds.includes(t.id));

  return (
    <div className="min-h-screen bg-parchment-light bg-paper-texture text-ink p-4 md:p-8 animate-in fade-in duration-500 overflow-x-hidden relative">
      {/* Dekorative Eck-Elemente (wie im Dashboard) */}
      <div className="fixed top-6 left-6 w-16 h-16 border-t-2 border-l-2 border-primary/20 pointer-events-none z-0"></div>
      <div className="fixed top-6 right-6 w-16 h-16 border-t-2 border-r-2 border-primary/20 pointer-events-none z-0"></div>
      <div className="fixed bottom-6 left-6 w-16 h-16 border-b-2 border-l-2 border-primary/20 pointer-events-none z-0"></div>
      <div className="fixed bottom-6 right-6 w-16 h-16 border-b-2 border-r-2 border-primary/20 pointer-events-none z-0"></div>

      <header className="relative z-10 max-w-6xl mx-auto mb-16 border-b-2 border-primary/20 pb-10 flex flex-col items-center">
        <h1 className="text-5xl md:text-7xl font-display text-ink drop-shadow-sm text-center">Trophäen-Wand</h1>
        <p className="font-medieval text-primary/60 text-xs md:text-sm mt-2 italic tracking-[0.4em] uppercase text-center">Relikte eurer ruhmreichen Siege</p>
      </header>

      <main className="relative z-10 max-w-5xl mx-auto space-y-24">
        {/* Aktive Trophäen-Slots */}
        <section>
          <div className="flex items-center justify-center space-x-4 mb-16">
            <div className="h-px bg-primary/20 flex-grow max-w-[150px]"></div>
            <h2 className="font-display text-center text-sm md:text-lg uppercase tracking-[0.4em] text-primary/70">
              Aktuelle Zierde
            </h2>
            <div className="h-px bg-primary/20 flex-grow max-w-[150px]"></div>
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8 justify-items-center">
            {[0, 1, 2].map(i => {
              const trophy = equippedTrophies[i];
              
              return (
                <div key={i} className="relative group">
                  {/* Die Ketten-Illustration passend zum Parchment-Stil */}
                  <div className="absolute -top-16 left-1/2 -translate-x-1/2 w-[2px] h-20 bg-primary/30 z-0 shadow-sm"></div>
                  <div className="absolute -top-1 left-1/2 -translate-x-1/2 w-4 h-4 rounded-full bg-parchment-card border-2 border-primary shadow-sm z-10"></div>
                  
                  <div 
                    onClick={() => trophy && onEquip(trophy.id)}
                    className={`w-52 transition-all duration-500 cursor-pointer flex flex-col items-center ${
                        trophy ? 'scale-100 hover:scale-105' : 'scale-95 opacity-75'
                    }`}
                  >
                    {trophy ? (
                      <div className="w-full flex flex-col items-center">
                        {/* Der Schild-Hintergrund (Mount) */}
                        <div className="relative w-48 h-48">
                            {/* Wappenschild in hellem Holz/Parchment-Optik */}
                            <div className="absolute inset-0 bg-parchment-card rounded-[20%_20%_50%_50%] border-4 border-primary/30 shadow-[0_15px_30px_rgba(0,0,0,0.15)] group-hover:border-secondary transition-colors overflow-hidden">
                               <div className="absolute inset-0 bg-paper-texture opacity-30"></div>
                            </div>
                            
                            {/* Innerer Ausschnitt für das Bild */}
                            <div className="absolute inset-3.5 rounded-full overflow-hidden border-2 border-primary/20 bg-white/40 flex items-center justify-center shadow-inner">
                                <img 
                                    src={trophy.imageUrl} 
                                    alt={trophy.name} 
                                    className={`w-full h-full object-cover transition-all duration-700 group-hover:scale-110 ${
                                        trophy.status === 'slain' ? 'saturate-[1.1]' : 'grayscale opacity-60'
                                    }`}
                                />
                                <div className="absolute inset-0 bg-ink/70 opacity-0 group-hover:opacity-100 flex items-center justify-center transition-opacity z-20">
                                    <span className="material-symbols-outlined text-white text-3xl">close</span>
                                </div>
                            </div>

                            {/* Status Badge */}
                            <div className={`absolute -bottom-1 -right-1 w-9 h-9 rounded-full flex items-center justify-center border-2 shadow-lg z-30 transition-transform group-hover:scale-110 ${
                              trophy.status === 'slain' ? 'bg-green-600 text-white border-white/40' : 'bg-secondary text-ink border-black/10'
                            }`}>
                              <span className="material-symbols-outlined text-[20px]">
                                {trophy.status === 'slain' ? 'target' : 'toll'}
                              </span>
                            </div>
                        </div>

                        {/* Die Messing-Plakette */}
                        <div className="mt-5 relative w-full bg-secondary p-[1.5px] rounded-sm shadow-xl transform hover:rotate-0 transition-transform -rotate-1">
                            <div className="bg-[#D4AF37] px-3 py-2.5 rounded-sm border border-black/10 flex flex-col items-center text-center">
                                <h3 className="font-medieval text-xs text-ink font-bold uppercase tracking-tight mb-1 leading-none">{trophy.name}</h3>
                                <div className="h-px bg-ink/10 w-2/3 mb-1.5"></div>
                                <p className="text-[9px] text-ink font-bold italic h-6 flex items-center justify-center leading-tight">
                                    {trophy.status === 'slain' ? trophy.slainEffect : trophy.baseEffect}
                                </p>
                            </div>
                        </div>
                      </div>
                    ) : (
                      <div className="w-48 h-48 rounded-[20%_20%_50%_50%] border-2 border-dashed border-primary/75 flex flex-col items-center justify-center text-primary/75 group-hover:text-secondary group-hover:border-secondary transition-all bg-white/30">
                        <span className="material-symbols-outlined text-4xl mb-2">add_circle</span>
                        <span className="text-[9px] uppercase tracking-widest font-bold">Platz frei</span>
                      </div>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        </section>

        {/* Trenner im Dashboard-Stil */}
        <div className="flex items-center justify-center space-x-6 opacity-30 py-4">
            <div className="h-px bg-primary flex-grow max-w-[200px]"></div>
            <span className="material-symbols-outlined text-primary text-2xl">swords</span>
            <div className="h-px bg-primary flex-grow max-w-[200px]"></div>
        </div>

        {/* Lager / Inventar - 3er Grid ab Tablet (md) */}
        <section className="pb-20">
          <h2 className="font-display text-center text-sm md:text-lg uppercase tracking-[0.4em] mb-12 text-primary/40">
            In der Trophäen-Truhe
          </h2>
          
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 md:gap-8">
            {inventoryTrophies.map(trophy => {
              return (
                <div 
                  key={trophy.id}
                  className="group flex flex-col items-center bg-parchment-card/30 p-6 md:p-8 rounded-lg border border-parchment-border hover:border-secondary transition-all hover:bg-white/40 shadow-sm"
                >
                  {/* Wappenschild */}
                  <div className="relative w-36 h-36 md:w-40 md:h-40 mb-6">
                    <div className="absolute inset-0 bg-parchment-border/40 rounded-[20%_20%_50%_50%] border-2 border-primary/20 shadow-sm"></div>
                    <div className="absolute inset-2.5 rounded-full overflow-hidden border-2 border-primary/10 bg-white/20 flex items-center justify-center">
                        <img 
                            src={trophy.imageUrl} 
                            alt={trophy.name} 
                            className={`w-full h-full object-cover transition-all duration-700 group-hover:scale-110 ${
                                trophy.status === 'slain' ? 'saturate-[1.1]' : 'grayscale opacity-50'
                            }`}
                        />
                    </div>

                    {/* Status Toggle Badge */}
                    <button 
                      onClick={(e) => { e.stopPropagation(); onToggleStatus(trophy.id); }}
                      className={`absolute -top-1 -right-1 w-8 h-8 md:w-9 md:h-9 rounded-full flex items-center justify-center border transition-all z-30 shadow-md transform hover:scale-110 active:scale-95 ${
                        trophy.status === 'slain' 
                          ? 'bg-green-600 text-white border-white/40' 
                          : 'bg-secondary text-ink border-black/10'
                      }`}
                    >
                      <span className="material-symbols-outlined text-[16px] md:text-[20px]">
                        {trophy.status === 'slain' ? 'target' : 'toll'}
                      </span>
                    </button>
                  </div>
                  
                  <h3 className="font-medieval text-ink text-base md:text-lg font-bold uppercase text-center mb-1">{trophy.name}</h3>
                  <div className="flex flex-col items-center">
                    <p className="text-[9px] md:text-[10px] text-primary/60 font-display font-bold uppercase tracking-widest">
                      {trophy.monsterType}
                    </p>
                    <p className="text-[8px] text-ink/40 uppercase font-bold tracking-tighter mt-1">
                      {trophy.status === 'slain' ? 'Erlegt' : 'Gekauft'}
                    </p>
                  </div>
                  
                  <div className="w-full h-px bg-primary/10 mb-4 mt-3"></div>
                  
                  <p className="text-[10px] md:text-[11px] text-ink/70 font-body leading-tight text-center italic mb-6 h-12 overflow-hidden">
                      {trophy.description}
                  </p>

                  <button 
                      onClick={() => onEquip(trophy.id)}
                      disabled={equippedIds.length >= 3}
                      className="w-full py-3 bg-primary/10 hover:bg-secondary hover:text-ink text-primary border border-primary/20 rounded text-[9px] md:text-[10px] font-bold uppercase tracking-widest transition-all disabled:opacity-20"
                  >
                      An die Wand hängen
                  </button>
                </div>
              );
            })}
          </div>
          
          {inventoryTrophies.length === 0 && (
            <div className="text-center py-20 border-2 border-dashed border-primary/10 rounded-lg">
                <span className="material-symbols-outlined text-5xl text-primary/10 mb-4">inventory_2</span>
                <p className="font-medieval text-xl text-ink/30 italic">Die Truhe ist leer.</p>
            </div>
          )}
        </section>

        {/* Zurück Button */}
        <section className="flex flex-col items-center pb-12">
          <button 
            onClick={onBack} 
            className="group flex flex-col items-center space-y-4 transition-all duration-500 hover:scale-110 active:scale-95 px-12 py-6 rounded-lg bg-parchment-card border border-parchment-border shadow-md hover:shadow-xl"
          >
            <span className="material-symbols-outlined text-primary text-5xl group-hover:-translate-y-1 transition-transform">fireplace</span>
            <div className="text-center">
              <span className="block font-display uppercase tracking-[0.5em] text-sm text-ink group-hover:text-secondary transition-colors">Zurück zum Lager</span>
              <span className="block text-[10px] font-medieval text-ink/40 italic mt-1">Legt die Ausrüstung ab</span>
            </div>
          </button>
        </section>
      </main>

      <footer className="mt-20 text-center text-ink/20 pb-12 border-t border-primary/5 pt-10">
          <p className="text-[10px] font-display tracking-[0.6em] uppercase">Unvergänglicher Ruhm im Jahre 1618</p>
      </footer>
    </div>
  );
};

export default TrophyView;
