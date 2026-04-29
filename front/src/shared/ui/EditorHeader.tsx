import {css} from '../../../styled-system/css'
import {Link, useParams} from 'react-router-dom'
import { getLastNovelId } from '../lib/novelSession.ts';
import {novelsApi} from "../api/client.ts";
import {useState} from "react";

interface EditorHeaderProps {
    active?: 'scenes' | 'editor' | 'assets'
    novelId:string;
}
export default function EditorHeader({ active = 'editor' }: EditorHeaderProps) {
    const { novelId } = useParams<{ novelId: string }>();
    const currentNovelId = novelId ?? getLastNovelId() ?? '0';
    const [isExporting, setIsExporting] = useState(false);
    const handleExport = async () => {
        if (!currentNovelId || currentNovelId === '0') return;

        try {
            setIsExporting(true);
            const response = await novelsApi.exportToRenPy(currentNovelId);
            const blob = new Blob([response.data], { type: 'application/zip' });
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `novel-project-${currentNovelId}.zip`);
            document.body.appendChild(link);
            link.click();

            if (link.parentNode) {
                link.parentNode.removeChild(link);
            }
            window.URL.revokeObjectURL(url);

        } catch (error) {
            console.error('Ошибка экспорта:', error);
            alert('Не удалось сгенерировать Ren\'Py проект. Проверьте консоль.');
        } finally {
            setIsExporting(false);
        }
    };

    return (
        <header className={css({
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            position: 'relative',
            width: '100%',
        })}>
            <nav className={css({
                display: 'flex',
                gap: '40px',
                fontSize: '20px',
                fontWeight: '500'
            })}>
                <Link
                    to={`/editor/${currentNovelId}`}
                    className={css({
                        color: 'black',
                        position: 'relative',
                        px: '30px',
                        py: '6px',
                        zIndex: '1',
                        backgroundColor: '#DFC6D1',
                        _before: active === 'editor' ? {
                            content: '""',
                            position: 'absolute',
                            inset: 0,
                            zIndex: -1,
                            backgroundColor: 'white',
                        } : {}
                    })}
                >
                    Редактор
                </Link>

                <Link
                    to={`/editor/${currentNovelId}/scenes`}
                    className={css({
                        color: 'black',
                        position: 'relative',
                        px: '30px',
                        py: '6px',
                        zIndex: '1',
                        backgroundColor: '#DFC6D1',
                        _before: active === 'scenes' ? {
                            content: '""',
                            position: 'absolute',
                            inset: 0,
                            zIndex: -1,
                            backgroundColor: 'white',
                        } : {}
                    })}
                >
                    Сцены
                </Link>

                <Link
                    to={`/editor/assets/${currentNovelId}`}
                    className={css({
                        color: 'black',
                        position: 'relative',
                        px: '30px',
                        py: '6px',
                        zIndex: '1',
                        backgroundColor: '#DFC6D1',
                        _before: active === 'assets' ? {
                            content: '""',
                            position: 'absolute',
                            inset: 0,
                            zIndex: -1,
                            backgroundColor: 'white',
                        } : {}
                    })}
                >
                    Ассеты
                </Link>
            </nav>

            <button
                onClick={handleExport}
                disabled={isExporting}
                className={css({
                    position: 'absolute',
                    right: '5px',
                    top: '30%',
                    transform: 'translateY(-50%)',

                    backgroundColor: isExporting ? '#ccc' : 'white',
                    color: 'black',
                    px: '25px',
                    py: '8px',
                    borderRadius: '12px',
                    fontSize: '16px',
                    fontWeight: 'bold',
                    cursor: isExporting ? 'not-allowed' : 'pointer',
                    transition: 'all 0.2s',
                    _hover: !isExporting ? {
                        backgroundColor: '#DFC6D1',
                        boxShadow: '0 0 20px rgba(112, 86, 97, 0.4)',
                    } : {}
                })}
            >
                {isExporting ? 'Экспорт...' : 'Экспорт Ren\'Py'}
            </button>
        </header>
    )
}