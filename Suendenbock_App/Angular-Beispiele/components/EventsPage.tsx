
import React, { useState } from 'react';

type EventType = 'Spieltag' | 'Vorbereitung' | 'Ausflug';

const CHORE_NAMES = [
  'Boxen wieder einräumen',
  'Müll sammeln',
  'Schmeckiedienst',
  'Alles zurück ins Lager',
  'Mittagshilfe',
  'Frühschicht',
  'Geschirrdienst',
  'Müllentsorgung',
  'Tafel'
];

const SPECIAL_CHORES = ['Frühschicht', 'Schmeckiedienst'];

const PEOPLE = [
  'Gott',
  'Jeremias',
  'Salome',
  'Jewa',
  'Jonata',
  'Hironimus',
  'Gabriel',
  'Alle'
];

interface GameEvent {
  id: number;
  title: string;
  date: string; // Internal format YYYY-MM-DD for input
  displayDate: string; // German format DD.MM.YYYY
  month: string;
  day: string;
  type: EventType;
  startTime: string;
  endTime: string;
  description: string;
  participants: number;
  chores?: Record<string, string>;
}

const initialEvents: GameEvent[] = [
  {
    id: 1,
    title: 'Großer Schlachtzug am Inn',
    date: '1618-10-31',
    displayDate: '31.10.1618',
    day: '31',
    month: 'Okt',
    type: 'Spieltag',
    startTime: '18:00',
    endTime: '22:30',
    description: 'Wenn der Nebel aufzieht, kommen sie. Verteidigt die Tore gegen die Horde der Schatten.',
    participants: 45,
    chores: {
      'Schmeckiedienst': 'Salome',
      'Tafel': 'Jeremias',
      'Müllentsorgung': 'Alle',
      'Boxen wieder einräumen': 'Jonata',
      'Geschirrdienst': 'Hironimus',
      'Frühschicht': 'Jeremias'
    }
  },
  {
    id: 2,
    title: 'Waffenkammer Inventur',
    date: '1618-11-05',
    displayDate: '05.11.1618',
    day: '05',
    month: 'Nov',
    type: 'Vorbereitung',
    startTime: '19:00',
    endTime: '20:30',
    description: 'Besprechung der neuen Gildensteuern und Planung der kommenden Feldzüge.',
    participants: 12
  }
];

export const EventsPage: React.FC<{ onBack: () => void }> = ({ onBack }) => {
  const [events, setEvents] = useState<GameEvent[]>(initialEvents);
  const [rsvps, setRsvps] = useState<Record<number, 'yes' | 'maybe' | 'no'>>({});
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [visibleChores, setVisibleChores] = useState<Record<number, boolean>>({});

  // Form State
  const [formData, setFormData] = useState({
    title: '',
    date: '',
    type: 'Spieltag' as EventType,
    startTime: '',
    endTime: '',
    description: '',
    chores: {} as Record<string, string>
  });

  const toggleChoresVisibility = (eventId: number) => {
    setVisibleChores(prev => ({ ...prev, [eventId]: !prev[eventId] }));
  };

  const handleRSVP = (eventId: number, status: 'yes' | 'maybe' | 'no') => {
    setRsvps({ ...rsvps, [eventId]: status });
  };

  const openCreateModal = () => {
    setEditingId(null);
    setFormData({
      title: '',
      date: '',
      type: 'Spieltag',
      startTime: '',
      endTime: '',
      description: '',
      chores: {}
    });
    setIsModalOpen(true);
  };

  const openEditModal = (event: GameEvent) => {
    setEditingId(event.id);
    setFormData({
      title: event.title,
      date: event.date,
      type: event.type,
      startTime: event.startTime,
      endTime: event.endTime,
      description: event.description,
      chores: event.chores || {}
    });
    setIsModalOpen(true);
  };

  const handleChoreChange = (chore: string, person: string) => {
    setFormData(prev => ({
      ...prev,
      chores: { ...prev.chores, [chore]: person }
    }));
  };

  const handleSaveEvent = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.title || !formData.date || !formData.startTime || !formData.endTime) return;

    const dateObj = new Date(formData.date);
    const day = dateObj.getDate().toString().padStart(2, '0');
    const months = ['Jan', 'Feb', 'Mär', 'Apr', 'Mai', 'Jun', 'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Dez'];
    const monthStr = months[dateObj.getMonth()];

    const updatedEvent: GameEvent = {
      id: editingId || Date.now(),
      title: formData.title,
      date: formData.date,
      displayDate: dateObj.toLocaleDateString('de-DE'),
      day,
      month: monthStr,
      type: formData.type,
      startTime: formData.startTime,
      endTime: formData.endTime,
      description: formData.description,
      participants: editingId ? (events.find(e => e.id === editingId)?.participants || 0) : 0,
      chores: formData.type === 'Spieltag' ? formData.chores : undefined
    };

    if (editingId) {
      setEvents(prev => prev.map(e => e.id === editingId ? updatedEvent : e).sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime()));
    } else {
      setEvents(prev => [...prev, updatedEvent].sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime()));
    }

    setIsModalOpen(false);
  };

  const handleDeleteEvent = (id: number) => {
    if (window.confirm('Soll dieser Termin wirklich aus den Annalen gelöscht werden?')) {
      setEvents(prev => prev.filter(e => e.id !== id));
    }
  };

  const getTypeStyle = (type: EventType) => {
    switch (type) {
      case 'Spieltag': return 'bg-red-900/20 text-red-700 border-red-900/30 dark:text-red-400';
      case 'Vorbereitung': return 'bg-primary/20 text-primary border-primary/30';
      case 'Ausflug': return 'bg-secondary/20 text-secondary border-secondary/30 dark:text-secondary';
      default: return 'bg-gray-100 text-gray-700 border-gray-200';
    }
  };

  return (
    <div className="flex-grow max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-12 w-full animate-in slide-in-from-left duration-500">
      <div className="mb-10 flex flex-col md:flex-row md:items-center justify-between gap-6">
        <div>
          <button 
            onClick={onBack}
            className="flex items-center gap-2 text-primary hover:text-secondary transition-colors mb-4 group"
          >
            <span className="material-icons-outlined group-hover:-translate-x-1 transition-transform">arrow_back</span>
            <span className="text-xs font-bold uppercase tracking-widest">Zurück zur Halle</span>
          </button>
          <h1 className="font-display text-4xl text-primary">Chronik der Ereignisse</h1>
          <p className="text-sm opacity-60 italic mt-2">Dienstpläne und Schlachtenbereitschaft für das Reich.</p>
        </div>
        
        <button 
          onClick={openCreateModal}
          className="bg-primary hover:bg-yellow-600 text-white font-bold py-3 px-8 rounded shadow-lg transition-all active:scale-95 flex items-center justify-center gap-2 text-sm uppercase tracking-widest"
        >
          <span className="material-icons-outlined">event_note</span>
          Termin verkünden
        </button>
      </div>

      <div className="space-y-6">
        {events.map((event) => {
          const choresList = event.chores ? Object.entries(event.chores) : [];
          const hasChores = choresList.length > 0;
          const isChoresVisible = visibleChores[event.id];

          return (
            <div 
              key={event.id}
              className="bg-surface-light dark:bg-surface-dark border border-border-light dark:border-border-dark rounded-xl overflow-hidden shadow-soft hover:shadow-glow transition-all flex flex-col relative group"
            >
              {/* Context Actions (Edit/Delete) */}
              <div className="absolute top-4 right-4 flex gap-2 opacity-0 group-hover:opacity-100 transition-opacity z-10">
                <button 
                  onClick={() => openEditModal(event)}
                  className="p-2 bg-white/80 dark:bg-black/80 rounded-full text-primary hover:text-secondary shadow-md transition-all hover:scale-110"
                  title="Termin bearbeiten"
                >
                  <span className="material-icons-outlined text-sm">edit</span>
                </button>
                <button 
                  onClick={() => handleDeleteEvent(event.id)}
                  className="p-2 bg-white/80 dark:bg-black/80 rounded-full text-red-600 hover:text-red-800 shadow-md transition-all hover:scale-110"
                  title="Termin löschen"
                >
                  <span className="material-icons-outlined text-sm">delete</span>
                </button>
              </div>

              <div className="flex flex-col md:flex-row">
                {/* Date Badge */}
                <div className="bg-primary/10 dark:bg-primary/5 w-full md:w-32 flex flex-col items-center justify-center p-6 border-b md:border-b-0 md:border-r border-border-light dark:border-border-dark shrink-0">
                  <span className="text-xs font-bold uppercase tracking-widest opacity-60 mb-1">{event.month}</span>
                  <span className="font-display text-5xl text-primary">{event.day}</span>
                  <span className="text-[10px] font-bold opacity-40 mt-2 tracking-tighter">ANNO 1618</span>
                </div>

                {/* Event Content */}
                <div className="p-6 flex-grow">
                  <div className="flex flex-wrap items-center justify-between mb-3 gap-2">
                    <div className="flex flex-wrap items-center gap-3">
                      <span className={`px-2 py-0.5 rounded border text-[10px] font-bold uppercase tracking-widest ${getTypeStyle(event.type)}`}>
                        {event.type}
                      </span>
                      <div className="flex items-center gap-1 text-xs opacity-60">
                        <span className="material-icons-outlined text-sm">schedule</span>
                        {event.startTime} - {event.endTime} Uhr
                      </div>
                    </div>
                    {event.type === 'Spieltag' && (
                      <span className={`flex items-center gap-1 text-[10px] font-bold uppercase tracking-widest ${hasChores ? 'text-primary' : 'opacity-30'}`}>
                        <span className="material-icons-outlined text-sm">assignment</span>
                        {hasChores ? 'Dienstplan aktiv' : 'Keine Chores'}
                      </span>
                    )}
                  </div>

                  <h2 className="font-display text-2xl mb-2 pr-16">{event.title}</h2>
                  <p className="text-sm opacity-80 leading-relaxed mb-6">{event.description}</p>

                  <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 pt-4 border-t border-border-light/30 dark:border-border-dark/30">
                    <div className="flex gap-4 items-center">
                      <div className="flex items-center gap-2 text-xs opacity-50 font-bold uppercase">
                        <span className="material-icons-outlined text-sm">group</span>
                        {event.participants + (rsvps[event.id] === 'yes' ? 1 : 0)} Teilnehmer
                      </div>
                      {event.type === 'Spieltag' && hasChores && (
                        <button 
                          onClick={() => toggleChoresVisibility(event.id)}
                          className="text-[10px] font-bold uppercase tracking-widest text-primary hover:text-secondary underline underline-offset-4 flex items-center gap-1"
                        >
                          <span className="material-icons-outlined text-sm">{isChoresVisible ? 'visibility_off' : 'visibility'}</span>
                          {isChoresVisible ? 'Dienstplan ausblenden' : 'Dienstplan einsehen'}
                        </button>
                      )}
                    </div>

                    <div className="flex gap-2">
                      <button 
                        onClick={() => handleRSVP(event.id, 'yes')}
                        className={`flex-1 sm:flex-none px-4 py-1.5 rounded text-[10px] font-bold uppercase tracking-widest border transition-all ${
                          rsvps[event.id] === 'yes' 
                            ? 'bg-green-600 border-green-600 text-white shadow-md' 
                            : 'border-green-600/50 text-green-700 hover:bg-green-600 hover:text-white dark:text-green-400'
                        }`}
                      >
                        Teilnehmen
                      </button>
                      <button 
                        onClick={() => handleRSVP(event.id, 'maybe')}
                        className={`flex-1 sm:flex-none px-4 py-1.5 rounded text-[10px] font-bold uppercase tracking-widest border transition-all ${
                          rsvps[event.id] === 'maybe' 
                            ? 'bg-primary border-primary text-white shadow-md' 
                            : 'border-primary/50 text-primary hover:bg-primary hover:text-white'
                        }`}
                      >
                        Vielleicht
                      </button>
                      <button 
                        onClick={() => handleRSVP(event.id, 'no')}
                        className={`flex-1 sm:flex-none px-4 py-1.5 rounded text-[10px] font-bold uppercase tracking-widest border transition-all ${
                          rsvps[event.id] === 'no' 
                            ? 'bg-red-800 border-red-800 text-white shadow-md' 
                            : 'border-red-800/50 text-red-800 hover:bg-red-800 hover:text-white dark:text-red-500'
                        }`}
                      >
                        Ablehnen
                      </button>
                    </div>
                  </div>
                </div>
              </div>

              {/* Chore Matrix Display */}
              {isChoresVisible && event.chores && (
                <div className="p-6 bg-background-light/40 dark:bg-background-dark/40 border-t border-border-light dark:border-border-dark animate-in slide-in-from-top-4 duration-300">
                  <div className="flex items-center justify-between mb-4">
                    <h3 className="font-display text-xl text-primary flex items-center gap-2">
                      <span className="material-icons-outlined">grid_view</span>
                      Dienstplan - Chore Matrix
                    </h3>
                    <span className="text-[10px] font-bold text-primary italic uppercase tracking-widest opacity-60">
                      Hervorgehobene Dienste = Prio-Aufgaben
                    </span>
                  </div>
                  <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
                    {CHORE_NAMES.map(chore => {
                      const isSpecial = SPECIAL_CHORES.includes(chore);
                      return (
                        <div 
                          key={chore} 
                          className={`flex items-center justify-between p-3 rounded border transition-all ${
                            isSpecial 
                              ? 'bg-primary/10 border-primary/50 shadow-glow relative overflow-hidden' 
                              : 'bg-surface-light dark:bg-surface-dark border-border-light/40 shadow-sm'
                          }`}
                        >
                          <div className="flex flex-col">
                            <span className={`text-xs font-bold ${isSpecial ? 'text-primary' : 'opacity-70'} flex items-center gap-1`}>
                              {isSpecial && <span className="material-icons-outlined text-xs">priority_high</span>}
                              {chore}
                            </span>
                            <span className="text-[8px] opacity-40 uppercase tracking-[0.2em] mt-0.5">Aufgabe</span>
                          </div>
                          <span className={`text-[10px] font-bold uppercase tracking-widest px-2.5 py-1 rounded shadow-sm ${
                            event.chores?.[chore] 
                              ? (isSpecial ? 'bg-primary text-white' : 'bg-primary/20 text-primary') 
                              : 'bg-gray-200 text-gray-400 opacity-50'
                          }`}>
                            {event.chores?.[chore] || 'Offen'}
                          </span>
                        </div>
                      );
                    })}
                  </div>
                </div>
              )}
            </div>
          );
        })}
      </div>

      {/* Modal for Creating/Editing Events */}
      {isModalOpen && (
        <div className="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm animate-in fade-in duration-200">
          <div className="bg-surface-light dark:bg-surface-dark border-2 border-primary rounded-lg shadow-glow w-full max-w-2xl overflow-hidden relative animate-in zoom-in-95 duration-200 max-h-[90vh] flex flex-col">
            <div className="absolute top-0 left-0 w-full h-1 bg-primary"></div>
            
            <form onSubmit={handleSaveEvent} className="p-6 space-y-4 flex flex-col overflow-hidden">
              <div className="flex justify-between items-center mb-2 shrink-0">
                <h3 className="font-display text-xl text-primary font-bold">
                  {editingId ? 'Termin bearbeiten' : 'Neues Ereignis verkünden'}
                </h3>
                <button 
                  type="button" 
                  onClick={() => setIsModalOpen(false)}
                  className="text-gray-400 hover:text-primary transition-colors"
                >
                  <span className="material-icons-outlined">close</span>
                </button>
              </div>

              <div className="space-y-4 overflow-y-auto pr-2 custom-scrollbar flex-grow">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-4">
                    <div className="space-y-1">
                      <label className="text-[10px] font-bold uppercase tracking-widest opacity-60">Titel</label>
                      <input 
                        type="text" required
                        value={formData.title}
                        onChange={(e) => setFormData({...formData, title: e.target.value})}
                        className="w-full bg-background-light dark:bg-background-dark border-border-light rounded text-sm focus:ring-primary focus:border-primary"
                      />
                    </div>
                    <div className="grid grid-cols-2 gap-4">
                      <div className="space-y-1">
                        <label className="text-[10px] font-bold uppercase tracking-widest opacity-60">Datum</label>
                        <input 
                          type="date" required
                          value={formData.date}
                          onChange={(e) => setFormData({...formData, date: e.target.value})}
                          className="w-full bg-background-light dark:bg-background-dark border-border-light rounded text-sm focus:ring-primary focus:border-primary"
                        />
                      </div>
                      <div className="space-y-1">
                        <label className="text-[10px] font-bold uppercase tracking-widest opacity-60">Art</label>
                        <select 
                          value={formData.type}
                          onChange={(e) => setFormData({...formData, type: e.target.value as EventType})}
                          className="w-full bg-background-light dark:bg-background-dark border-border-light rounded text-sm focus:ring-primary focus:border-primary"
                        >
                          <option value="Spieltag">Spieltag</option>
                          <option value="Vorbereitung">Vorbereitung</option>
                          <option value="Ausflug">Ausflug</option>
                        </select>
                      </div>
                    </div>
                    <div className="grid grid-cols-2 gap-4">
                      <div className="space-y-1">
                        <label className="text-[10px] font-bold uppercase tracking-widest opacity-60">Von</label>
                        <input type="time" required value={formData.startTime} onChange={(e) => setFormData({...formData, startTime: e.target.value})} className="w-full bg-background-light dark:bg-background-dark border-border-light rounded text-sm focus:ring-primary focus:border-primary"/>
                      </div>
                      <div className="space-y-1">
                        <label className="text-[10px] font-bold uppercase tracking-widest opacity-60">Bis</label>
                        <input type="time" required value={formData.endTime} onChange={(e) => setFormData({...formData, endTime: e.target.value})} className="w-full bg-background-light dark:bg-background-dark border-border-light rounded text-sm focus:ring-primary focus:border-primary"/>
                      </div>
                    </div>
                    <div className="space-y-1">
                      <label className="text-[10px] font-bold uppercase tracking-widest opacity-60">Beschreibung</label>
                      <textarea rows={3} value={formData.description} onChange={(e) => setFormData({...formData, description: e.target.value})} className="w-full bg-background-light dark:bg-background-dark border-border-light rounded text-sm resize-none focus:ring-primary focus:border-primary" />
                    </div>
                  </div>

                  {/* Chore Matrix Setup */}
                  <div className={`space-y-4 p-4 border rounded-lg bg-primary/5 border-primary/20 transition-all ${formData.type === 'Spieltag' ? 'opacity-100 scale-100' : 'opacity-30 grayscale pointer-events-none scale-95'}`}>
                    <h4 className="font-display text-lg text-primary flex items-center gap-2">
                      <span className="material-icons-outlined">checklist</span>
                      Chore Matrix
                    </h4>
                    <p className="text-[10px] opacity-60 italic mb-2">Teile die Chores für den Spieltag ein.</p>
                    <div className="space-y-2 max-h-[300px] overflow-y-auto pr-1 custom-scrollbar">
                      {CHORE_NAMES.map(chore => (
                        <div key={chore} className="flex flex-col gap-1">
                          <label className={`text-[9px] font-bold uppercase flex items-center gap-1 ${SPECIAL_CHORES.includes(chore) ? 'text-primary' : 'opacity-50'}`}>
                            {SPECIAL_CHORES.includes(chore) && <span className="material-icons-outlined text-[10px]">star</span>}
                            {chore}
                          </label>
                          <select 
                            value={formData.chores[chore] || ''}
                            onChange={(e) => handleChoreChange(chore, e.target.value)}
                            className={`w-full bg-surface-light dark:bg-surface-dark border rounded text-[11px] py-1 px-2 focus:ring-primary focus:border-primary ${SPECIAL_CHORES.includes(chore) ? 'border-primary/50 ring-1 ring-primary/20' : 'border-border-light'}`}
                          >
                            <option value="">Nicht zugewiesen</option>
                            {PEOPLE.map(person => <option key={person} value={person}>{person}</option>)}
                          </select>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>
              </div>

              <div className="flex gap-3 pt-4 shrink-0">
                <button 
                  type="button"
                  onClick={() => setIsModalOpen(false)}
                  className="flex-1 px-4 py-2 border border-border-light dark:border-border-dark text-[10px] font-bold uppercase tracking-widest rounded hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
                >
                  Abbrechen
                </button>
                <button 
                  type="submit"
                  className="flex-1 px-4 py-2 bg-primary hover:bg-yellow-600 text-white text-[10px] font-bold uppercase tracking-widest rounded shadow-glow transition-all active:scale-95"
                >
                  {editingId ? 'Änderungen speichern' : 'Besiegeln'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
