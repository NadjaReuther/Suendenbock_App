
import React from 'react';

interface ParchmentCardProps {
  children: React.ReactNode;
  className?: string;
}

export const ParchmentCard: React.FC<ParchmentCardProps> = ({ children, className = "" }) => {
  return (
    <div className={`bg-card-bg rounded-lg shadow-xl border-2 border-parchment-border p-6 relative overflow-hidden ${className}`}>
      {/* Decorative Corners */}
      <div className="absolute top-2 left-2 w-4 h-4 border-t-2 border-l-2 border-primary rounded-tl-sm pointer-events-none"></div>
      <div className="absolute top-2 right-2 w-4 h-4 border-t-2 border-r-2 border-primary rounded-tr-sm pointer-events-none"></div>
      <div className="absolute bottom-2 left-2 w-4 h-4 border-b-2 border-l-2 border-primary rounded-bl-sm pointer-events-none"></div>
      <div className="absolute bottom-2 right-2 w-4 h-4 border-b-2 border-r-2 border-primary rounded-br-sm pointer-events-none"></div>
      
      {children}
    </div>
  );
};
