using System.Web.Http;

namespace LendingLibrary
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Serialize to JSON
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            json.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "Books",
                routeTemplate: "api/books/{id}",
                defaults: new { controller = "BooksApi", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var container = UnityConfig.Container;
            config.DependencyResolver = new UnityResolver(container);
        }
    }
}
