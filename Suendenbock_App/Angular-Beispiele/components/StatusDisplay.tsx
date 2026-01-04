
import React from 'react';
import { PlayerStatus } from '../types';

interface StatusDisplayProps {
  status: PlayerStatus;
}

const StatusDisplay: React.FC<StatusDisplayProps> = ({ status }) => {
  const items = [
    { value: status.generatedPokus, label: 'Generierter Pokus' },
    { value: `${status.healthPercent}%`, label: 'Gesundheit' },
    { value: status.openQuests, label: 'Offene Quests' },
    { value: `LVL ${status.level}`, label: 'Charakter' },
  ];

  return (
    <section className="mt-12 w-full max-w-4xl bg-parchment-card/40 border-y-2 border-secondary/20 py-10 px-6 rounded-sm">
      <h3 className="font-medieval text-center text-lg text-ink/70 mb-8 uppercase tracking-widest">
        Aktueller Status
      </h3>
      
      <div className="grid grid-cols-2 md:grid-cols-4 gap-8">
        {items.map((item, idx) => (
          <div key={idx} className="flex flex-col items-center">
            <span className="text-3xl font-bold text-primary mb-1">
              {item.value}
            </span>
            <span className="text-[10px] text-center uppercase tracking-widest text-ink/60 font-body">
              {item.label}
            </span>
          </div>
        ))}
      </div>
    </section>
  );
};

export default StatusDisplay;
