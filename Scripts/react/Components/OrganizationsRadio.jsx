import React from 'react';
import { useFormContext } from 'react-hook-form';

import '../styles/MainStyle.css';
import ErrorMessage from './ErrorMessage';

const OrganizationsRadio = (props) => {
    const { register, formState: { errors } } = useFormContext();


    const getLabelName = () => {
        const orgHeading = "Velg forsikringstakernr eller organisasjonsnr:";
        const personHeading = "Velg personsnummer:";
        const personHeadingTilleggForSensitivSak = "Personsensitivsak";

        if (props.TypeCode == 2 && !props.organizations.includes('-')) {
            if (props.typename != "eeg_sak") {
                return personHeading;
            }
            else {
                return personHeadingTilleggForSensitivSak + "<br />" + personHeading;
            }
        }
        else if (props.TypeCode == 1 && props.organizations.includes('-'))
            return orgHeading;

        return "";
    };

    const labelName = getLabelName();
    /*if (props.organizations.length < 0) {
        return <p>Vennligst koble denne saken til riktig kunde. Denne saken er koblet til en kontakt som har overordnet kunde som ikke har organisasjonsnummer.</p>
    } */

    if (props.organizations.includes('-')) {
        return <p>Vennligst koble denne saken til riktig kunde. "Denne saken er koblet til en kontakt som ikke har overordnet kunde.</p>
    }

    if (props.organizations.includes('---')) {
        <p>{`Vennligst kontakte CRM Administrator med denne informasjonen. ${props.entityId} ${props.typename}. Denne saken er koblet til en kontakt som ikke har kontakttype.`} </p>
    }

    if (props.organizations.includes('--')) {
        return <p>Vennligst koble denne saken til riktig kunde. Denne saken er koblet til en mulig kunde som ikke har personsnummer.</p>
    }

    if (props.organizations.includes('Kontakt')) {
        return <p>Vennligst koble denne saken til riktig kunde. Denne saken er koblet til en kontakt.</p>
    }




    return (
      
        <div className="FieldContainer">
            <div className={`${errors.organization?.type === 'required' && 'WrapperError'}` }>
                <span htmlFor="org">{labelName}</span>
            {props.organizations && props.organizations.map(i =>
                <div key={i}>
                    <input
                        
                        type="radio"
                        id={i}
                        {...register('organization', { required: true })}
                        name="organization"
                        style={{ marginBottom: '0.5rem' }}
                        value={i}
                        key={i} />
                    <label htmlFor={i}>{i} </label>
                </div>)}
                </div>
            {errors.organization?.type === 'required' && <ErrorMessage style={{ width: '100%' }}>Forsikringstakernr eller organisasjonsnummer mangler</ErrorMessage>}
        </div>
        
    );
}

export default OrganizationsRadio;