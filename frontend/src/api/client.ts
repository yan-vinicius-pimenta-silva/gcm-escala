import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'http://localhost:5062/api',
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  // REVIEW: window.location.href forces a full page reload, destroying all React state.
  // Use the router's navigate or clear auth context and let the route guard redirect.
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default apiClient;
