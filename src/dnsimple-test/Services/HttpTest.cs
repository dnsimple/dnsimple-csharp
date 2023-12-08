using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using dnsimple;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class HttpTest
    {
        [Test]
        public void ReturnsARequestBuilder()
        {
            var http = new HttpService(new RestClient(), new RequestBuilder());
            Assert.That(http.RequestBuilder(""), Is.InstanceOf<RequestBuilder>());

        }

        [Test]
        public void InvalidRequest()
        {
            var client = new Mock<IRestClient>();
            var response = new Mock<IRestResponse>();
            var request = new Mock<IRestRequest>();
            var http = new HttpService(client.Object, new RequestBuilder());

            response.SetupProperty(mock => mock.StatusCode,
                HttpStatusCode.Unauthorized);
            response.Setup(mock => mock.IsSuccessful).Returns(false);
            response.Setup(mock => mock.Content)
                .Returns("{\"message\": \"Authentication failed\"}");

            client.Setup(mock => mock.Execute(request.Object))
                .Returns(response.Object);

            Assert.Throws(Is.TypeOf<AuthenticationException>()
                    .And.Message.EqualTo("Authentication failed"),
                delegate { http.Execute(request.Object); });
        }

        [Test]
        public void SetsTheHeaders()
        {
            var response = new MockDnsimpleClient("response.http").Identity.Whoami();

            Assert.Multiple(() =>
            {
                Assert.That(response.RateLimit, Is.EqualTo(4000));
                Assert.That(response.RateLimitRemaining, Is.EqualTo(3991));
                Assert.That(response.RateLimitReset, Is.EqualTo(1450451976));
            });
        }
    }

    [TestFixture]
    public class RequestBuilderTest
    {
        [SetUp]
        public void Setup()
        {
            _builder = new RequestBuilder("some/path");
        }

        [TearDown]
        public void TearDown()
        {
            _builder.Reset();
        }

        private RequestBuilder _builder;

        [Test]
        public void BuildsRequestWithCustomHeaders()
        {
            var headers = new Collection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Accept", "application/json"),
                new KeyValuePair<string, string>("Content-Type",
                    "application/x-www-form-urlencoded")
            };

            _builder.AddHeaders(headers);

            var request = _builder.Request;

            Assert.That(request.Parameters.Count, Is.EqualTo(2));
        }

        [Test]
        public void BuildsRequestWithOneParameter()
        {
            var parameter = new KeyValuePair<string, string>("sort", "name:desc");

            _builder.AddParameter(parameter);

            var request = _builder.Request;

            Assert.That(request.Parameters.Count, Is.EqualTo(1));
        }

        [Test]
        public void BuildsRequestWithParameters()
        {
            var parameters = new Collection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", "1"),
                new KeyValuePair<string, string>("client_secret", "secret"),
                new KeyValuePair<string, string>("grant_type",
                    "authorization_code"),
                new KeyValuePair<string, string>("code", "some_code")
            };

            _builder.AddParameters(parameters);

            var request = _builder.Request;

            Assert.That(request.Parameters.Count, Is.EqualTo(4));
        }

        [Test]
        public void WhenThereAreNoOptions()
        {
            _builder.AddParameters(new List<KeyValuePair<string, string>>());
            Assert.That(_builder.Request.Parameters.Count, Is.EqualTo(0));
        }

        [Test]
        public void BuildsRequestWithPath()
        {
            const string path = "some/other/path";
            _builder.Reset().AddPath(path);

            Assert.That(_builder.Request.Resource, Is.EqualTo(path));
        }

        [Test]
        public void Resets()
        {
            _builder.Method(Method.HEAD);

            Assert.That(_builder.Reset().Request, Is.Null);
        }

        [Test]
        public void SetsTheMethod()
        {
            _builder.Method(Method.POST);
            var request = _builder.Request;

            Assert.That(request.Method, Is.EqualTo(Method.POST));
        }

        [Test]
        public void AddsJsonPayloadToTheBody()
        {
            var record = new DelegationSignerRecord
            {
                Algorithm = "superMemo",
                Digest = "digest",
                DigestType = "type",
                Keytag = "keytag"
            };

            _builder.AddJsonPayload(record);
            var request = _builder.Request;

            Assert.That(JsonConvert
                    .DeserializeObject<DelegationSignerRecord>(
                        request.Parameters.First().Value.ToString()), Is.EqualTo(record));
        }
    }

    [TestFixture]
    public class PaginationTest
    {
        private JToken _jToken;

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", "pages-1of3.http");
            _jToken = JObject.Parse(loader.ExtractJsonPayload());
        }

        [Test]
        public void ReturnsAPaginationStruct()
        {
            var pagination = Pagination.From(_jToken);

            Assert.Multiple(() =>
            {
                Assert.That(pagination.CurrentPage, Is.EqualTo(1));
                Assert.That(pagination.PerPage, Is.EqualTo(2));
                Assert.That(pagination.TotalEntries, Is.EqualTo(5));
                Assert.That(pagination.TotalPages, Is.EqualTo(3));
            });
        }
    }
}
