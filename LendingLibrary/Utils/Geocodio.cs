using System;
using System.Data.Entity.Spatial;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LendingLibrary.Models;
using Newtonsoft.Json.Linq;
using Unity.Attributes;
using static System.Web.Configuration.WebConfigurationManager;

namespace LendingLibrary.Utils
{
    public class Geocodio : IGeocoder, IDisposable
    {
        protected readonly string apiKey;
        protected readonly Uri apiUrl;
        protected readonly HttpClient client;

        [InjectionConstructor]
        public Geocodio() : this(AppSettings["geocodio-apiurl"], AppSettings["geocodio-apikey"], new HttpClient())
        {
        }

        public Geocodio(HttpClient client): this(AppSettings["geocodio-apiurl"], AppSettings["geocodio-apikey"], client)
        {
        }

        public Geocodio(string apiUrl, string apiKey) : this(apiUrl, apiKey, new HttpClient())
        {
        }

        public Geocodio(string apiUrl, string apiKey, HttpClient client)
        {
            this.apiKey = apiKey;
            this.apiUrl = new Uri(apiUrl);
            this.client = client;
        }

        public virtual async Task<GeoPoint> GeocodeAsync(string address)
        {
            var result = default(GeoPoint);

            var builder = new UriBuilder(apiUrl);
            builder.Port = -1;

            var query = HttpUtility.ParseQueryString(builder.Query);
            query["q"] = address;
            query["api_key"] = apiKey;

            builder.Query = query.ToString();

            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, builder.ToString()))
            {
                httpRequest.Headers.Add("Accept", "application/json");

                using (var httpResponse = await client.SendAsync(httpRequest, default(CancellationToken)))
                {
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var body = await httpResponse.Content.ReadAsStringAsync();
                        var response = JObject.Parse(body);

                        var location = response["results"]?[0]?["location"];

                        if (location != null)
                        {
                            var lat = location?["lat"]?.Value<double>();
                            var lng = location?["lng"]?.Value<double>();

                            if (lat.HasValue && lng.HasValue)
                            {
                                return new GeoPoint(lat.Value, lng.Value);
                            }
                        }
                    }
                }
            }

            return result;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    client.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
