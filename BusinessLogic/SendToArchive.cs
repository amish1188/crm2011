using KlpCrm.Filenet.Common.Enums;
using KlpCrm.Filenet.Library.Xrm;
using KlpCrm.Filenet.Web.ReactApplication.Models;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace KlpCrm.Filenet.Web.ReactApplication.BusinessLogic
{
    /*VALIDATION
        -check if forsikrinstaker or org is checked
    -what about kassenr
    -if item archive is checked dokumentbeskrivelse cannot be empty

     */
    public class SendToArchive
    {
        //after validation get the mime type from extension
        public static void SendToSos(
            List<Common.CargoObjects.FileNetCargo> fileNetCargo, 
            EntityWithAttachments item, 
            string mimeType, 
            Guid entityId, 
            string entityTypeName, 
            int customerTypeCode,
            string department, 
            string kasseNummer, 
            string forsikringstakerNummer
            )
        {
            GenerateCargoObjects(fileNetCargo, item, mimeType, entityId, entityTypeName, customerTypeCode, department, kasseNummer, forsikringstakerNummer);
        }

        public static void WrapItUp(string typeName, List<Common.CargoObjects.FileNetCargo> fileNetCargo, string id, int typeCode, string kasseNummer,
            string forsikringstakerNummer, string description)
        {
            Business.Archive archive = new Business.Archive();
            try
            {
                var connection = new KlpCrmConnector.Connection(Config.CrmConnectionString);
                var service = connection.Service;

                string indekseringsnokkel = GetIncidentsCustomerSocSecNmbOrOrgNmb(Guid.Parse(id), typeName, typeCode, kasseNummer, forsikringstakerNummer);

                archive.SendToArchive(fileNetCargo, service);
                archive.SaveEntitiesChanges(fileNetCargo, service);
                archive.CloseCase(GetEntityEnumLogic.GetEntityEnum(typeName), Guid.Parse(id), GetIncidentsCustomerSocSecNmbOrOrgNmb(Guid.Parse(id), typeName, typeCode, kasseNummer, forsikringstakerNummer), description, service);
            }

            catch (Exception ex)
            {
                if (ex.Data.Contains("User"))
                {
                    throw new Exception($"CloseIncident User: {ex.Message}");
                }
                else
                {
                    throw new Exception($"CloseIncident wrapitup: {ex.Message}");
                }
            }
        }

        public static string GetSelectedDokBeskrivelseKode(string selectedDokBeskrivelse)
        {
            string result = "";

            if (selectedDokBeskrivelse.Contains("(") && selectedDokBeskrivelse.Contains(")"))
            {
                int indexParenthesisStart = 0;
                int indexparenthesisEnd = 0;

                indexParenthesisStart = selectedDokBeskrivelse.LastIndexOf("(");
                indexparenthesisEnd = selectedDokBeskrivelse.LastIndexOf(")");

                result = selectedDokBeskrivelse.Substring(indexParenthesisStart + 1, selectedDokBeskrivelse.Length - (indexParenthesisStart + 2));
            }

            return result;
        }

        private static NetworkCredential Credentials()
        {
            NetworkCredential credentials = new NetworkCredential();

            credentials.UserName = ConfigurationManager.AppSettings["CrmUserUsername"];
            credentials.Password = ConfigurationManager.AppSettings["CrmUserPassword"];
            credentials.Domain = ConfigurationManager.AppSettings["CrmUserDomain"];

            return credentials;
        }

        public static string GetIncidentsCustomerSocSecNmbOrOrgNmb(Guid entityId, string entityTypeName, int customerTypeCode, string kasseNummer, string forsikringstakerNummer)
        {
            string incidentCustomerSocialSecurityNmb = null;
            string fnrororgnr = OrgNumber.GetOrgNrOrFnrFromIncident(entityId, entityTypeName);
            if (customerTypeCode == 2 && fnrororgnr != "-")
            {
              incidentCustomerSocialSecurityNmb = fnrororgnr;
            }
            else if (customerTypeCode == 1 && kasseNummer != "-1")
                incidentCustomerSocialSecurityNmb = forsikringstakerNummer.Trim() + "0" + kasseNummer.Trim();
            else if (customerTypeCode == 1 && kasseNummer == "-1")
                incidentCustomerSocialSecurityNmb = forsikringstakerNummer.Trim(); 
            return incidentCustomerSocialSecurityNmb;
        }

        private static string GetInsuranceHolderName(Guid entityId)
        {
            var connection = new KlpCrmConnector.Connection(Config.CrmConnectionString);
            var service = connection.Service;

            Incident incident =
                    service.Retrieve(Incident.EntityLogicalName, entityId, new ColumnSet("customerid")).ToEntity
            <Incident>();
            Account account =
                   service.Retrieve(Account.EntityLogicalName, incident.CustomerId.Id,
                                           new ColumnSet("klp_organisasjonsnummer", "name")).ToEntity<Account>();
            string insuranceHolderName = account.Name;
            return insuranceHolderName;
        }

        private static string GetDepartmentText(string val)
        {
            switch (val)
            {
                case "1000":
                    return "Liv";
                case "1001":
                    return "Bedriftspensjon";
                case "3000":
                    return "Asker Pensjonskasse";
                default:
                    return "Liv";
            }
        }



        private static void GenerateCargoObjects(
            List<Common.CargoObjects.FileNetCargo> fileNetCargo,
            EntityWithAttachments item,
            string mimeType, 
            Guid entityId,
            string entityTypeName, 
            int customerTypeCode,
            string department, 
            string kasseNummer, 
            string forsikringstakerNummer
            )
        {

            //most likely don't need that code sinc we check validation through annotations in a model
            /*string besk = GetSelectedDokBeskrivelseKode(item.Description);
            if (besk == string.Empty)
            {
                return false;
            } */

                Common.CargoObjects.FileNetCargo cargo = new Common.CargoObjects.FileNetCargo(Credentials());

                cargo.entityId = item.Id;

                cargo.fileNetDocumentField.documentTitle = item.Subject;
                cargo.fileNetDocumentField.documentTitleUse = true;

                if (int.Parse(kasseNummer) != -1)
                {
                    string indekseringsnokkel = GetIncidentsCustomerSocSecNmbOrOrgNmb(entityId, entityTypeName, customerTypeCode, kasseNummer, forsikringstakerNummer);
                    cargo.fileNetDocumentField.indekseringsnokkel = indekseringsnokkel;
                    cargo.fileNetDocumentField.kannelid = new string[]
                    {
                            "LNVN=" + GetInsuranceHolderName(entityId),
                            "KASSE=" +  kasseNummer.Trim(),
                            "IDX=" + indekseringsnokkel
                    };

                    cargo.fileNetDocumentField.kannelidUse = true;
                }
                else
                {
                    cargo.fileNetDocumentField.indekseringsnokkel = GetIncidentsCustomerSocSecNmbOrOrgNmb(entityId, entityTypeName, customerTypeCode, kasseNummer, forsikringstakerNummer);
                    cargo.fileNetDocumentField.kannelidUse = false;
                }
                cargo.fileNetDocumentField.indekseringsnokkelUse = (cargo.fileNetDocumentField.indekseringsnokkel != string.Empty);
                cargo.fileNetDocumentField.dokumentkode = GetSelectedDokBeskrivelseKode(item.Description);
                cargo.fileNetDocumentField.dokumentkodeUse = true;
                cargo.fileNetDocumentField.forsikringsgivernr = forsikringstakerNummer;
                cargo.fileNetDocumentField.forsikringsgivernrUse = true;
                cargo.fileNetDocumentField.indeksfilter = ConfigurationManager.AppSettings[KlpCrm.Constants.Web.CrmArchive.IndeksFilter];
                cargo.fileNetDocumentField.indeksfilterUse = true;
                cargo.fileNetDocumentField.skanneruid = ConfigurationManager.AppSettings[KlpCrm.Constants.Web.CrmArchive.SkannerUID];
                cargo.fileNetDocumentField.skanneruidUse = true;
                cargo.fileNetDocumentField.dokankomststatus = ConfigurationManager.AppSettings[KlpCrm.Constants.Web.CrmArchive.DokankomstStatus];
                cargo.fileNetDocumentField.dokankomststatusUse = true;

                cargo.fileNetDocument.objectstore = GetDepartmentText(department);
                cargo.fileNetDocument.folder = ConfigurationManager.AppSettings[KlpCrm.Constants.Web.CrmArchive.FileNetFolder];
                cargo.fileNetDocument.dokumentklasse = ConfigurationManager.AppSettings[KlpCrm.Constants.Web.CrmArchive.FileNetDokumentklasse];
                    //cargo.fileNetDocument.mimetype = mimeType != string.Empty ? mimeType : appConfig.Get(KlpCrm.Constants.Web.CrmArchive.FileNetMimetype);                

                fileNetCargo.Add(cargo);
        }
    }
}