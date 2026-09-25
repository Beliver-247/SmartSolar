import api from './axios';

export const usersApi = {
  getAll: async () => {
    const response = await api.get('/api/users');
    return response.data;
  },
  
  create: async (userData) => {
    const response = await api.post('/api/users', userData);
    return response.data;
  }
};
