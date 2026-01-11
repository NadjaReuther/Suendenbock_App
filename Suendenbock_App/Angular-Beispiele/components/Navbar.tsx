
import React from 'react';

interface NavbarProps {
  darkMode: boolean;
  toggleDarkMode: () => void;
}

export const Navbar: React.FC<NavbarProps> = ({ darkMode, toggleDarkMode }) => {
  return (
    <nav className="sticky top-0 z-50 bg-background-light/95 dark:bg-background-dark/95 backdrop-blur-sm border-b border-border-light dark:border-border-dark shadow-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16 items-center">
          <div className="flex items-center space-x-4">
            <span className="font-display text-2xl text-primary font-bold tracking-wider cursor-pointer">
              Sündenbock 1618
            </span>
          </div>
          
          <div className="flex items-center space-x-4">
            <button 
              onClick={toggleDarkMode}
              className="p-2 rounded-full hover:bg-gray-200 dark:hover:bg-gray-800 transition-colors text-primary"
              aria-label="Design-Modus umschalten"
            >
              <span className="material-icons-outlined">
                {darkMode ? 'light_mode' : 'dark_mode'}
              </span>
            </button>
            <a className="text-xs font-bold uppercase tracking-widest hover:text-primary transition-colors" href="#">
              Login
            </a>
          </div>
        </div>
      </div>
    </nav>
  );
};
