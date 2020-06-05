using System;
using System.Collections.Generic;
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
        public ContactsService Contacts { get; }
        public DomainsService Domains { get; }
        public HttpService Http { get; }
        public IdentityService Identity { get; }
        public OAuth2Service OAuth { get; }
        public RegistrarService Registrar { get; }
        public ServicesService Services { get; }
        public TemplatesService Templates { get; }
        public TldsService Tlds { get; }
        public VanityNameServersService VanityNameServers { get; }
        public WebhooksService Webhooks { get; }
        public ZonesService Zones { get; }
        public string UserAgent { get; }

        public MockDnsimpleClient(string fixture)
        {
            Fixture = fixture;
            UserAgent = "Testing user agent";

            Accounts = new AccountsService(this);
            Certificates = new CertificatesService(this);
            Contacts = new ContactsService(this);
            Domains = new DomainsService(this);
            Http = new MockHttpService("v2", Fixture, BaseUrl);
            Identity = new IdentityService(this);
            OAuth = new OAuth2Service(Http);
            Registrar = new RegistrarService(this);
            Services = new ServicesService(this);
            Tlds = new TldsService(this);
            Templates = new TemplatesService(this);
            VanityNameServers = new VanityNameServersService(this);
            Webhooks = new WebhooksService(this);
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

        public void SetUserAgent(string customUserAgent)
        {
        }

        public string RequestSentTo()
        {
            return ((MockHttpService) Http).RequestUrlSent;
        }

        public Method HttpMethodUsed()
        {
            return ((MockHttpService) Http).MethodSent;
        }

        public string PayloadSent()
        {
            return ((MockHttpService) Http).PayloadSent;
        }
    }

    public class MockHttpService : HttpService
    {
        private readonly FixtureLoader _fixtureLoader;
        private readonly string _baseUrl;
        
        public HttpStatusCode StatusCode { get; set; }
        public string RequestUrlSent { get; private set; }
        public Method MethodSent { get; private set; }
        public string PayloadSent { get; private set; }


        public MockHttpService(string version, string fixture, string baseUrl)
        {
            _baseUrl = baseUrl;
            _fixtureLoader = new FixtureLoader(version, fixture);
        }

        public override RequestBuilder RequestBuilder(string path)
        {
            return new RequestBuilder(path);
        }

        public override IRestResponse Execute(IRestRequest request)
        {
            RequestUrlSent = new RestClient($"{_baseUrl}/v2/").BuildUri(request).ToString();
            MethodSent = request.Method;
            try
            {
                PayloadSent = (string) request.Parameters.Find(x =>
                    x.ContentType.Equals("application/json")).Value;
            }
            catch (Exception e)
            {
                // ignored
            }

            var rawPayload = _fixtureLoader.ExtractJsonPayload();

            string message;
            switch (StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new DnsimpleValidationException(JObject.Parse(rawPayload));
                case HttpStatusCode.NotImplemented:
                    message = JObject.Parse(rawPayload)["message"]?.ToString();
                    throw new DnsimpleException(message);
                case HttpStatusCode.GatewayTimeout:
                    message = JObject.Parse(rawPayload)["message"]?.ToString();
                    throw new DnsimpleException(message);
                case HttpStatusCode.NotFound:
                    message = JObject.Parse(rawPayload)["message"]?.ToString();
                    throw new NotFoundException(message);
            }

            return new MockResponse(_fixtureLoader);
        }
    }

    public class MockResponse : RestResponse
    {
        public MockResponse(FixtureLoader loader)
        {
            Content = loader.ExtractJsonPayload();
            Headers = loader.ExtractHeaders();
        }
        
        public void SetHeaders(List<Parameter> headers)
        {
            Headers = headers;
        }
    }
}