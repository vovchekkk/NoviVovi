import { createBrowserRouter } from 'react-router-dom'

import Home from './pages/Home'
import Novels from './pages/Novels'
import Editor from './pages/Editor'

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
        path: '*',           // 404 страница
        element: <div>Страница не найдена</div>,
    },
])