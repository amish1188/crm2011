import React from 'react';
import { useFormContext } from 'react-hook-form';

import EntityItem from './EntityItem';
import '../styles/MainStyle.css';
import ErrorMessage from './ErrorMessage';

const EntitesList = (props) => {
    const { setValue, getValues, clearErrors, register, formState: { errors } } = useFormContext();

    const setEntitiesIdsToArchiveHandler = (v) => {
        clearErrors('selectedEntitiesIds');
        const vals = getValues("selectedEntitiesIds")
        setValue("selectedEntitiesIds", [...vals, v]);
    }

    const removeEntitiesToArchiveHandler = (v) => {
        const vals = getValues("selectedEntitiesIds");
        const removed = vals.filter(i => i !== v);
        setValue("selectedEntitiesIds", removed);
    }

    return (
        <>
            <input {...register("selectedEntitiesIds", { required: true })} name={"selectedEntitiesIds"} type="hidden" />
            <div style={{ marginBottom:'0' }} className={`Wrapper ${errors.selectedEntitiesIds?.type === 'required' && "WrapperError"}`}>
                {props.entitesList && props.entitesList.map(i =>
                        <EntityItem
                            key={i.Id}
                            setErrorNotification={props.setErrorNotification}
                            description={props.description}
                            setEntitiesIdsToArchive={setEntitiesIdsToArchiveHandler}
                            removeEntitiesToArchive={removeEntitiesToArchiveHandler}
                            entityData={i} />
                    )}
            </div>
            {errors.selectedEntitiesIds?.type === 'required' && <ErrorMessage style={{ width: '100%', margin: '0' }}>Du har ikke valgt et element som skal arkiveres</ErrorMessage>}
        </>
    );
};

export default EntitesList;