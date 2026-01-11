
import React, { useState } from 'react';

interface Ticket {
  id: number;
  title: string;
  reporter: string;
  description: string;
  status: 'pending' | 'resolved';
  date: string;
  category?: string;
}

const initialTickets: Ticket[] = [
  { id: 1, title: 'Grafikfehler in der Taverne', reporter: 'Jeremias', description: 'Die Texturen der Bierkrüge flackern.', status: 'pending', date: '12.10.1618', category: 'Bug' },
  { id: 2, title: 'Goldbeutel verliert Münzen', reporter: 'Salome', description: '2-3 Goldstücke fehlen sporadisch.', status: 'resolved', date: '10.10.1618', category: 'Bug' },
];

const categories = [
  { id: 'bug', label: 'Bug' },
  { id: 'support', label: 'Support' },
  { id: 'suggestion', label: 'Vorschlag' },
  { id: 'other', label: 'Sonstiges' },
];

export const SupportSection: React.FC = () => {
  const [tickets, setTickets] = useState<Ticket[]>(initialTickets);
  const [showTickets, setShowTickets] = useState<boolean>(false);
  const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
  const [newTicket, setNewTicket] = useState({ category: 'bug', title: '', description: '' });

  const toggleStatus = (id: number) => {
    setTickets(prev => prev.map(t => t.id === id ? { ...t, status: t.status === 'pending' ? 'resolved' : 'pending' } : t));
  };

  const deleteTicket = (id: number) => {
    if (window.confirm('Soll dieser Schandfleck wirklich endgültig aus den Akten getilgt werden?')) {
      setTickets(prev => prev.filter(t => t.id !== id));
    }
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!newTicket.title || !newTicket.description) return;
    const ticket: Ticket = {
      id: Date.now(),
      title: newTicket.title,
      reporter: 'Du',
      description: newTicket.description,
      status: 'pending',
      date: new Date().toLocaleDateString('de-DE'),
      category: categories.find(c => c.id === newTicket.category)?.label || 'Allgemein'
    };
    setTickets([ticket, ...tickets]);
    setIsModalOpen(false);
    setShowTickets(true);
    setNewTicket({ category: 'bug', title: '', description: '' });
  };

  return (
    <div className="flex flex-col">
      {/* Uniform External Header */}
      <div className="flex items-center justify-between border-b-2 border-primary pb-2 mb-4 h-[40px]">
        <h2 className="font-display text-xl text-primary flex items-center gap-2">
          <span className="material-icons-outlined">build</span>
          Schandflecken
        </h2>
        <span className="material-icons-outlined text-primary opacity-50">report_problem</span>
      </div>

      <div className="bg-surface-light dark:bg-surface-dark border border-border-light dark:border-border-dark rounded-lg shadow-soft p-5 flex flex-col h-[320px]">
        <div className="flex-grow overflow-hidden flex flex-col">
          <p className="text-[11px] opacity-60 italic leading-relaxed mb-4 shrink-0">
            Fallen euch Risse in der Matrix oder technische Gebrechen auf? Meldet Schandflecken hier.
          </p>

          <div className="space-y-3 shrink-0 mb-4">
            <button 
              onClick={() => setIsModalOpen(true)}
              className="w-full bg-primary hover:bg-yellow-600 text-white text-[10px] font-bold uppercase tracking-widest py-2 rounded shadow transition-all flex items-center justify-center gap-2"
            >
              <span className="material-icons-outlined text-sm">add</span>
              Ticket erstellen
            </button>
            <button 
              onClick={() => setShowTickets(!showTickets)}
              className={`w-full text-[10px] font-bold uppercase tracking-widest py-2 rounded border transition-all flex items-center justify-center gap-2 ${
                showTickets ? 'bg-secondary text-white border-secondary' : 'border-primary text-primary hover:bg-primary/5'
              }`}
            >
              <span className="material-icons-outlined text-sm">{showTickets ? 'visibility_off' : 'list_alt'}</span>
              {showTickets ? 'Tickets verbergen' : 'Tickets einsehen'}
            </button>
          </div>

          <div className={`flex-grow overflow-y-auto pr-1 custom-scrollbar space-y-3 transition-all duration-300 ${showTickets ? 'opacity-100' : 'opacity-0 pointer-events-none'}`}>
            {tickets.length > 0 ? tickets.map((ticket) => (
              <div key={ticket.id} className="p-3 bg-background-light/50 dark:bg-background-dark/50 border border-border-light/20 rounded-lg group relative">
                <div className="pr-6">
                  <div className="flex items-center gap-2 mb-1">
                    <div className={`w-1.5 h-1.5 rounded-full ${ticket.status === 'resolved' ? 'bg-green-500' : 'bg-primary animate-pulse'}`}></div>
                    <h4 className={`font-bold text-[11px] truncate ${ticket.status === 'resolved' ? 'line-through opacity-40' : ''}`}>{ticket.title}</h4>
                  </div>
                  <p className="text-[10px] opacity-70 line-clamp-1 italic">"{ticket.description}"</p>
                </div>
                <div className="absolute top-2 right-2 flex flex-col gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                  <button onClick={() => toggleStatus(ticket.id)} className="p-1 hover:text-green-500 transition-colors" title="Status ändern">
                    <span className="material-icons-outlined text-xs">done</span>
                  </button>
                  <button onClick={() => deleteTicket(ticket.id)} className="p-1 hover:text-red-500 transition-colors" title="Löschen">
                    <span className="material-icons-outlined text-xs">delete</span>
                  </button>
                </div>
              </div>
            )) : (
              <p className="text-center text-[10px] opacity-40 py-4 italic">Keine gemeldeten Schandflecken.</p>
            )}
          </div>
        </div>
      </div>

      {isModalOpen && (
        <div className="fixed inset-0 z-[200] flex items-center justify-center p-4 bg-black/70 backdrop-blur-md animate-in fade-in duration-200">
          <div className="bg-surface-light dark:bg-surface-dark border-2 border-primary rounded-lg shadow-glow w-full max-w-sm p-6">
            <h3 className="font-display text-xl text-primary font-bold mb-4">Schandfleck melden</h3>
            <form onSubmit={handleSubmit} className="space-y-4">
              <select 
                value={newTicket.category}
                onChange={(e) => setNewTicket({...newTicket, category: e.target.value})}
                className="w-full bg-background-light dark:bg-background-dark border-border-light rounded text-sm p-2 focus:ring-primary focus:border-primary"
              >
                {categories.map(cat => <option key={cat.id} value={cat.id}>{cat.label}</option>)}
              </select>
              <input 
                type="text" required value={newTicket.title}
                onChange={(e) => setNewTicket({...newTicket, title: e.target.value})}
                placeholder="Was ist passiert?"
                className="w-full bg-background-light dark:bg-background-dark border-border-light rounded text-sm p-2 focus:ring-primary focus:border-primary"
              />
              <textarea 
                required rows={3} value={newTicket.description}
                onChange={(e) => setNewTicket({...newTicket, description: e.target.value})}
                placeholder="Details..."
                className="w-full bg-background-light dark:bg-background-dark border-border-light rounded text-sm p-2 resize-none focus:ring-primary focus:border-primary"
              />
              <div className="flex gap-2 pt-2">
                <button type="button" onClick={() => setIsModalOpen(false)} className="flex-1 py-2 border text-[10px] font-bold uppercase rounded">Abbruch</button>
                <button type="submit" className="flex-1 py-2 bg-primary text-white text-[10px] font-bold uppercase rounded shadow-md">Senden</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
