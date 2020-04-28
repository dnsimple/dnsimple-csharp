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
        private string Fixture { get; }

        public AccountsService Accounts { get; }
        public CertificatesService Certificates { get; }
        public DomainsService Domains { get; }
        public HttpService Http { get; }
        public IdentityService Identity { get; }
        public OAuth2Service OAuth { get; }
        public RegistrarService Registrar { get; set; }
        public TldsService Tlds { get; set; }
        public ZonesService Zones { get; }

        public MockDnsimpleClient(string fixture)
        {
            Fixture = fixture;

            Accounts = new AccountsService(this);
            Certificates = new CertificatesService(this);
            Domains = new DomainsService(this);
            Http = new MockHttpService("v2", Fixture, BaseUrl);
            Identity = new IdentityService(this);
            OAuth = new OAuth2Service(Http);
            Registrar = new RegistrarService(this);
            Tlds = new TldsService(this);
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

        public string RequestSentTo()
        {
            return ((MockHttpService) Http).RequestUrlSent;
        }

        public Method HttpMethodUsed()
        {
            return ((MockHttpService) Http).MethodSent;
        }
    }

    public class MockHttpService : HttpService
    {
        private readonly FixtureLoader _fixtureLoader;
        private readonly string _baseUrl;
        
        public HttpStatusCode StatusCode { get; set; }
        public string RequestUrlSent { get; private set; }
        public Method MethodSent { get; private set; }

        public MockHttpService(string version, string fixture, string baseUrl)
        {
            _baseUrl = baseUrl;
            _fixtureLoader = new FixtureLoader(version, fixture);
        }
        
        public override RequestBuilder RequestBuilder(string path)
        {
            return new RequestBuilder(path);
        }

        public override JToken Execute(IRestRequest request)
        {
            RequestUrlSent = new RestClient($"{_baseUrl}/v2/").BuildUri(request).ToString();
            MethodSent = request.Method;
            
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
                case HttpStatusCode.NotFound:
                    message = JObject.Parse(rawPayload)["message"]?.ToString();
                    throw new NotFoundException(message);
            }
            return !string.IsNullOrEmpty(rawPayload)
                ? JObject.Parse(rawPayload)
                : null;
        }
    }
}