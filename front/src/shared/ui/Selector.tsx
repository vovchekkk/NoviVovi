import {useEffect, useState} from "react";
import ReactSelect from 'react-select';
import {css} from '../../../styled-system/css'

interface SelectorProps {
    title: string;
    options: Option[];
    value?: string;
    onChange?: (val: string | null) => void;
    onBlur?: () => void;
    disabled?: boolean;
}

type Option = {
    value: string;
    label: string;
};

export default function Selector({ title, options, value, onChange, onBlur, disabled }: SelectorProps) {


    const currentOption = options.find(o => o.value === value) || null;
    const customStyles = {
        control: (base: any, state: any) => ({
            ...base,
            backgroundColor: '#fff',
            borderColor: state.isFocused ? '#775D68' : '#5e4a52',
            boxShadow: state.isFocused ? '0 0 0 3px rgba(119, 93, 104,0.25)' : 'none',
            '&:hover': { borderColor: '#775D68' },
            height: '48px',
            borderRadius: '8px',
            paddingLeft: '12px',
        }),
        option: (base: any, { isFocused, isSelected }: any) => ({
            ...base,
            backgroundColor: isSelected ? '#5e4a52' : isFocused ? 'rgba(223, 198, 209, 0.25)' : '#fff',
            color: isSelected ? '#fff' : '#212529',
            padding: '12px 16px',
        }),
        menu: (base: any) => ({
            ...base,
            borderRadius: '8px',
            boxShadow: '0 10px 25px -5px rgb(0 0 0 / 0.1)',
            marginTop: '4px',
        }),
    };

    return (
        <div className={css({ display: "flex", flexDirection: "column", gap: '10px', width: '300px', margin: '0 auto' })}>
            <div style={{ fontSize:'18px', textAlign:'left' }}>{title}</div>
            <div style={{ width: '100%' }}>
                <ReactSelect
                    options={options}
                    value={currentOption} // Важно: передаем объект
                    onChange={(val: Option | null) => onChange?.(val ? val.value : null)} // Важно: отдаем только ID
                    onBlur={onBlur}
                    isDisabled={disabled}
                    isClearable={true}
                    isSearchable={true}
                    styles={customStyles}
                    placeholder="Выберите..."
                />
            </div>
        </div>
    );
}
