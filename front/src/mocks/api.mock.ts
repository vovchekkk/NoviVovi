import MockAdapter from 'axios-mock-adapter';
import { api } from '../shared/api/client';
import type {
  NovelResponse,
  LabelResponse,
  CharacterResponse,
  StepResponse,
  ShowReplicaStepResponse,
  ShowMenuStepResponse,
  NovelGraphResponse,
  Transform,
  CharacterStateResponse,
} from '../shared/api/types';

// ============================================================================
// Helper Functions
// ============================================================================

let idCounter = 1000;
const nextId = (): string => `${(idCounter++).toString()}`;

const defaultTransform = (): Transform => ({
  x: 0,
  y: 0,
  width: 100,
  height: 100,
  scale: 1,
  rotation: 0,
  zIndex: 1,
});

const parseBody = <T extends object>(data: unknown): Partial<T> => {
  if (!data) return {};
  if (typeof data === 'string') {
    try {
      return JSON.parse(data) as Partial<T>;
    } catch {
      return {};
    }
  }
  if (typeof data === 'object') {
    return data as Partial<T>;
  }
  return {};
};

const getPath = (url?: string): string => (url ?? '').split('?')[0];

const matchUrl = (url: string | undefined, pattern: RegExp): RegExpMatchArray | null => {
  const path = getPath(url);
  return path.match(pattern);
};

// ============================================================================
// In-Memory Storage
// ============================================================================

const novels: Record<string, NovelResponse> = {};
const labels: Record<string, Record<string, LabelResponse>> = {};
const characters: Record<string, Record<string, CharacterResponse>> = {};
const steps: Record<string, Record<string, Record<string, StepResponse>>> = {};

// ============================================================================
// Seed Data
// ============================================================================

const seedNovel = (id: string, title: string): void => {
  const startLabelId = nextId();
  
  novels[id] = {
    id,
    title,
    startLabelId,
    labelIds: [startLabelId],
    characterIds: [],
  };

  labels[id] = {};
  characters[id] = {};
  steps[id] = {};

  // Create start label
  const startLabel: LabelResponse = {
    id: startLabelId,
    name: 'Начало',
    novelId: id,
    steps: [],
  };
  labels[id][startLabelId] = startLabel;
  steps[id][startLabelId] = {};

  // Не создаем начальный step - пусть пользователь сам добавит

  // Create demo characters
  const char1Id = nextId();
  const char2Id = nextId();

  const char1: CharacterResponse = {
    id: char1Id,
    name: 'Анна',
    nameColor: '#3b82f6',
    description: 'Главная героиня',
    characterStates: [
      {
        id: nextId(),
        name: 'Радость',
        description: 'Радостное выражение',
        image: { id: nextId(), url: 'https://placehold.co/200x200?text=Joy' },
        localTransform: defaultTransform(),
      },
      {
        id: nextId(),
        name: 'Грусть',
        description: 'Грустное выражение',
        image: { id: nextId(), url: 'https://placehold.co/200x200?text=Sad' },
        localTransform: defaultTransform(),
      },
    ],
  };

  const char2: CharacterResponse = {
    id: char2Id,
    name: 'Борис',
    nameColor: '#10b981',
    description: 'Второстепенный персонаж',
    characterStates: [
      {
        id: nextId(),
        name: 'Спокойствие',
        description: 'Спокойное выражение',
        image: { id: nextId(), url: 'https://placehold.co/200x200?text=Calm' },
        localTransform: defaultTransform(),
      },
    ],
  };

  characters[id][char1Id] = char1;
  characters[id][char2Id] = char2;
  novels[id].characterIds = [char1Id, char2Id];
};

// Seed initial data
seedNovel('0', 'Демо-новелла');
seedNovel('1', 'Учебная новелла');

// ============================================================================
// Mock Adapter Setup
// ============================================================================

const mock = new MockAdapter(api, {
  delayResponse: 250,
  onNoMatch: 'passthrough',
});

// ============================================================================
// Novels Endpoints
// ============================================================================

const novelsPattern = /^\/novels$/;
const novelPattern = /^\/novels\/([^/]+)$/;
const graphPattern = /^\/novels\/([^/]+)\/graph$/;

mock.onGet(novelsPattern).reply(() => {
  return [200, Object.values(novels)];
});

mock.onPost(novelsPattern).reply((config) => {
  const payload = parseBody<{ title?: string }>(config.data);
  const id = nextId();
  const title = payload.title?.trim() || `Новая новелла ${id}`;
  
  seedNovel(id, title);
  
  return [201, novels[id]];
});

mock.onGet(novelPattern).reply((config) => {
  const match = matchUrl(config.url, novelPattern);
  if (!match) return [404, { message: 'Novel not found' }];
  
  const novelId = match[1];
  if (!novels[novelId]) return [404, { message: 'Novel not found' }];
  
  return [200, novels[novelId]];
});

mock.onPatch(novelPattern).reply((config) => {
  const match = matchUrl(config.url, novelPattern);
  if (!match) return [404, { message: 'Novel not found' }];
  
  const novelId = match[1];
  if (!novels[novelId]) return [404, { message: 'Novel not found' }];
  
  const payload = parseBody<{ title?: string }>(config.data);
  if (payload.title) {
    novels[novelId] = { ...novels[novelId], title: payload.title };
  }
  
  return [200, novels[novelId]];
});

mock.onDelete(novelPattern).reply((config) => {
  const match = matchUrl(config.url, novelPattern);
  if (!match) return [404, { message: 'Novel not found' }];
  
  const novelId = match[1];
  if (!novels[novelId]) return [404, { message: 'Novel not found' }];
  
  delete novels[novelId];
  delete labels[novelId];
  delete characters[novelId];
  delete steps[novelId];
  
  return [204];
});

mock.onGet(graphPattern).reply((config) => {
  const match = matchUrl(config.url, graphPattern);
  if (!match) return [404, { message: 'Graph not found' }];
  
  const novelId = match[1];
  if (!novels[novelId]) return [404, { message: 'Novel not found' }];
  
  const novelLabels = labels[novelId] || {};
  const graph: NovelGraphResponse = {
    nodes: Object.values(novelLabels).map((label) => ({
      type: 'default' as const,
      labelId: label.id,
      labelName: label.name,
    })),
    edges: [],
  };
  
  return [200, graph];
});

// ============================================================================
// Labels Endpoints
// ============================================================================

const labelsPattern = /^\/novels\/([^/]+)\/labels$/;
const labelPattern = /^\/novels\/([^/]+)\/labels\/([^/]+)$/;

mock.onGet(labelsPattern).reply((config) => {
  const match = matchUrl(config.url, labelsPattern);
  if (!match) return [404, { message: 'Labels not found' }];
  
  const novelId = match[1];
  if (!novels[novelId]) return [404, { message: 'Novel not found' }];
  
  return [200, Object.values(labels[novelId] || {})];
});

mock.onPost(labelsPattern).reply((config) => {
  const match = matchUrl(config.url, labelsPattern);
  if (!match) return [404, { message: 'Cannot create label' }];
  
  const novelId = match[1];
  if (!novels[novelId]) return [404, { message: 'Novel not found' }];
  
  const payload = parseBody<{ name?: string }>(config.data);
  const labelId = nextId();
  const name = payload.name?.trim() || 'Новая сцена';
  
  const label: LabelResponse = {
    id: labelId,
    name,
    novelId,
    steps: [],
  };
  
  labels[novelId][labelId] = label;
  steps[novelId][labelId] = {};
  novels[novelId].labelIds.push(labelId);
  
  return [201, label];
});

mock.onGet(labelPattern).reply((config) => {
  const match = matchUrl(config.url, labelPattern);
  if (!match) return [404, { message: 'Label not found' }];
  
  const [, novelId, labelId] = match;
  const label = labels[novelId]?.[labelId];
  if (!label) return [404, { message: 'Label not found' }];
  
  return [200, label];
});

mock.onPatch(labelPattern).reply((config) => {
  const match = matchUrl(config.url, labelPattern);
  if (!match) return [404, { message: 'Label not found' }];
  
  const [, novelId, labelId] = match;
  const label = labels[novelId]?.[labelId];
  if (!label) return [404, { message: 'Label not found' }];
  
  const payload = parseBody<{ name?: string }>(config.data);
  if (payload.name) {
    label.name = payload.name;
  }
  
  return [200, label];
});

mock.onDelete(labelPattern).reply((config) => {
  const match = matchUrl(config.url, labelPattern);
  if (!match) return [404, { message: 'Label not found' }];
  
  const [, novelId, labelId] = match;
  if (!labels[novelId]?.[labelId]) return [404, { message: 'Label not found' }];
  
  delete labels[novelId][labelId];
  delete steps[novelId][labelId];
  novels[novelId].labelIds = novels[novelId].labelIds.filter((id) => id !== labelId);
  
  return [204];
});

// ============================================================================
// Characters Endpoints
// ============================================================================

const charactersPattern = /^\/novels\/([^/]+)\/characters$/;
const characterPattern = /^\/novels\/([^/]+)\/characters\/([^/]+)$/;

mock.onGet(charactersPattern).reply((config) => {
  const match = matchUrl(config.url, charactersPattern);
  if (!match) return [404, { message: 'Characters not found' }];
  
  const novelId = match[1];
  if (!novels[novelId]) return [404, { message: 'Novel not found' }];
  
  return [200, Object.values(characters[novelId] || {})];
});

mock.onPost(charactersPattern).reply((config) => {
  const match = matchUrl(config.url, charactersPattern);
  if (!match) return [404, { message: 'Cannot create character' }];
  
  const novelId = match[1];
  if (!novels[novelId]) return [404, { message: 'Novel not found' }];
  
  const payload = parseBody<{ name?: string; nameColor?: string; description?: string }>(config.data);
  const characterId = nextId();
  
  const character: CharacterResponse = {
    id: characterId,
    name: payload.name?.trim() || 'Новый персонаж',
    nameColor: payload.nameColor || '#ffffff',
    description: payload.description,
    characterStates: [],
  };
  
  characters[novelId][characterId] = character;
  novels[novelId].characterIds.push(characterId);
  
  return [201, character];
});

mock.onGet(characterPattern).reply((config) => {
  const match = matchUrl(config.url, characterPattern);
  if (!match) return [404, { message: 'Character not found' }];
  
  const [, novelId, characterId] = match;
  const character = characters[novelId]?.[characterId];
  if (!character) return [404, { message: 'Character not found' }];
  
  return [200, character];
});

mock.onPatch(characterPattern).reply((config) => {
  const match = matchUrl(config.url, characterPattern);
  if (!match) return [404, { message: 'Character not found' }];
  
  const [, novelId, characterId] = match;
  const character = characters[novelId]?.[characterId];
  if (!character) return [404, { message: 'Character not found' }];
  
  const payload = parseBody<{ name?: string; nameColor?: string; description?: string }>(config.data);
  
  if (payload.name) character.name = payload.name;
  if (payload.nameColor) character.nameColor = payload.nameColor;
  if (payload.description !== undefined) character.description = payload.description;
  
  return [200, character];
});

mock.onDelete(characterPattern).reply((config) => {
  const match = matchUrl(config.url, characterPattern);
  if (!match) return [404, { message: 'Character not found' }];
  
  const [, novelId, characterId] = match;
  if (!characters[novelId]?.[characterId]) return [404, { message: 'Character not found' }];
  
  delete characters[novelId][characterId];
  novels[novelId].characterIds = novels[novelId].characterIds.filter((id) => id !== characterId);
  
  return [204];
});

// ============================================================================
// Steps Endpoints
// ============================================================================

const stepsPattern = /^\/novels\/([^/]+)\/labels\/([^/]+)\/steps$/;
const stepPattern = /^\/novels\/([^/]+)\/labels\/([^/]+)\/steps\/([^/]+)$/;

mock.onGet(stepsPattern).reply((config) => {
  const match = matchUrl(config.url, stepsPattern);
  if (!match) return [404, { message: 'Steps not found' }];
  
  const [, novelId, labelId] = match;
  const labelSteps = steps[novelId]?.[labelId];
  if (!labelSteps) return [200, []];
  
  return [200, Object.values(labelSteps)];
});

mock.onPost(stepsPattern).reply((config) => {
  const match = matchUrl(config.url, stepsPattern);
  if (!match) return [404, { message: 'Cannot create step' }];
  
  const [, novelId, labelId] = match;
  if (!labels[novelId]?.[labelId]) return [404, { message: 'Label not found' }];
  
  const payload = parseBody<any>(config.data);
  const stepId = nextId();
  
  let step: StepResponse;
  
    switch (payload.type) {
      case 'show_replica':
        step = {
          type: 'show_replica',
          id: stepId,
          replica: {
            id: nextId(),
            speakerId: payload.speakerId || '',
            text: payload.text || '',
          },
          transition: { type: 'next_step' },
        };
        break;
      case 'show_menu':
        step = {
          type: 'show_menu',
          id: stepId,
          menu: {
            choices: (payload.choices || []).map((c: any) => ({
              text: c.text || '',
              transition: {
                type: 'choice',
                targetLabelId: c.targetLabelId || '',
              },
            })),
          },
          transition: { type: 'next_step' },
        };
        break;
      case 'jump':
        step = {
          type: 'jump',
          id: stepId,
          targetLabelId: payload.targetLabelId || '',
          transition: { type: 'jump', targetLabelId: payload.targetLabelId || '' },
        };
        break;
      case 'show_background':
        step = {
          type: 'show_background',
          id: stepId,
          imageId: payload.imageId || '',
          transform: payload.transform || defaultTransform(),
          transition: { type: 'next_step' },
        };
        break;
      case 'show_character':
        step = {
          type: 'show_character',
          id: stepId,
          characterId: payload.characterId || '',
          characterStateId: payload.characterStateId || '',
          transform: payload.transform || defaultTransform(),
          transition: { type: 'next_step' },
        };
        break;
      case 'hide_character':
        step = {
          type: 'hide_character',
          id: stepId,
          characterId: payload.characterId || '',
          transition: { type: 'next_step' },
        };
        break;
      default:
        step = {
          type: 'show_replica',
          id: stepId,
          replica: {
            id: nextId(),
            speakerId: '',
            text: 'Новый шаг',
          },
          transition: { type: 'next_step' },
        };
    }
  
  steps[novelId][labelId][stepId] = step;
  labels[novelId][labelId].steps.push(step);
  
  return [201, step];
});

mock.onGet(stepPattern).reply((config) => {
  const match = matchUrl(config.url, stepPattern);
  if (!match) return [404, { message: 'Step not found' }];
  
  const [, novelId, labelId, stepId] = match;
  const step = steps[novelId]?.[labelId]?.[stepId];
  if (!step) return [404, { message: 'Step not found' }];
  
  return [200, step];
});

mock.onPatch(stepPattern).reply((config) => {
  const match = matchUrl(config.url, stepPattern);
  if (!match) return [404, { message: 'Step not found' }];
  
  const [, novelId, labelId, stepId] = match;
  const step = steps[novelId]?.[labelId]?.[stepId];
  if (!step) return [404, { message: 'Step not found' }];
  
  const payload = parseBody<any>(config.data);
  
  // Update step based on type
  if (step.type === 'show_replica' && payload.text !== undefined) {
    step.replica.text = payload.text;
  }
  if (step.type === 'show_replica' && payload.speakerId !== undefined) {
    step.replica.speakerId = payload.speakerId;
  }
  if (step.type === 'jump' && payload.targetLabelId !== undefined) {
    step.targetLabelId = payload.targetLabelId;
  }
  if (step.type === 'show_menu' && payload.choices !== undefined) {
    step.menu.choices = payload.choices.map((c: any) => ({
      text: c.text || '',
      transition: {
        type: 'choice',
        targetLabelId: c.targetLabelId || '',
      },
    }));
  }
  
  return [200, step];
});

mock.onDelete(stepPattern).reply((config) => {
  const match = matchUrl(config.url, stepPattern);
  if (!match) return [404, { message: 'Step not found' }];
  
  const [, novelId, labelId, stepId] = match;
  if (!steps[novelId]?.[labelId]?.[stepId]) return [404, { message: 'Step not found' }];
  
  delete steps[novelId][labelId][stepId];
  labels[novelId][labelId].steps = labels[novelId][labelId].steps.filter((s) => s.id !== stepId);
  
  return [204];
});

// ============================================================================
// Images Endpoints
// ============================================================================

// Хранилище загруженных изображений
const uploadedImages: Record<string, { url: string; data: any }> = {};

const uploadUrlPattern = /^\/novels\/([^/]+)\/images\/upload-url$/;
const imagePattern = /^\/novels\/([^/]+)\/images\/([^/]+)$/;
const confirmUploadPattern = /^\/novels\/([^/]+)\/images\/([^/]+)\/confirm$/;

// Шаг 1: Получить URL для загрузки
mock.onPost(uploadUrlPattern).reply((config) => {
  const match = matchUrl(config.url, uploadUrlPattern);
  if (!match) return [404, { message: 'Novel not found' }];
  
  const [, novelId] = match;
  const payload = parseBody<any>(config.data);
  const imageId = nextId();
  
  // Создаем fake upload URL
  const uploadUrl = `https://fake-cloud-storage.com/upload/${imageId}`;
  const viewUrl = `https://placehold.co/1200x675?text=Image-${imageId}`;
  
  // Сохраняем информацию об изображении
  uploadedImages[imageId] = {
    url: viewUrl,
    data: payload
  };
  
  return [200, {
    imageId,
    uploadUrl,
    viewUrl
  }];
});

// Шаг 2: Загрузка файла на fake URL
mock.onPut(/https:\/\/fake-cloud-storage\.com\/upload\/(.+)/).reply((config) => {
  // Просто возвращаем успех, файл "загружен"
  return [200];
});

// Шаг 3: Подтверждение загрузки (опционально)
mock.onPost(confirmUploadPattern).reply((config) => {
  const match = matchUrl(config.url, confirmUploadPattern);
  if (!match) return [404, { message: 'Image not found' }];
  
  const [, novelId, imageId] = match;
  const imageData = uploadedImages[imageId];
  
  if (!imageData) return [404, { message: 'Image not found' }];
  
  return [200, {
    id: imageId,
    url: imageData.url,
    status: 'Active'
  }];
});

// Получить изображение по ID
mock.onGet(imagePattern).reply((config) => {
  const match = matchUrl(config.url, imagePattern);
  if (!match) return [404, { message: 'Image not found' }];
  
  const [, novelId, imageId] = match;
  const imageData = uploadedImages[imageId];
  
  if (!imageData) {
    // Возвращаем placeholder если изображение не найдено
    return [200, { 
      id: imageId,
      url: `https://placehold.co/1200x675?text=Image-${imageId}` 
    }];
  }
  
  return [200, { 
    id: imageId,
    url: imageData.url 
  }];
});

export default mock;
