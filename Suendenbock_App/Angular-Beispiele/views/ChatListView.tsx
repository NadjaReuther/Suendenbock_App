
import React, { useState, useRef } from 'react';
import { ViewType, Character, ChatConversation } from '../types';
import { ParchmentCard } from '../components/ParchmentCard';

interface ChatListViewProps {
  onNavigate: (view: ViewType) => void;
  onSelectChat: (chat: ChatConversation) => void;
  onDeleteChat: (chatId: string) => void;
  isUserOnlineVisible: boolean;
  conversations: ChatConversation[];
  currentUser: string;
  totalUnread?: number;
  processedMembers: Character[];
}

export const ChatListView: React.FC<ChatListViewProps> = ({ 
  onNavigate, 
  onSelectChat,
  onDeleteChat,
  isUserOnlineVisible, 
  conversations,
  currentUser,
  totalUnread = 0,
  processedMembers
}) => {
  const [showMembers, setShowMembers] = useState(true);
  const [swipedChatId, setSwipedChatId] = useState<string | null>(null);
  const [chatToDelete, setChatToDelete] = useState<ChatConversation | null>(null);
  
  const touchStartPos = useRef<number | null>(null);

  const handleTouchStart = (e: React.TouchEvent, chatId: string) => {
    touchStartPos.current = e.touches[0].clientX;
  };

  const handleTouchMove = (e: React.TouchEvent, chatId: string) => {
    if (touchStartPos.current === null) return;
    const currentX = e.touches[0].clientX;
    const diff = touchStartPos.current - currentX;

    // Nur swipen wenn nach links (diff > 40)
    if (diff > 40) {
      setSwipedChatId(chatId);
    } else if (diff < -40) {
      setSwipedChatId(null);
    }
  };

  const confirmDelete = () => {
    if (chatToDelete) {
      onDeleteChat(chatToDelete.id);
      setChatToDelete(null);
      setSwipedChatId(null);
    }
  };

  return (
    <div className="min-h-screen pb-24 animate-fadeIn flex flex-col bg-[#F7F3E8]">
      {/* Header */}
      <header className="bg-card-bg border-b border-parchment-border p-4 sticky top-0 z-20 shadow-sm flex items-center justify-between">
        <h1 className="font-serif font-bold text-xl text-text-dark tracking-wider uppercase">Briefwechsel</h1>
        <div className="flex items-center space-x-2">
          <button className="material-icons text-primary text-xl">search</button>
        </div>
      </header>

      <div className="flex-1 overflow-y-auto">
        {/* Horizontal Online Members List */}
        <section className="bg-[#FDFBF7] border-b border-[#E8DCC4]">
          <button 
            onClick={() => setShowMembers(!showMembers)}
            className="w-full px-4 py-3 flex items-center justify-between group active:bg-black/5"
          >
            <h3 className="font-serif text-xs font-bold text-gray-700 uppercase tracking-widest italic">Präsenz</h3>
            <span className={`material-icons text-primary transition-transform duration-300 ${showMembers ? 'rotate-180' : ''}`}>
              expand_more
            </span>
          </button>
          
          {showMembers && (
            <div className="flex items-center space-x-6 overflow-x-auto pb-6 pt-2 px-6 animate-fadeIn no-scrollbar">
              {processedMembers
                .filter(m => m.name !== currentUser && m.id !== 'm7') 
                .map((member) => (
                <div 
                  key={member.id} 
                  className="flex flex-col items-center space-y-1.5 cursor-pointer active:scale-95 transition-transform flex-shrink-0"
                  onClick={() => onNavigate('NEW_CHAT')}
                >
                  <div className="relative">
                    <div className="w-14 h-14 rounded-full border-2 border-[#E8DCC4] overflow-hidden p-0.5 bg-white shadow-sm">
                      <img src={member.imageUrl} alt={member.name} className="w-full h-full object-cover rounded-full" />
                    </div>
                    <div className={`absolute bottom-0.5 right-0.5 w-3.5 h-3.5 border-2 border-white rounded-full ${member.isOnline ? 'bg-green-500 shadow-[0_0_5px_rgba(34,197,94,0.6)]' : 'bg-gray-300'}`}></div>
                  </div>
                  <span className="text-[9px] font-serif uppercase font-bold text-text-dark tracking-tight text-center truncate w-20 text-shadow-none">
                    {member.name}
                  </span>
                </div>
              ))}
            </div>
          )}
        </section>

        {/* Conversation List */}
        <section className="p-2">
          <h3 className="px-4 py-3 font-serif text-xs font-bold text-gray-700 uppercase tracking-widest italic">Briefe</h3>
          {conversations.length > 0 ? (
            conversations.map((chat) => {
              const isPartnerInitiator = chat.participant.name === currentUser;
              let partnerName = isPartnerInitiator ? chat.initiatorName : chat.participant.name;
              const partnerData = processedMembers.find(m => m.name === partnerName);
              let partnerImg = isPartnerInitiator 
                ? (partnerData?.imageUrl || 'https://i.pravatar.cc/150?u=npc') 
                : chat.participant.imageUrl;

              if (partnerName === 'NPC' && chat.npcMask) {
                partnerName = chat.npcMask;
              }

              const isSwiped = swipedChatId === chat.id;

              return (
                <div key={chat.id} className="relative overflow-hidden mb-1 rounded-lg">
                  {/* Delete Action Background */}
                  <div className="absolute inset-0 bg-red-600 flex justify-end items-center">
                    <button 
                      onClick={() => setChatToDelete(chat)}
                      className="w-20 h-full flex flex-col items-center justify-center text-white"
                    >
                      <span className="material-icons">delete</span>
                      <span className="text-[8px] font-bold uppercase">Löschen</span>
                    </button>
                  </div>

                  {/* Swipable Content */}
                  <button 
                    onClick={() => isSwiped ? setSwipedChatId(null) : onSelectChat(chat)}
                    onTouchStart={(e) => handleTouchStart(e, chat.id)}
                    onTouchMove={(e) => handleTouchMove(e, chat.id)}
                    style={{ transform: isSwiped ? 'translateX(-80px)' : 'translateX(0px)' }}
                    className="w-full flex items-center space-x-3 p-3 bg-white border-b border-black/5 last:border-0 relative z-10 transition-transform duration-300"
                  >
                    <div className="relative flex-shrink-0">
                      <div className="w-12 h-12 rounded-full overflow-hidden border border-parchment-border shadow-sm">
                        <img src={partnerImg} alt={partnerName} className="w-full h-full object-cover" />
                      </div>
                      {partnerData?.isOnline && (
                        <div className="absolute bottom-0 right-0 w-3 h-3 bg-green-500 border-2 border-white rounded-full"></div>
                      )}
                    </div>
                    <div className="flex-1 text-left overflow-hidden">
                      <div className="flex justify-between items-center mb-0.5">
                        <h4 className="font-serif text-sm font-bold text-text-dark uppercase truncate pr-2">
                          {chat.title}
                        </h4>
                        <span className="text-[9px] text-gray-400 font-sans flex-shrink-0">{chat.time}</span>
                      </div>
                      <div className="flex items-center space-x-1">
                        <span className="text-[9px] font-bold text-primary uppercase font-serif">{partnerName}:</span>
                        <p className="text-xs text-gray-500 truncate font-sans">{chat.lastMessage}</p>
                      </div>
                    </div>
                    {chat.unread ? (
                      <div className="w-5 h-5 bg-primary rounded-full flex items-center justify-center text-[10px] text-white font-bold flex-shrink-0">
                        {chat.unread}
                      </div>
                    ) : null}
                  </button>
                </div>
              );
            })
          ) : (
            <div className="p-8 text-center">
              <span className="material-icons text-gray-300 text-4xl mb-2">history_edu</span>
              <p className="text-xs text-gray-400 font-serif uppercase">Keine aktuellen Briefwechsel</p>
            </div>
          )}
        </section>
      </div>

      {/* Security Confirmation Modal */}
      {chatToDelete && (
        <div className="fixed inset-0 z-[60] flex items-center justify-center p-6 bg-black/40 backdrop-blur-sm animate-fadeIn">
          <ParchmentCard className="w-full max-w-sm text-center">
            <span className="material-icons text-red-500 text-5xl mb-4">report_problem</span>
            <h3 className="font-serif text-lg font-bold text-text-dark uppercase tracking-widest mb-2">Brief verbrennen?</h3>
            <p className="text-xs text-gray-600 font-sans mb-8 px-4">
              Seid Ihr gewiss, dass Ihr den Briefwechsel mit <span className="font-bold text-primary uppercase italic">{chatToDelete.participant.name}</span> für immer aus Euren Aufzeichnungen tilgen wollt?
            </p>
            <div className="flex flex-col space-y-3">
              <button 
                onClick={confirmDelete}
                className="w-full py-3 bg-red-600 text-white rounded font-serif font-bold uppercase tracking-widest text-xs shadow-md active:scale-95 transition-transform"
              >
                Wirklich verbrennen
              </button>
              <button 
                onClick={() => setChatToDelete(null)}
                className="w-full py-3 border-2 border-parchment-border text-gray-500 rounded font-serif font-bold uppercase tracking-widest text-xs active:scale-95 transition-transform"
              >
                Abbrechen
              </button>
            </div>
          </ParchmentCard>
        </div>
      )}

      <nav className="fixed bottom-0 left-0 right-0 bg-[#FDFBF7] border-t border-[#E8DCC4] flex justify-around items-center py-3 shadow-lg z-30">
        <button onClick={() => onNavigate('DASHBOARD')} className="flex flex-col items-center text-gray-400">
          <span className="material-icons">home</span>
          <span className="text-[10px] font-serif uppercase">Home</span>
        </button>
        <button onClick={() => onNavigate('NEWS')} className="flex flex-col items-center text-gray-400">
          <span className="material-icons">newspaper</span>
          <span className="text-[10px] font-serif uppercase">News</span>
        </button>
        <div className="relative -top-6">
          <button 
            onClick={() => onNavigate('NEW_CHAT')}
            className="bg-primary text-white w-14 h-14 rounded-full shadow-xl flex items-center justify-center border-4 border-[#F7F3E8] active:scale-90 transition-transform"
          >
            <span className="material-icons text-2xl">add</span>
          </button>
        </div>
        <button onClick={() => onNavigate('CHAT_LIST')} className="flex flex-col items-center text-primary relative">
          <span className="material-icons">chat</span>
          <span className="text-[10px] font-serif uppercase">Chat</span>
          {totalUnread > 0 && (
            <div className="absolute -top-1 -right-1 w-4 h-4 bg-primary text-white text-[9px] font-bold rounded-full flex items-center justify-center border border-white">
              {totalUnread}
            </div>
          )}
        </button>
        <button onClick={() => onNavigate('PROFILE')} className="flex flex-col items-center text-gray-400">
          <span className="material-icons">person</span>
          <span className="text-[10px] font-serif uppercase">Profil</span>
        </button>
      </nav>
    </div>
  );
};
