import React from 'react';

import '../styles/SpinnerStyle.css';

const Spinner = props => (
    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', height: '80vh', justifyContent: 'center' }}>
        <div className="lds-roller">
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
        </div>
        {
            props.text && <span style={{ fontWeight: '400', margin: '1rem' }}>{props.text}</span>
        }
        
    </div>
);

export default Spinner;