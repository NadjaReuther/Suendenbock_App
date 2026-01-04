
import React, { useState, useRef, useEffect } from 'react';
import { MapMarker, MarkerType, Quest } from '../types';

interface MapViewProps {
  onBack: () => void;
  quests: Quest[];
  onNavigateToQuest?: (questId: string) => void;
}

const MARKER_CONFIG: Record<MarkerType, { icon: string, color: string, label: string }> = {
  quest: { icon: 'priority_high', color: 'bg-amber-500 text-black', label: 'Quest' },
  info: { icon: 'info', color: 'bg-blue-500 text-white', label: 'Info' },
  danger: { icon: 'skull', color: 'bg-red-600 text-white', label: 'Gefahr' },
  settlement: { icon: 'fort', color: 'bg-stone-700 text-white', label: 'Ort' },
};

const MapView: React.FC<MapViewProps> = ({ onBack, quests, onNavigateToQuest }) => {
  const [imageUrl, setImageUrl] = useState<string | null>(localStorage.getItem('map_image'));
  const [markers, setMarkers] = useState<MapMarker[]>(JSON.parse(localStorage.getItem('map_markers') || '[]'));
  const [activeMarkerType, setActiveMarkerType] = useState<MarkerType>('quest');
  const [pendingMarker, setPendingMarker] = useState<{ x: number, y: number } | null>(null);
  const [viewingMarkerId, setViewingMarkerId] = useState<string | null>(null);
  const [newLabel, setNewLabel] = useState('');
  const [newDescription, setNewDescription] = useState('');
  const [selectedQuestId, setSelectedQuestId] = useState('');
  const fileInputRef = useRef<HTMLInputElement>(null);
  const mapContainerRef = useRef<HTMLDivElement>(null);

  const activeQuests = quests.filter(q => q.status === 'active');

  useEffect(() => {
    if (imageUrl) localStorage.setItem('map_image', imageUrl);
    localStorage.setItem('map_markers', JSON.stringify(markers));
  }, [imageUrl, markers]);

  const handleFileUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (event) => {
        const result = event.target?.result as string;
        setImageUrl(result);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleMapClick = (e: React.MouseEvent) => {
    if (!mapContainerRef.current || !imageUrl) return;
    if ((e.target as HTMLElement).closest('.marker-item') || (e.target as HTMLElement).closest('.marker-popup')) return;

    const rect = mapContainerRef.current.getBoundingClientRect();
    const x = ((e.clientX - rect.left) / rect.width) * 100;
    const y = ((e.clientY - rect.top) / rect.height) * 100;
    setPendingMarker({ x, y });
    setViewingMarkerId(null);
    setNewLabel('');
    setNewDescription('');
    setSelectedQuestId('');
  };

  const addMarker = (e: React.FormEvent) => {
    e.preventDefault();
    if (!pendingMarker) return;

    let label = newLabel.trim();
    let description = newDescription.trim();
    let questId: string | undefined = undefined;

    if (activeMarkerType === 'quest') {
      const quest = activeQuests.find(q => q.id === selectedQuestId);
      if (!quest) return;
      label = quest.title;
      questId = quest.id;
    }

    if (label || activeMarkerType === 'quest') {
      const marker: MapMarker = {
        id: Date.now().toString(),
        x: pendingMarker.x,
        y: pendingMarker.y,
        label: label,
        type: activeMarkerType,
        questId: questId,
        description: activeMarkerType === 'info' ? description : undefined
      };
      setMarkers([...markers, marker]);
      setPendingMarker(null);
      setNewLabel('');
      setNewDescription('');
      setSelectedQuestId('');
    }
  };

  const removeMarker = (id: string) => {
    setMarkers(markers.filter(m => m.id !== id));
    setViewingMarkerId(null);
  };

  const viewingMarker = markers.find(m => m.id === viewingMarkerId);
  const linkedQuest = viewingMarker?.questId ? quests.find(q => q.id === viewingMarker.questId) : null;

  return (
    <div className="min-h-screen bg-parchment-light bg-paper-texture text-ink p-4 md:p-8 animate-in fade-in duration-500 flex flex-col items-center overflow-x-hidden">
      <header className="relative z-10 w-full max-w-6xl mb-12 flex flex-col md:flex-row items-center justify-between border-b-2 border-primary/20 pb-8">
        <div className="text-center md:text-left mb-4 md:mb-0">
          <h1 className="text-4xl md:text-6xl font-display text-ink drop-shadow-sm">Weltkarte</h1>
          <p className="font-medieval text-primary/60 text-xs italic tracking-widest uppercase mt-1">Die Pfade des Schicksals</p>
        </div>

        <div className="flex gap-4">
          <button 
            onClick={() => fileInputRef.current?.click()}
            className="flex items-center space-x-2 bg-parchment-card border border-parchment-border px-6 py-3 rounded shadow-sm text-[10px] font-display uppercase tracking-widest transition-all hover:bg-white hover:border-secondary text-ink"
          >
            <span className="material-symbols-outlined text-sm">upload_file</span>
            <span>Karte ändern</span>
          </button>
          <input ref={fileInputRef} type="file" accept="image/png, image/jpeg" className="hidden" onChange={handleFileUpload} />
        </div>
      </header>

      <main className="relative z-10 w-full max-w-6xl flex flex-col lg:grid lg:grid-cols-4 gap-12 items-start">
        <aside className="w-full space-y-6">
          <div className="bg-parchment-card border border-parchment-border p-6 rounded shadow-md">
            <h3 className="font-medieval text-primary text-xl mb-4 border-b border-primary/10 pb-2">Kartentasche</h3>
            <p className="text-[10px] uppercase font-bold text-ink/40 mb-4 tracking-widest">Marker Typ wählen</p>
            <div className="grid grid-cols-2 lg:grid-cols-1 gap-3">
              {(Object.entries(MARKER_CONFIG) as [MarkerType, typeof MARKER_CONFIG['quest']][]).map(([type, cfg]) => (
                <button
                  key={type}
                  onClick={() => { setActiveMarkerType(type); setPendingMarker(null); setViewingMarkerId(null); }}
                  className={`flex items-center space-x-3 p-3 rounded border transition-all ${
                    activeMarkerType === type 
                      ? 'bg-secondary/20 border-secondary text-ink shadow-sm ring-1 ring-secondary/30' 
                      : 'bg-white/50 border-parchment-border text-ink/50 hover:border-primary'
                  }`}
                >
                  <span className={`material-symbols-outlined text-lg ${activeMarkerType === type ? 'animate-pulse text-primary' : ''}`}>{cfg.icon}</span>
                  <span className="text-[10px] font-display uppercase tracking-wider font-bold">{cfg.label}</span>
                </button>
              ))}
            </div>
          </div>
        </aside>

        <div className="lg:col-span-3 flex flex-col items-center w-full">
          {imageUrl ? (
            <div className="relative p-3 bg-ink rounded shadow-[0_25px_60px_rgba(0,0,0,0.3)] border-[10px] border-ink">
              <div className="absolute -top-1 -left-1 w-6 h-6 border-t-4 border-l-4 border-secondary/60 rounded-tl-lg z-20"></div>
              <div className="absolute -top-1 -right-1 w-6 h-6 border-t-4 border-r-4 border-secondary/60 rounded-tr-lg z-20"></div>
              <div className="absolute -bottom-1 -left-1 w-6 h-6 border-b-4 border-l-4 border-secondary/60 rounded-bl-lg z-20"></div>
              <div className="absolute -bottom-1 -right-1 w-6 h-6 border-b-4 border-r-4 border-secondary/60 rounded-br-lg z-20"></div>

              <div 
                ref={mapContainerRef}
                className="relative cursor-crosshair rounded-sm"
                onClick={handleMapClick}
              >
                <img 
                  src={imageUrl} 
                  alt="Weltkarte" 
                  className="max-w-full h-auto block rounded-sm shadow-inner"
                />
                
                {markers.map(m => (
                  <div
                    key={m.id}
                    onClick={(e) => { e.stopPropagation(); setViewingMarkerId(m.id); setPendingMarker(null); }}
                    style={{ left: `${m.x}%`, top: `${m.y}%` }}
                    className="absolute -translate-x-1/2 -translate-y-1/2 marker-item z-30 group/marker"
                  >
                    <div className={`w-8 h-8 rounded-full flex items-center justify-center shadow-xl border-2 border-black/40 transition-all hover:scale-125 cursor-pointer ${MARKER_CONFIG[m.type].color} ${viewingMarkerId === m.id ? 'ring-4 ring-white/50 scale-110 shadow-[0_0_20px_rgba(255,255,255,0.4)]' : ''}`}>
                      <span className="material-symbols-outlined text-[18px]">{MARKER_CONFIG[m.type].icon}</span>
                    </div>
                  </div>
                ))}

                {viewingMarker && (
                  <div 
                    style={{ left: `${viewingMarker.x}%`, top: `${viewingMarker.y}%` }}
                    className="absolute -translate-x-1/2 -translate-y-[115%] z-[100] marker-popup"
                    onClick={(e) => e.stopPropagation()}
                  >
                    <div className="bg-parchment-card p-5 rounded shadow-[0_20px_50px_rgba(0,0,0,0.5)] border-2 border-secondary w-72 animate-in slide-in-from-bottom-2 zoom-in-95">
                      <div className={`flex justify-between items-start gap-2 ${viewingMarker.type === 'settlement' || viewingMarker.type === 'danger' ? '' : 'mb-4 border-b border-primary/10 pb-2'}`}>
                        <div className="flex items-start gap-3 flex-1 min-w-0">
                          <span className={`flex-shrink-0 material-symbols-outlined text-sm ${MARKER_CONFIG[viewingMarker.type].color} rounded-full p-1.5 shadow-sm mt-0.5`}>{MARKER_CONFIG[viewingMarker.type].icon}</span>
                          <h4 className="font-medieval text-ink text-sm font-bold uppercase whitespace-normal break-words leading-tight">
                            {viewingMarker.label}
                          </h4>
                        </div>
                        <div className="flex gap-1.5 flex-shrink-0 ml-2">
                          <button 
                            onClick={() => removeMarker(viewingMarker.id)} 
                            title="Marker löschen"
                            className="w-7 h-7 flex items-center justify-center bg-crimson/10 hover:bg-crimson text-crimson hover:text-white rounded shadow-sm transition-all"
                          >
                            <span className="material-symbols-outlined text-[16px]">delete</span>
                          </button>
                          <button 
                            onClick={() => setViewingMarkerId(null)} 
                            className="w-7 h-7 flex items-center justify-center bg-ink/5 hover:bg-ink hover:text-white text-ink rounded shadow-sm transition-all"
                          >
                            <span className="material-symbols-outlined text-[16px]">close</span>
                          </button>
                        </div>
                      </div>

                      {(viewingMarker.type === 'info' || viewingMarker.type === 'quest') && (
                        <div className="text-ink/80 font-body">
                          {linkedQuest ? (
                            <div className="space-y-4">
                              <div>
                                <p className="text-[10px] uppercase font-bold text-primary tracking-widest border-l-2 border-primary pl-2 mb-2">Zugeordnete Quest</p>
                                <p className="text-xs italic leading-snug bg-white/40 p-2 rounded border border-primary/5">"{linkedQuest.description}"</p>
                              </div>
                              <div className="flex flex-wrap gap-1.5 mt-2">
                                {linkedQuest.assignedTo.map(c => (
                                  <span key={c} className="text-[9px] bg-primary text-white px-2 py-0.5 rounded shadow-sm font-bold uppercase tracking-tight">{c}</span>
                                ))}
                              </div>
                              {onNavigateToQuest && (
                                <button 
                                  onClick={() => onNavigateToQuest(linkedQuest.id)}
                                  className="w-full mt-2 flex items-center justify-center space-x-2 py-2 bg-secondary text-black text-[9px] font-bold uppercase rounded shadow-md hover:saturate-150 transition-all border border-black/5"
                                >
                                  <span className="material-symbols-outlined text-xs">auto_stories</span>
                                  <span>Zur Chronik springen</span>
                                </button>
                              )}
                            </div>
                          ) : viewingMarker.description ? (
                            <div className="space-y-2">
                              <p className="text-[10px] uppercase font-bold text-blue-800 tracking-widest border-l-2 border-blue-800 pl-2">Information</p>
                              <p className="text-xs italic leading-relaxed bg-white/40 p-2 rounded border border-blue-800/5">{viewingMarker.description}</p>
                            </div>
                          ) : (
                            <p className="text-xs italic leading-relaxed text-ink/60">Keine weiteren Details vermerkt.</p>
                          )}
                        </div>
                      )}
                    </div>
                  </div>
                )}

                {pendingMarker && (
                  <div 
                    style={{ left: `${pendingMarker.x}%`, top: `${pendingMarker.y}%` }}
                    className="absolute -translate-x-1/2 -translate-y-1/2 z-[110] marker-popup"
                    onClick={(e) => e.stopPropagation()}
                  >
                    <div className="bg-parchment-card p-5 rounded shadow-[0_20px_50px_rgba(0,0,0,0.6)] border-2 border-secondary w-64 animate-in zoom-in-95">
                      <h4 className="font-medieval text-ink text-xs mb-4 font-bold uppercase tracking-widest text-center border-b border-primary/10 pb-2">
                        {activeMarkerType === 'quest' ? 'Quest zuweisen' : 'Punkt benennen'}
                      </h4>
                      <form onSubmit={addMarker}>
                        {activeMarkerType === 'quest' ? (
                          <div className="mb-5">
                            {activeQuests.length > 0 ? (
                              <select
                                autoFocus
                                required
                                value={selectedQuestId}
                                onChange={(e) => setSelectedQuestId(e.target.value)}
                                className="w-full bg-white border border-parchment-border rounded px-3 py-2 text-xs text-ink outline-none focus:border-secondary font-medieval shadow-inner"
                              >
                                <option value="" disabled>Quest wählen...</option>
                                {activeQuests.map(q => (
                                  <option key={q.id} value={q.id}>{q.title}</option>
                                ))}
                              </select>
                            ) : (
                              <p className="text-[10px] text-crimson italic font-bold text-center py-2 bg-crimson/5 rounded">Keine aktiven Quests verfügbar!</p>
                            )}
                          </div>
                        ) : (
                          <div className="space-y-4 mb-4">
                            <input 
                              autoFocus
                              required
                              type="text" 
                              value={newLabel}
                              onChange={(e) => setNewLabel(e.target.value)}
                              className="w-full bg-white border border-parchment-border rounded px-3 py-2 text-xs text-ink outline-none focus:border-secondary font-medieval shadow-inner"
                              placeholder={activeMarkerType === 'settlement' ? 'Dorfname' : activeMarkerType === 'danger' ? 'Gefahrenquelle' : 'Kurztitel'}
                            />
                            {activeMarkerType === 'info' && (
                              <textarea
                                required
                                rows={3}
                                value={newDescription}
                                onChange={(e) => setNewDescription(e.target.value)}
                                className="w-full bg-white border border-parchment-border rounded px-3 py-2 text-xs text-ink outline-none focus:border-secondary font-body shadow-inner"
                                placeholder="Detaillierte Informationen eingeben..."
                              />
                            )}
                          </div>
                        )}
                        <div className="flex gap-2">
                          <button 
                            type="submit" 
                            disabled={activeMarkerType === 'quest' && !selectedQuestId}
                            className="flex-grow py-2.5 bg-secondary text-black text-[10px] font-bold uppercase rounded hover:saturate-150 disabled:opacity-30 transition-all shadow-md active:scale-95"
                          >
                            Punkt setzen
                          </button>
                          <button 
                            type="button" 
                            onClick={() => setPendingMarker(null)} 
                            className="px-4 py-2.5 bg-ink/5 text-ink text-[10px] rounded hover:bg-ink/10 font-bold uppercase transition-all shadow-sm"
                          >
                            X
                          </button>
                        </div>
                      </form>
                    </div>
                  </div>
                )}
              </div>
            </div>
          ) : (
            <div className="w-full aspect-video flex flex-col items-center justify-center border-4 border-dashed border-primary/20 rounded-xl bg-parchment-card/60 group hover:bg-parchment-card transition-all cursor-pointer shadow-inner" onClick={() => fileInputRef.current?.click()}>
              <span className="material-symbols-outlined text-8xl text-primary opacity-20 group-hover:opacity-40 mb-4 transition-all">map</span>
              <p className="font-medieval text-2xl text-primary/40 uppercase tracking-widest">Keine Karte geladen</p>
              <p className="text-[10px] text-ink/30 uppercase tracking-[0.4em] mt-2">PNG oder JPEG Format empfohlen</p>
            </div>
          )}

          <div className="mt-20 flex flex-col items-center">
            <button 
              onClick={onBack} 
              className="group flex flex-col items-center space-y-4 transition-all duration-500 hover:scale-110 active:scale-95 px-12 py-6 rounded-lg bg-parchment-card border border-parchment-border shadow-md hover:shadow-xl"
            >
              <span className="material-symbols-outlined text-primary text-5xl group-hover:-translate-y-1 transition-transform">fireplace</span>
              <div className="text-center">
                <span className="block font-display uppercase tracking-[0.5em] text-sm text-ink group-hover:text-secondary transition-colors">Zurück zum Lager</span>
                <span className="block text-[10px] font-medieval text-ink/40 italic mt-1">Die Schatten werden länger</span>
              </div>
            </button>
          </div>
        </div>
      </main>

      <footer className="mt-32 text-center text-ink/10 pb-12 w-full max-w-4xl border-t border-primary/10 pt-10">
          <p className="text-[10px] font-display tracking-[0.6em] uppercase">Sündenbock 1618 - Kartografie der Vergessenen</p>
      </footer>
    </div>
  );
};

export default MapView;
