import MockAdapter from 'axios-mock-adapter';
import api from '../api.tsx';

type Transform = {
    x: number;
    y: number;
    width: number;
    height: number;
    scale: number;
    rotation: number;
    zIndex: number;
};

type SceneState = {
    background: {
        imageId: string;
        transform: Transform;
    } | null;
    characters: Array<Record<string, unknown>>;
};

type StepType = 'background' | 'show' | 'hide' | 'replica' | 'jump' | 'choice';

type ChoiceItem = {
    id: string;
    name: string;
    text: string;
    targetLabelId: string;
};

type ChoiceMenuRequest = {
    id: string;
    name: string | null;
    text: string | null;
    choices: ChoiceItem[];
};

type StepRecord = {
    id: string;
    type: StepType;
    state: SceneState;
    imageId?: string;
    transform?: Transform;
    characterId?: string;
    characterStateId?: string;
    targetId?: string;
    text?: string;
    name?: string;
    menuRequest?: ChoiceMenuRequest;
};

type StepDraftChoice = {
    id?: string;
    name?: string | null;
    text?: string | null;
    targetLabelId?: string;
    transition?: {
        targetLabelId?: string;
    };
};

type StepDraft = Partial<StepRecord> & {
    choices?: StepDraftChoice[];
};

type NovelRecord = {
    id: string;
    title: string;
    description: string;
};

type LabelRecord = {
    id: string;
    name: string;
};

type CharacterStateRecord = {
    id: string;
    name: string;
    fileUrl?: string;
};

type CharacterRecord = {
    id: string;
    name: string;
    color: string;
    characterStates: CharacterStateRecord[];
};

type GraphEdge = {
    id: string;
    type: string;
    sourceLabelId: string;
    targetLabelId: string;
    choiceId?: string;
    text?: string;
};

type GraphData = {
    nodes: Array<{ id: string; name: string }>;
    edges: GraphEdge[];
};

type StepLocation = {
    novelId: string;
    labelId: string;
};

const defaultTransform = (): Transform => ({
    x: 0,
    y: 0,
    width: 100,
    height: 100,
    scale: 1,
    rotation: 0,
    zIndex: 1,
});

const emptyState = (): SceneState => ({
    background: null,
    characters: [],
});

let idCounter = 1000;
const nextId = (prefix: string): string => `${prefix}-${idCounter++}`;

const novelsById: Record<string, NovelRecord> = {};
const labelsByNovelId: Record<string, Record<string, LabelRecord>> = {};
const charactersByNovelId: Record<string, Record<string, CharacterRecord>> = {};
const stepsByNovelId: Record<string, Record<string, Record<string, StepRecord>>> = {};
const stepLocationById: Record<string, StepLocation> = {};
const graphByNovelId: Record<string, GraphData> = {};
const imageUrlById: Record<string, string> = {};

const asArray = <T,>(dictionary: Record<string, T>): T[] => Object.values(dictionary);
const clone = <T,>(value: T): T => structuredClone(value);

const parseBody = <T extends object>(data: unknown): Partial<T> => {
    if (!data) {
        return {};
    }

    if (typeof data === 'string') {
        try {
            return JSON.parse(data) as Partial<T>;
        } catch {
            return {};
        }
    }

    if (typeof FormData !== 'undefined' && data instanceof FormData) {
        const objectData: Record<string, unknown> = {};
        data.forEach((value, key) => {
            objectData[key] = value;
        });
        return objectData as Partial<T>;
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

const normalizeSceneState = (state?: SceneState): SceneState => ({
    background: state?.background ?? null,
    characters: Array.isArray(state?.characters) ? state.characters : [],
});

const normalizeStepType = (type: unknown): StepType => {
    if (type === 'show_background') return 'background';
    if (type === 'show_character') return 'show';
    if (type === 'hide_character') return 'hide';
    if (type === 'menu') return 'choice';
    if (type === 'background' || type === 'show' || type === 'hide' || type === 'replica' || type === 'jump' || type === 'choice') {
        return type;
    }
    return 'replica';
};

const normalizeChoice = (choice: StepDraftChoice): ChoiceItem => ({
    id: choice.id ?? nextId('choice'),
    name: choice.name ?? choice.text ?? 'Вариант',
    text: choice.text ?? '',
    targetLabelId: choice.targetLabelId ?? choice.transition?.targetLabelId ?? '',
});

const createStep = (draft: StepDraft, forceId?: string): StepRecord => {
    const type = normalizeStepType(draft.type);
    const step: StepRecord = {
        id: forceId ?? draft.id ?? nextId('step'),
        type,
        state: normalizeSceneState(draft.state),
    };

    switch (type) {
        case 'background':
            step.imageId = draft.imageId ?? '';
            step.transform = draft.transform ?? defaultTransform();
            return step;
        case 'show':
            step.characterId = draft.characterId ?? '';
            step.characterStateId = draft.characterStateId ?? '';
            step.transform = draft.transform ?? defaultTransform();
            return step;
        case 'hide':
            step.characterId = draft.characterId ?? '';
            return step;
        case 'jump':
            step.targetId = draft.targetId ?? '';
            return step;
        case 'choice': {
            const menuChoices = draft.menuRequest?.choices?.map(normalizeChoice) ?? [];
            const draftChoices = draft.choices?.map(normalizeChoice) ?? [];
            const choices = menuChoices.length > 0 ? menuChoices : draftChoices;

            step.name = draft.name ?? '';
            step.text = draft.text ?? '';
            step.menuRequest = {
                id: draft.menuRequest?.id ?? nextId('menu'),
                name: draft.menuRequest?.name ?? null,
                text: draft.menuRequest?.text ?? null,
                choices,
            };
            return step;
        }
        case 'replica':
        default:
            step.characterId = draft.characterId ?? '';
            step.text = draft.text ?? '';
            return step;
    }
};

const putStep = (novelId: string, labelId: string, step: StepRecord): void => {
    if (!stepsByNovelId[novelId]) {
        stepsByNovelId[novelId] = {};
    }
    if (!stepsByNovelId[novelId][labelId]) {
        stepsByNovelId[novelId][labelId] = {};
    }
    stepsByNovelId[novelId][labelId][step.id] = step;
    stepLocationById[step.id] = { novelId, labelId };
};

const seedLabel = (novelId: string, name: string): LabelRecord => {
    const label: LabelRecord = {
        id: nextId('label'),
        name,
    };
    labelsByNovelId[novelId][label.id] = label;
    return label;
};

const seedStep = (novelId: string, labelId: string, text: string): void => {
    const step = createStep({
        type: 'replica',
        text,
        state: emptyState(),
    });
    putStep(novelId, labelId, step);
};

const syncGraphNodes = (novelId: string): void => {
    const labels = asArray(labelsByNovelId[novelId] ?? {});
    const existingGraph = graphByNovelId[novelId] ?? { nodes: [], edges: [] };
    const validLabelIds = new Set(labels.map((label) => label.id));

    const edges = existingGraph.edges.filter((edge) =>
        validLabelIds.has(edge.sourceLabelId) && validLabelIds.has(edge.targetLabelId)
    );

    graphByNovelId[novelId] = {
        nodes: labels.map((label) => ({ id: label.id, name: label.name })),
        edges,
    };
};

const ensureNovelContext = (novelId: string, title?: string): void => {
    if (!novelsById[novelId]) {
        novelsById[novelId] = {
            id: novelId,
            title: title ?? `Новелла ${novelId}`,
            description: '',
        };
    }

    if (!labelsByNovelId[novelId]) {
        labelsByNovelId[novelId] = {};
        const intro = seedLabel(novelId, 'Вступление');
        const branch = seedLabel(novelId, 'Развилка');
        seedStep(novelId, intro.id, 'Начало сцены');
        seedStep(novelId, branch.id, 'Продолжение сцены');
    }

    if (!charactersByNovelId[novelId]) {
        const annaId = nextId('char');
        const borisId = nextId('char');

        charactersByNovelId[novelId] = {
            [annaId]: {
                id: annaId,
                name: 'Анна',
                color: '#3b82f6',
                characterStates: [
                    { id: nextId('state'), name: 'Радость', fileUrl: 'https://placehold.co/100x100?text=joy' },
                    { id: nextId('state'), name: 'Грусть', fileUrl: 'https://placehold.co/100x100?text=sad' },
                ],
            },
            [borisId]: {
                id: borisId,
                name: 'Борис',
                color: '#10b981',
                characterStates: [
                    { id: nextId('state'), name: 'Спокойствие', fileUrl: 'https://placehold.co/100x100?text=calm' },
                ],
            },
        };
    }

    if (!stepsByNovelId[novelId]) {
        stepsByNovelId[novelId] = {};
    }

    for (const label of asArray(labelsByNovelId[novelId])) {
        if (!stepsByNovelId[novelId][label.id]) {
            stepsByNovelId[novelId][label.id] = {};
            seedStep(novelId, label.id, `Сцена "${label.name}"`);
        }
    }

    if (!graphByNovelId[novelId]) {
        graphByNovelId[novelId] = { nodes: [], edges: [] };
    }

    syncGraphNodes(novelId);
};

const deleteLabelSteps = (novelId: string, labelId: string): void => {
    const steps = stepsByNovelId[novelId]?.[labelId];
    if (!steps) return;

    for (const stepId of Object.keys(steps)) {
        delete stepLocationById[stepId];
    }
    delete stepsByNovelId[novelId][labelId];
};

const findStepLocation = (stepId: string): StepLocation | null => {
    const saved = stepLocationById[stepId];
    if (saved) {
        return saved;
    }

    for (const [novelId, stepsByLabel] of Object.entries(stepsByNovelId)) {
        for (const [labelId, steps] of Object.entries(stepsByLabel)) {
            if (steps[stepId]) {
                const location = { novelId, labelId };
                stepLocationById[stepId] = location;
                return location;
            }
        }
    }

    return null;
};

const removeStepById = (stepId: string): boolean => {
    const location = findStepLocation(stepId);
    if (!location) {
        return false;
    }

    const steps = stepsByNovelId[location.novelId]?.[location.labelId];
    if (!steps?.[stepId]) {
        return false;
    }

    delete steps[stepId];
    delete stepLocationById[stepId];
    return true;
};

ensureNovelContext('0', 'Демо-новелла');
ensureNovelContext('1', 'Учебная новелла');

const mock = new MockAdapter(api, {
    delayResponse: 250,
    onNoMatch: 'passthrough',
});

const novelsCollectionPattern = /^\/?(?:api\/)?novels$/;
const novelItemPattern = /^\/?(?:api\/)?novels\/([^/]+)$/;
const labelsCollectionPattern = /^\/?(?:api\/)?novels\/([^/]+)\/labels$/;
const labelItemPattern = /^\/?(?:api\/)?novels\/([^/]+)\/labels\/([^/]+)$/;
const stepsCollectionPattern = /^\/?(?:api\/)?novels\/([^/]+)\/labels\/([^/]+)\/steps$/;
const stepItemPattern = /^\/?(?:api\/)?steps\/([^/]+)$/;
const charactersCollectionPattern = /^\/?(?:api\/)?novels\/([^/]+)\/characters$/;
const characterItemPattern = /^\/?(?:api\/)?novels\/([^/]+)\/characters\/([^/]+)$/;
const statesCollectionPattern = /^\/?(?:api\/)?novels\/([^/]+)\/characters\/([^/]+)\/states$/;
const emotionsCollectionPattern = /^\/?(?:api\/)?novels\/([^/]+)\/characters\/([^/]+)\/emotions$/;
const graphPattern = /^\/?(?:api\/)?novels\/([^/]+)\/graph$/;
const uploadUrlPattern = /^\/?(?:api\/)?images\/upload-url$/;
const imagePattern = /^\/?(?:api\/)?images\/([^/]+)$/;

mock.onGet(novelsCollectionPattern).reply(() => {
    return [200, clone(asArray(novelsById))];
});

mock.onPost(novelsCollectionPattern).reply((config) => {
    const payload = parseBody<{ title?: string; name?: string; description?: string }>(config.data);
    const novelId = nextId('novel');
    const title = payload.title?.trim() || payload.name?.trim() || `Новая новелла ${novelId}`;

    ensureNovelContext(novelId, title);
    novelsById[novelId].description = payload.description ?? '';

    return [201, clone(novelsById[novelId])];
});

mock.onGet(novelItemPattern).reply((config) => {
    const match = matchUrl(config.url, novelItemPattern);
    if (!match) return [404, { message: 'Novel not found' }];

    const novelId = match[1];
    ensureNovelContext(novelId);

    return [200, clone(novelsById[novelId])];
});

mock.onPatch(novelItemPattern).reply((config) => {
    const match = matchUrl(config.url, novelItemPattern);
    if (!match) return [404, { message: 'Novel not found' }];

    const novelId = match[1];
    ensureNovelContext(novelId);

    const payload = parseBody<Partial<NovelRecord>>(config.data);
    novelsById[novelId] = {
        ...novelsById[novelId],
        title: payload.title?.trim() || novelsById[novelId].title,
        description: payload.description ?? novelsById[novelId].description,
    };

    return [200, clone(novelsById[novelId])];
});

mock.onGet(labelsCollectionPattern).reply((config) => {
    const match = matchUrl(config.url, labelsCollectionPattern);
    if (!match) return [404, { message: 'Labels not found' }];

    const novelId = match[1];
    ensureNovelContext(novelId);

    return [200, clone(asArray(labelsByNovelId[novelId]))];
});

mock.onPost(labelsCollectionPattern).reply((config) => {
    const match = matchUrl(config.url, labelsCollectionPattern);
    if (!match) return [404, { message: 'Cannot create label' }];

    const novelId = match[1];
    ensureNovelContext(novelId);

    const payload = parseBody<{ name?: string }>(config.data);
    const labelName = payload.name?.trim() || 'Новая сцена';
    const label = {
        id: nextId('label'),
        name: labelName,
    };

    labelsByNovelId[novelId][label.id] = label;
    seedStep(novelId, label.id, `Сцена "${labelName}"`);
    syncGraphNodes(novelId);

    return [201, clone(label)];
});

mock.onPatch(labelItemPattern).reply((config) => {
    const match = matchUrl(config.url, labelItemPattern);
    if (!match) return [404, { message: 'Label not found' }];

    const novelId = match[1];
    const labelId = match[2];
    ensureNovelContext(novelId);

    const label = labelsByNovelId[novelId]?.[labelId];
    if (!label) return [404, { message: 'Label not found' }];

    const payload = parseBody<{ name?: string }>(config.data);
    label.name = payload.name?.trim() || label.name;
    syncGraphNodes(novelId);

    return [200, clone(label)];
});

mock.onDelete(labelItemPattern).reply((config) => {
    const match = matchUrl(config.url, labelItemPattern);
    if (!match) return [404, { message: 'Label not found' }];

    const novelId = match[1];
    const labelId = match[2];
    ensureNovelContext(novelId);

    if (!labelsByNovelId[novelId][labelId]) {
        return [404, { message: 'Label not found' }];
    }

    delete labelsByNovelId[novelId][labelId];
    deleteLabelSteps(novelId, labelId);
    syncGraphNodes(novelId);

    return [200, { success: true }];
});

mock.onGet(stepsCollectionPattern).reply((config) => {
    const match = matchUrl(config.url, stepsCollectionPattern);
    if (!match) return [404, { message: 'Steps not found' }];

    const novelId = match[1];
    const labelId = match[2];
    ensureNovelContext(novelId);

    const steps = stepsByNovelId[novelId]?.[labelId];
    if (!steps) return [200, []];

    return [200, clone(asArray(steps))];
});

mock.onPost(stepsCollectionPattern).reply((config) => {
    const match = matchUrl(config.url, stepsCollectionPattern);
    if (!match) return [404, { message: 'Cannot create step' }];

    const novelId = match[1];
    const labelId = match[2];
    ensureNovelContext(novelId);

    const payload = parseBody<StepDraft>(config.data);
    const step = createStep(payload);
    putStep(novelId, labelId, step);

    return [201, clone(step)];
});

mock.onGet(stepItemPattern).reply((config) => {
    const match = matchUrl(config.url, stepItemPattern);
    if (!match) return [404, { message: 'Step not found' }];

    const stepId = match[1];
    const location = findStepLocation(stepId);
    if (!location) return [404, { message: 'Step not found' }];

    const step = stepsByNovelId[location.novelId]?.[location.labelId]?.[stepId];
    if (!step) return [404, { message: 'Step not found' }];

    return [200, clone(step)];
});

mock.onPatch(stepItemPattern).reply((config) => {
    const match = matchUrl(config.url, stepItemPattern);
    if (!match) return [404, { message: 'Step not found' }];

    const stepId = match[1];
    const location = findStepLocation(stepId);
    if (!location) return [404, { message: 'Step not found' }];

    const prev = stepsByNovelId[location.novelId]?.[location.labelId]?.[stepId];
    if (!prev) return [404, { message: 'Step not found' }];

    const payload = parseBody<StepDraft>(config.data);
    const step = createStep({ ...prev, ...payload }, stepId);
    putStep(location.novelId, location.labelId, step);

    return [200, clone(step)];
});

mock.onDelete(stepItemPattern).reply((config) => {
    const match = matchUrl(config.url, stepItemPattern);
    if (!match) return [404, { message: 'Step not found' }];

    const stepId = match[1];
    const removed = removeStepById(stepId);
    if (!removed) return [404, { message: 'Step not found' }];

    return [200, { success: true }];
});

mock.onGet(charactersCollectionPattern).reply((config) => {
    const match = matchUrl(config.url, charactersCollectionPattern);
    if (!match) return [404, { message: 'Characters not found' }];

    const novelId = match[1];
    ensureNovelContext(novelId);

    return [200, clone(asArray(charactersByNovelId[novelId]))];
});

mock.onPost(charactersCollectionPattern).reply((config) => {
    const match = matchUrl(config.url, charactersCollectionPattern);
    if (!match) return [404, { message: 'Cannot create character' }];

    const novelId = match[1];
    ensureNovelContext(novelId);

    const payload = parseBody<{ name?: string; color?: string; nameColor?: string }>(config.data);
    const character: CharacterRecord = {
        id: nextId('char'),
        name: payload.name?.trim() || 'Новый персонаж',
        color: payload.color || payload.nameColor || '#ffffff',
        characterStates: [],
    };

    charactersByNovelId[novelId][character.id] = character;
    return [201, clone(character)];
});

mock.onGet(characterItemPattern).reply((config) => {
    const match = matchUrl(config.url, characterItemPattern);
    if (!match) return [404, { message: 'Character not found' }];

    const novelId = match[1];
    const characterId = match[2];
    ensureNovelContext(novelId);

    const character = charactersByNovelId[novelId]?.[characterId];
    if (!character) return [404, { message: 'Character not found' }];

    return [200, clone(character)];
});

mock.onPatch(characterItemPattern).reply((config) => {
    const match = matchUrl(config.url, characterItemPattern);
    if (!match) return [404, { message: 'Character not found' }];

    const novelId = match[1];
    const characterId = match[2];
    ensureNovelContext(novelId);

    const character = charactersByNovelId[novelId]?.[characterId];
    if (!character) return [404, { message: 'Character not found' }];

    const payload = parseBody<{
        name?: string;
        color?: string;
        emotions?: Array<{ id?: string; name?: string; fileUrl?: string }>;
        characterStates?: Array<{ id?: string; name?: string; fileUrl?: string }>;
    }>(config.data);

    if (payload.name?.trim()) {
        character.name = payload.name.trim();
    }
    if (payload.color) {
        character.color = payload.color;
    }

    const nextStates = payload.characterStates ?? payload.emotions;
    if (Array.isArray(nextStates)) {
        character.characterStates = nextStates.map((state) => ({
            id: state.id ?? nextId('state'),
            name: state.name?.trim() || 'Состояние',
            fileUrl: state.fileUrl ?? `https://placehold.co/100x100?text=${encodeURIComponent(state.name ?? 'state')}`,
        }));
    }

    return [200, clone(character)];
});

mock.onDelete(characterItemPattern).reply((config) => {
    const match = matchUrl(config.url, characterItemPattern);
    if (!match) return [404, { message: 'Character not found' }];

    const novelId = match[1];
    const characterId = match[2];
    ensureNovelContext(novelId);

    if (!charactersByNovelId[novelId][characterId]) {
        return [404, { message: 'Character not found' }];
    }

    delete charactersByNovelId[novelId][characterId];
    return [200, { success: true }];
});

const getStatesResponse = (url?: string): [number, CharacterStateRecord[] | { message: string }] => {
    const match = matchUrl(url, statesCollectionPattern) ?? matchUrl(url, emotionsCollectionPattern);
    if (!match) return [404, { message: 'States not found' }];

    const novelId = match[1];
    const characterId = match[2];
    ensureNovelContext(novelId);

    const character = charactersByNovelId[novelId]?.[characterId];
    if (!character) return [404, { message: 'Character not found' }];

    return [200, clone(character.characterStates)];
};

mock.onGet(statesCollectionPattern).reply((config) => getStatesResponse(config.url));
mock.onGet(emotionsCollectionPattern).reply((config) => getStatesResponse(config.url));

mock.onPost(statesCollectionPattern).reply((config) => {
    const match = matchUrl(config.url, statesCollectionPattern);
    if (!match) return [404, { message: 'Cannot create state' }];

    const novelId = match[1];
    const characterId = match[2];
    ensureNovelContext(novelId);

    const character = charactersByNovelId[novelId]?.[characterId];
    if (!character) return [404, { message: 'Character not found' }];

    const payload = parseBody<{ name?: string; file?: File }>(config.data);
    const fileName = payload.file?.name ? payload.file.name.split('.')[0] : '';
    const stateName = payload.name?.trim() || fileName || `Состояние ${character.characterStates.length + 1}`;

    const state: CharacterStateRecord = {
        id: nextId('state'),
        name: stateName,
        fileUrl: `https://placehold.co/100x100?text=${encodeURIComponent(stateName)}`,
    };

    character.characterStates.push(state);
    return [201, clone(state)];
});

mock.onGet(graphPattern).reply((config) => {
    const match = matchUrl(config.url, graphPattern);
    if (!match) return [404, { message: 'Graph not found' }];

    const novelId = match[1];
    ensureNovelContext(novelId);

    return [200, clone(graphByNovelId[novelId])];
});

mock.onPatch(graphPattern).reply((config) => {
    const match = matchUrl(config.url, graphPattern);
    if (!match) return [404, { message: 'Graph not found' }];

    const novelId = match[1];
    ensureNovelContext(novelId);

    const payload = parseBody<Partial<GraphData>>(config.data);
    graphByNovelId[novelId] = {
        nodes: Array.isArray(payload.nodes) ? payload.nodes : graphByNovelId[novelId].nodes,
        edges: Array.isArray(payload.edges) ? payload.edges : graphByNovelId[novelId].edges,
    };
    syncGraphNodes(novelId);

    return [200, clone(graphByNovelId[novelId])];
});

mock.onPost(uploadUrlPattern).reply(() => {
    const imageId = nextId('image');
    imageUrlById[imageId] = `https://placehold.co/1200x675?text=${encodeURIComponent(imageId)}`;

    return [200, {
        imageId,
        uploadUrl: 'https://fake-cloud-storage.com/upload',
    }];
});

mock.onPut('https://fake-cloud-storage.com/upload').reply(200);

mock.onGet(imagePattern).reply((config) => {
    const match = matchUrl(config.url, imagePattern);
    if (!match) return [404, { message: 'Image not found' }];

    const imageId = match[1];
    const url = imageUrlById[imageId] ?? `https://placehold.co/1200x675?text=${encodeURIComponent(imageId)}`;
    imageUrlById[imageId] = url;

    return [200, { url }];
});

export default mock;
