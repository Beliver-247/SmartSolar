import api from './axios';

export const stationsApi = {
  getAll: async () => {
    const response = await api.get('/api/stations');
    return response.data;
  },
  
  create: async (data) => {
    const response = await api.post('/api/stations', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/api/stations/${id}`, data);
    return response.data;
  },

  updateSlots: async (id, totalSlots) => {
    // The backend does not have a dedicated slot update endpoint.
    // We must fetch the station and update the whole thing.
    const getRes = await api.get(`/api/stations/${id}`);
    const payload = getRes.data;
    payload.batterySlots = parseInt(totalSlots, 10);
    const response = await api.put(`/api/stations/${id}`, payload);
    return response.data;
  },

  deactivate: async (id) => {
    const response = await api.delete(`/api/stations/${id}`);
    return response.data;
  },

  getSlots: async (stationId) => {
    const response = await api.get(`/api/stations/${stationId}/slots`);
    return response.data;
  },

  createSlot: async (stationId, data) => {
    const response = await api.post(`/api/stations/${stationId}/slots`, data);
    return response.data;
  }
};
