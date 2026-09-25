import React from 'react';
import { AlertCircle } from 'lucide-react';

const ErrorBanner = ({ message }) => {
  if (!message) return null;

  return (
    <div className="bg-red-50 border border-red-200 rounded-md p-4 flex items-start space-x-3 text-red-700">
      <AlertCircle className="shrink-0 mt-0.5" size={20} />
      <div className="text-sm">{message}</div>
    </div>
  );
};

export default ErrorBanner;
