
import React from 'react';

export const Hero: React.FC = () => {
  return (
    <div className="text-center mb-16 relative">
      <div className="absolute top-1/2 left-0 w-full h-px bg-gradient-to-r from-transparent via-primary to-transparent opacity-30"></div>
      <h1 className="relative inline-block px-8 bg-background-light dark:bg-background-dark font-display text-5xl md:text-6xl text-primary drop-shadow-sm">
        Versammlungshalle
      </h1>
      <p className="mt-4 text-lg italic opacity-80 font-body">
        Ein Ort für Diskussionen, Ankündigungen und Planungen des Reiches.
      </p>
      <div className="flex justify-center gap-4 mt-6 text-primary">
        <span className="material-icons-outlined">swords</span>
      </div>
    </div>
  );
};
