using dnsimple;
using dnsimple.Services;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace dnsimple_test
{
    public class MockDnsimpleClient : IClient
    {
        public string BaseUrl { get; } = "https://api.sandbox.dnsimple.com";
        public string Version { get; } = "v2";
        public IdentityService Identity { get; }

        private string Fixture { get; }
        public HttpService Http { get; }
        public OAuth2Service OAuth { get; }
        public AccountsService Accounts { get; }

        public MockDnsimpleClient(string fixture)
        {
            Fixture = fixture;

            Identity = new IdentityService(this);
            Http = new MockHttpService("v2", Fixture);
            OAuth = new OAuth2Service(Http);
            Accounts = new AccountsService(this);
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

        public MockHttpService(string version, string fixture)
        {
            _fixtureLoader = new FixtureLoader(version);
            Fixture = fixture;
        }

        private string Fixture { get; }

        public override RequestBuilder RequestBuilder(string path)
        {
            return new RequestBuilder(path);
        }

        public override JToken Execute(RestRequest request)
        {
            return JObject.Parse(_fixtureLoader.JsonPartFrom(Fixture));
        }
    }
}