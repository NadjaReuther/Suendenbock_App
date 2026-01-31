
import React, { useState } from 'react';
import { ViewType } from '../types';
import { ParchmentCard } from '../components/ParchmentCard';
import { InputField } from '../components/InputField';
import { Button } from '../components/Button';

interface ChangePasswordViewProps {
  onNavigate: (view: ViewType) => void;
  currentPasswordHash: string;
  onChangePassword: (newPass: string) => void;
}

export const ChangePasswordView: React.FC<ChangePasswordViewProps> = ({ 
  onNavigate, 
  currentPasswordHash, 
  onChangePassword 
}) => {
  const [oldPass, setOldPass] = useState('');
  const [newPass, setNewPass] = useState('');
  const [confirmPass, setConfirmPass] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (oldPass !== currentPasswordHash) {
      setError('Das aktuelle Passwort ist nicht korrekt.');
      return;
    }

    if (newPass.length < 6) {
      setError('Das neue Passwort muss mindestens 6 Zeichen lang sein.');
      return;
    }

    if (newPass !== confirmPass) {
      setError('Die Passwortbestätigung stimmt nicht überein.');
      return;
    }

    onChangePassword(newPass);
    setSuccess(true);
    setTimeout(() => {
      onNavigate('SECURITY_SETTINGS');
    }, 2000);
  };

  return (
    <div className="min-h-screen pb-24 animate-fadeIn flex flex-col bg-[#F7F3E8]">
      <header className="bg-card-bg border-b border-parchment-border p-4 sticky top-0 z-20 shadow-sm flex items-center justify-between">
        <button 
          onClick={() => onNavigate('SECURITY_SETTINGS')}
          className="material-icons text-primary"
        >
          arrow_back
        </button>
        <h1 className="font-serif font-bold text-xl text-text-dark tracking-wider uppercase">Passwort</h1>
        <div className="w-6"></div>
      </header>

      <div className="px-4 py-8 flex-1 overflow-y-auto flex flex-col items-center">
        <ParchmentCard className="w-full max-w-sm">
          <h2 className="text-xl text-center font-serif text-text-dark mb-6 font-bold uppercase tracking-tight">
            Zugang erneuern
          </h2>

          {success ? (
            <div className="text-center py-8 space-y-4 animate-bounce">
              <span className="material-icons text-6xl text-green-500">check_circle</span>
              <p className="font-serif text-sm text-text-dark uppercase font-bold tracking-widest">
                Das Siegel wurde erneuert!
              </p>
            </div>
          ) : (
            <form onSubmit={handleSubmit} className="space-y-5">
              {error && (
                <div className="text-red-700 text-[10px] text-center font-serif italic bg-red-50 p-2 rounded border border-red-200">
                  {error}
                </div>
              )}

              <InputField
                id="old-pass"
                label="Aktuelles Passwort"
                type="password"
                placeholder="Aktuelles Passwort"
                icon="lock_open"
                value={oldPass}
                onChange={(e) => setOldPass(e.target.value)}
                showToggle
              />

              <div className="h-px bg-gray-100 w-full"></div>

              <InputField
                id="new-pass"
                label="Neues Passwort"
                type="password"
                placeholder="Neues Passwort"
                icon="vpn_key"
                value={newPass}
                onChange={(e) => setNewPass(e.target.value)}
                showToggle
              />

              <InputField
                id="confirm-pass"
                label="Passwort bestätigen"
                type="password"
                placeholder="Bestätigung"
                icon="verified"
                value={confirmPass}
                onChange={(e) => setConfirmPass(e.target.value)}
                showToggle
              />

              <div className="pt-2">
                <Button type="submit">
                  Passwort ändern
                </Button>
              </div>
            </form>
          )}
        </ParchmentCard>
        
        <p className="mt-8 text-[10px] text-gray-400 font-serif uppercase text-center px-4 leading-relaxed">
          Wählt ein starkes Passwort, um Eure Geheimnisse vor den Schatten zu bewahren.
        </p>
      </div>
    </div>
  );
};
