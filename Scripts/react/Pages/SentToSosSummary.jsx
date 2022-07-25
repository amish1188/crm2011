import React from 'react';
import { useLocation } from 'react-router-dom';

import '../styles/MainStyle.css';

const SentToSosSummary = () => {

    const { state } = useLocation();
    return (
        <>
            <div className="Card">
                <h1 style={{fontWeight: '400'}}>Enheter sendt til arkiv:</h1>
                {state.Entities && state.Entities.map(e =>
                    <table key={e.Id} id={e.Id}>
                        <tbody>
                            <tr>
                                <td> <h3>{e.LeadingText}</h3> </td>
                            </tr>
                        <tr>
                            <td>Emne:</td>
                            <td>{e.Subject}</td>
                        </tr>
                        <tr>
                            <td>Fra:</td>
                            <td> {e.From}</td>
                        </tr>
                        <tr>
                            <td>Til:</td>
                            <td>{e.To}</td>
                        </tr>
                        <tr>
                            <td>Faktisk slutt:</td>
                            <td>{e.ActualEnd}</td>
                        </tr>
                        <tr>
                            <td>Dokumentbeskrivelse:</td>
                            <td><input className="TextArea" type="text" value={e.Description} disabled /></td>
                        </tr>
                        <tr>
                            <td>Status:</td>
                            <td>Sendt til arkiv</td>
                            </tr>
                            </tbody>
                    </table>
                )}
                </div>
            </>
    );
};

export default SentToSosSummary;