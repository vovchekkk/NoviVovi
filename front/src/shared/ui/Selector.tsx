import { useState } from "react";
import ReactSelect from 'react-select';
import {css} from '../../../styled-system/css'

interface SelectorProps {
    title: string;
    options: string[];
}

type Option = {
    value: string;
    label: string;
};

export default function Selector({ title, options }: SelectorProps) {
    const selectOptions: Option[] = options.map(option => ({
        value: option,
        label: option,
    }));

    const [selectedOption, setSelectedOption] = useState<Option | null>(null);
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
        <div className={css({
            display: "flex",
            flexDirection: "column",
        })}>
            <div style={{ fontSize:'18px', textAlign:'left' }}>{title}</div>
            <div style={{ width: 300, margin: '0 auto' }}>
                <ReactSelect
                    options={selectOptions}
                    value={selectedOption}
                    onChange={setSelectedOption}
                    isClearable={true}
                    isSearchable={true}
                    styles={customStyles}
                />
            </div>
        </div>
    );
}