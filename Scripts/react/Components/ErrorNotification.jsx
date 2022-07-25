import React from 'react';

import ErrorIcon from '../images/errorIcon.png';
import '../styles/MainStyle.css';

const ErrorNotification = (props) => (
    <div className="ErrorMessageContainer">
        <img alt="error icon" src={ErrorIcon} />
        <label style={{ marginLeft:'0.7rem' }}>
            {props.errorMessage}
        </label>
    </div>    
);

export default ErrorNotification;