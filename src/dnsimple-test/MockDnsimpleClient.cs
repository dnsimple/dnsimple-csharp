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
        public AccountsService Accounts { get; }

        public MockDnsimpleClient(string fixture)
        {
            Fixture = fixture;

            Accounts = new AccountsService(this);
            Domains = new DomainsService(this);
            Http = new MockHttpService("v2", Fixture);
            Identity = new IdentityService(this);
            OAuth = new OAuth2Service(Http);
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
            var payload = _fixtureLoader.ExtractJsonPayload();

            if (StatusCode != HttpStatusCode.NotImplemented)
                return !string.IsNullOrEmpty(payload)
                    ? JObject.Parse(payload)
                    : null;
            var message = JObject.Parse(payload)["message"].ToString();
            throw new DnSimpleException(message);
        }
    }
}