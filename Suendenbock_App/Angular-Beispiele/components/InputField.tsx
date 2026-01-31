
import React from 'react';

interface InputFieldProps {
  id: string;
  label: string;
  type?: string;
  placeholder: string;
  icon: string;
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  showToggle?: boolean;
}

export const InputField: React.FC<InputFieldProps> = ({ 
  id, label, type = "text", placeholder, icon, value, onChange, showToggle 
}) => {
  const [showPassword, setShowPassword] = React.useState(false);
  const actualType = type === 'password' && showPassword ? 'text' : type;

  return (
    <div className="relative w-full">
      <label className="sr-only" htmlFor={id}>{label}</label>
      <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
        <span className="material-icons text-gray-400 text-lg">{icon}</span>
      </div>
      <input
        id={id}
        type={actualType}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        className="block w-full pl-10 pr-10 py-3 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 text-black focus:outline-none focus:ring-1 focus:ring-primary focus:border-primary sm:text-sm shadow-sm transition-colors font-sans"
        required
      />
      {showToggle && (
        <div className="absolute inset-y-0 right-0 pr-3 flex items-center">
          <button 
            type="button" 
            onClick={() => setShowPassword(!showPassword)}
            className="text-gray-400 hover:text-gray-500 focus:outline-none"
          >
            <span className="material-icons text-lg">
              {showPassword ? 'visibility_off' : 'visibility'}
            </span>
          </button>
        </div>
      )}
    </div>
  );
};
