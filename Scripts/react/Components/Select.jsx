import React from 'react';
import { useFormContext } from 'react-hook-form';
import '../styles/MainStyle.css';

const Select = (props) => {

    const { register, setValue, formState: { errors } } = useFormContext();


    const onChangeHandler = (e) => {
        setValue(props.name, e.target.value);
    }

    return (
        <div className="FieldContainer">
            <span>{props.label}</span>
            <fluent-select placeholder="Velg noe" class={`${errors[props.name]?.type === 'required' ? 'SelectError' : 'Select'}`}>
                <fluent-option onClick={onChangeHandler} selected="true" value={props.options[0].value}>{props.options[0].text}</fluent-option>
                {props.options.slice(1).map(opt => <fluent-option key={opt.value} onClick={onChangeHandler} value={opt.value}>{opt.text}</fluent-option>) }
            </fluent-select>
            <input {...register(props.name, { required: true })} name={props.name} type="hidden" defaultValue={props.options[0].value} />
            {errors[props.name]?.type === 'required' && <ErrorMessage>Forsikringstakernr eller organisasjonsnummer mangler</ErrorMessage>}
        </div>
    );
}

export default Select;