import {css} from '../../../styled-system/css'

interface AssetsBlockProps {
    title: string;
}

export default function AssetsBlock({title}: AssetsBlockProps){
    return (
        <div className={css({
            height: '40%',
            display: 'flex',
            alignItems: 'stretch',
            padding: '4px',
            margin: '5px',
            fontWeight: 'bold',
            backgroundColor: '#F8EDEB',
            borderRadius: '12px',
            width: '90%',
            textAlign: 'center',
            fontSize: '20px',
            _hover: {
                bg: '#b08799',
                color: 'background',
                borderColor: '#775D68',
                transform: 'translateY(-2px)',
                boxShadow: '0 0 40px rgba(119, 93, 104, 0.5)',
            },
            _selected: {
                bg: '#705661',
                borderColor: '#775D68',
            }
        })}>
            <div className={css({
                padding: '10px',
                backgroundColor: 'white',
                borderRadius: '12px',
                flex: 1,
                _selected:{
                    backgroundColor: '#EAD8E0',
                }
            })}>
                {title}
            </div>
        </div>
    )
}