import React, { useState } from 'react';

import '../styles/MainStyle.css';
import Attachment from './Attachment';

const EntityItem = (props) => {

    const [checked, setChecked] = useState(false);
    const { entityData } = props;

    const onChangeHandler = (e) => {
        //set id of entities to archive
        setChecked(!checked);
        if (e.target.checked) {
            props.setEntitiesIdsToArchive(entityData.Id);
            if (entityData.Attachments) {
                entityData.Attachments.map(i => props.setEntitiesIdsToArchive(i.Id));
            }
        } else {
            props.removeEntitiesToArchive(entityData.Id)
            if (entityData.Attachments) {
                entityData.Attachments.map(i => props.removeEntitiesToArchive(i.Id));
            }
        }
    } 

    return (
        <div style={{ padding: '1rem 0' }}>
           
            <h3>{entityData.LeadingText}</h3> 
            <table id={entityData.Id}>
                <tbody>
                <tr>
                    <td>Emne:</td>
                    <td>{entityData.Subject}</td>
                </tr>
                <tr>
                    <td>Fra:</td>
                    <td>{entityData.From}</td>
                </tr>
                <tr>
                    <td>Til:</td>
                    <td>{entityData.To}</td>
                </tr>
                <tr>
                    <td>Faktisk slutt:</td>
                    <td>{entityData.ActualEnd}</td>
                </tr>
                <tr>
                    <td>Dokumentbeskrivelse:</td>
                    <td><input className="TextArea" type="text" value={props.description} disabled /></td>
                </tr>
                <tr>
                    <td>Arkiveres:</td>
                    <td> <input onChange={onChangeHandler} type="checkbox" checked={checked} disabled={!entityData.ArchivingEnabled} /></td>
                </tr>
                {entityData.Attachments &&
                    <tr>
                        <td valign="top">Vedlegg:</td>
                        <td>
                            <div style={{ display: 'flex', flexDirection: 'column' }}>
                                {entityData.Attachments.map(a => <Attachment setErrorNotification={props.setErrorNotification} key={a.Id} id={a.Id} subject={a.Subject} leadingText={a.LeadingText} />)}
                            </div>
                        </td>
                        </tr>}
                    </tbody>
            </table>
        </div>
    );
};

export default EntityItem; 