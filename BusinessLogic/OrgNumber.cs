using KlpCrm.Filenet.Library.Xrm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace KlpCrm.Filenet.Web.ReactApplication.BusinessLogic
{
    public class OrgNumber
    {
        public static string GetOrgNrOrFnrFromIncident(Guid entityID, string entityTypeName)
        {
            string result = string.Empty;

            switch (GetEntityEnumLogic.GetEntityEnum(entityTypeName))
            {
                case Common.Enums.EntityTypes.NONE:
                    break;
                case Common.Enums.EntityTypes.EMAIL:
                    break;
                case Common.Enums.EntityTypes.INCIDENT:
                    Guid incidentId = entityID;
                    result = GetOrgNrOrFnrFromIncidentCrmOperation(incidentId);
                    break;
                case Common.Enums.EntityTypes.ACTIVITYMIMEATTACHMENT:
                    break;
                case Common.Enums.EntityTypes.EEG_SAK:
                    Guid eegsakId = entityID;
                    result = GetOrgNrOrFnrFromEegSakCrmOperation(eegsakId);
                    break;
                default:
                    result = string.Empty;
                    break;
            }
            return result;
        }
        private static string GetOrgNrOrFnrFromIncidentCrmOperation(Guid incidentId)
        {
            var connection = new KlpCrmConnector.Connection(Config.CrmConnectionString);
            var service = connection.Service;

            string result = string.Empty;

            Incident incident =
                service.Retrieve(Incident.EntityLogicalName, incidentId, new ColumnSet("customerid")).ToEntity
                    <Incident>();
            if (incident.CustomerId != null && incident.CustomerId.LogicalName == Account.EntityLogicalName)
            {
                Account account =
                    service.Retrieve(Account.EntityLogicalName, incident.CustomerId.Id,
                                            new ColumnSet("klp_organisasjonsnummer", "name")).ToEntity<Account>();
                result = account.Klp_Organisasjonsnummer;
                //forsikringstakerNavn.Value = account.Name;
            }
            else if (incident.CustomerId != null && incident.CustomerId.LogicalName == Contact.EntityLogicalName)
            {
                int kunde = 1;
                Contact contact = service.Retrieve(Contact.EntityLogicalName, incident.CustomerId.Id,
                                            new ColumnSet("klp_fodselsnummer", "eeg_kontakttype", "parentcustomerid")).ToEntity<Contact>();
                if (new OptionSetValue(kunde) == (OptionSetValue)contact["eeg_kontakttype"])
                {
                    result = contact.Klp_Fodselsnummer;
                }
                else
                {
                    if (contact.ParentCustomerId != null && contact.ParentCustomerId.LogicalName == Account.EntityLogicalName)
                    {
                        Account parentCustomer = service.Retrieve(Account.EntityLogicalName, contact.ParentCustomerId.Id,
                                            new ColumnSet("klp_organisasjonsnummer", "name")).ToEntity<Account>();
                        result = parentCustomer.Klp_Organisasjonsnummer;
                        //forsikringstakerNavn.Value = parentCustomer.Name;
                    }
                    else
                    {
                        result = "-";
                    }
                }
            }
            return result;
        }

        private static string GetOrgNrOrFnrFromEegSakCrmOperation(Guid eegsakId)
        {
            var connection = new KlpCrmConnector.Connection(Config.CrmConnectionString);
            var service = connection.Service;

            string result = string.Empty;

            eeg_sak eegSak =
                service.Retrieve(eeg_sak.EntityLogicalName, eegsakId, new ColumnSet(true)).ToEntity<eeg_sak>();
            if (eegSak.eeg_CustomerContactId != null)
            {
                Contact contact = service.Retrieve(Contact.EntityLogicalName, eegSak.eeg_CustomerContactId.Id,
                    new ColumnSet("eeg_kontakttype", "klp_fodselsnummer", "parentcustomerid")).ToEntity<Contact>();

                if (contact.eeg_Kontakttype == null)
                {
                    result = "---";
                }
                // Kunde
                else if (contact.eeg_Kontakttype.Value == 1)
                {
                    result = contact.Klp_Fodselsnummer;
                }
                // Mulig kunde
                else if (contact.eeg_Kontakttype.Value == 2)
                {
                    result = !string.IsNullOrEmpty(contact.Klp_Fodselsnummer) ? contact.Klp_Fodselsnummer : "Mulig kunde uten pnr";
                }
                // Kontakt 
                else if (contact.eeg_Kontakttype.Value == 0)
                {
                    result = "Kontakt";
                }
            }
            return result;
        }
    }
}