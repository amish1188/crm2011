
const url = 'https://as-sandbox-klpcrmfilenet.ase-sandbox-crm.p.azurewebsites.net/';
//for local testing add localhost
//const url = 'https://localhost:44314/';


export function fetchOrganizations(id, typename) {
    return fetch(`${url}api/CrmArchiveApi/GetOrgNumAndInsuranceTakerNum?id=${id}&typename=${typename}`,
        {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
};

export function fetchEntities(id, typename) {
    return fetch(`${url}api/CrmArchiveApi/GetEntitiesList?id=${id}&typename=${typename}`,
        {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
};

export function sendToSos(itemToArchive, id, typename, TypeCode) {
    return fetch(`${url}api/CrmArchiveApi/SendToSos?id=${id}&typename=${typename}&Typecode=${TypeCode}`,
        {
            method: 'POST',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(itemToArchive)
        }
    );
}

export function fetchAttachmentBody(id) {
    return fetch(`${url}api/CrmArchiveApi/GetAttachmentView?id=${id}`,
        {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
}

export function fetchKasseNumbers(data) {
    return fetch(`${url}api/CrmArchiveApi/GetKasseNumberItems`,
        {
            method: 'POST',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        }
    );
}

export function fetchNames(body, id, typename, TypeCode) {
    return fetch(`${url}api/CrmArchiveApi/GetDescription?id=${id}&typename=${typename}&Typecode=${TypeCode}`,
        {
            method: 'POST',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(body)
        });
}