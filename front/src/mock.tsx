import MockAdapter from 'axios-mock-adapter';
import api from "./api.tsx";
// import cat from "./assets/cat.png";

const mock = new MockAdapter(api, {
    delayResponse: 600,
    onNoMatch: 'throwException'
});

const catImage = {
    url: ' ',
    transform:{
        width:53,
        x:27,
        scale:0.4,
    }
}
// 1. Расширенное хранилище данных
let mockCharacters = [
    { id: '1', name: 'Анна', color: '#3b82f6', characterStates: [{id:"6", name:'Грусть'}, {id:"98", name:'Радость'}] },
    { id: '6', name: 'Мария', color: '#ef4444', characterStates: [{id:"96", name:'Грусть'}, {id:"97", name:'Радость'}] },
    { id: '5', name: 'Борис', color: '#10b981', characterStates: [{id:"6", name:'Грусть'}, {id:"94", name:'Радость'}] },
];

let mockSteps = [
    { id: '3', type: 'background', imageId: '2' },
    { id: '1', type: 'show', characterId: '5' },
    { id: '6', type: 'show', characterId: '6' },
    { id: '4', type: 'hide', characterId: '5' },
    { id: '40', type: 'choice' }
];

let mockSteps2 = [
    { id: '30', type: 'background', imageId: '2' },
    { id: '10', type: 'show', characterId: '5' },
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
mock.onGet('steps/3').reply(200, { id: '3', type: 'background', imageId: '2' });
mock.onGet('novels/0/characters').reply(200, mockCharacters);
// mock.onGet(/\/images\/.+$/).reply(200, {url:"https://img.freepik.com/premium-psd/png-adorable-gray-white-cat_53876-483460.jpg?semt=ais_hybrid&w=740&q=80"});
mock.onGet('/novels/0/labels').reply(200, mockLabels);
mock.onGet('/novels/0/labels/1/steps').reply(200, mockSteps);
mock.onGet('/novels/0/labels/2/steps').reply(200, mockSteps2);
mock.onGet('/novels/0/labels/3/steps').reply(200, mockSteps);

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

mock.onPost('/novels/0/labels/1/steps').reply((config) => {
    const body = JSON.parse(config.data);
    const newStep = {
        id: String(Date.now()),
        type: 'show',
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

mock.onPost(/\/characters\/\d+$/).reply((config) => {
    const body = JSON.parse(config.data);
    const response = {
        imageId: '1',
        uploadUrl: '',
        viewUrl:'src/assets/upload.jpg'
    }
    return [201, response];
});

mock.onPost('images/upload-url').reply(200, {
    imageId: "mock_image_123",
    uploadUrl: "https://fake-cloud-storage.com/upload"
});

mock.onPut("https://fake-cloud-storage.com/upload").reply(200);

mock.onGet("/images/mock_image_123").reply(200, {
    url: "src/assets/upload.jpg"
});
mock.onGet("/images/2").reply(200, {
    url: "https://png.pngtree.com/thumb_back/fh260/background/20240622/pngtree-the-sun-at-sundown-sunset-sky-background-image_15806374.jpg"
});

mock.onGet("/images/5").reply(200, {
    url: "https://img.freepik.com/premium-psd/png-adorable-gray-white-cat_53876-483460.jpg?semt=ais_hybrid&w=740&q=80"
});

mock.onGet("/images/6").reply(200, {
    url: "https://w7.pngwing.com/pngs/93/689/png-transparent-pet-sitting-feral-cat-dog-cat-mammal-cat-like-mammal-animals-thumbnail.png"
});

export default mock;