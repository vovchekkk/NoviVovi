import {css} from '../../../styled-system/css'
import {useEffect, useState} from "react";

interface EmotionBlockProps {
    index: number;
    register: any;
    setValue: any;
    watch: any;
    onRemove: () => void;
    errors?: any;
    // onChange: (event: React.ChangeEvent<HTMLInputElement>) => void
}

export default function EmotionBlock({index, register, setValue, watch, onRemove, errors}: EmotionBlockProps) {
    const imageFile = watch(`emotions.${index}.imageFile`);
    const fileUrl = watch(`emotions.${index}.fileUrl`);

    const [preview, setPreview] = useState<string | null>(null);

    useEffect(() => {
        if (imageFile && imageFile instanceof File) {
            const objectUrl = URL.createObjectURL(imageFile);
            setPreview(objectUrl);

            return () => URL.revokeObjectURL(objectUrl);
        }

        if (fileUrl) {
            setPreview(fileUrl);
            return;
        }

        setPreview(null);
    }, [imageFile, fileUrl]);
    return (
        <div className={css({
            display: 'flex',
            flexDirection: 'column',
            backgroundColor: '#DFC6D1',
            padding: '15px',
            borderRadius: '12px',
            gap: '10px',
            position: 'relative',
            border: '1px solid rgba(0,0,0,0.1)'
        })}>
            <button
                type="button"
                onClick={onRemove}
                className={css({
                    position: 'absolute',
                    top: '5px',
                    right: '10px',
                    cursor: 'pointer',
                    color: '#775D68',
                    _hover: {color: 'red'}
                })}
            >
                ✕
            </button>

            <div className={css({display: 'flex', gap: '20px'})}>
                <div className={css({
                    width: '100px',
                    height: '100px',
                    backgroundColor: 'white',
                    borderRadius: '8px',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    overflow: 'hidden',
                    border: '1px solid #ccc',
                    flexShrink: 0
                })}>
                    {preview ? (
                        <img
                            src={preview}
                            alt="Preview"
                            className={css({width: '100%', height: '100%', objectFit: 'cover'})}
                        />
                    ) : (
                        <span className={css({fontSize: '10px', color: '#999', textAlign: 'center'})}>
                            Нет фото
                        </span>
                    )}
                </div>
                <div className={css({display: 'flex', flexDirection: 'column'})}>
                    <div className={css({display: 'flex', flexDirection: 'column', gap: '4px'})}>
                        <label className={css({fontSize: '14px', fontWeight: 'bold'})}>Название</label>
                        <div className={css({display: 'flex', width: '100%'})}>
                            <input
                                {...register(`emotions.${index}.name`)}
                                placeholder="Напр: Радость"
                                className={css({
                                    backgroundColor: 'white',
                                    width: '50%',
                                    p: '8px',
                                    borderRadius: '8px',
                                    border: errors?.name ? '1px solid red' : '1px solid #ccc'
                                })}
                            />
                        </div>
                        {errors?.name &&
                            <span className={css({color: 'red', fontSize: '12px'})}>{errors.name.message}</span>}
                    </div>

                    <div className={css({display: 'flex', flexDirection: 'column', gap: '4px'})}>
                        <label className={css({fontSize: '14px', fontWeight: 'bold'})}>Картинка эмоции</label>
                        <label className={css({display: 'flex', width: '100%', alignItems: 'center', gap: '10px'})}>
                            <input
                                type="file"
                                accept="image/*"
                                className={css({display: 'none'})}
                                onChange={(e) => {
                                    const file = e.target.files?.[0];
                                    if (file) {
                                        setValue(`emotions.${index}.imageFile`, file);
                                    }
                                }}
                            />
                            <div className={css({
                                px: '15px',
                                py: '8px',
                                bg: '#775D68',
                                color: 'white',
                                borderRadius: '8px',
                                fontSize: '14px',
                                _hover: {bg: '#604a54'},
                                transition: 'background 0.2s'
                            })}>
                                Загрузить фото
                            </div>
                            <span className={css({
                                fontSize: '13px',
                                color: imageFile ? 'black' : '#888',
                                maxWidth: '200px',
                                whiteSpace: 'nowrap',
                                overflow: 'hidden',
                                textOverflow: 'ellipsis'
                            })}>
                    {imageFile ? imageFile.name : 'Файл еще не выбран'}
                </span>
                        </label>
                    </div>
                </div>
            </div>
        </div>
    );
};