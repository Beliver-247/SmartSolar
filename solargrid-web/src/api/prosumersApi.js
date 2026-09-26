import api from './axios';

export const prosumersApi = {
  getAll: async () => {
    const response = await api.get('/api/prosumers');
    return response.data;
  },
  
  getById: async (id) => {
    const response = await api.get(`/api/prosumers/${id}`);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/api/prosumers/${id}`, data);
    return response.data;
  },

  deactivate: async (id) => {
    const response = await api.put(`/api/prosumers/${id}/deactivate`);
    return response.data;
  },

  reactivate: async (id) => {
    const response = await api.put(`/api/prosumers/${id}/reactivate`);
    return response.data;
  }
};
