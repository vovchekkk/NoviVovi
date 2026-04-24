import {css} from '../../../styled-system/css'
interface TextProps {
    value: string;
    onChange?: (value: string) => void;
}
export default function Text({value, onChange}: TextProps) {
    return (
        <input
            type='text'
            value={value}
            onChange={(e) => onChange && onChange(e.target.value)}
            className={css({
                    border: '1px solid black',
                    borderRadius: '8px',
                    padding: '10px',
                    backgroundColor: 'white',
                }
            )
            }/>
    )
}