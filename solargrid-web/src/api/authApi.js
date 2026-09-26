import api from './axios';

/**
 * Authentication API endpoints
 */
export const authApi = {
  login: async (credentials) => {
    const response = await api.post('/api/auth/login', credentials);
    return response.data;
  },
  
  register: async (data) => {
    const response = await api.post('/api/auth/register', data);
    return response.data;
  }
};
