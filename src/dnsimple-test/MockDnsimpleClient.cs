using System.Net;
using dnsimple;
using dnsimple.Services;
using Newtonsoft.Json.Linq;
using RestSharp;
using ICredentials = dnsimple.ICredentials;

namespace dnsimple_test
{
    public class MockDnsimpleClient : IClient
    {
        public string BaseUrl { get; } = "https://api.sandbox.dnsimple.com";
        public string Version { get; } = "v2";
        public DomainsService Domains { get; }
        public IdentityService Identity { get; }

        private string Fixture { get; }
        public HttpService Http { get; }
        public OAuth2Service OAuth { get; }
        public ZonesService Zones { get; }
        public AccountsService Accounts { get; }

        public MockDnsimpleClient(string fixture)
        {
            Fixture = fixture;

            Accounts = new AccountsService(this);
            Domains = new DomainsService(this);
            Http = new MockHttpService("v2", Fixture);
            Identity = new IdentityService(this);
            OAuth = new OAuth2Service(Http);
            Zones = new ZonesService(this);
        }


        public void StatusCode(HttpStatusCode statusCode)
        {
            ((MockHttpService)Http).StatusCode = statusCode;
        }
        
        public void ChangeBaseUrlTo(string baseUrl)
        {
            // Not needed at the moment, but having to implement...
        }

        public string VersionedBaseUrl()
        {
            // Not needed at the moment, but having to implement...
            return "";
        }

        public void AddCredentials(ICredentials credentials)
        {
            // Not needed at the moment, but having to implement...
        }
    }

    public class MockHttpService : HttpService
    {
        private readonly FixtureLoader _fixtureLoader;
        public HttpStatusCode StatusCode { get; set; }

        public MockHttpService(string version, string fixture)
        {
            _fixtureLoader = new FixtureLoader(version, fixture);
        }
        
        public override RequestBuilder RequestBuilder(string path)
        {
            return new RequestBuilder(path);
        }

        public override JToken Execute(IRestRequest request)
        {
            var rawPayload = _fixtureLoader.ExtractJsonPayload();

            string message;
            switch (StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new DnSimpleValidationException(JObject.Parse(rawPayload));
                case HttpStatusCode.NotImplemented:
                    message = JObject.Parse(rawPayload)["message"]?.ToString();
                    throw new DnSimpleException(message);
                case HttpStatusCode.GatewayTimeout:
                    message = JObject.Parse(rawPayload)["message"]?.ToString();
                    throw new DnSimpleException(message);
            }
            return !string.IsNullOrEmpty(rawPayload)
                ? JObject.Parse(rawPayload)
                : null;
        }
    }
}