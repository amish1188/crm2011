using KlpCrm.Constants;
using KlpCrm.Filenet.Common.Enums;
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
    public class ForsikringsTakerLogic
    {
        public static List<Klp_forsikringstakernr> ListForsikrinsgtakernumre(Guid entityId, string entityTypeName)
        {
            var connection = new KlpCrmConnector.Connection(Config.CrmConnectionString);
            var service = connection.Service;

            List<Klp_forsikringstakernr> resultForsikringstakernr = new List<Klp_forsikringstakernr>();
            Incident _incident = null;


            switch (GetEntityEnumLogic.GetEntityEnum(entityTypeName))
            {
                case EntityTypes.INCIDENT:
                    _incident = service.Retrieve(Incident.EntityLogicalName, entityId, new ColumnSet(true)).ToEntity<Incident>();
                    break;
                default:
                    resultForsikringstakernr = null;
                    break;
            }

            if (_incident != null && _incident.CustomerId != null && _incident.CustomerId.LogicalName == Account.EntityLogicalName)
            {
                Account account = service.Retrieve(Account.EntityLogicalName, _incident.CustomerId.Id,
                                            new ColumnSet(Crm.Account_AccountId, Crm.Account_CustomerTypeCode, Crm.Account_KlpOrganisasjonsnummer)).
                        ToEntity<Account>();

                // Get forsikringstakernummer
                if (account.AccountId != null)
                    resultForsikringstakernr = GetForsikringstakernummer(account.AccountId.Value.ToString(), service);
            }

            return resultForsikringstakernr;
        }

        private static List<Klp_forsikringstakernr> GetForsikringstakernummer(string accountid, IOrganizationService service)
        {
            QueryByAttribute query = new QueryByAttribute();
            query.EntityName = Klp_forsikringstakernr.EntityLogicalName;
            query.ColumnSet = new ColumnSet(true);
            query.Attributes.AddRange("klp_kundeid", "statecode");
            query.Values.AddRange(accountid, 0);

            EntityCollection entityCollection = service.RetrieveMultiple(query);

            return entityCollection.Entities.Cast<Klp_forsikringstakernr>().ToList();

        }
    }
}