import React, { useState } from 'react';
import { useFormContext } from 'react-hook-form';

import { divisjonOptions, kassenrOptions } from '../utils/selectOptions';
import OrganizationsRadio from './OrganizationsRadio';
import Select from './Select';
import Autocomplete from './Autocomplete';
import { fetchKasseNumbers, fetchNames } from '../Apis/apiCalls';
import '../styles/MainStyle.css';

const FormCard = (props) => {

    const { getValues } = useFormContext();
    const [kasseNumbers, setKasseNumbers] = useState(kassenrOptions);
    const [suggestions, setSuggestions] = useState();

    const getKasseNumbers = async (values) => {
        const response = await fetchKasseNumbers(values);
        if (!response.ok) {
            const error = await response.json();
            props.setErrorNotification({ isError: true, errorMessage: error.ExceptionMessage });
        } else {
            const data = await response.json();
            if (data) {
                let arr = [...kassenrOptions];
                Object.entries(data).forEach(o => {
                    const obj = { value: o[1], text: o[0] };
                    const isFound = arr.some(i => i.value === obj.value);
                    if (!isFound) {
                        arr.push(obj);
                    }
                });
                setKasseNumbers(arr);
            }
        }
    } 

    const filterDb = async (typed) => {
        const body = {
            forsikringstakerNummer: getValues("organization"),
            department: getValues("department"),
            kassenummer: getValues("kasseNummer"),
            typed: typed
        }

        const res = await fetchNames(body, props.id, props.typename, props.TypeCode);
        if (!res.ok) {
            const error = await res.json();
            props.setErrorNotification({ isError: true, errorMessage: error.ExceptionMessage });
        } else {
            const data = await res.json();
            if (data) {
                setSuggestions(data);
            }
        }
    }

    const onTextChangeHandler = () => {
        const data = {
            forsikringstakerNummer: getValues("organization"),
            department: getValues("department"),
            description: getValues("description")
        }
        getKasseNumbers(data);
    }



    return (
        <>
            {props.organizations &&
                <>
                    <OrganizationsRadio TypeCode={props.TypeCode} entityId={props.id} typename={props.typename} organizations={props.organizations} />
                <Select name={"department"} label="Divisjon" options={divisjonOptions} />
                <Autocomplete filterDb={filterDb} onTextChangeHandler={onTextChangeHandler} description={props.description} setDescription={props.setDescription} label="Dokumentbeskrivelse" suggestions={suggestions} />
                <Select getKasseNumbers={props.getKasseNumbers} name={"kasseNummer"} label="Kasse nummer" options={kasseNumbers} />
                </>
            }
        </>

    );
}

export default FormCard;