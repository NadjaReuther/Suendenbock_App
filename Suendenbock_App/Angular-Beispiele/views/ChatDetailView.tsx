
import React, { useState, useEffect, useRef } from 'react';
import { ViewType, ChatConversation, Message, DiceResult, SuccessStatus, Character } from '../types';

interface ChatDetailViewProps {
  chat: ChatConversation;
  messages: Message[];
  onSendMessage: (message: Message) => void;
  onNavigate: (view: ViewType) => void;
  currentUser: string;
  processedMembers: Character[];
  isPeeking?: boolean;
}

type RollType = 'STANDARD' | 'ROUND';
type AdvantageType = 'NONE' | 'ADVANTAGE' | 'DISADVANTAGE';

const DiceIcon = ({ className }: { className?: string }) => (
  <span className={`material-icons ${className} flex items-center justify-center`}>casino</span>
);

export const ChatDetailView: React.FC<ChatDetailViewProps> = ({ 
  chat, 
  messages, 
  onSendMessage, 
  onNavigate, 
  currentUser, 
  processedMembers,
  isPeeking = false
}) => {
  const scrollRef = useRef<HTMLDivElement>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const touchStartRef = useRef<number | null>(null);

  const [inputValue, setInputValue] = useState('');
  const [showActionMenu, setShowActionMenu] = useState(false);
  const [showDiceModal, setShowDiceModal] = useState(false);
  const [showGallery, setShowGallery] = useState(false);
  const [fullscreenImageIndex, setFullscreenImageIndex] = useState<number | null>(null);
  
  const [rollType, setRollType] = useState<RollType>('STANDARD');
  const [bonusD6, setBonusD6] = useState(0); 
  const [advantage, setAdvantage] = useState<AdvantageType>('NONE');
  const [talentValue, setTalentValue] = useState<string>('');

  const galleryImages = messages.filter(m => m.imageUrl).map(m => m.imageUrl!);

  useEffect(() => {
    if (scrollRef.current) {
      scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
    }
  }, [messages]);

  const handleTalentChange = (val: string) => {
    setTalentValue(val);
    const numericVal = parseInt(val);
    if (isNaN(numericVal) || numericVal <= 50) {
      setBonusD6(0);
    }
  };

  const handleSend = (text?: string, diceData?: DiceResult, imageUrl?: string) => {
    const finalMsg = text || inputValue;
    if (!finalMsg.trim() && !diceData && !imageUrl) return;
    
    const newMessage: Message = {
      id: Date.now().toString(),
      sender: currentUser, 
      text: finalMsg || undefined,
      imageUrl: imageUrl,
      timestamp: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
      isMe: true,
      diceResult: diceData
    };
    
    onSendMessage(newMessage);
    setInputValue('');
    setShowActionMenu(false);
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (event) => {
        if (event.target?.result) {
          handleSend(undefined, undefined, event.target.result as string);
        }
      };
      reader.readAsDataURL(file);
      e.target.value = '';
    }
  };

  const performRoll = () => {
    const rollDie = (max: number) => Math.floor(Math.random() * max) + 1;
    let primaryDieMax = rollType === 'STANDARD' ? 100 : 4;
    let roll1Raw = rollDie(primaryDieMax);
    let roll2Raw = advantage !== 'NONE' ? rollDie(primaryDieMax) : roll1Raw;

    const normalize = (val: number) => (rollType === 'STANDARD' && val === 100 ? 0 : val);
    let roll1 = normalize(roll1Raw);
    let roll2 = normalize(roll2Raw);
    
    let chosenRoll = roll1;
    if (advantage === 'ADVANTAGE') chosenRoll = Math.min(roll1, roll2);
    if (advantage === 'DISADVANTAGE') chosenRoll = Math.max(roll1, roll2);

    let d6Results: number[] = [];
    if (rollType === 'STANDARD') {
      for (let i = 0; i < bonusD6; i++) d6Results.push(rollDie(6));
    }
    
    const d6Sum = d6Results.reduce((a, b) => a + b, 0);
    const finalResult = chosenRoll - d6Sum;

    let status: SuccessStatus = 'NONE';
    const tVal = talentValue ? parseInt(talentValue) : undefined;

    if (rollType === 'STANDARD' && tVal !== undefined) {
      if (finalResult <= 0) {
        status = 'PERFECT';
      } else {
        const critThreshold = Math.floor(tVal * 0.1) + (bonusD6 * 2);
        if (finalResult <= critThreshold) {
          status = 'CRITICAL';
        } else if (finalResult <= tVal) {
          status = 'SUCCESS';
        } else {
          status = 'FAILURE';
        }
      }
    }

    const diceData: DiceResult = {
      id: Date.now().toString(),
      rollType: rollType === 'STANDARD' ? '1d100' : '1d4',
      primaryRolls: advantage !== 'NONE' ? [roll1, roll2] : [roll1],
      chosenPrimary: chosenRoll,
      advantage: advantage,
      bonusD6: d6Results,
      bonusSum: d6Sum,
      finalResult: finalResult,
      talentValue: tVal,
      successStatus: status,
      timestamp: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    };

    handleSend("Würfelwurf", diceData);
    setShowDiceModal(false);
    setBonusD6(0);
    setAdvantage('NONE');
  };

  const openFullscreen = (url: string) => {
    const idx = galleryImages.indexOf(url);
    if (idx !== -1) setFullscreenImageIndex(idx);
  };

  const handleFullscreenTouchStart = (e: React.TouchEvent) => {
    touchStartRef.current = e.touches[0].clientX;
  };

  const handleFullscreenTouchEnd = (e: React.TouchEvent) => {
    if (touchStartRef.current === null || fullscreenImageIndex === null) return;
    const touchEnd = e.changedTouches[0].clientX;
    const diff = touchStartRef.current - touchEnd;
    const threshold = 50;

    if (Math.abs(diff) > threshold) {
      if (diff > 0 && fullscreenImageIndex < galleryImages.length - 1) {
        setFullscreenImageIndex(fullscreenImageIndex + 1);
      } else if (diff < 0 && fullscreenImageIndex > 0) {
        setFullscreenImageIndex(fullscreenImageIndex - 1);
      }
    }
    touchStartRef.current = null;
  };

  const renderDiceCard = (dice: DiceResult) => {
    const formulaString = `${dice.rollType}${dice.talentValue !== undefined ? ' vs. ' + dice.talentValue : ''}${dice.bonusD6.length > 0 ? ' • ' + dice.bonusD6.length + ' Meist.' : ''}`;
    let resultColorClass = "text-primary";
    let statusLabel = "";

    switch (dice.successStatus) {
      case 'PERFECT':
        resultColorClass = "text-green-500 animate-pulse font-black drop-shadow-[0_0_12px_rgba(34,197,94,0.5)]";
        statusLabel = "PERFEKT";
        break;
      case 'CRITICAL':
        resultColorClass = "text-yellow-400 font-black drop-shadow-[0_0_5px_rgba(250,204,21,0.5)]";
        statusLabel = "KRITISCH";
        break;
      case 'SUCCESS':
        resultColorClass = "text-white font-bold";
        statusLabel = "ERFOLGREICH";
        break;
      case 'FAILURE':
        resultColorClass = "text-red-500 font-bold";
        statusLabel = "MISSLUNGEN";
        break;
      default:
        resultColorClass = "text-white";
    }

    return (
      <div className="bg-[#1a1c1e] text-white p-4 rounded-xl shadow-2xl border-2 border-primary/40 min-w-[220px] flex flex-col space-y-2 transition-all animate-slideUp">
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-3">
            <DiceIcon className="text-4xl text-white/80" />
            <div className="flex flex-col">
              <div className="flex items-baseline space-x-1">
                <span className="text-xl font-black tracking-tight text-white/95">{dice.chosenPrimary}</span>
                {dice.bonusSum > 0 && <span className="text-gray-500 font-medium text-xs">-{dice.bonusSum}</span>}
              </div>
            </div>
          </div>
          <div className="flex items-center space-x-3">
            <div className="h-10 w-px bg-gray-800 mx-1"></div>
            <div className="flex flex-col items-end min-w-[70px]">
              <span className={`text-3xl font-black ${resultColorClass} leading-none tracking-tighter`}>{dice.finalResult}</span>
              {statusLabel && <span className={`text-[8px] font-black tracking-tighter mt-1 ${resultColorClass} uppercase opacity-90`}>{statusLabel}</span>}
            </div>
          </div>
        </div>
        <div className="text-[9px] text-gray-500 font-sans tracking-[0.15em] uppercase font-bold opacity-60 border-t border-gray-800 pt-1">
          {formulaString}
        </div>
      </div>
    );
  };

  const isPartnerInitiator = chat.participant.name === currentUser;
  let partnerName = isPartnerInitiator ? chat.initiatorName : chat.participant.name;
  
  const partnerData = processedMembers.find(m => m.name === partnerName);
  
  let partnerImg = isPartnerInitiator 
    ? (partnerData?.imageUrl || 'https://i.pravatar.cc/150?u=npc') 
    : chat.participant.imageUrl;

  if (partnerName === 'NPC' && chat.npcMask) {
    partnerName = chat.npcMask;
  }

  // Admin Theme Constants
  const headerBg = isPeeking ? 'bg-[#2d0a0a]' : 'bg-card-bg';
  const headerBorder = isPeeking ? 'border-primary/30' : 'border-parchment-border';
  const titleColor = isPeeking ? 'text-primary' : 'text-primary';
  const subTitleColor = isPeeking ? 'text-white/80' : 'text-text-dark';
  const bodyOverlay = isPeeking ? 'before:absolute before:inset-0 before:bg-red-950/20 before:pointer-events-none' : '';

  return (
    <div className={`h-screen flex flex-col ${isPeeking ? 'bg-[#1a0f0f]' : 'bg-[#F7F3E8]'} animate-fadeIn overflow-hidden`}>
      <header className={`${headerBg} border-b ${headerBorder} p-4 sticky top-0 z-20 shadow-sm flex items-center justify-between space-x-3 flex-shrink-0 transition-colors`}>
        <button 
          onClick={() => onNavigate(isPeeking ? 'GOD_MODE' : 'CHAT_LIST')} 
          className="material-icons text-primary"
        >
          arrow_back
        </button>
        <div className={`w-10 h-10 rounded-full overflow-hidden border ${isPeeking ? 'border-primary/50' : 'border-parchment-border'} shadow-sm flex-shrink-0`}>
          <img src={partnerImg} alt={partnerName} className="w-full h-full object-cover" />
        </div>
        <div className="flex-1 overflow-hidden">
          <h2 className={`font-serif text-xs font-bold ${titleColor} uppercase tracking-widest truncate`}>{chat.title}</h2>
          <div className="flex items-center space-x-1">
            <span className={`text-[9px] ${subTitleColor} font-sans font-bold uppercase truncate`}>{partnerName}</span>
            {partnerData?.isOnline && <span className="w-1.5 h-1.5 rounded-full bg-green-500"></span>}
          </div>
        </div>
        <button onClick={() => setShowGallery(true)} className="material-icons text-primary p-1">photo_library</button>
      </header>

      <div ref={scrollRef} className={`flex-1 overflow-y-auto p-4 space-y-4 parchment-texture relative flex flex-col scroll-smooth ${bodyOverlay}`}>
        {messages.map((msg) => {
          const isSenderMe = msg.isMe;
          // In Peeking mode, always show the real sender's name
          let displaySenderName = isPeeking ? msg.sender : (isSenderMe ? 'Ich' : msg.sender);

          return (
            <div key={msg.id} className={`flex ${isSenderMe ? 'justify-end' : 'justify-start'}`}>
              {msg.diceResult ? (
                <div className="flex flex-col items-end space-y-1">
                  {renderDiceCard(msg.diceResult)}
                  <span className={`text-[7px] opacity-40 uppercase font-bold tracking-tighter ${isPeeking ? 'text-white' : 'text-black'}`}>
                    {displaySenderName} • {msg.timestamp}
                  </span>
                </div>
              ) : msg.imageUrl ? (
                <div className="flex flex-col items-end space-y-1">
                  <div 
                    onClick={() => openFullscreen(msg.imageUrl!)}
                    className={`max-w-[80%] p-1 bg-white border-2 ${isPeeking ? 'border-primary/40' : 'border-parchment-border'} rounded-lg shadow-lg cursor-pointer active:scale-95 transition-transform`}
                  >
                    <img src={msg.imageUrl} alt="Anhang" className="rounded w-full object-cover max-h-80" />
                  </div>
                  <span className={`text-[7px] opacity-40 uppercase font-bold tracking-tighter ${isPeeking ? 'text-white' : 'text-black'}`}>
                    {displaySenderName} • {msg.timestamp}
                  </span>
                </div>
              ) : (
                <div className={`max-w-[85%] p-3 rounded-xl shadow-sm text-sm font-sans relative whitespace-pre-wrap ${
                  isSenderMe 
                  ? (isPeeking ? 'bg-red-900/40 text-white rounded-br-none border border-primary/20' : 'bg-[#EFE7D6] text-black rounded-br-none border border-[#DCCEB0]') 
                  : (isPeeking ? 'bg-black/40 text-white rounded-bl-none border border-white/5' : 'bg-white text-black rounded-bl-none border border-gray-100')
                }`}>
                  <p className="leading-relaxed">{msg.text}</p>
                  <span className="text-[7px] opacity-40 block text-right mt-1 uppercase font-bold tracking-tighter">
                    {displaySenderName} • {msg.timestamp}
                  </span>
                </div>
              )}
            </div>
          );
        })}
      </div>

      <div className={`p-4 ${isPeeking ? 'bg-[#2d0a0a] border-primary/20' : 'bg-white border-[#E8DCC4]'} border-t flex items-center space-x-2 flex-shrink-0 z-10 transition-colors`}>
        <input 
          type="file" 
          ref={fileInputRef} 
          className="hidden" 
          accept="image/*" 
          onChange={handleFileChange}
        />
        <button onClick={() => setShowActionMenu(!showActionMenu)} className={`material-icons text-gray-400 text-2xl transition-transform duration-300 ${showActionMenu ? 'rotate-45 text-primary' : ''}`}>add</button>
        <div className="flex-1 relative">
          <input 
            type="text" 
            value={inputValue} 
            onChange={(e) => setInputValue(e.target.value)} 
            placeholder={isPeeking ? "Eingriff in das Schicksal..." : "Schreibt Eure Antwort..."}
            className={`w-full ${isPeeking ? 'bg-black/40 border-primary/20 text-white placeholder-primary/40' : 'bg-gray-50 border-gray-100 text-black placeholder-gray-500'} border rounded-full py-2.5 px-4 text-sm focus:ring-1 focus:ring-primary focus:bg-white transition-all outline-none`} 
            onKeyPress={(e) => e.key === 'Enter' && handleSend()} 
          />
        </div>
        <button onClick={() => handleSend()} className={`material-icons transition-all duration-300 transform active:scale-90 ${inputValue.trim() ? 'text-primary rotate-[-45deg]' : 'text-gray-300'}`}>send</button>
      </div>

      {showActionMenu && (
        <div className="absolute bottom-20 left-4 z-40 animate-slideUp">
          <div className={`${isPeeking ? 'bg-[#2d0a0a] border-primary/40 shadow-[0_0_20px_rgba(212,175,55,0.2)]' : 'bg-white border-parchment-border'} rounded-2xl shadow-2xl border p-2 space-y-1`}>
            <button 
              onClick={() => { setShowActionMenu(false); setShowDiceModal(true); }}
              className={`flex items-center space-x-3 w-full p-3 ${isPeeking ? 'hover:bg-primary/10 text-white' : 'hover:bg-gray-50 text-black'} rounded-xl transition-colors`}
            >
              <div className="w-12 h-12 bg-primary/10 rounded-full flex items-center justify-center text-primary">
                <DiceIcon className="text-3xl" />
              </div>
              <span className="font-serif text-xs font-bold uppercase tracking-wider">Würfeln</span>
            </button>
            <button 
              onClick={() => { setShowActionMenu(false); fileInputRef.current?.click(); }}
              className={`flex items-center space-x-3 w-full p-3 ${isPeeking ? 'hover:bg-primary/10 text-white' : 'hover:bg-gray-50 text-black'} rounded-xl transition-colors`}
            >
              <div className="w-12 h-12 bg-accent-blue/10 rounded-full flex items-center justify-center text-accent-blue">
                <span className="material-icons text-3xl">image</span>
              </div>
              <span className="font-serif text-xs font-bold uppercase tracking-wider">Bild senden</span>
            </button>
          </div>
          <div className={`${isPeeking ? 'bg-[#2d0a0a] border-b border-r border-primary/40' : 'bg-white border-b border-r border-parchment-border'} w-4 h-4 rotate-45 mx-4 -mt-2`}></div>
        </div>
      )}

      {/* Fullscreen Image Modal */}
      {fullscreenImageIndex !== null && (
        <div 
          className="fixed inset-0 z-[100] bg-black/95 flex items-center justify-center p-0 animate-fadeIn"
          onTouchStart={handleFullscreenTouchStart}
          onTouchEnd={handleFullscreenTouchEnd}
          onClick={(e) => {
            if (e.target === e.currentTarget) setFullscreenImageIndex(null);
          }}
        >
          <div className="absolute top-0 left-0 right-0 p-6 flex items-center justify-between z-[110] bg-gradient-to-b from-black/50 to-transparent">
            <span className="text-white font-serif text-[10px] font-bold uppercase tracking-widest">
              Bild {fullscreenImageIndex + 1} von {galleryImages.length}
            </span>
            <button onClick={() => setFullscreenImageIndex(null)} className="text-white material-icons text-3xl">close</button>
          </div>
          
          <div className="w-full h-full flex items-center justify-center overflow-hidden">
            <img 
              key={galleryImages[fullscreenImageIndex]}
              src={galleryImages[fullscreenImageIndex]} 
              alt="Vollbild" 
              className="max-w-full max-h-full object-contain shadow-2xl animate-fadeIn" 
            />
          </div>

          <div className="absolute bottom-10 left-0 right-0 flex justify-center space-x-2 pointer-events-none">
             {galleryImages.map((_, i) => (
               <div key={i} className={`w-1.5 h-1.5 rounded-full transition-all ${i === fullscreenImageIndex ? 'bg-primary scale-125' : 'bg-white/20'}`}></div>
             ))}
          </div>
        </div>
      )}

      {/* Mediathek Modal */}
      {showGallery && (
        <div className="fixed inset-0 z-[80] bg-black/40 backdrop-blur-sm animate-fadeIn flex flex-col">
          <div className={`${isPeeking ? 'bg-[#1a0f0f]' : 'bg-white'} flex-1 mt-12 rounded-t-3xl shadow-2xl flex flex-col overflow-hidden`}>
            <header className={`p-4 border-b ${isPeeking ? 'border-primary/20' : 'border-gray-100'} flex items-center justify-between`}>
              <div className="flex items-center space-x-2">
                <span className="material-icons text-primary">photo_library</span>
                <h3 className={`font-serif font-bold uppercase tracking-widest text-sm ${isPeeking ? 'text-primary' : 'text-black'}`}>Mediathek</h3>
              </div>
              <button onClick={() => setShowGallery(false)} className="material-icons text-gray-400">close</button>
            </header>
            <div className="flex-1 overflow-y-auto p-4">
              {galleryImages.length > 0 ? (
                <div className="grid grid-cols-3 gap-2">
                  {galleryImages.map((img, idx) => (
                    <div 
                      key={idx} 
                      onClick={() => {
                        setFullscreenImageIndex(idx);
                        setShowGallery(false);
                      }}
                      className={`aspect-square rounded-lg overflow-hidden border ${isPeeking ? 'border-primary/10 bg-black/40' : 'border-gray-100 bg-gray-50'} active:scale-95 transition-transform`}
                    >
                      <img src={img} alt={`Galerie ${idx}`} className="w-full h-full object-cover" />
                    </div>
                  ))}
                </div>
              ) : (
                <div className="h-full flex flex-col items-center justify-center text-gray-300 opacity-50 space-y-4">
                  <span className="material-icons text-6xl">no_photography</span>
                  <p className="font-serif uppercase tracking-widest text-[10px] font-bold">Keine Brief-Beilagen gefunden</p>
                </div>
              )}
            </div>
            <p className="p-4 text-center text-[8px] text-gray-400 font-serif uppercase tracking-[0.2em]">Sündenbock Archiv • {chat.title}</p>
          </div>
        </div>
      )}

      {showDiceModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-6 bg-black/40 backdrop-blur-sm animate-fadeIn">
          <div className={`${isPeeking ? 'bg-[#1a0f0f] border-primary/50 shadow-[0_0_30px_rgba(212,175,55,0.1)]' : 'bg-white'} w-full max-w-sm rounded-2xl shadow-2xl overflow-hidden border-2`}>
            <div className="bg-primary p-4 flex justify-between items-center text-white shadow-md">
              <div className="flex items-center space-x-3">
                <DiceIcon className="text-2xl" />
                <h3 className="font-serif font-bold uppercase tracking-widest text-sm">Das Schicksal befragen</h3>
              </div>
              <button onClick={() => setShowDiceModal(false)} className="material-icons text-sm">close</button>
            </div>
            
            <div className={`p-6 space-y-6 ${isPeeking ? 'text-white' : 'text-black'} overflow-y-auto max-h-[85vh]`}>
              <div className="space-y-2">
                <label className="text-[10px] font-bold text-gray-400 uppercase tracking-widest block px-1">Art des Wurfes</label>
                <div className={`${isPeeking ? 'bg-black/40' : 'bg-gray-100'} p-1 rounded-xl flex`}>
                  <button 
                    onClick={() => setRollType('STANDARD')} 
                    className={`flex-1 py-2.5 text-[10px] font-bold uppercase rounded-lg transition-all ${rollType === 'STANDARD' ? 'bg-white shadow-md text-primary scale-[1.02]' : 'text-gray-500'}`}
                  >
                    1D100
                  </button>
                  <button 
                    onClick={() => setRollType('ROUND')} 
                    className={`flex-1 py-2.5 text-[10px] font-bold uppercase rounded-lg transition-all ${rollType === 'ROUND' ? 'bg-white shadow-md text-primary scale-[1.02]' : 'text-gray-500'}`}
                  >
                    1D4
                  </button>
                </div>
              </div>

              {rollType === 'STANDARD' && (
                <div className="space-y-6 animate-fadeIn">
                  <div className="grid grid-cols-2 gap-4">
                    <div className="space-y-2">
                      <label className="text-[10px] font-bold text-gray-400 uppercase tracking-widest block px-1">Talentwert</label>
                      <input 
                        type="number" 
                        value={talentValue}
                        onChange={(e) => handleTalentChange(e.target.value)}
                        placeholder="z.B. 45"
                        className={`w-full ${isPeeking ? 'bg-black/20 border-primary/20 text-white' : 'bg-gray-50 border-gray-200 text-black'} border rounded-xl py-3 px-4 text-sm focus:ring-1 focus:ring-primary outline-none transition-all`}
                      />
                    </div>
                    <div className="space-y-2">
                      <label className="text-[10px] font-bold text-gray-400 uppercase tracking-widest block px-1">Meisterschaften</label>
                      <div className={`flex items-center border rounded-xl overflow-hidden transition-all ${parseInt(talentValue) > 50 ? (isPeeking ? 'bg-black/20 border-primary/20' : 'bg-gray-50 border-gray-200') : (isPeeking ? 'bg-black/40 border-primary/5 opacity-50' : 'bg-gray-100 border-gray-200 opacity-50 cursor-not-allowed')}`}>
                        <button 
                          disabled={parseInt(talentValue) <= 50}
                          onClick={() => setBonusD6(Math.max(0, bonusD6 - 1))} 
                          className="px-3 py-3 text-primary disabled:text-gray-300 active:bg-primary/10 transition-colors"
                        >
                          <span className="material-icons text-sm">remove</span>
                        </button>
                        <span className="flex-1 text-center font-bold text-sm text-shadow-none">{bonusD6}</span>
                        <button 
                          disabled={parseInt(talentValue) <= 50}
                          onClick={() => setBonusD6(Math.min(3, bonusD6 + 1))} 
                          className="px-3 py-3 text-primary disabled:text-gray-300 active:bg-primary/10 transition-colors"
                        >
                          <span className="material-icons text-sm">add</span>
                        </button>
                      </div>
                    </div>
                  </div>

                  <div className="space-y-2">
                    <label className="text-[10px] font-bold text-gray-400 uppercase tracking-widest block px-1">Schicksalsgunst</label>
                    <div className="flex space-x-2">
                      {(['NONE', 'ADVANTAGE', 'DISADVANTAGE'] as const).map(type => (
                        <button
                          key={type}
                          onClick={() => setAdvantage(type)}
                          className={`flex-1 py-2.5 rounded-lg border text-[8px] font-bold uppercase transition-all ${
                            advantage === type 
                            ? 'bg-primary/10 border-primary text-primary shadow-sm' 
                            : (isPeeking ? 'bg-black/20 border-white/5 text-gray-500' : 'bg-white border-gray-200 text-gray-400')
                          }`}
                        >
                          {type === 'NONE' ? 'Normal' : type === 'ADVANTAGE' ? 'Vorteil' : 'Nachteil'}
                        </button>
                      ))}
                    </div>
                  </div>
                </div>
              )}

              <div className="pt-2">
                <button 
                  onClick={performRoll} 
                  className="w-full bg-primary text-white py-4 rounded-2xl font-serif font-bold uppercase tracking-[0.2em] shadow-xl active:scale-95 transition-transform"
                >
                  Die Würfel werfen
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
