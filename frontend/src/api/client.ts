import axios, { AxiosError, type InternalAxiosRequestConfig } from 'axios';
import { ProblemDetailsSchema, type ProblemDetails } from '@/types/api';

const baseURL = import.meta.env.VITE_API_BASE_URL || '/api/v1';

export const apiClient = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request Interceptor: Attach Mock Bearer Token
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    config.headers.Authorization = `Bearer mock-token`;
    return config;
  },
  (error: unknown) => Promise.reject(error),
);

// Response Interceptor: Parse RFC 7807 ProblemDetails
apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.data) {
      const parsed = ProblemDetailsSchema.safeParse(error.response.data);
      if (parsed.success) {
        return Promise.reject(parsed.data);
      }
    }
    const fallbackError: ProblemDetails = {
      title: error.message || 'An unexpected error occurred',
      status: error.response?.status || 500,
    };
    return Promise.reject(fallbackError);
  },
);

export default apiClient;
