
import React from 'react';
import { DashboardAction } from '../types';

interface ActionCardProps {
  action: DashboardAction;
  onClick: () => void;
}

const ActionCard: React.FC<ActionCardProps> = ({ action, onClick }) => {
  return (
    <button
      onClick={onClick}
      className="group relative bg-parchment-card p-8 rounded-lg shadow-md hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 border border-parchment-border flex flex-col items-center justify-center min-h-[220px] w-full"
    >
      {/* Corner Brackets */}
      <div className="absolute top-2 left-2 w-5 h-5 border-t-2 border-l-2 border-primary/40 group-hover:border-secondary transition-colors"></div>
      <div className="absolute top-2 right-2 w-5 h-5 border-t-2 border-r-2 border-primary/40 group-hover:border-secondary transition-colors"></div>
      <div className="absolute bottom-2 left-2 w-5 h-5 border-b-2 border-l-2 border-primary/40 group-hover:border-secondary transition-colors"></div>
      <div className="absolute bottom-2 right-2 w-5 h-5 border-b-2 border-r-2 border-primary/40 group-hover:border-secondary transition-colors"></div>

      <div className="mb-4 text-primary group-hover:text-secondary group-hover:scale-110 transition-all duration-300">
        <span className="material-symbols-outlined text-6xl">
          {action.icon}
        </span>
      </div>
      
      <h2 className="font-medieval text-2xl text-ink uppercase tracking-wide group-hover:text-primary transition-colors">
        {action.title}
      </h2>
      
      <p className="text-[10px] text-gray-500 mt-2 font-display tracking-[0.2em] uppercase">
        {action.subtitle}
      </p>
    </button>
  );
};

export default ActionCard;
