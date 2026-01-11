
import React from 'react';

interface PollsWidgetProps {
  onViewAll: () => void;
}

export const PollsWidget: React.FC<PollsWidgetProps> = ({ onViewAll }) => {
  return (
    <div className="flex flex-col w-full h-full">
      {/* Absolute Align Header */}
      <div className="flex items-center justify-between border-b-2 border-primary mb-4 h-[40px] leading-none">
        <h2 className="font-display text-xl text-primary flex items-center gap-2">
          <span className="material-icons-outlined">poll</span>
          Umfragen
        </h2>
      </div>
      <div className="bg-surface-light dark:bg-surface-dark border border-border-light dark:border-border-dark p-5 rounded-lg shadow-sm hover:shadow-glow transition-all flex flex-col h-[320px]">
        <div className="flex-grow space-y-4 overflow-hidden">
          <div 
            onClick={onViewAll}
            className="block p-3 rounded bg-background-light dark:bg-background-dark border border-border-light dark:border-border-dark hover:border-primary cursor-pointer transition-all group active:scale-[0.98]"
          >
            <span className="text-xs font-bold text-primary uppercase mb-1 block">Aktiv</span>
            <h4 className="font-bold text-sm group-hover:text-primary transition-colors">Welches Event wünscht ihr euch als nächstes?</h4>
            <div className="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2 mt-2 overflow-hidden">
              <div className="bg-primary h-2 rounded-full transition-all duration-1000" style={{ width: '70%' }}></div>
            </div>
            <div className="flex justify-between items-center mt-2">
              <span className="text-[10px] font-bold text-primary opacity-0 group-hover:opacity-100 transition-opacity">Stimme abgeben &rarr;</span>
              <span className="text-[10px] opacity-60">342 Stimmen</span>
            </div>
          </div>
          <div 
            onClick={onViewAll}
            className="block p-3 rounded bg-background-light dark:bg-background-dark border border-border-light dark:border-border-dark opacity-70 hover:opacity-100 cursor-pointer transition-all hover:border-primary active:scale-[0.98]"
          >
            <span className="text-xs font-bold text-gray-500 uppercase mb-1 block">Beendet</span>
            <h4 className="font-bold text-sm">Balancing der Monsterklasse "Nachtmahr"</h4>
          </div>
        </div>
        <div className="mt-4 text-center border-t border-border-light/20 pt-4">
          <button 
            onClick={onViewAll}
            className="text-[10px] font-bold uppercase tracking-widest text-primary hover:text-white hover:bg-primary px-4 py-2 rounded border border-primary transition-all active:scale-95 inline-block"
          >
            Alle Umfragen
          </button>
        </div>
      </div>
    </div>
  );
};
