import { api } from '@/shared/api/axios';
import type { Novel } from './types';
export const userApi = {
    getAll: () => api.get<Novel[]>('/novels'),
    getById: (id: string) => api.get(`/novels/${id}`),
    update: (id: string,  User) => api.put(`/novels/${id}`, data),
    delete: (id: string) => api.delete(`/novels/${id}`),
};