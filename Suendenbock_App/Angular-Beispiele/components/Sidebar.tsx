
import React, { useState } from 'react';

const initialPlayers = [
  { name: 'Jeremias', paid: true, type: 'PayPal', icon: 'payments' },
  { name: 'Salome', paid: true, type: 'Überweisung', icon: 'account_balance' },
  { name: 'Jewa', paid: false, type: 'Ausstehend', icon: 'hourglass_empty' },
  { name: 'Jonata', paid: true, type: 'Bar', icon: 'savings' },
  { name: 'Hironimus', paid: true, type: 'PayPal', icon: 'payments' },
  { name: 'Gabriel', paid: false, type: 'Ausstehend', icon: 'hourglass_empty' },
];

export const Sidebar: React.FC = () => {
  const [players, setPlayers] = useState(initialPlayers);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const togglePaidStatus = (index: number) => {
    const newPlayers = [...players];
    newPlayers[index].paid = !newPlayers[index].paid;
    if (newPlayers[index].paid && newPlayers[index].type === 'Ausstehend') {
      newPlayers[index].type = 'PayPal';
      newPlayers[index].icon = 'payments';
    } else if (!newPlayers[index].paid) {
      newPlayers[index].type = 'Ausstehend';
      newPlayers[index].icon = 'hourglass_empty';
    }
    setPlayers(newPlayers);
  };

  return (
    <div className="flex flex-col w-full">
      {/* Absolute Align Header */}
      <div className="flex items-center justify-between border-b-2 border-primary mb-4 h-[40px] leading-none">
        <h2 className="font-display text-xl text-primary flex items-center gap-2">
          <span className="material-icons-outlined">token</span>
          Monatsbeitrag
        </h2>
        <span className="material-icons-outlined text-primary opacity-50">payments</span>
      </div>

      <div className="bg-surface-light dark:bg-surface-dark border border-border-light dark:border-border-dark rounded-lg shadow-soft flex flex-col h-[320px] p-5 relative overflow-hidden">
        <div className="absolute -top-2 -right-2 opacity-5 pointer-events-none">
          <span className="material-icons-outlined text-6xl">account_balance_wallet</span>
        </div>
        
        <div className="flex-grow overflow-y-auto pr-1 custom-scrollbar space-y-2 mb-4">
          {players.map((player, idx) => (
            <div key={idx} className="flex items-center justify-between p-2 rounded bg-background-light/50 dark:bg-background-dark/50 border border-border-light/30 dark:border-border-dark/30 transition-all hover:border-primary/30">
              <div className="flex items-center gap-2">
                <div className={`w-2 h-2 rounded-full ${player.paid ? 'bg-green-500 shadow-[0_0_5px_rgba(34,197,94,0.3)]' : 'bg-red-500 shadow-[0_0_5px_rgba(239,68,68,0.3)] animate-pulse'}`}></div>
                <span className="text-sm font-bold tracking-tight">{player.name}</span>
              </div>
              <div className="flex items-center gap-1.5 opacity-80">
                <span className="material-icons-outlined text-[14px] text-primary">{player.icon}</span>
                <span className="text-[9px] uppercase font-bold tracking-widest">{player.type}</span>
              </div>
            </div>
          ))}
        </div>

        <div className="mt-auto pt-4 border-t border-border-light/30 dark:border-border-dark/30">
          <button 
            onClick={() => setIsEditModalOpen(true)}
            className="w-full py-2 border border-primary text-primary hover:bg-primary hover:text-white text-[10px] font-bold uppercase tracking-widest rounded transition-all active:scale-95 flex items-center justify-center gap-2"
          >
            <span className="material-icons-outlined text-sm">edit_note</span>
            Beiträge verwalten
          </button>
        </div>
      </div>

      {isEditModalOpen && (
        <div className="fixed inset-0 z-[200] flex items-center justify-center p-4 bg-black/70 backdrop-blur-md animate-in fade-in duration-200">
          <div className="bg-surface-light dark:bg-surface-dark border-2 border-primary rounded-lg shadow-glow w-full max-w-md p-6">
             <div className="flex justify-between items-center mb-6">
                <h3 className="font-display text-xl text-primary font-bold">Zahlungseingänge</h3>
                <button onClick={() => setIsEditModalOpen(false)} className="text-gray-400 hover:text-primary transition-colors">
                  <span className="material-icons-outlined">close</span>
                </button>
              </div>
              <div className="space-y-4 max-h-[60vh] overflow-y-auto pr-2 custom-scrollbar">
                {players.map((p, i) => (
                  <div key={i} className="p-3 border rounded border-border-light dark:border-border-dark flex justify-between items-center bg-background-light/20 dark:bg-background-dark/20">
                    <span className="font-bold text-sm">{p.name}</span>
                    <button onClick={() => togglePaidStatus(i)} className={`text-[10px] font-bold uppercase px-3 py-1 rounded-full transition-colors ${p.paid ? 'bg-green-100 text-green-700 hover:bg-green-200' : 'bg-red-100 text-red-700 hover:bg-red-200'}`}>
                      {p.paid ? 'Bezahlt' : 'Offen'}
                    </button>
                  </div>
                ))}
              </div>
              <button onClick={() => setIsEditModalOpen(false)} className="w-full mt-6 bg-primary hover:bg-yellow-600 text-white py-3 rounded text-xs uppercase font-bold tracking-widest shadow-lg transition-all active:scale-95">Änderungen besiegeln</button>
          </div>
        </div>
      )}
    </div>
  );
};
