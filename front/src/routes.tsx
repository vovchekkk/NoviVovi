import { createBrowserRouter } from 'react-router-dom'

import Home from './pages/Home'
import Novels from './pages/Novels'
import Editor from './pages/Editor'
import Assets from "./pages/Assets.tsx";
import Scenes from "./pages/Scenes.tsx";

export const router = createBrowserRouter([
    {
        path: '/',
        element: <Home />,
    },
    {
        path: '/novels',
        element: <Novels />,
    },
    {
        path: '/editor',
        element: <Editor />,
    },
    {
        path: '/editor/assets',
        element: <Assets />,
    },
    {
        path: '/editor/scenes',
        element: <Scenes />,
    },
    {
        path: '/editor/:novelId',
        element: <Editor />,
    },
    {
        path: '*',           // 404 страница
        element: <div>Страница не найдена</div>,
    },
])