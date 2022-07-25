import React, { useState } from 'react';

import Form from '../Components/Form';
import ErrorNotification from '../Components/ErrorNotification.jsx';
import '../styles/MainStyle.css';


const CrmArchive = () => {
    
    const [errorNotification, setErrorNotification] = useState({ isError: false, errorMessage: '' });

    const setErrorNotificationHandler = v => setErrorNotification(v);

    return (
        <>
            {errorNotification.isError && <ErrorNotification errorMessage={errorNotification.errorMessage} />}
            <Form setErrorNotification={setErrorNotificationHandler} />
        </>
    );
};

export default CrmArchive;