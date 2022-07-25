import React, { useState } from 'react';

import { getMimeType } from '../utils/getMimeType';
import { fetchAttachmentBody } from '../Apis/apiCalls';
import '../styles/MainStyle.css';

const Attachment = props => {

    const [isAttachmentLoading, setIsAttachmentLoading] = useState(false);

    const viewPdf = async (id) => {
        setIsAttachmentLoading(true);
        const res = await fetchAttachmentBody(id);
        setIsAttachmentLoading(false);

        if (!res.ok) {
            const error = await response.json();
            props.setErrorNotification({ isError: true, errorMessage: error.ExceptionMessage });
        }
        else {
            const data = await res.json();
            const type = getMimeType(props.subject);
            let byteCharacters = atob(data.Body);
            let byteNumbers = new Array(byteCharacters.length);
            for (var i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }
            let byteArray = new Uint8Array(byteNumbers);
            let blob = new Blob([byteArray], { type: type});

            if (window.navigator && window.navigator.msSaveOrOpenBlob) {
                window.navigator.msSaveOrOpenBlob(blob, entityData.Subject);
            }
            else {
                var fileURL = URL.createObjectURL(blob);
                window.open(fileURL);
            }
        }
    }

    return (
        <div className="Row">
            <a style={{ color: '#2198F6', marginRight: '1rem', cursor: 'pointer' }} onClick={() => viewPdf(props.id)}>{props.subject}</a>
            {isAttachmentLoading && <p>Laster ned</p>}
        </div>
    );
}

export default Attachment;