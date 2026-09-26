import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import ProtectedRoute from './ProtectedRoute';
import Layout from '../components/layout/Layout';

// Pages
import Login from '../pages/auth/Login';
import UserList from '../pages/users/UserList';
import ProsumerList from '../pages/prosumers/ProsumerList';
import StationList from '../pages/nodes/StationList';
import ReservationList from '../pages/reservations/ReservationList';

const AppRoutes = () => {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />

      {/* Protected routes wrapped in Layout */}
      <Route element={<ProtectedRoute />}>
        <Route element={<Layout />}>
          
          {/* Dashboard / Default routing */}
          <Route path="/" element={<Navigate to="/stations" replace />} />

          {/* Both Backoffice and GridOperator can access stations and reservations */}
          <Route path="/stations" element={<StationList />} />
          <Route path="/reservations" element={<ReservationList />} />

          {/* Only Backoffice can access users and prosumers */}
          <Route element={<ProtectedRoute allowedRoles={['Backoffice']} />}>
            <Route path="/users" element={<UserList />} />
            <Route path="/prosumers" element={<ProsumerList />} />
          </Route>

        </Route>
      </Route>

      {/* Catch-all */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};

export default AppRoutes;
