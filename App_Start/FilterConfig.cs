using System.Web;
using System.Web.Mvc;

namespace KlpCrm.Filenet.Web.ReactApplication
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
