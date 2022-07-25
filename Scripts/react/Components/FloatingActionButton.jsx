import React from 'react';

import ArchiveIcon from '../images/archive.png';

const FloatingActionButton = props => (
    <button className="FAB" onClick={props.handleSubmit}>
        <img style={{ width: '20px', marginRight: '1rem' }} alt="archive icon" src={ArchiveIcon} />
        <p>Send til SOS</p>
    </button>
);

export default FloatingActionButton;