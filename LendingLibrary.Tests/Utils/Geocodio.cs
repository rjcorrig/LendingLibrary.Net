using NUnit.Framework;
using Moq;
using System.Net.Http;
using LendingLibrary.Utils;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Text;

namespace LendingLibrary.Tests.Utils
{
    [TestFixture()]
    public class GeocodioTests
    {
        protected string invalidApiResponse = @"{ ""error"":""Invalid API key"" }";
        protected string goodApiResponse = @"{
  ""input"": {
    ""address_components"": {
      ""number"": ""1109"",
      ""predirectional"": ""N"",
      ""street"": ""Highland"",
      ""suffix"": ""St"",
      ""formatted_street"": ""N Highland St"",
      ""city"": ""Arlington"",
      ""state"": ""VA"",
      ""zip"": ""22201"",
      ""country"": ""US""
    },
    ""formatted_address"": ""1109 N Highland St, Arlington, VA 22201""
  },
  ""results"": [
    {
      ""address_components"": {
        ""number"": ""1109"",
        ""predirectional"": ""N"",
        ""street"": ""Highland"",
        ""suffix"": ""St"",
        ""formatted_street"": ""N Highland St"",
        ""city"": ""Arlington"",
        ""county"": ""Arlington County"",
        ""state"": ""VA"",
        ""zip"": ""22201"",
        ""country"": ""US""
      },
      ""formatted_address"": ""1109 N Highland St, Arlington, VA 22201"",
      ""location"": {
        ""lat"": 38.886665,
        ""lng"": -77.094733
      },
      ""accuracy"": 1,
      ""accuracy_type"": ""rooftop"",
      ""source"": ""Virginia GIS Clearinghouse""
    }
  ]
}";

        [Test()]
        public async Task Geocode_returns_null_on_Error()
        {
            var client = new Mock<HttpClient>();

            var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent(invalidApiResponse, Encoding.UTF8, "application/json")
            };

            client.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(response);

            var geo = new Geocodio("https://www.example.org", "foo", client.Object);
            var location = await geo.GeocodeAsync("foobar");

            Assert.IsNull(location);
        }

        [Test()]
        public async Task Geocode_returns_DbGeometry_on_Success()
        {
            var client = new Mock<HttpClient>();

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(goodApiResponse, Encoding.UTF8, "application/json")
            };

            client.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(response);

            var geo = new Geocodio("https://www.example.org", "foo", client.Object);
            var location = await geo.GeocodeAsync("foobar");

            Assert.IsNotNull(location);
            Assert.AreEqual(38.886665, location.Latitude);
            Assert.AreEqual(-77.094733, location.Longitude);
        }
    }
}
