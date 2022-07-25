import React, { useState, useRef, useEffect } from 'react';
import { useFormContext } from 'react-hook-form';

import '../styles/MainStyle.css';
import ErrorMessage from './ErrorMessage';

const Autocomplete = (props) => {

    const { register, setValue, clearErrors, formState: { errors } } = useFormContext();
    const [activeSuggestion, setActiveSuggestion] = useState(0);
    const [show, setShow] = useState(false);
    const [userInput, setUserInput] = useState(props.description || '');
    const ref = useRef();
    const [clicked, setClicked] = useState(false);


    useEffect(() => {
        const checkIfClickedOutside = e => {
            if (show && ref.current && !ref.current.contains(e.target)) {
                setShow(false);
            }
        }
        document.addEventListener("mousedown", checkIfClickedOutside)
        return () => {
            document.removeEventListener("mousedown", checkIfClickedOutside);
        }
    }, [show]);


    useEffect(() => {
        const delay = setTimeout(() => {
            if (userInput && !clicked) {
                props.filterDb(userInput);
                setShow(true);
            }
        }, 1000);
        return () => clearTimeout(delay);
    }, [userInput]); 

    const onChangeHandler = e => {
        setClicked(false);
        setUserInput(e.target.value);
    }

    const onKeyDownHandler = e => {
        //Enter
        if (e.keyCode === 13) {
            setClicked(true);
            setActiveSuggestion(0);
            setShow(false);
            setValue('description', props.suggestions[activeSuggestion])
            clearErrors('description');
            setUserInput(props.suggestions[activeSuggestion]);
            props.setDescription(props.suggestions[activeSuggestion])
            props.onTextChangeHandler();
        }
        //key up
        else if (e.keyCode === 38) {
            if (activeSuggestion === 0) {
                return;
            }
            setActiveSuggestion(activeSuggestion - 1);
        }
        //key down
        else if (e.keyCode === 40) {
            if (activeSuggestion + 1 === props.suggestions.length) {
                return;
            }
            setActiveSuggestion(activeSuggestion + 1);
        }
    }

    const onClickHandler = s => {
        setClicked(true);
        setActiveSuggestion(0);
        setShow(false);
        setUserInput(s);
        setValue('description', s);
        clearErrors('description');
        props.setDescription(s)
        props.onTextChangeHandler();
    }

    let suggestionsList;

    if (show && userInput) {
        if (props.suggestions && props.suggestions.length > 0) {
            suggestionsList = (
                <ul>
                    {props.suggestions.map((s, i) => {
                        let className;
                        if (i === activeSuggestion) {
                            //set class on active suggestion
                            className = "suggestion-active";
                        }
                        return (
                            <li key={s} className={className} onClick={() => onClickHandler(s)}>{s}</li>
                        );
                    })}
                </ul>
            );
        }
    };

    return (
        <div className="FieldContainer" ref={ref} >
            <label htmlFor="description">{props.label}</label>
            <input
                autoComplete="off"
                onChange={onChangeHandler}
                onKeyDown={onKeyDownHandler}
                value={userInput}
                className={`TextArea ${errors.description?.type === 'required' ? 'TextAreaError' : 'TextAreaDefault'}`} />
            <input name="description" {...register('description', { required: true })} value={''} type="hidden" /> 
            {suggestionsList}
            {errors.description?.type === 'required' && <ErrorMessage>Dokumentbeskrivelse mangler</ErrorMessage>}
        </div>  
    );
}

export default Autocomplete;