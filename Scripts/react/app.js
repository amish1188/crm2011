import React from 'react';
import { Routes, Route, useLocation } from 'react-router-dom';

import CrmArchive from './Pages/CrmArchive.jsx';
import SentToSosSummary from './Pages/SentToSosSummary.jsx';
import KlpLogo from './images/KLP_logo.png';
import './styles/MainStyle.css';



const App = () => {
    let location = useLocation();

    return (
            <div className="Container">
                <div className="Row">
                    <img alt="klp logo" style={{ maxWidth: '80px' }} src={KlpLogo} />
                <span style={{ fontFamily: 'Segoe UI', fontSize: '1.5rem', marginBottom: '0', marginLeft: '1rem' }}>{location.pathname.includes('oppsummering') ? 'Sendt til arkiv oppsummering' : 'Crm Archive'}</span>
                </div>
                <div style={{ width: '100%', borderBottom: '1px solid #747474' }}></div>


                <Routes>
                    <Route path="CrmArchive/:id/:typename/:TypeCode" element={<CrmArchive />} />
                    <Route path="CrmArchive/oppsummering" element={<SentToSosSummary />} />
                    <Route path="*" element={<h1>Siden finnes ikke</h1>} />
                </Routes>
            </div>
    );
}

export default App;
