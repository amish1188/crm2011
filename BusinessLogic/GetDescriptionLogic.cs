using ExternalServices.FileNetServiceAddOns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KlpCrm.Filenet.Web.ReactApplication.BusinessLogic
{

   
    public class GetDescriptionLogic
    {
        public static string[] GetNames(string prefixText, int count, string contextKey)
        {
            KlpCrm.Filenet.Web.Business.Archive businessFileNet = new Business.Archive();
            HentDokumentBeskrivelserRequest request = new HentDokumentBeskrivelserRequest();
            Dictionary<string, string> dokBesk = new Dictionary<string, string>();
            Dictionary<string, string> cleanResult = new Dictionary<string, string>();

            //initialize request object
            request = new ExternalServices.FileNetServiceAddOns.HentDokumentBeskrivelserRequest();
            request.dokumentfilter = new ExternalServices.FileNetServiceAddOns.DokumentFilter();

            string[] parameters = contextKey.Split(',');

            string forsikringsgiver = parameters[0];
            string fodselsnummer = parameters[1];
            string forsikringstakernummer = parameters[2];
            string organisasjonsnummer = parameters[3];

            //get dokumentbeskrivelser
            request.dokumentfilter.forsikringsgiver = forsikringsgiver;
            request.dokumentfilter.fodselsnummer = fodselsnummer;
            request.dokumentfilter.forsikringstakernummer = forsikringstakernummer;
            request.dokumentfilter.organisasjonsnummer = organisasjonsnummer;

            dokBesk = businessFileNet.GetDokumentkoder(request);

            foreach (var item in dokBesk)
            {
                if (item.Value != null && item.Key != null)
                {
                    System.Diagnostics.Trace.WriteLine("Added key: " + item.Key + ", value=" + item.Value);
                    if (item.Key.ToLower().Contains(prefixText.ToLower()) || item.Value.ToLower().Contains(prefixText.ToLower()))
                    {
                        cleanResult.Add(item.Key, item.Value);
                    }
                }
            }

            if (cleanResult.Count < count)
            {
                count = cleanResult.Count;
            }

            string[] result = new string[count];
            int itemCounter = 0;

            foreach (var item in cleanResult)
            {
                if (itemCounter > (count - 1))
                    break;

                result[itemCounter] = item.Value + "(" + item.Key + ")";
                itemCounter += 1;
            }

            return result;
        }

        public static string SetDokBeskrivelseContextKeyMainTxtDokBesk(string id, string typename,int TypeCode, string department, string organisationNumber, string kasseNummer, string[] names)
        {
           
            //string ftakernrValg = this.rbListForsikringstakernr.SelectedItem.Text;
            //we wont use it any longer i guess. names should get returned in GetNames method and then modify the result in this method
            //AjaxControlToolkit.AutoCompleteExtender ace = (AjaxControlToolkit.AutoCompleteExtender)caller.FindControl(KlpCrm.Constants.Web.CrmArchive.AjaxDokBeskrivelse);
             
            string forsikringsgiverValue = department;
            //const string forsikringsgiver = "1000";
            string fodselsnummer = string.Empty;
            string forsikringstakernummer = string.Empty;
            string organisasjonsnummer = string.Empty;

            // Personkunde
            if (TypeCode == 2 && organisationNumber != "-")
            {
                fodselsnummer = SendToArchive.GetIncidentsCustomerSocSecNmbOrOrgNmb(Guid.Parse(id), typename, TypeCode, kasseNummer, organisationNumber);
            }
            else if (TypeCode == 1)
            {
                if (organisationNumber != null && organisationNumber.Length != 9)
                    forsikringstakernummer = SendToArchive.GetIncidentsCustomerSocSecNmbOrOrgNmb(Guid.Parse(id), typename, TypeCode, kasseNummer, organisationNumber);
                if (organisationNumber != null && organisationNumber.Length == 9)
                    organisasjonsnummer = SendToArchive.GetIncidentsCustomerSocSecNmbOrOrgNmb(Guid.Parse(id), typename, TypeCode, kasseNummer, organisationNumber);
            }

            //bruk "names" istedenfor men har ikke noe peiling hva som er greia med ContextKey
            //ace.ContextKey = forsikringsgiverValue + "," + fodselsnummer + "," + forsikringstakernummer + "," + organisasjonsnummer;

            return "";
        }
    }
}