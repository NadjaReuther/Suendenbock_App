
import React, { useState } from 'react';
import { ParchmentCard } from '../components/ParchmentCard';
import { InputField } from '../components/InputField';
import { Button } from '../components/Button';
import { MEMBERS } from '../App';

interface LoginViewProps {
  onLogin: (username: string) => void;
  correctPasswordHash: string;
}

export const LoginView: React.FC<LoginViewProps> = ({ onLogin, correctPasswordHash }) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    // Suche den Charakter in der Liste (Groß-/Kleinschreibung ignorieren für bessere UX)
    const foundMember = MEMBERS.find(
      m => m.name.toLowerCase() === username.trim().toLowerCase()
    );

    if (foundMember && password === correctPasswordHash) {
      setError(null);
      // Wir loggen mit dem exakten Namen aus der Liste ein (z.B. "Jewa" statt "jewa")
      onLogin(foundMember.name);
    } else {
      setError('Die Pforten bleiben geschlossen. Ungültige Anmeldedaten.');
    }
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center px-6 py-8 animate-fadeIn">
      <div className="text-center mb-10">
        <h1 className="text-4xl md:text-5xl text-text-dark font-serif font-bold mb-2 drop-shadow-sm tracking-widest uppercase text-shadow-sm">
          Sündenbock 1618
        </h1>
        <div className="h-0.5 w-16 mx-auto bg-primary rounded-full opacity-70 mb-2"></div>
        <p className="text-gray-600 font-serif text-xs uppercase tracking-[0.3em]">
          Rollenspiel Community
        </p>
      </div>

      <ParchmentCard className="w-full max-w-sm">
        <h2 className="text-2xl text-center font-serif text-text-dark mb-8 font-bold">
          Anmeldung
        </h2>

        <form onSubmit={handleSubmit} className="space-y-6">
          {error && (
            <div className="text-red-700 text-xs text-center font-serif italic bg-red-50 p-2 rounded border border-red-200 animate-pulse">
              {error}
            </div>
          )}

          <InputField
            id="username"
            label="Benutzername"
            placeholder="Name Eures Charakters"
            icon="person"
            value={username}
            onChange={(e) => {
              setUsername(e.target.value);
              if(error) setError(null);
            }}
          />

          <InputField
            id="password"
            label="Passwort"
            type="password"
            placeholder="Passwort"
            icon="lock"
            value={password}
            onChange={(e) => {
              setPassword(e.target.value);
              if(error) setError(null);
            }}
            showToggle
          />

          <div className="flex items-center justify-between text-xs">
            <div className="flex items-center">
              <input
                id="remember-me"
                type="checkbox"
                className="h-4 w-4 text-primary focus:ring-primary border-gray-300 rounded"
              />
              <label htmlFor="remember-me" className="ml-2 block text-gray-700 font-sans">
                Angemeldet bleiben
              </label>
            </div>
            <a href="#" className="font-medium text-accent-blue hover:text-indigo-500 font-sans">
              Passwort vergessen?
            </a>
          </div>

          <Button type="submit">
            Einloggen
          </Button>
        </form>
      </ParchmentCard>

      <div className="mt-12 text-center">
        <p className="text-[10px] text-gray-500 font-serif uppercase tracking-widest opacity-60">
          © 2023 SÜNDENBOCK APP. ALL RIGHTS RESERVED.
        </p>
      </div>
    </div>
  );
};
