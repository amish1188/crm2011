using KlpCrm.Filenet.Common.Enums;
using KlpCrm.Filenet.Library.Xrm;
using KlpCrm.Filenet.Web.ReactApplication.BusinessLogic;
using KlpCrm.Filenet.Web.ReactApplication.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace KlpCrm.Filenet.Web.ReactApplication.Controllers
{
    [AllowCrossSiteJson]
    public class CrmArchiveApiController : ApiController
    {
        // GET: CrmArchiveApi
        [HttpGet]
        public Organizations GetOrgNumAndInsuranceTakerNum(string id, string typename)
        {
            Incident incident;
            //List<ExternalServices.Crm2011Service.Entity> incidentEmails;
            EntityCollection incidentEmails;
            //List<ExternalServices.Crm2011Service.Entity> incidentEmailAttachments;
            EntityCollection incidentEmailAttachments;

            eeg_sak eegSak;
            EntityCollection eegsakEmailAttachments;
            EntityCollection eegsakEmails;
            EntityCollection eegsakTasks;
            Organizations organizations = new Organizations();
            if (id != null)
            {
                string organisasjonellerfnr;
                List<Klp_forsikringstakernr> resultForsikringstakernr = new List<Klp_forsikringstakernr>();
                if (GetEntityEnumLogic.GetEntityEnum(typename) != EntityTypes.EEG_SAK)
                {
                    resultForsikringstakernr = ForsikringsTakerLogic.ListForsikrinsgtakernumre(Guid.Parse(id), typename);
                }
                organisasjonellerfnr = OrgNumber.GetOrgNrOrFnrFromIncident(Guid.Parse(id), typename);

                if(resultForsikringstakernr.Count > 0)
                {
                    foreach (Klp_forsikringstakernr forsikringstakernr in resultForsikringstakernr)
                    {
                        if (organizations.OrganizationsList == null || !organizations.OrganizationsList.Contains(forsikringstakernr.Klp_Forsikringstakernummer))
                            organizations.OrganizationsList.Add(forsikringstakernr.Klp_Forsikringstakernummer);
                    }
                }
                organizations.OrganizationsList.Add(organisasjonellerfnr);
            };
            
            return organizations;
        }

        [HttpPost]
        public ListDictionary GetKasseNumberItems([FromBody] GetKasseNumberListInput getKasseNumberList)
        {
            ListDictionary listItems = new ListDictionary();
            var dupa = getKasseNumberList.ForsikringstakerNummer.Trim().Length;
            if (getKasseNumberList.ForsikringstakerNummer.Trim().Length != 9 && getKasseNumberList.ForsikringstakerNummer.Trim().Length != 11)
            {
                //listItems = GetAvailableKasseNrLogic.setKasseNumbersValues(getKasseNumberList.ForsikringstakerNummer, getKasseNumberList.Department, getKasseNumberList.Description);
                
                //main method(the one commented above) doesnt work cause of PostfordelingServiceClient throwing errors on connections so i used dummy data
                listItems.Add("KS00 Gruppeliv/Læregruppeliv", "00");
                listItems.Add("KS10 Felles pensjonsordning for fylkeskommuner", "10");
                listItems.Add("KS15 Pensjonsordning for sykehusleger", "15");
            }
            return listItems;
                
        }


        [HttpGet]
        public Attachment GetAttachmentView(string id)
        {
            var connection = new KlpCrmConnector.Connection(Config.CrmConnectionString);
            var service = connection.Service;

           var response = service.Retrieve(ActivityMimeAttachment.EntityLogicalName, Guid.Parse(id), new ColumnSet(true)).ToEntity<ActivityMimeAttachment>();
           //var res = JsonConvert.DeserializeObject<ActivityMimeAttachment>(attachment);
           Attachment attachment = new Attachment();
            attachment.Id = response.Id.ToString();
            attachment.Body = response.Body;
            return attachment;
        } 

        //check if not sak

        [HttpGet]
        public object GetEntitiesList(string id, string typename)
        {
            try
            {
                if (GetEntityEnumLogic.GetEntityEnum(typename) != EntityTypes.EEG_SAK)
                {
                    EntityCollection result = EntitiesList.ListEntities(Guid.Parse(id), typename);
                    List<Entity> entityList = result.Entities.ToList();
                    List<EntityAttributes> entityAttributesList = new List<EntityAttributes>();
                    foreach(Entity entity in entityList)
                    {
                        EntityAttributes entityAttributes = new EntityAttributes();
                        if (entity.LogicalName == "activitymimeattachment")
                        {
                            ActivityMimeAttachment att = new ActivityMimeAttachment();
                            att = (ActivityMimeAttachment)entity;
                            entityAttributes.ActivityId = att.ActivityId.Id;
                        }
                        entityAttributes.Id = entity.Id;
                        entityAttributes.LeadingText = GetEntityAttributes.GetSubjectLeadingText(entity, typename);
                        entityAttributes.Subject = GetEntityAttributes.GetEntitySubject(entity);
                        entityAttributes.From = GetEntityAttributes.GetEntityFra(entity);
                        entityAttributes.To = GetEntityAttributes.GetEntityTil(entity);
                        entityAttributes.ActualEnd = GetEntityAttributes.GetEntityActualEnd(entity);
                        entityAttributes.ArchivingEnabled = GetEntityAttributes.ArkiveresEnabled(entity);
                        entityAttributesList.Add(entityAttributes);
                    }
                    return entityAttributesList;
                }
                else
                {
                    //havent tested this one since i could test only incident typename. most likely it has to be mapped into sth similar as above
                    //to get the right result in the frontend
                    EntityCollection resultEegSak = EntitiesList.ListEntitiesForEegSak(Guid.Parse(id), typename);
                    List<Entity> entityList = resultEegSak.Entities.ToList();
                    var entityToString = JsonConvert.SerializeObject(entityList);
                    var des = JsonConvert.DeserializeObject(entityToString);
                    return des;
                }
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //get dokument beksirvelse
        [HttpPost]
        public IEnumerable<string> GetDescription(string id, string typename, int TypeCode, [FromBody]GetDescriptionInput obj)
        {
            //string[] names = GetDescriptionLogic.GetNames(obj.Typed, 25, "");
            //return GetDescriptionLogic.SetDokBeskrivelseContextKeyMainTxtDokBesk(id, typename, TypeCode, obj.Department, obj.ForsikringstakerNummer, obj.Kassenummer, names);

            string[] dummyNames = { "Marcin", "Rino", "Pepe", "Johan", "Thomas", "Tor" };
            return Array.FindAll(dummyNames, s => s.Contains(obj.Typed));

        }

        [HttpPost]
        public System.Web.Http.Results.OkNegotiatedContentResult<ItemToArchive> SendToSos(string id, string typename, int TypeCode, [FromBody]ItemToArchive itemsToArchive)
        {
            List<Common.CargoObjects.FileNetCargo> fileNetCargo = new List<Common.CargoObjects.FileNetCargo>();

            if (itemsToArchive.Entities.Count > 0)
            {
                foreach (var item in itemsToArchive.Entities)
                {
                    string fileExtension = string.Empty;
                    if (item.Subject.LastIndexOf(".") != -1)
                    {
                        fileExtension = item.Subject.Substring(item.Subject.LastIndexOf("."));
                    }
                    
                    string fileMimeType = Library.MimeTypeFactory.getMimeType(fileExtension);
                    SendToArchive.SendToSos(
                        fileNetCargo, 
                        item, 
                        fileMimeType, 
                        Guid.Parse(id), 
                        typename, 
                        TypeCode, 
                        itemsToArchive.Department,
                        itemsToArchive.KasseNummer, 
                        itemsToArchive.ForsikringstakerNummer
                        );
                }
            }

            //det blir feil med connections hvis man kjører archive metoder
            /*SendToArchive.WrapItUp(
                typename, 
                fileNetCargo, 
                id, 
                TypeCode, 
                itemsToArchive.KasseNummer, 
                itemsToArchive.ForsikringstakerNummer, 
                itemsToArchive.Description);
            //logger.Info("isValid"); */
            return Ok(itemsToArchive);
        }
    }
}