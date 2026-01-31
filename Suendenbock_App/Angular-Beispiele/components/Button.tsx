
import React from 'react';

interface ButtonProps {
  children: React.ReactNode;
  onClick?: () => void;
  type?: "button" | "submit";
  variant?: "primary" | "secondary";
  className?: string;
  disabled?: boolean;
}

export const Button: React.FC<ButtonProps> = ({ 
  children, onClick, type = "button", variant = "primary", className = "", disabled = false 
}) => {
  const baseStyles = "w-full flex justify-center py-3 px-4 border border-transparent rounded-md shadow-md text-sm font-bold font-serif uppercase tracking-wider transition-all transform active:scale-95 disabled:opacity-50";
  
  const variants = {
    primary: "text-white bg-gradient-to-r from-accent-blue to-indigo-600 hover:from-indigo-600 hover:to-accent-blue",
    secondary: "text-primary bg-transparent border-primary hover:bg-primary hover:text-white"
  };

  return (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled}
      className={`${baseStyles} ${variants[variant]} ${className}`}
    >
      {children}
    </button>
  );
};
