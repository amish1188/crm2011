using KlpCrm.Filenet.Web.ReactApplication.Filters;
using System.Net.Http.Headers;
using System.Web.Http;

namespace KlpCrm.Filenet.Web.ReactApplication
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.Filters.Add(new ValidateModelAttribute());
            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html"));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}