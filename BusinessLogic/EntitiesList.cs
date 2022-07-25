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
    public class EntitiesList
    {
        public static EntityCollection ListEntities(Guid entityID, string entityTypeName)
        {
            EntityCollection result = new EntityCollection();

            switch (GetEntityEnumLogic.GetEntityEnum(entityTypeName))
            {
                case KlpCrm.Filenet.Common.Enums.EntityTypes.NONE:
                    //log.Debug("ListEntities: Type is NONE");
                    result = null;
                    break;
                case KlpCrm.Filenet.Common.Enums.EntityTypes.EMAIL:
                    //log.Debug("ListEntities: Type is EMAIL");
                    result = GetEmail(entityID);
                    break;
                case KlpCrm.Filenet.Common.Enums.EntityTypes.INCIDENT:
                    //log.Debug("ListEntities: Type is INCIDENT");
                    //check if incident has open activities. throws exeption if it has
                    if (IncidentHasOpenActivities(entityID) == false)
                    {
                        //result = this.GetIncident(entityID);
                        result = GetSak(entityID);
                    }
                    else
                    {
                        throw new Exception("Saken har åpne aktiviteter! Disse må lukkes før saken kan arkiveres.");
                    }
                    break;
                case KlpCrm.Filenet.Common.Enums.EntityTypes.ACTIVITYMIMEATTACHMENT:
                    //log.Debug("ListEntities: Type is ACTIVITYMIMEATTACHMENT");
                    result = GetEmailAttachments(entityID);
                    break;
                case KlpCrm.Filenet.Common.Enums.EntityTypes.EEG_SAK:
                    //log.Debug("ListEntities: Type is EEG_SAK");
                    if (!EegSakHasOpenActivities(entityID))
                    {
                        result = GetEegSak(entityID);
                    }
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }

        //this code doesnt really make sense since exactly the same logic is used in ListEntities method. Verify if it can b
        public static EntityCollection ListEntitiesForEegSak(Guid entityID, string entityTypeName)
        {
            EntityCollection result = new EntityCollection();

            switch (GetEntityEnumLogic.GetEntityEnum(entityTypeName))
            {

                case Common.Enums.EntityTypes.EEG_SAK:
                    if (!EegSakHasOpenActivities(entityID))
                    {
                        result = GetEegSak(entityID);
                    }
                    break;
            }

            return result;
        }

        private static EntityCollection GetEmail(Guid emailId)
        {
            var connection = new KlpCrmConnector.Connection(Config.CrmConnectionString);
            var service = connection.Service;

            Business.Entity businessEntity  = new Business.Entity("", "","");
            EntityCollection incidentEmails = new EntityCollection();

            incidentEmails.Entities.Add(businessEntity.GetEmail(emailId, service));

            return incidentEmails;
        }

        private static EntityCollection GetEmailAttachments(Guid emailId)
        {
            EntityCollection incidentEmailAttachments = new EntityCollection();

            var connection = new KlpCrmConnector.Connection(Config.CrmConnectionString);
            var service = connection.Service;

            Business.Entity businessEntity = new Business.Entity("", "", "");
            incidentEmailAttachments = businessEntity.GetEmailAttachments(emailId, service);

            return incidentEmailAttachments;
        }

        private static bool IncidentHasOpenActivities(Guid entityID)
        {
            Business.Archive businessArchive = new Business.Archive();
            try
            {
                businessArchive.IncidentHasOpenActivities(entityID);
                return false;
            }
            catch (Exception ex)
            {
                //add error message!!!
                return true;
            }
        }

        private static EntityCollection GetSak(Guid sakId)
        {
            EntityCollection resultSak = new EntityCollection();

            Incident incident = new Incident();
            EntityCollection incidentEmails = new EntityCollection();
            EntityCollection incidentEmailAttachments = new EntityCollection();
            Business.Entity businessEntity = new Business.Entity("", "", "");

            var connection = new KlpCrmConnector.Connection(Config.CrmConnectionString);
            var service = connection.Service;

            incident = service.Retrieve(Incident.EntityLogicalName, sakId, new ColumnSet(true)).ToEntity<Incident>();

            QueryByAttribute query = new QueryByAttribute();
            query.EntityName = Email.EntityLogicalName;
            query.ColumnSet = new ColumnSet(true);
            query.Attributes.AddRange("regardingobjectid");
            query.Values.AddRange(sakId);
            query.AddOrder("actualend", OrderType.Descending);
            query.PageInfo = new PagingInfo() { PageNumber = 1, Count = 50 };

            incidentEmails = service.RetrieveMultiple(query);

            foreach (Entity entity in incidentEmails.Entities)
            {
                resultSak.Entities.AddRange(businessEntity.GetEmailAttachments(entity.Id, service).Entities.ToList());
                resultSak.Entities.Add(entity);
            }

            return resultSak;
        }

        private static EntityCollection GetEegSak(Guid eegsakId)
        {
            EntityCollection resultEegSak = new EntityCollection();

            eeg_sak eegSak = new eeg_sak();
            EntityCollection eegsakEmails = new EntityCollection();
            EntityCollection eegsakEmailAttachments = new EntityCollection();
            EntityCollection eegsakTasks = new EntityCollection();
            Business.Entity businessEntity = new Business.Entity("", "", "");

            var connection = new KlpCrmConnector.Connection(Config.CrmConnectionString);
            var service = connection.Service;

            eegSak = service.Retrieve(eeg_sak.EntityLogicalName, eegsakId, new ColumnSet(true)).ToEntity<eeg_sak>();

            QueryByAttribute query = new QueryByAttribute();
            query.EntityName = Email.EntityLogicalName;
            query.ColumnSet = new ColumnSet(true);
            query.Attributes.AddRange("regardingobjectid");
            query.Values.AddRange(eegsakId);
            query.AddOrder("actualend", OrderType.Descending);
            query.PageInfo = new PagingInfo() { PageNumber = 1, Count = 50 };

            eegsakEmails = service.RetrieveMultiple(query);

            foreach (Entity entity in eegsakEmails.Entities)
            {
                resultEegSak.Entities.AddRange(businessEntity.GetEmailAttachmentsForEegSak(entity.Id, service).Entities.ToList());
                resultEegSak.Entities.Add(entity);
                //eegsakEmailAttachments.Entities.AddRange(businessEntity.GetEmailAttachmentsForEegSak(entity.Id, service).Entities.ToList());
            }

            //jaal
            QueryByAttribute taskQuery = new QueryByAttribute();
            taskQuery.EntityName = Task.EntityLogicalName;
            taskQuery.ColumnSet = new ColumnSet(true);
            taskQuery.Attributes.AddRange("regardingobjectid");
            taskQuery.Values.AddRange(eegsakId);
            taskQuery.PageInfo = new PagingInfo() { PageNumber = 1, Count = 50 };

            eegsakTasks = service.RetrieveMultiple(taskQuery);

            foreach (Entity entity in eegsakTasks.Entities)
            {
                //resultEegSak.Entities.AddRange(businessEntity.GetEmailAttachmentsForEegSak(entity.Id, service).Entities.ToList());
                resultEegSak.Entities.Add(entity);
                //eegsakEmailAttachments.Entities.AddRange(businessEntity.GetEmailAttachmentsForEegSak(entity.Id, service).Entities.ToList());
            }


            //if (eegSak != null)
            //{
            //result.Add(incident);
            //}
            //if (eegsakEmails != null)
            //{
            //    resultEegSak.Entities.AddRange(eegsakEmails.Entities);
            //}
            //if (eegsakEmailAttachments != null)
            //{
            //    resultEegSak.Entities.AddRange(eegsakEmailAttachments.Entities);
            //}

            return resultEegSak;
        }

        private static bool EegSakHasOpenActivities(Guid entityID)
        {
            Business.Archive businessArchive = new Business.Archive();
            try
            {
                businessArchive.EegSakHasOpenActivities(entityID);

                return false;
            }
            catch (Exception ex)
            {
                //add error message
                //this.DisplayErrorMessage(ex);
                //this.lstEntities.Enabled = false;

                return true;
            }
        }
    }
}