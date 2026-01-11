
import React from 'react';

export const Footer: React.FC = () => {
  return (
    <footer className="bg-surface-light dark:bg-surface-dark border-t border-border-light dark:border-border-dark py-12 mt-12">
      <div className="max-w-7xl mx-auto px-4 text-center">
        <span className="font-display text-3xl text-primary font-bold tracking-widest block mb-4">Sündenbock 1618</span>
        
        <div className="flex justify-center gap-4 text-primary opacity-50 mb-6">
          <span className="material-icons-outlined">castle</span>
          <span className="material-icons-outlined">shield</span>
          <span className="material-icons-outlined">auto_stories</span>
        </div>

        <p className="text-xs opacity-60 leading-relaxed">
          © {new Date().getFullYear()} - SUENDENBOCK APP - AUTHOR: ACIDLADY<br />
          Design Inspired by Source Material for suna.home64.de
        </p>
        
        <div className="flex justify-center flex-wrap gap-x-8 gap-y-2 mt-6">
          <a className="text-xs uppercase tracking-widest font-bold hover:text-primary transition-colors" href="#">Impressum</a>
          <a className="text-xs uppercase tracking-widest font-bold hover:text-primary transition-colors" href="#">Datenschutz</a>
          <a className="text-xs uppercase tracking-widest font-bold hover:text-primary transition-colors" href="#">Kontakt</a>
        </div>

        <div className="mt-8 text-[10px] opacity-30 italic font-body">
          In den Schatten von 1618 beginnt deine Reise.
        </div>
      </div>
    </footer>
  );
};
