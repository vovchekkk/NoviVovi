const LAST_NOVEL_ID_COOKIE = 'novivovi_last_novel_id';
const COOKIE_LIFETIME_SECONDS = 60 * 60 * 24 * 30;

export const setLastNovelId = (novelId: string): void => {
    if (!novelId) {
        return;
    }

    document.cookie = `${LAST_NOVEL_ID_COOKIE}=${encodeURIComponent(novelId)}; path=/; max-age=${COOKIE_LIFETIME_SECONDS}; samesite=lax`;
};

export const getLastNovelId = (): string | null => {
    const cookieItem = document.cookie
        .split('; ')
        .find((item) => item.startsWith(`${LAST_NOVEL_ID_COOKIE}=`));

    if (!cookieItem) {
        return null;
    }

    const encodedValue = cookieItem.split('=')[1];
    return encodedValue ? decodeURIComponent(encodedValue) : null;
};
