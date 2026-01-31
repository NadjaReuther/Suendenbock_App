
import React from 'react';
import { ViewType, ChatConversation, Character } from '../types';

interface GodModeViewProps {
  onNavigate: (view: ViewType) => void;
  conversations: ChatConversation[];
  onSelectChat: (chat: ChatConversation) => void;
  processedMembers: Character[];
}

export const GodModeView: React.FC<GodModeViewProps> = ({ 
  onNavigate, 
  conversations, 
  onSelectChat, 
  processedMembers 
}) => {
  return (
    <div className="min-h-screen pb-24 animate-fadeIn flex flex-col bg-[#1a0f0f]">
      {/* Admin Header */}
      <header className="bg-[#2d0a0a] border-b border-primary/30 p-4 sticky top-0 z-20 shadow-xl flex items-center justify-between">
        <div className="flex items-center space-x-3">
          <button 
            onClick={() => onNavigate('DASHBOARD')}
            className="material-icons text-primary"
          >
            arrow_back
          </button>
          <div className="flex flex-col">
            <h1 className="font-serif font-bold text-lg text-white tracking-[0.2em] uppercase">Omniscience</h1>
            <p className="text-[8px] text-primary uppercase font-bold tracking-[0.4em]">Creator Oversight Panel</p>
          </div>
        </div>
        <span className="material-icons text-primary">security</span>
      </header>

      <div className="flex-1 overflow-y-auto px-4 py-6 space-y-6">
        <section>
          <div className="flex items-center justify-between mb-4 border-b border-white/10 pb-2">
            <h3 className="font-serif text-[10px] font-bold text-gray-400 uppercase tracking-widest italic">Alle aktiven Briefwechsel</h3>
            <span className="text-[9px] font-mono text-primary font-bold">{conversations.length} THREADS ACTIVE</span>
          </div>

          <div className="space-y-3">
            {conversations.map((chat) => {
              const initiator = chat.initiatorName;
              const recipient = chat.participant.name;
              
              const initiatorData = processedMembers.find(m => m.name === initiator);
              const recipientData = processedMembers.find(m => m.name === recipient);

              return (
                <button
                  key={chat.id}
                  onClick={() => onSelectChat(chat)}
                  className="w-full bg-[#251818] border border-white/5 rounded-xl p-4 text-left shadow-lg active:scale-[0.98] transition-all hover:border-primary/30"
                >
                  <div className="flex justify-between items-start mb-3">
                    <div className="flex items-center space-x-2">
                      <span className="material-icons text-[14px] text-primary">history_edu</span>
                      <h4 className="font-serif text-sm font-bold text-white uppercase tracking-wider truncate max-w-[150px]">
                        {chat.title}
                      </h4>
                    </div>
                    <span className="text-[8px] font-mono text-gray-500 uppercase">{chat.time}</span>
                  </div>

                  <div className="flex items-center justify-between bg-black/30 p-2 rounded-lg mb-3">
                    <div className="flex items-center space-x-2 flex-1">
                      <img src={initiatorData?.imageUrl} className="w-6 h-6 rounded-full border border-white/10 object-cover" />
                      <span className="text-[10px] font-bold text-gray-300 uppercase truncate">{initiator}</span>
                    </div>
                    <span className="material-icons text-[12px] text-gray-600 mx-2">swap_horiz</span>
                    <div className="flex items-center space-x-2 flex-1 justify-end">
                      <span className="text-[10px] font-bold text-gray-300 uppercase truncate">{recipient}</span>
                      <img src={recipientData?.imageUrl} className="w-6 h-6 rounded-full border border-white/10 object-cover" />
                    </div>
                  </div>

                  <div className="relative pl-3 border-l-2 border-primary/20">
                    <p className="text-[11px] text-gray-400 font-sans italic line-clamp-2">
                      <span className="text-primary/60 font-bold uppercase text-[9px] mr-1">Last:</span>
                      {chat.lastMessage}
                    </p>
                  </div>
                  
                  {chat.unread && chat.unread > 0 ? (
                    <div className="mt-2 flex justify-end">
                      <span className="bg-primary text-black text-[7px] font-bold px-1.5 py-0.5 rounded uppercase tracking-tighter">
                        {chat.unread} unread by participant
                      </span>
                    </div>
                  ) : null}
                </button>
              );
            })}
          </div>
        </section>
      </div>

      {/* Admin Disclaimer */}
      <div className="fixed bottom-0 left-0 right-0 p-4 bg-gradient-to-t from-black to-transparent pointer-events-none">
        <p className="text-[8px] text-primary/40 font-serif uppercase text-center py-2 bg-black/60 rounded-full border border-primary/10 tracking-[0.3em]">
          MODERATOR TOOLS • WATCHING THE SHADOWS
        </p>
      </div>
    </div>
  );
};
