import MockAdapter from 'axios-mock-adapter';
import api from "./api.tsx";

console.log('🚀 Axios Mock Adapter (Emotions Edition) загружен');

const mock = new MockAdapter(api, {
    delayResponse: 600,
    onNoMatch: 'throwException'
});

// 1. Расширенное хранилище данных
let mockCharacters = [
    { id: '1', name: 'Анна', color: '#3b82f6' },
    { id: '2', name: 'Мария', color: '#ef4444' },
    { id: '3', name: 'Борис', color: '#10b981' },
];

let mockSteps = [
    { id: '1', type: 'hide', characterId: '2' },
    { id: '3', type: 'background', imageId: '2' },
    { id: '2', type: 'background', imageId: '2' },
];

// Храним эмоции отдельно, привязывая к ID персонажа
let mockEmotionsStore = {
    '1': [
        { id: 'e1', name: 'Радость', fileUrl: 'https://placehold.co/100' },
        { id: 'e2', name: 'Грусть', fileUrl: 'https://placehold.co/100' },
    ],
    '2': [
        { id: 'e3', name: 'Злость', fileUrl: 'https://placehold.co/100' }
    ],
    '3': []
};

let mockLabels = [
    { id:'1', name:'Label1'},
    { id:'2', name:'Label2'},
    { id:'3', name:'Label3'},
]

// === ОБРАБОТЧИКИ ===

// Получение списка персонажей
mock.onGet('/characters').reply(200, mockCharacters);
mock.onGet('/novels/0/labels').reply(200, mockLabels);
mock.onGet('/novels/0/labels/0/steps').reply(200, mockSteps);

// Получение персонажа по ID
mock.onGet(/\/characters\/\d+$/).reply((config) => {
    const id = config.url.split('/').pop();
    const character = mockCharacters.find(c => c.id === id);
    return character ? [200, character] : [404];
});

// Получение эмоций конкретного персонажа
mock.onGet(/\/characters\/.+\/emotions/).reply((config) => {
    const charId = config.url.split('/')[2];
    console.log(`✅ Загрузка эмоций для персонажа ${charId}`);
    return [200, mockEmotionsStore[charId] || []];
});

// ГЛАВНОЕ: Сохранение персонажа ВМЕСТЕ с эмоциями (PATCH)
mock.onPatch(/\/characters\/.+/).reply((config) => {
    const charId = config.url.split('/').pop();
    const updatedData = JSON.parse(config.data); // Здесь лежат {name, color, emotions}

    console.log('📦 Мок получил данные для сохранения:', updatedData);

    // 1. Обновляем данные персонажа
    mockCharacters = mockCharacters.map(c =>
        c.id === charId
            ? { ...c, name: updatedData.name, color: updatedData.color }
            : c
    );

    // 2. Обновляем список эмоций в нашем хранилище
    // Если в объекте пришли эмоции, сохраняем их (присваивая ID новым, если их нет)
    if (updatedData.emotions) {
        mockEmotionsStore[charId] = updatedData.emotions.map(e => ({
            ...e,
            id: e.id || 'e_new_' + Date.now() + Math.random()
        }));
    }

    return [200, { message: "Данные успешно сохранены" }];
});

mock.onPatch(/\/labels\/.+/).reply((config) => {
    const charId = config.url.split('/').pop();
    const updatedData = JSON.parse(config.data);

    console.log('📦 Мок получил данные для сохранения:', updatedData);

    mockLabels = mockLabels.map(c =>
        c.id === charId
            ? { ...c, name: updatedData.name}
            : c
    );

    return [200, { message: "Данные успешно сохранены" }];
});

// Создание нового персонажа
mock.onPost('/characters').reply((config) => {
    const body = JSON.parse(config.data);
    const newChar = {
        id: String(Date.now()),
        name: body.name || 'Новый персонаж',
        color: '#ffffff'
    };

    mockCharacters.push(newChar);
    mockEmotionsStore[newChar.id] = [];

    return [201, newChar];
});

mock.onPost('/novels/0/labels/0/steps').reply((config) => {
    const body = JSON.parse(config.data);
    const newStep = {
        id: String(Date.now()),
        type: 'hide',
    };

    mockSteps.push(newStep);

    return [201, newStep];
});
mock.onDelete(/\/labels\/.+/).reply((config) => {
    const urlParts = config.url.split('/');
    const id = urlParts[urlParts.length - 1];

    const charIndex = mockLabels.findIndex(char => char.id === id);
    if (charIndex !== -1) {
        mockLabels.splice(charIndex, 1);

        console.log(`Персонаж с ID ${id} удален из моков`);
        return [200, { success: true }];
    } else {
        return [404, { message: 'Персонаж не найден' }];
    }
});

mock.onPost('/novels/0/labels').reply((config) => {
    const body = JSON.parse(config.data);
    const newChar = {
        id: String(Date.now()),
        name: body.name || 'Новый персонаж',
    };

    return [201, newChar];
});

export default mock;