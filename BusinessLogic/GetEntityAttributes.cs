using KlpCrm.Filenet.Library.Xrm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KlpCrm.Filenet.Web.ReactApplication.BusinessLogic
{
    public class GetEntityAttributes
    {
        public static string GetSubjectLeadingText(object obj, string entityTypeName)
        {
            string leadingText = "";
            if (GetEntityEnumLogic.GetEntityEnum(entityTypeName) != Common.Enums.EntityTypes.EEG_SAK)
            {
                Entity ent = (Entity)obj;
                //ExternalServices.Crm2011Service.Entity ent = (ExternalServices.Crm2011Service.Entity) obj;

                if (ent.LogicalName.ToUpper() == Common.Enums.EntityTypes.EMAIL.ToString().ToUpper())
                {
                    leadingText = "Epost"; //h3
                }
                else if (ent.LogicalName.ToUpper() ==
                         Common.Enums.EntityTypes.INCIDENT.ToString().ToUpper())
                {
                    leadingText = "Sak"; //h1
                }
                else if (ent.LogicalName.ToUpper() ==
                         Common.Enums.EntityTypes.ACTIVITYMIMEATTACHMENT.ToString().ToUpper())
                {
                    leadingText = "Vedlegg"; //h1
                }
            }
            else
            {

                if (obj.GetType() == typeof(Email))
                {
                    leadingText = "Epost"; //h3
                }
                else if (obj.GetType() == typeof(ActivityMimeAttachment))
                {
                    leadingText = leadingText = "Vedlegg"; //h1
                }
                else if (obj.GetType() == typeof(Task))
                {
                    leadingText = "Oppgave"; //h1
                }
            }

            return leadingText;
        }
        public static string GetEntitySubject(object obj, string entityTypeName = null)
        {
            string result = "";
            if (GetEntityEnumLogic.GetEntityEnum(entityTypeName) != Common.Enums.EntityTypes.EEG_SAK)
            {
                //ExternalServices.Crm2011Service.Entity ent = (ExternalServices.Crm2011Service.Entity) obj;
                Entity ent = (Entity)obj;
                if (ent.LogicalName == Common.Enums.EntityTypes.EMAIL.ToString().ToLower())
                {
                    if (ent.Contains(Constants.Crm.MailSubject))
                    {
                        result = ent[Constants.Crm.MailSubject].ToString();
                    }
                    else
                    {
                        result = "[mangler emne]";
                    }
                }

                if (ent.LogicalName == Common.Enums.EntityTypes.INCIDENT.ToString().ToLower())
                {
                    result = ent[Constants.Crm.IncidentTitle].ToString();
                }

                if (ent.LogicalName == Common.Enums.EntityTypes.TASK.ToString().ToLower())
                {
                    result = "PS Område: " + ent.FormattedValues["klp_psomrde"].ToString() + " " + ent[Constants.Crm.TaskSubject].ToString();
                }

                if (ent.LogicalName ==
                    Common.Enums.EntityTypes.ACTIVITYMIMEATTACHMENT.ToString().ToLower())
                {
                    if (ent.Contains(Constants.Crm.ActivityMimeAttachment_FileName))
                    {
                        result = ent[Constants.Crm.ActivityMimeAttachment_FileName].ToString();
                    }
                    else
                    {
                        result = "[mangler emne]";
                    }
                }
            }
            else
            {
                if (obj.GetType() == typeof(Email))
                {
                    Email email = (Email)obj;
                    if (!string.IsNullOrEmpty(email.Subject))
                    {
                        result = email.Subject;
                    }
                    else
                    {
                        result = "[mangler emne]";
                    }
                }
                else if (obj.GetType() == typeof(ActivityMimeAttachment))
                {
                    ActivityMimeAttachment activityMimeAttachment = (ActivityMimeAttachment)obj;
                    if (!string.IsNullOrEmpty(activityMimeAttachment.FileName))
                    {
                        result = activityMimeAttachment.FileName;
                    }
                    else
                    {
                        result = "[mangler emne]";
                    }
                }
                else if (obj.GetType() == typeof(Task))
                {

                    Entity ent = (Entity)obj;
                    var psOmråde = "";
                    if (ent.FormattedValues.Contains("klp_psomrde"))
                    {
                        psOmråde = ent.FormattedValues["klp_psomrde"].ToString();
                    }
                    result = "PS Område: " + psOmråde + " Emne: " + ent[Constants.Crm.TaskSubject].ToString();
                    //result = ent[KlpCrm.Constants.Crm.TaskSubject].ToString();

                }
            }
            return result;
        }

        public static string GetEntityFra(object obj)
        {
            var connection = new KlpCrmConnector.Connection(Config.CrmConnectionString);
            var service = connection.Service;

            string result = "";
            if (obj.GetType() == typeof(Email))
            {
                Email email = (Email)obj;
                if (email.From != null)
                {
                    result += GetEntityTilFraAttributeValueString(email.From);
                }
                else
                {
                    result = "[mangler fra]";
                }
            }
            else if (obj.GetType() == typeof(ActivityMimeAttachment))
            {
                ActivityMimeAttachment activityMimeAttachment = (ActivityMimeAttachment)obj;
                Email email =
                    service.Retrieve(Email.EntityLogicalName, activityMimeAttachment.GetAttributeValue<EntityReference>("activityid").Id, new ColumnSet("from")).ToEntity<Email>();
                if (email.From != null)
                {
                    result += GetEntityTilFraAttributeValueString(email.From);
                }
                else
                {
                    result = "[mangler fra]";
                }
            }
            else if (obj.GetType() == typeof(Task))
            {
                Entity ent = (Entity)obj;
                result = ent.GetAttributeValue<EntityReference>("createdby").Name;
            }
            else
            {
                ExternalServices.Crm2011Service.Entity ent = (ExternalServices.Crm2011Service.Entity)obj;

                if (ent.LogicalName == Common.Enums.EntityTypes.EMAIL.ToString().ToLower())
                {
                    Email email = service.Retrieve(Email.EntityLogicalName, ent.Id, new ColumnSet("from", "sender")).ToEntity<Email>();
                    if (email.From != null)
                    {
                        result += GetEntityTilFraAttributeValueString(email.From);

                    }
                    else
                    {
                        result = "[mangler fra]";
                    }
                }
                else if (ent.LogicalName == Common.Enums.EntityTypes.ACTIVITYMIMEATTACHMENT.ToString().ToLower())
                {
                    ActivityMimeAttachment activityMimeAttachment = service.Retrieve(ActivityMimeAttachment.EntityLogicalName, ent.Id, new ColumnSet("activityid")).ToEntity<ActivityMimeAttachment>();

                    Email email =
                        service.Retrieve(Email.EntityLogicalName, activityMimeAttachment.GetAttributeValue<EntityReference>("activityid").Id,
                        new ColumnSet("from")).ToEntity<Email>();

                    if (email.From != null)
                    {
                        result += GetEntityTilFraAttributeValueString(email.From);

                    }
                    else
                    {
                        result = "[mangler fra]";
                    }
                }
            }

            return result;
        }

        public static string GetEntityTil(object obj)
        {
            var connection = new KlpCrmConnector.Connection(Config.CrmConnectionString);
            var service = connection.Service;
           
            string result = "";
            if (obj.GetType() == typeof(Email))
            {
                Email email = (Email)obj;
                if (email.To != null)
                {
                    result += GetEntityTilFraAttributeValueString(email.To);
                }
                else
                {
                    result = "[mangler til]";
                }
            }
            else if (obj.GetType() == typeof(ActivityMimeAttachment))
            {
                ActivityMimeAttachment activityMimeAttachment = (ActivityMimeAttachment)obj;


                Email email =
                    service.Retrieve(Email.EntityLogicalName, activityMimeAttachment.GetAttributeValue<EntityReference>("activityid").Id, new ColumnSet("to", "torecipients")).ToEntity<Email>();
                if (email.To != null)
                {
                    result += GetEntityTilFraAttributeValueString(email.To);
                }
                else
                {
                    result = "[mangler til]";
                }
            }
            else if (obj.GetType() == typeof(Task))
            {
                Entity ent = (Entity)obj;
                result = ent.GetAttributeValue<EntityReference>("ownerid").Name;
            }
            else
            {
                ExternalServices.Crm2011Service.Entity ent = (ExternalServices.Crm2011Service.Entity)obj;

                if (ent.LogicalName == Common.Enums.EntityTypes.EMAIL.ToString().ToLower())
                {
                    Email email = service.Retrieve(Email.EntityLogicalName, ent.Id, new ColumnSet("to", "torecipients")).ToEntity<Email>();
                    if (email.To != null)
                    {
                        result += GetEntityTilFraAttributeValueString(email.To);

                    }
                    else
                    {
                        result = "[mangler til]";
                    }
                }
                else if (ent.LogicalName == Common.Enums.EntityTypes.ACTIVITYMIMEATTACHMENT.ToString().ToLower())
                {
                    ActivityMimeAttachment activityMimeAttachment = service.Retrieve(ActivityMimeAttachment.EntityLogicalName, ent.Id, new ColumnSet("activityid")).ToEntity<ActivityMimeAttachment>();

                    Email email =
                        service.Retrieve(Email.EntityLogicalName, activityMimeAttachment.Id,
                        new ColumnSet("to", "torecipients")).ToEntity<Email>();

                    if (email.To != null)
                    {
                        result += GetEntityTilFraAttributeValueString(email.To);
                    }
                    else
                    {
                        result = "[mangler til]";
                    }
                }
            }
            return result;
        }

        public static string GetEntityActualEnd(object obj)
        {
            var connection = new KlpCrmConnector.Connection(Config.CrmConnectionString);
            var service = connection.Service;

            string result = "";
            if (obj.GetType() == typeof(Email))
            {
                Email email = (Email)obj;
                if (email.ActualEnd != null && !string.IsNullOrEmpty(email.ActualEnd.Value.ToLocalTime().ToString()))
                {
                    result = email.ActualEnd.Value.ToLocalTime().ToString();
                }
                else
                {
                    result = "[Ikke tilgjengelig]";
                }
            }
            else if (obj.GetType() == typeof(ActivityMimeAttachment))
            {
                ActivityMimeAttachment activityMimeAttachment = (ActivityMimeAttachment)obj;

                Email email =
                    service.Retrieve(Email.EntityLogicalName, activityMimeAttachment.GetAttributeValue<EntityReference>("activityid").Id, new ColumnSet("actualend")).ToEntity<Email>();

                if (email.ActualEnd != null && !string.IsNullOrEmpty(email.ActualEnd.Value.ToLocalTime().ToString()))
                {
                    result = email.ActualEnd.Value.ToLocalTime().ToString();
                }
                else
                {
                    result = "[Ikke tilgjengelig]";

                }

            }
            else if (obj.GetType() == typeof(Task))
            {
                Task task = (Task)obj;
                task =
                        service.Retrieve(Task.EntityLogicalName, task.Id, new ColumnSet("actualend")).ToEntity<Task>();

                if (task.ActualEnd != null && !string.IsNullOrEmpty(task.ActualEnd.Value.ToLocalTime().ToString()))
                {
                    result = task.ActualEnd.Value.ToLocalTime().ToString();
                }
                else
                {
                    result = "";

                }
            }
            else
            {
                ExternalServices.Crm2011Service.Entity ent = (ExternalServices.Crm2011Service.Entity)obj;

                if (ent.LogicalName == KlpCrm.Filenet.Common.Enums.EntityTypes.EMAIL.ToString().ToLower())
                {
                    KlpCrm.Filenet.Library.Xrm.Email email = service.Retrieve(KlpCrm.Filenet.Library.Xrm.Email.EntityLogicalName, ent.Id, new ColumnSet("actualend")).ToEntity<KlpCrm.Filenet.Library.Xrm.Email>();
                    if (email.ActualEnd != null && !string.IsNullOrEmpty(email.ActualEnd.Value.ToLocalTime().ToString()))
                    {
                        result = email.ActualEnd.Value.ToLocalTime().ToString();
                    }
                    else
                    {
                        result = "[Ikke tilgjengelig]";

                    }
                }
                else if (ent.LogicalName == KlpCrm.Filenet.Common.Enums.EntityTypes.ACTIVITYMIMEATTACHMENT.ToString().ToLower())
                {
                    KlpCrm.Filenet.Library.Xrm.ActivityMimeAttachment activityMimeAttachment = service.Retrieve(ActivityMimeAttachment.EntityLogicalName, ent.Id, new ColumnSet("activityid")).ToEntity<ActivityMimeAttachment>();

                    KlpCrm.Filenet.Library.Xrm.Email email =
                        service.Retrieve(KlpCrm.Filenet.Library.Xrm.Email.EntityLogicalName, activityMimeAttachment.Id,
                        new ColumnSet("actualend")).ToEntity<KlpCrm.Filenet.Library.Xrm.Email>();

                    if (email.ActualEnd != null && !string.IsNullOrEmpty(email.ActualEnd.Value.ToLocalTime().ToString()))
                    {
                        result = email.ActualEnd.Value.ToLocalTime().ToString();
                    }
                    else
                    {
                        result = "[Ikke tilgjengelig]";

                    }
                }
            }

            return result;
        }

        ///<summary>
        ///Disable Arkiveres checkbox if attachment is not pdf.
        ///</summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ArkiveresEnabled(object obj)
        {
            Type t = obj.GetType();
            string emneValue = GetEntitySubject(obj);
            bool? returnValue = null;

            if (t == typeof(ActivityMimeAttachment))
            {
                if (emneValue.LastIndexOf(".") != -1)
                {
                    foreach (string s in Library.MimeTypeFactory.supportedTypes)
                    {
                        if (emneValue.EndsWith(s, StringComparison.InvariantCultureIgnoreCase))
                        {
                            returnValue = true;
                            break;
                        }
                    }
                    if (returnValue == null)
                        returnValue = false;
                }
            }
            else if (t == typeof(Task))
            {
                returnValue = true;
            }
            else if (t != typeof(Email))
            {
                ExternalServices.Crm2011Service.Entity ent = (ExternalServices.Crm2011Service.Entity)obj;
                if (ent.LogicalName == Common.Enums.EntityTypes.ACTIVITYMIMEATTACHMENT.ToString().ToLower())
                {
                    if (emneValue.LastIndexOf(".") != -1)
                    {
                        foreach (string s in Library.MimeTypeFactory.supportedTypes)
                        {
                            if (emneValue.EndsWith(s, StringComparison.InvariantCultureIgnoreCase))
                            {
                                returnValue = true;
                                break;
                            }
                        }
                        if (returnValue == null)
                            returnValue = false;
                    }
                }
            }

            return returnValue == null ? true : returnValue.Value;
        }

        public static string GetEntityTilFraAttributeValueString(IEnumerable<ActivityParty> entityList)
        {
            string result = "";
            foreach (Entity entity in entityList)
            {
                result += entity.GetAttributeValue<EntityReference>("partyid") != null ? entity.GetAttributeValue<EntityReference>("partyid").Name :
                                    entity.GetAttributeValue<string>("addressused") + "; ";
            }
            return result;
        }
    }
}