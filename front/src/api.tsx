import axios from "axios";
import { useState, useEffect } from 'react';
const API_BASE ='http://localhost:5136/api';

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