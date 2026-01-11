
import React from 'react';

interface EventsWidgetProps {
  onViewAll: () => void;
}

export const EventsWidget: React.FC<EventsWidgetProps> = ({ onViewAll }) => {
  return (
    <div className="flex flex-col w-full h-full">
      {/* Absolute Align Header */}
      <div className="flex items-center justify-between border-b-2 border-primary mb-4 h-[40px] leading-none">
        <h2 className="font-display text-xl text-primary flex items-center gap-2">
          <span className="material-icons-outlined">map</span>
          Akt-Planung
        </h2>
      </div>
      <div className="bg-surface-light dark:bg-surface-dark border border-border-light dark:border-border-dark p-5 rounded-lg shadow-sm hover:shadow-glow transition-all flex flex-col h-[320px]">
        <div className="flex-grow space-y-3 overflow-hidden">
          <div 
            onClick={onViewAll}
            className="flex gap-3 items-center p-2 hover:bg-background-light dark:hover:bg-background-dark rounded transition-all cursor-pointer group active:scale-[0.98]"
          >
            <div className="bg-red-900 text-white text-center rounded w-12 py-1 shrink-0 group-hover:scale-105 transition-transform shadow-md">
              <div className="text-[10px] uppercase">Okt</div>
              <div className="font-bold font-display text-lg leading-none">31</div>
            </div>
            <div className="min-w-0">
              <h4 className="font-bold text-sm group-hover:text-primary transition-colors truncate">Großer Schlachtzug</h4>
              <p className="text-[10px] opacity-70">Spieltag: 18:00 - 22:30</p>
            </div>
          </div>
          <div 
            onClick={onViewAll}
            className="flex gap-3 items-center p-2 hover:bg-background-light dark:hover:bg-background-dark rounded transition-all cursor-pointer group active:scale-[0.98]"
          >
            <div className="bg-primary text-white text-center rounded w-12 py-1 shrink-0 group-hover:scale-105 transition-transform shadow-md">
              <div className="text-[10px] uppercase">Nov</div>
              <div className="font-bold font-display text-lg leading-none">05</div>
            </div>
            <div className="min-w-0">
              <h4 className="font-bold text-sm group-hover:text-primary transition-colors truncate">Rüstungscheck</h4>
              <p className="text-[10px] opacity-70">Vorbereitung: 19:00 - 20:30</p>
            </div>
          </div>
        </div>
        <div className="mt-4 text-center border-t border-border-light/20 pt-4">
          <button 
            onClick={onViewAll}
            className="text-[10px] font-bold uppercase tracking-widest text-primary hover:text-white hover:bg-primary px-4 py-2 rounded border border-primary transition-all active:scale-95 inline-block"
          >
            Zum Kalender
          </button>
        </div>
      </div>
    </div>
  );
};
