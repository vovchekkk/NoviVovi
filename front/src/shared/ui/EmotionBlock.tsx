import {css} from '../../../styled-system/css'
import {useEffect, useState} from "react";

interface EmotionBlockProps {
    index: number;
    register: any;
    setValue: any;
    watch: any;
    onRemove: () => void;
    errors?: any;
    isActive: boolean;
    onSelect: () => void;
    // onChange: (event: React.ChangeEvent<HTMLInputElement>) => void
}
const CompactInput = ({ label, name, register, step = "1" }: any) => (
    <div className={css({ display: 'flex', flexDirection: 'column', gap: '2px' })}>
        <span className={css({ fontSize: '10px', fontWeight: 'bold', color: '#666' })}>{label}</span>
        <input
            type="number"
            step={step}
            {...register(name, { valueAsNumber: true })}
            className={css({
                width: '100%',
                p: '4px',
                fontSize: '12px',
                borderRadius: '4px',
                border: '1px solid #ccc'
            })}
        />
    </div>
);
export default function EmotionBlock({index, register, setValue, watch, onRemove, errors, isActive, onSelect}: EmotionBlockProps) {
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
        <div
            onClick={onSelect}
            className={css({
                display: 'flex',
                flexDirection: 'column',
                backgroundColor: isActive ? '#fdf0f5' : '#DFC6D1',
                padding: '15px',
                borderRadius: '12px',
                gap: '10px',
                position: 'relative',
                border: isActive ? '2px solid #775D68' : '1px solid rgba(0,0,0,0.1)',
                cursor: 'pointer',
                transition: 'all 0.2s'
            })}
        >

            <div className={css({ display: 'flex', gap: '20px' })}>

                <div className={css({ flex: 1, display: 'flex', flexDirection: 'column', gap: '8px' })}>
                    <input
                        {...register(`emotions.${index}.name`)}
                        placeholder="Название (напр. Радость)"
                        className={css({ p: '8px', borderRadius: '8px', border: '1px solid #ccc' })}
                    />

                    <div className={css({
                        display: 'grid',
                        gridTemplateColumns: 'repeat(4, 1fr)',
                        gap: '8px',
                        backgroundColor: 'rgba(255,255,255,0.5)',
                        p: '8px',
                        borderRadius: '8px'
                    })}>
                        <CompactInput label="X (%)" name={`emotions.${index}.transform.x`} register={register} />
                        <CompactInput label="Y (%)" name={`emotions.${index}.transform.y`} register={register} />
                        <CompactInput label="W (%)" name={`emotions.${index}.transform.width`} register={register} />
                        <CompactInput label="H (%)" name={`emotions.${index}.transform.height`} register={register} />
                        <CompactInput label="Scale" name={`emotions.${index}.transform.scale`} register={register} step="0.1" />
                        <CompactInput label="Rotate" name={`emotions.${index}.transform.rotation`} register={register} />
                    </div>

                    <label className={css({ cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '10px' })}>
                        <input type="file" className={css({ display: 'none' })} onChange={(e) => {
                            const file = e.target.files?.[0];
                            if (file) setValue(`emotions.${index}.imageFile`, file);
                        }} />
                        <div className={css({ bg: '#775D68', color: 'white', px: '10px', py: '4px', borderRadius: '6px', fontSize: '12px' })}>
                            Загрузить файл
                        </div>
                    </label>
                </div>
            </div>
        </div>
    );
};