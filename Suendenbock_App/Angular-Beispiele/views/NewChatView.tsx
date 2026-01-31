
import React, { useState } from 'react';
import { ViewType, Character } from '../types';

interface NewChatViewProps {
  onNavigate: (view: ViewType) => void;
  currentUser: string;
  onStartChat: (member: Character, title: string, npcIdentity?: string) => void;
  processedMembers: Character[];
}

const NPC_IDENTITIES = ['Okko', 'Emma', 'Kunigunde', 'Albert', 'Valentin', 'Yannis', 'Thom'];

export const NewChatView: React.FC<NewChatViewProps> = ({ onNavigate, currentUser, onStartChat, processedMembers }) => {
  const [title, setTitle] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedIdentity, setSelectedIdentity] = useState<string | null>(null);

  const isNPC = currentUser === 'NPC';
  const isTitleValid = title.trim().length > 0;
  const canChooseRecipient = isNPC ? (isTitleValid && selectedIdentity) : isTitleValid;

  return (
    <div className="min-h-screen pb-24 animate-fadeIn flex flex-col bg-[#F7F3E8]">
      {/* Header */}
      <header className="bg-card-bg border-b border-parchment-border p-4 sticky top-0 z-20 shadow-sm flex items-center justify-between">
        <button onClick={() => onNavigate('CHAT_LIST')} className="material-icons text-primary">close</button>
        <h1 className="font-serif font-bold text-xl text-text-dark tracking-wider uppercase">
          {isNPC ? 'Brief des Meisters' : 'Brief verfassen'}
        </h1>
        <div className="w-6"></div>
      </header>

      <div className="flex-1 overflow-y-auto p-4 space-y-6">
        {/* Step 1: Betreff */}
        <section className="space-y-2">
          <label className="font-serif text-[10px] font-bold text-gray-500 uppercase tracking-[0.2em] px-1">Betreff des Schreibens</label>
          <div className="relative">
            <span className="material-icons absolute left-3 top-3 text-primary text-sm">history_edu</span>
            <input 
              type="text" 
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder="z.B. Geheimtreffen, Kräuterhandel..." 
              className="w-full pl-10 pr-4 py-3 bg-white border border-parchment-border rounded-lg text-sm font-sans text-black focus:ring-1 focus:ring-primary outline-none shadow-inner"
            />
          </div>
        </section>

        {/* Step 2: NPC Identity Selection */}
        {isNPC && isTitleValid && (
          <section className="space-y-3 animate-fadeIn">
            <label className="font-serif text-[10px] font-bold text-primary uppercase tracking-[0.2em] px-1">Als wen möchtet Ihr schreiben?</label>
            <div className="flex flex-wrap gap-2">
              {NPC_IDENTITIES.map(name => (
                <button
                  key={name}
                  onClick={() => setSelectedIdentity(name)}
                  className={`px-4 py-2 rounded-full border text-[10px] font-serif font-bold uppercase transition-all shadow-sm ${
                    selectedIdentity === name 
                    ? 'bg-primary border-primary text-white scale-105 shadow-md' 
                    : 'bg-white border-parchment-border text-gray-500'
                  }`}
                >
                  {name}
                </button>
              ))}
            </div>
          </section>
        )}

        {/* Step 3: Empfängersuche */}
        <section className={`space-y-4 transition-opacity duration-300 ${canChooseRecipient ? 'opacity-100' : 'opacity-40 pointer-events-none'}`}>
          <div className="h-px bg-parchment-border/30 w-full"></div>
          
          <div className="relative">
            <span className="material-icons absolute left-3 top-2.5 text-gray-400 text-sm">search</span>
            <input 
              type="text" 
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="Empfänger suchen..." 
              className="w-full pl-10 pr-4 py-2 bg-white/50 border border-parchment-border/50 rounded-full text-xs font-sans text-black focus:ring-1 focus:ring-primary outline-none"
            />
          </div>

          <h3 className="font-serif text-[10px] font-bold text-gray-500 uppercase tracking-[0.2em] border-b border-parchment-border pb-1">
            Gildenmitglieder wählen
          </h3>

          <div className="space-y-1">
            {processedMembers.filter(m => {
              const matchesSearch = m.name.toLowerCase().includes(searchTerm.toLowerCase());
              const isNotSelf = m.name !== currentUser;
              const isNotNPCIfUser = !isNPC ? m.id !== 'm7' : true;
              return matchesSearch && isNotSelf && isNotNPCIfUser;
            }).map((member) => (
              <button 
                key={member.id}
                onClick={() => onStartChat(member, title, selectedIdentity || undefined)}
                className="w-full flex items-center space-x-4 p-3 active:bg-black/5 transition-colors rounded-lg border border-transparent active:border-parchment-border/50"
              >
                <div className="relative">
                  <div className="w-12 h-12 rounded-full overflow-hidden border border-parchment-border shadow-sm">
                    <img src={member.imageUrl} alt={member.name} className="w-full h-full object-cover" />
                  </div>
                  <div className={`absolute bottom-0 right-0 w-3 h-3 rounded-full border-2 border-[#F7F3E8] ${member.isOnline ? 'bg-green-500' : 'bg-gray-300'}`}></div>
                </div>
                <div className="flex-1 text-left">
                  <h4 className="font-serif text-sm font-bold text-text-dark uppercase">{member.name}</h4>
                  <p className="text-[10px] text-gray-500 font-sans italic">{member.class} • Stufe {member.level}</p>
                </div>
                <span className="material-icons text-primary text-sm">send</span>
              </button>
            ))}
          </div>
        </section>
      </div>

      <div className="fixed bottom-0 left-0 right-0 p-4 bg-gradient-to-t from-[#F7F3E8] to-transparent pointer-events-none">
        <p className="text-[9px] text-gray-400 font-serif uppercase text-center pointer-events-auto bg-[#F7F3E8]/80 py-2 rounded-full border border-parchment-border/20">
          {isNPC && selectedIdentity ? `Ihr schreibt als ${selectedIdentity.toUpperCase()}` : 'Ein versiegelter Brief ist ein Versprechen.'}
        </p>
      </div>
    </div>
  );
};
