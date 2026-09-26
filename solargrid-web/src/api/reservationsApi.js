import api from './axios';

export const reservationsApi = {
  getAll: async () => {
    const response = await api.get('/api/reservations');
    return response.data;
  },
  
  create: async (data) => {
    const response = await api.post('/api/reservations', data);
    return response.data;
  },

  modify: async (id, data) => {
    const response = await api.put(`/api/reservations/${id}`, data);
    return response.data;
  },

  cancel: async (id) => {
    const response = await api.delete(`/api/reservations/${id}`);
    return response.data;
  },

  approve: async (id) => {
    const response = await api.put(`/api/reservations/${id}/approve`);
    return response.data;
  },

  complete: async (id) => {
    const response = await api.put(`/api/transfers/${id}/complete`);
    return response.data;
  }
};
