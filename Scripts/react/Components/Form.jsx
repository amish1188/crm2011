import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useForm, FormProvider} from 'react-hook-form';

import { fetchOrganizations, fetchEntities, sendToSos, fetchKasseNumbers } from '../Apis/apiCalls';
import { groupAttachementsByEmail } from './../utils/groupByEmail';
import FormCard from './FormCard';
import EntitesList from './EntitesList';
import Spinner from './Spinner';
import '../styles/MainStyle.css';
import FloatingActionButton from './FloatingActionButton';

const Form = (props) => {

    const { id, typename, TypeCode } = useParams();
    let navigate = useNavigate();
    const methods = useForm();
   
    const [isLoading, setIsLoading] = useState(true);
    const [description, setDescription] = useState('');
    const [organizations, setOrganizations] = useState();
   
    const [entities, setEntities] = useState();
    const [entitiesError, setEntitiesError] = useState(false);
    const [sortedList, setSortedList] = useState();
    const [spinnerText, setSpinnerText] = useState('Laster ned data...');

    const setDescriptionToEntites = (v) => {
        setDescription(v);
        const arrCopy = [...entities];
        arrCopy.forEach(function (e, i) {
            this[i].Description = v;
        }, arrCopy);
        setEntities(arrCopy);
    }

    const handleResponse = async (response) => {
        if (!response.ok) {
            const error = await response.json();
            if (!error.ExceptionMessage && error.Message) {
                props.setErrorNotification({ isError: true, errorMessage: error.Message });
            } else {
                props.setErrorNotification({ isError: true, errorMessage: error.ExceptionMessage });
            }
        }
        else {
            return response.json();
        }
    }

    const checkKeyDown = (e) => {
        if (e.code === 'Enter') e.preventDefault();
    };
   

    useEffect(() => {
        async function fetchOrgAndEntities() {
            try {
                let [orgResult, entitiesResult] = await Promise.all([fetchOrganizations(id, typename), fetchEntities(id, typename)]);
                setIsLoading(false);
                const entitiesDataRes = await handleResponse(entitiesResult);
                const orgsDataRes = await handleResponse(orgResult);
                if (orgsDataRes && entitiesDataRes) {
                    setOrganizations(orgsDataRes.OrganizationsList);
                    setEntities(s => ([...entitiesDataRes]));
                    const groupedByEmail = groupAttachementsByEmail([...entitiesDataRes])
                    setSortedList(groupedByEmail);
                    setIsLoading(false);
                }
            } catch (error) {
                setIsLoading(false);
                console.log(error);
            }
        }
        fetchOrgAndEntities();
    }, []);

    
     /*
     const entites = {
        kasseNummer: kasseNmr,
        forsikringstakerNummer: organization,
        department: department,
        description: description,
        entities: [
            {
                id: ,
                leadingText:
                subject:
                description:
                from:
                to:
                actualEnd:
                archivingEnabled:
            }
        ]
     
     }
     */
    const submitFormHandler = async data => {
            if (data.selectedEntitiesIds.length > 0) {
                props.setErrorNotification({ isError: false, errorMessage: '' });
                let entitiesToArchive = [];

                data.selectedEntitiesIds.forEach(eId => {
                    const entityToArchive = entities.find(e => e.Id == eId);
                    entitiesToArchive.push(entityToArchive);
                });



                const itemToArchive = {
                    kasseNummer: data.kasseNummer,
                    forsikringstakerNummer: data.organization,
                    department: data.department,
                    description: data.description,
                    entities: entitiesToArchive
                }
                setSpinnerText("Sender til arkiv...");
                setIsLoading(true);
                const sendToSosResponse = await sendToSos(itemToArchive, id, typename, TypeCode);
                setIsLoading(false);
                const archivedItems = await handleResponse(sendToSosResponse);
                if (archivedItems) {
                    navigate("/CrmArchive/oppsummering", { state: archivedItems });
                }
            } else {
                setEntitiesError(true);
            }
    }

    if (isLoading) {
        return <Spinner text={spinnerText} />;
    }

    if (organizations && entities) {
    return (
        <FormProvider {...methods}>
            <form onSubmit={methods.handleSubmit(submitFormHandler)} onKeyDown={(e) => checkKeyDown(e)} className="Card">
                <FormCard
                    organizations={organizations}
                    setDescription={setDescriptionToEntites}
                    description={description}
                    id={id}
                    typename={typename}
                    TypeCode={TypeCode}
                    setErrorNotification={props.setErrorNotification}
                />
                <EntitesList
                    description={description}
                    entitiesError={entitiesError}
                    id={id}
                    setErrorNotification={props.setErrorNotification}
                    typename={typename}
                    entitesList={sortedList}
                />
                <FloatingActionButton handleSubmit={methods.handleSubmit} />
             </form>
         </FormProvider>
        );
    }
 };

export default Form;
