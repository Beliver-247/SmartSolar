import React from 'react';
import { NavLink } from 'react-router-dom';
import { useAuth } from '../../auth/AuthContext';
import { MapPin, CalendarClock, Users, UserCircle } from 'lucide-react';

const Sidebar = () => {
  const { user } = useAuth();
  const isBackoffice = user?.role === 'Backoffice';

  const navItems = [
    { name: 'Stations', path: '/stations', icon: MapPin },
    { name: 'Reservations', path: '/reservations', icon: CalendarClock },
  ];

  if (isBackoffice) {
    navItems.push({ name: 'Prosumers', path: '/prosumers', icon: Users });
    navItems.push({ name: 'Web Users', path: '/users', icon: UserCircle });
  }

  return (
    <div className="w-64 bg-slate-800 text-white flex flex-col">
      <div className="h-16 flex items-center px-5 space-x-3 border-b border-slate-700">
        <img src="/logo.png" alt="Logo" className="w-8 h-8 rounded object-contain" />
        <span className="text-xl font-bold tracking-wider">SolarGrid</span>
      </div>
      <nav className="flex-1 px-4 py-6 space-y-2">
        {navItems.map((item) => {
          const Icon = item.icon;
          return (
            <NavLink
              key={item.path}
              to={item.path}
              className={({ isActive }) =>
                `flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors ${
                  isActive
                    ? 'bg-primary-600 text-white'
                    : 'text-slate-300 hover:bg-slate-700 hover:text-white'
                }`
              }
            >
              <Icon size={20} />
              <span>{item.name}</span>
            </NavLink>
          );
        })}
      </nav>
    </div>
  );
};

export default Sidebar;
