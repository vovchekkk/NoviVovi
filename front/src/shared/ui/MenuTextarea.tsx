import {css} from '../../../styled-system/css'
import Textarea from "./Textarea.tsx";

interface MenuTextareaProps {
    title: string;
}

export default function MenuTextarea({title}: MenuTextareaProps) {
    return (

        <div className={css({
            display: "flex",
            flexDirection: "column",
        })}>
            <div style={{ textAlign:'left', fontSize:'18px' }}>{title}</div>
            <Textarea></Textarea>
        </div>
    )
}