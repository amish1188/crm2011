import React, { useState } from 'react';
import '../styles/MainStyle.css';

const TextArea = (props) => {
    const [value, setValue] = useState('');

    const onChangeHandler = (e) => {
        setValue(e.target.value);
    };


    return (
        <div className="FieldContainer">
            <span>{props.label}</span>
            <textarea onChange={onChangeHandler} onBlur={props.setDescription} className="TextArea" />
        </div>  
    );
};

export default TextArea;