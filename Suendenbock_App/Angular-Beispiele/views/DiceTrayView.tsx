
import React, { useState } from 'react';
import { ViewType, DiceResult, SuccessStatus } from '../types';

interface DiceTrayViewProps {
  onNavigate: (view: ViewType) => void;
  onNewRoll: (roll: DiceResult) => void;
  rolls: DiceResult[];
  totalUnread?: number;
}

export const DiceTrayView: React.FC<DiceTrayViewProps> = ({ onNavigate, onNewRoll, rolls, totalUnread = 0 }) => {
  const [rollType, setRollType] = useState<'1d100' | '1d4'>('1d100');
  const [bonusD6, setBonusD6] = useState(0);
  const [advantage, setAdvantage] = useState<'NONE' | 'ADVANTAGE' | 'DISADVANTAGE'>('NONE');
  const [talentValue, setTalentValue] = useState<string>('');

  const handleTalentChange = (val: string) => {
    setTalentValue(val);
    const numericVal = parseInt(val);
    if (isNaN(numericVal) || numericVal <= 50) {
      setBonusD6(0);
    }
  };

  const performRoll = () => {
    const rollDie = (max: number) => Math.floor(Math.random() * max) + 1;
    let primaryDieMax = rollType === '1d100' ? 100 : 4;
    let roll1Raw = rollDie(primaryDieMax);
    let roll2Raw = advantage !== 'NONE' ? rollDie(primaryDieMax) : roll1Raw;

    const normalize = (val: number) => (rollType === '1d100' && val === 100 ? 0 : val);
    let roll1 = normalize(roll1Raw);
    let roll2 = normalize(roll2Raw);
    
    let chosenRoll = roll1;
    if (advantage === 'ADVANTAGE') chosenRoll = Math.min(roll1, roll2);
    if (advantage === 'DISADVANTAGE') chosenRoll = Math.max(roll1, roll2);

    let d6Results: number[] = [];
    if (rollType === '1d100') {
      for (let i = 0; i < bonusD6; i++) d6Results.push(rollDie(6));
    }
    
    const d6Sum = d6Results.reduce((a, b) => a + b, 0);
    const finalResult = chosenRoll - d6Sum;

    let status: SuccessStatus = 'NONE';
    const tVal = talentValue ? parseInt(talentValue) : undefined;

    if (rollType === '1d100' && tVal !== undefined) {
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

    const newRoll: DiceResult = {
      id: Date.now().toString(),
      rollType,
      primaryRolls: advantage !== 'NONE' ? [roll1, roll2] : [roll1],
      chosenPrimary: chosenRoll,
      advantage,
      bonusD6: d6Results,
      bonusSum: d6Sum,
      finalResult,
      talentValue: tVal,
      successStatus: status,
      timestamp: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    };

    onNewRoll(newRoll);
    // TalentValue und BonusD6 bleiben erhalten, da sie zusammengehören
    setAdvantage('NONE');
  };

  const getStatusStyle = (status: SuccessStatus) => {
    switch (status) {
      case 'PERFECT': return 'text-green-400 drop-shadow-[0_0_8px_rgba(74,222,128,0.6)] font-black';
      case 'CRITICAL': return 'text-yellow-400 drop-shadow-[0_0_5px_rgba(250,204,21,0.5)] font-bold';
      case 'SUCCESS': return 'text-white font-bold';
      case 'FAILURE': return 'text-red-500 font-bold';
      default: return 'text-gray-300';
    }
  };

  return (
    <div className="min-h-screen pb-24 animate-fadeIn flex flex-col bg-[#121212]">
      <header className="bg-[#1a1a1a] border-b border-primary/20 p-4 sticky top-0 z-20 shadow-md flex items-center justify-between">
        <button onClick={() => onNavigate('DASHBOARD')} className="material-icons text-primary">arrow_back</button>
        <h1 className="font-serif font-bold text-xl text-primary tracking-widest uppercase text-shadow-none">Würfelschale</h1>
        <button onClick={() => {}} className="material-icons text-primary/30">history</button>
      </header>

      <div className="flex-1 overflow-y-auto px-4 py-6 space-y-6">
        {/* Dice Setup */}
        <section className="bg-[#1e1e1e] rounded-2xl p-6 border border-primary/10 shadow-xl space-y-6">
          <div className="flex bg-[#121212] p-1 rounded-xl">
            <button 
              onClick={() => setRollType('1d100')}
              className={`flex-1 py-3 text-xs font-serif font-bold uppercase rounded-lg transition-all ${rollType === '1d100' ? 'bg-primary text-black shadow-lg scale-[1.02]' : 'text-gray-500'}`}
            >
              1d100 (Probe)
            </button>
            <button 
              onClick={() => setRollType('1d4')}
              className={`flex-1 py-3 text-xs font-serif font-bold uppercase rounded-lg transition-all ${rollType === '1d4' ? 'bg-primary text-black shadow-lg scale-[1.02]' : 'text-gray-500'}`}
            >
              1d4 (Schaden)
            </button>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-1.5">
              <label className="text-[10px] text-primary/50 font-bold uppercase tracking-widest px-1">Talentwert</label>
              <input 
                type="number" 
                value={talentValue}
                onChange={(e) => handleTalentChange(e.target.value)}
                placeholder="z.B. 45"
                className="w-full bg-[#121212] border border-primary/20 rounded-xl py-3 px-4 text-primary text-sm focus:ring-1 focus:ring-primary outline-none transition-all"
              />
            </div>
            <div className="space-y-1.5">
              <label className="text-[10px] text-primary/50 font-bold uppercase tracking-widest px-1">Meisterschaften (W6)</label>
              <div className={`flex items-center border rounded-xl overflow-hidden transition-all ${parseInt(talentValue) > 50 ? 'bg-[#121212] border-primary/20' : 'bg-[#1a1a1a] border-primary/5 opacity-50 cursor-not-allowed'}`}>
                <button 
                  disabled={parseInt(talentValue) <= 50}
                  onClick={() => setBonusD6(Math.max(0, bonusD6-1))} 
                  className="px-3 py-3 text-primary disabled:text-gray-700 active:bg-primary/10"
                >
                  <span className="material-icons text-sm">remove</span>
                </button>
                <span className="flex-1 text-center text-primary font-bold">{bonusD6}</span>
                <button 
                  disabled={parseInt(talentValue) <= 50}
                  onClick={() => setBonusD6(Math.min(3, bonusD6+1))} 
                  className="px-3 py-3 text-primary disabled:text-gray-700 active:bg-primary/10"
                >
                  <span className="material-icons text-sm">add</span>
                </button>
              </div>
            </div>
          </div>

          <div className="space-y-2">
            <label className="text-[10px] text-primary/50 font-bold uppercase tracking-widest px-1">Vorteil / Nachteil</label>
            <div className="flex space-x-2">
              {(['NONE', 'ADVANTAGE', 'DISADVANTAGE'] as const).map(type => (
                <button
                  key={type}
                  onClick={() => setAdvantage(type)}
                  className={`flex-1 py-2.5 rounded-lg border text-[9px] font-bold uppercase transition-all ${
                    advantage === type ? 'bg-primary/20 border-primary text-primary' : 'bg-[#121212] border-primary/5 text-gray-600'
                  }`}
                >
                  {type === 'NONE' ? 'Normal' : type === 'ADVANTAGE' ? 'Vorteil' : 'Nachteil'}
                </button>
              ))}
            </div>
          </div>

          <button 
            onClick={performRoll}
            className="w-full bg-primary text-black py-5 rounded-2xl font-serif font-black uppercase tracking-[0.3em] shadow-[0_0_20px_rgba(212,175,55,0.3)] active:scale-95 transition-transform"
          >
            Die Würfel werfen
          </button>
          
          {rollType === '1d100' && parseInt(talentValue) <= 50 && (
            <p className="text-[8px] text-primary/40 text-center uppercase tracking-widest italic mt-2">
              Meisterschaften sind erst ab einem Talentwert von über 50 möglich.
            </p>
          )}
        </section>

        {/* Results History */}
        <section className="space-y-4">
          <h3 className="text-primary/40 font-serif text-[10px] font-bold uppercase tracking-[0.3em] px-1">Historie</h3>
          <div className="space-y-3">
            {rolls.length > 0 ? (
              rolls.map((roll) => (
                <div key={roll.id} className="bg-[#1e1e1e] border-l-4 border-primary p-4 rounded-r-xl shadow-lg animate-slideUp">
                  <div className="flex justify-between items-center">
                    <div className="flex items-center space-x-4 text-shadow-none">
                      <span className="material-icons text-primary/30 text-3xl">casino</span>
                      <div>
                        <div className="flex items-baseline space-x-2">
                          <span className="text-2xl font-black text-white">{roll.finalResult}</span>
                          <span className={`text-[10px] uppercase font-bold tracking-widest ${getStatusStyle(roll.successStatus)}`}>
                            {roll.successStatus !== 'NONE' ? roll.successStatus : ''}
                          </span>
                        </div>
                        <p className="text-[9px] text-gray-500 uppercase font-bold tracking-wider">
                          {roll.rollType} {roll.talentValue ? `vs ${roll.talentValue}` : ''} • {roll.timestamp}
                        </p>
                      </div>
                    </div>
                    {roll.primaryRolls.length > 1 && (
                      <div className="text-[8px] text-gray-600 font-bold bg-[#121212] px-2 py-1 rounded">
                        {roll.primaryRolls.join(' & ')}
                      </div>
                    )}
                  </div>
                </div>
              ))
            ) : (
              <div className="text-center py-10 opacity-20">
                <span className="material-icons text-5xl mb-2">casino</span>
                <p className="text-xs uppercase font-serif tracking-widest text-shadow-none text-white">Noch keine Würfe</p>
              </div>
            )}
          </div>
        </section>
      </div>

      {/* Navigation */}
      <nav className="fixed bottom-0 left-0 right-0 bg-[#1a1a1a] border-t border-primary/10 flex justify-around items-center py-3 shadow-2xl z-30">
        <button onClick={() => onNavigate('DASHBOARD')} className="flex flex-col items-center text-gray-500">
          <span className="material-icons">home</span>
          <span className="text-[10px] font-serif uppercase">Home</span>
        </button>
        <button onClick={() => onNavigate('NEWS')} className="flex flex-col items-center text-gray-500">
          <span className="material-icons">newspaper</span>
          <span className="text-[10px] font-serif uppercase">News</span>
        </button>
        <div className="relative -top-6">
          <button onClick={() => onNavigate('NEW_CHAT')} className="bg-primary text-black w-14 h-14 rounded-full shadow-[0_0_15px_rgba(212,175,55,0.5)] flex items-center justify-center border-4 border-[#121212] active:scale-90 transition-transform">
            <span className="material-icons text-2xl">add</span>
          </button>
        </div>
        <button onClick={() => onNavigate('CHAT_LIST')} className="flex flex-col items-center text-gray-500 relative">
          <span className="material-icons">chat</span>
          <span className="text-[10px] font-serif uppercase">Chat</span>
          {totalUnread > 0 && (
            <div className="absolute -top-1 -right-1 w-4 h-4 bg-primary text-black text-[9px] font-bold rounded-full flex items-center justify-center border border-[#121212]">
              {totalUnread}
            </div>
          )}
        </button>
        <button onClick={() => onNavigate('PROFILE')} className="flex flex-col items-center text-gray-500">
          <span className="material-icons">person</span>
          <span className="text-[10px] font-serif uppercase">Profil</span>
        </button>
      </nav>
    </div>
  );
};
