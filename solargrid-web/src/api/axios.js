import axios from 'axios';

/**
 * Shared axios instance configured with the base API URL.
 * It intercepts requests to attach the JWT token.
 */
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('jwt_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// We will handle 401s globally via the AuthContext or another interceptor
// so that the user is logged out automatically.

export default api;
