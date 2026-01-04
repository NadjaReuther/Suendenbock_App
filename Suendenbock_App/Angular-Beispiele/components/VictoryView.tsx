
import React, { useEffect, useState } from 'react';

interface VictoryViewProps {
  onReturn: () => void;
}

const VictoryView: React.FC<VictoryViewProps> = ({ onReturn }) => {
  const [timeLeft, setTimeLeft] = useState(20);

  useEffect(() => {
    if (timeLeft <= 0) {
      onReturn();
      return;
    }

    const timer = setInterval(() => {
      setTimeLeft(prev => prev - 1);
    }, 1000);

    return () => clearInterval(timer);
  }, [timeLeft, onReturn]);

  return (
    <div className="fixed inset-0 z-[200] bg-black flex flex-col items-center justify-center overflow-hidden">
      {/* Subtiler Hintergrund-Glow */}
      <div className="absolute inset-0 bg-[radial-gradient(circle_at_center,_var(--tw-gradient-stops))] from-secondary/10 via-black to-black opacity-80"></div>
      
      {/* Pulsierender Fokus-Punkt in der Mitte */}
      <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] h-[600px] bg-secondary/5 blur-[120px] rounded-full animate-pulse"></div>

      <div className="relative z-10 flex flex-col items-center text-center px-4">
        <span className="material-symbols-outlined text-secondary text-8xl mb-6 animate-bounce drop-shadow-[0_0_25px_rgba(212,175,55,0.4)]">
          workspace_premium
        </span>
        
        <h1 className="text-7xl md:text-9xl font-display text-secondary uppercase tracking-[0.3em] mb-4 relative">
          Sieg!
        </h1>
        
        <p className="font-medieval text-2xl text-white/80 mb-12 tracking-wide max-w-2xl">
          Die Feinde sind bezwungen, eure Namen werden in den Tavernen besungen!
        </p>

        <div className="flex flex-col items-center">
          <button 
            onClick={onReturn}
            className="group relative bg-white/5 hover:bg-white/10 border border-white/20 px-12 py-4 rounded uppercase tracking-[0.2em] text-[12px] font-bold text-white transition-all hover:text-secondary hover:border-secondary shadow-xl active:scale-95"
          >
            Zurück zum Dashboard
          </button>
          
          {/* Ein ganz subtiler Fortschrittsbalken ganz unten am Button als kleiner Hinweis auf den automatischen Rücklauf */}
          <div className="w-full h-0.5 bg-white/10 mt-4 overflow-hidden rounded-full">
            <div 
              className="h-full bg-secondary/40 transition-all duration-1000 ease-linear"
              style={{ width: `${(timeLeft / 20) * 100}%` }}
            ></div>
          </div>
        </div>
      </div>
      
      <div className="fixed bottom-10 left-0 right-0 text-center opacity-20 pointer-events-none">
        <p className="text-[10px] font-display tracking-[0.5em] text-white uppercase">Sündenbock 1618 - Chronik der Helden</p>
      </div>
    </div>
  );
};

export default VictoryView;
