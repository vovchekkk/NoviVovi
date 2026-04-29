import axios from 'axios';
import type {
  NovelResponse,
  CreateNovelRequest,
  PatchNovelRequest,
  LabelResponse,
  AddLabelRequest,
  PatchLabelRequest,
  CharacterResponse,
  AddCharacterRequest,
  PatchCharacterRequest,
  StepResponse,
  AddStepRequest,
  PatchStepRequest,
  NovelGraphResponse,
} from './types';

const API_BASE = import.meta.env.VITE_API_URL || 'http://localhost:5136/api';

export const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' },
  timeout: 10000,
});

// Request interceptor for auth
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Handle unauthorized
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// ============================================================================
// Novels API
// ============================================================================

export const novelsApi = {
  /**
   * Get all novels
   */
  getAll: () => api.get<NovelResponse[]>('/novels'),

  /**
   * Get novel by ID
   */
  getById: (novelId: string) => api.get<NovelResponse>(`/novels/${novelId}`),

  /**
   * Create new novel
   */
  create: (data: CreateNovelRequest) => api.post<NovelResponse>('/novels', data),

  /**
   * Update novel
   */
  patch: (novelId: string, data: PatchNovelRequest) =>
    api.patch<NovelResponse>(`/novels/${novelId}`, data),

  /**
   * Delete novel
   */
  delete: (novelId: string) => api.delete(`/novels/${novelId}`),

  /**
   * Get novel graph (nodes and edges)
   */
  getGraph: (novelId: string) =>
    api.get<NovelGraphResponse>(`/novels/${novelId}/graph`),

  /**
   * Export novel to RenPy format
   */
  exportToRenPy: (novelId: string) =>
    api.get(`/novels/${novelId}/export/renpy`, { responseType: 'blob' }),
};

// ============================================================================
// Labels API
// ============================================================================

export const labelsApi = {
  /**
   * Get all labels for a novel
   */
  getAll: (novelId: string) =>
    api.get<LabelResponse[]>(`/novels/${novelId}/labels`),

  /**
   * Get label by ID
   */
  getById: (novelId: string, labelId: string) =>
    api.get<LabelResponse>(`/novels/${novelId}/labels/${labelId}`),

  /**
   * Create new label
   */
  create: (novelId: string, data: AddLabelRequest) =>
    api.post<LabelResponse>(`/novels/${novelId}/labels`, data),

  /**
   * Update label
   */
  patch: (novelId: string, labelId: string, data: PatchLabelRequest) =>
    api.patch<LabelResponse>(`/novels/${novelId}/labels/${labelId}`, data),

  /**
   * Delete label
   */
  delete: (novelId: string, labelId: string) =>
    api.delete(`/novels/${novelId}/labels/${labelId}`),
};

// ============================================================================
// Characters API
// ============================================================================

export const charactersApi = {
  /**
   * Get all characters for a novel
   */
  getAll: (novelId: string) =>
    api.get<CharacterResponse[]>(`/novels/${novelId}/characters`),

  /**
   * Get character by ID
   */
  getById: (novelId: string, characterId: string) =>
    api.get<CharacterResponse>(`/novels/${novelId}/characters/${characterId}`),

  /**
   * Create new character
   */
  create: (novelId: string, data: AddCharacterRequest) =>
    api.post<CharacterResponse>(`/novels/${novelId}/characters`, data),

  /**
   * Update character
   */
  patch: (novelId: string, characterId: string, data: PatchCharacterRequest) =>
    api.patch<CharacterResponse>(`/novels/${novelId}/characters/${characterId}`, data),

  /**
   * Delete character
   */
  delete: (novelId: string, characterId: string) =>
    api.delete(`/novels/${novelId}/characters/${characterId}`),
};

// ============================================================================
// Steps API
// ============================================================================

export const stepsApi = {
  /**
   * Get all steps for a label
   */
  getAll: (novelId: string, labelId: string) =>
    api.get<StepResponse[]>(`/novels/${novelId}/labels/${labelId}/steps`),

  /**
   * Get step by ID
   */
  getById: (novelId: string, labelId: string, stepId: string) =>
    api.get<StepResponse>(`/novels/${novelId}/labels/${labelId}/steps/${stepId}`),

  /**
   * Create new step
   */
  create: (novelId: string, labelId: string, data: AddStepRequest) =>
    api.post<StepResponse>(`/novels/${novelId}/labels/${labelId}/steps`, data),

  /**
   * Update step
   */
  patch: (novelId: string, labelId: string, stepId: string, data: PatchStepRequest) =>
    api.patch<StepResponse>(`/novels/${novelId}/labels/${labelId}/steps/${stepId}`, data),

  /**
   * Delete step
   */
  delete: (novelId: string, labelId: string, stepId: string) =>
    api.delete(`/novels/${novelId}/labels/${labelId}/steps/${stepId}`),
};

// ============================================================================
// Images API
// ============================================================================

export const imagesApi = {
  /**
   * Get upload URL for image
   */
  getUploadUrl: () =>
    api.post<{ imageId: string; uploadUrl: string }>('/images/upload-url'),

  /**
   * Get image by ID
   */
  getById: (imageId: string) =>
    api.get<{ url: string }>(`/images/${imageId}`),

  /**
   * Upload image to cloud storage
   */
  upload: (uploadUrl: string, file: File) =>
    axios.put(uploadUrl, file, {
      headers: {
        'Content-Type': file.type,
      },
    }),
};

export default api;
