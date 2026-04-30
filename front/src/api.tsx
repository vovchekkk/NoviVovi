import axios from "axios";
import { useState, useEffect } from 'react';

const getApiBase = () => {
    // Локальная разработка
    if (import.meta.env.DEV) {  // Для Vite
        return 'http://localhost:8080/api';
    }
    // Продакшен (API Gateway)
    return 'https://api.novivovi.ru/api';
};

const API_BASE = getApiBase();

export const api = axios.create({
    baseURL: API_BASE,
    headers: { 'Content-Type': 'application/json' },
});
interface apiDataProps {
    url: string;
}
export function useApiData({url}: apiDataProps){
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        const controller = new AbortController();

        const fetchData = async () => {
            setLoading(true);
            setError(null);
            try {
                const res = await api.get(url, { signal: controller.signal });
                setData(res.data);
            } catch (err) {
                if (!axios.isCancel(err)) {
                    setError(err.message || 'Произошла ошибка');
                }
            } finally {
                setLoading(false);
            }
        };

        fetchData();

        return () => controller.abort();
    }, [url]);

    return { data, loading, error };
}
export default api