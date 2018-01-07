using System;
using System.Data.Entity.Spatial;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using static System.Web.Configuration.WebConfigurationManager;

namespace LendingLibrary.Utils
{
    public class Geocodio : IGeocoder
    {
        protected readonly string apiKey;
        protected readonly Uri apiUrl;

        public Geocodio() : this(AppSettings["geocodio-url"], AppSettings["geocodio-apikey"])
        {
        }

        public Geocodio(string apiUrl, string apiKey)
        {
            this.apiKey = apiKey;
            this.apiUrl = new Uri(apiUrl);
        }

        public async Task<DbGeography> GeocodeAsync(string address)
        {
            DbGeography result = default(DbGeography);

            using (var httpClient = new HttpClient())
            {
                var builder = new UriBuilder(apiUrl);
                builder.Port = -1;
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["q"] = address;
                query["api_key"] = apiKey;
                builder.Query = query.ToString();

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, builder.ToString()))
                {
                    httpRequest.Headers.Add("Accept", "application/json");
                    using (var httpResponse = await httpClient.SendAsync(httpRequest)) 
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            var body = await httpResponse.Content.ReadAsStringAsync();
                            var response = JObject.Parse(body);

                            var location = response["results"]?[0]?["location"];

                            if (location != null) {
                                var lat = location?["lng"]?.Value<double>();
                                var lng = location?["lng"]?.Value<double>();

                                if (lat.HasValue && lng.HasValue) {
								    return DbGeography.FromText($"POINT ({lat} {lng}");
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
