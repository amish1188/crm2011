using KlpCrm.Filenet.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KlpCrm.Filenet.Web.ReactApplication.BusinessLogic
{
    public class GetEntityEnumLogic
    {
        public static EntityTypes GetEntityEnum(string entityTypeName)
        {
            EntityTypes result = EntityTypes.NONE;

            if (!string.IsNullOrEmpty(entityTypeName))
            {
                result = (EntityTypes)Enum.Parse(typeof(EntityTypes), entityTypeName, true);
            }

            return result;
        }
    }
}