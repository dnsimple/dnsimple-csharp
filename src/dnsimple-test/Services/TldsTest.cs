using System.Linq;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class TldsTest
    {
        private JToken _jToken;
        
        private const string ListTldsFixture = "listTlds/success.http";
        private const string GetTldFixture = "getTld/success.http";

        private const string GetTldExtendedAttributesFixture =
            "getTldExtendedAttributes/success.http";
        
        private const string GetTldExtendedAttributesNoAttributesFixture =
            "getTldExtendedAttributes/success-noattributes.http";

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListTldsFixture);
            _jToken = JObject.Parse(loader.ExtractJsonPayload());
        }
        
        [Test]
        public void TldsResponse()
        {
            var tlds = new PaginatedDnsimpleResponse<TldData>(_jToken).Data;
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual("ac", tlds.First().Tld);
                Assert.AreEqual(2, tlds.First().TldType);
                Assert.IsFalse(tlds.First().WhoisPrivacy);
                Assert.IsTrue(tlds.First().AutoRenewOnly);
                Assert.IsFalse(tlds.First().Idn);
                Assert.AreEqual(1, tlds.First().MinimumRegistration);
                Assert.IsTrue(tlds.First().RegistrationEnabled);
                Assert.IsTrue(tlds.First().RenewalEnabled);
                Assert.IsFalse(tlds.First().TransferEnabled);
            });
        }
        
        [Test]
        [TestCase("https://api.sandbox.dnsimple.com/v2/tlds")]
        public void ListTlds(string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListTldsFixture);
            var response = client.Tlds.ListTlds();
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, response.Data.Count);
                Assert.AreEqual(1, response.PaginationData.CurrentPage);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
        
        [Test]
        [TestCase("https://api.sandbox.dnsimple.com/v2/tlds?sort=tld:asc&per_page=42&page=7")]
        public void ListTldsSorted(string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListTldsFixture);
            var options = new TldListOptions
            {
                Pagination = new Pagination
                {
                    PerPage = 42,
                    Page = 7
                }
            }.SortByTld(Order.asc);
            
            client.Tlds.ListTlds(options);
            
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase("com", "https://api.sandbox.dnsimple.com/v2/tlds/com")]
        public void GetTld(string tldName, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetTldFixture);
            var tld = client.Tlds.GetTld(tldName).Data;
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual("com", tld.Tld);
                Assert.AreEqual(1, tld.TldType);
                Assert.IsTrue(tld.WhoisPrivacy);
                Assert.IsFalse(tld.AutoRenewOnly);
                Assert.IsTrue(tld.Idn);
                Assert.AreEqual(1, tld.MinimumRegistration);
                Assert.IsTrue(tld.RegistrationEnabled);
                Assert.IsTrue(tld.RenewalEnabled);
                Assert.IsTrue(tld.TransferEnabled);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase("com", "https://api.sandbox.dnsimple.com/v2/tlds/com/extended_attributes")]
        public void GetTldExtendedAttributes(string tldName, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetTldExtendedAttributesFixture);
            var attributes = client.Tlds.GetTldExtendedAttributes(tldName).Data;
            var attribute = attributes.First();
            var options = attribute.Options;
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(4, attributes.Count);
                
                Assert.AreEqual("uk_legal_type", attribute.Name);
                Assert.AreEqual("Legal type of registrant contact", attribute.Description);
                Assert.IsFalse(attribute.Required);
                
                Assert.AreEqual(17, options.Count);
                Assert.AreEqual("UK Individual", options.First().Title);
                Assert.AreEqual("IND", options.First().Value);
                Assert.AreEqual("UK Individual (our default value)", options.First().Description);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        public void GetTldExtendedAttributesNoAttributes()
        {
            var client = new MockDnsimpleClient(GetTldExtendedAttributesNoAttributesFixture);
            Assert.IsEmpty(client.Tlds.GetTldExtendedAttributes("boom").Data);
        }
    }
}