using System.Linq;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using NUnit.Framework;
using Pagination = dnsimple.Services.ListOptions.Pagination;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class TldsTest
    {
        private MockResponse _response;

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
            _response = new MockResponse(loader);
        }

        [Test]
        public void TldsResponse()
        {
            var tlds = new PaginatedResponse<TldData>(_response).Data;

            Assert.Multiple(() =>
            {
                Assert.That(tlds.First().Tld, Is.EqualTo("ac"));
                Assert.That(tlds.First().TldType, Is.EqualTo(2));
                Assert.That(tlds.First().WhoisPrivacy, Is.False);
                Assert.That(tlds.First().AutoRenewOnly, Is.True);
                Assert.That(tlds.First().Idn, Is.False);
                Assert.That(tlds.First().MinimumRegistration, Is.EqualTo(1));
                Assert.That(tlds.First().RegistrationEnabled, Is.True);
                Assert.That(tlds.First().RenewalEnabled, Is.True);
                Assert.That(tlds.First().TransferEnabled, Is.False);
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
                Assert.That(response.Data.Count, Is.EqualTo(2));
                Assert.That(response.Pagination.CurrentPage, Is.EqualTo(1));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
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
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
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
                Assert.That(tld.Tld, Is.EqualTo("com"));
                Assert.That(tld.TldType, Is.EqualTo(1));
                Assert.That(tld.WhoisPrivacy, Is.True);
                Assert.That(tld.AutoRenewOnly, Is.False);
                Assert.That(tld.Idn, Is.True);
                Assert.That(tld.MinimumRegistration, Is.EqualTo(1));
                Assert.That(tld.RegistrationEnabled, Is.True);
                Assert.That(tld.RenewalEnabled, Is.True);
                Assert.That(tld.TransferEnabled, Is.True);
                Assert.That(tld.DnssecInterfaceType, Is.EqualTo("ds"));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
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
                Assert.That(attributes.Count, Is.EqualTo(4));

                Assert.That(attribute.Name, Is.EqualTo("uk_legal_type"));
                Assert.That(attribute.Description, Is.EqualTo("Legal type of registrant contact"));
                Assert.That(attribute.Required, Is.False);

                Assert.That(options.Count, Is.EqualTo(17));
                Assert.That(options.First().Title, Is.EqualTo("UK Individual"));
                Assert.That(options.First().Value, Is.EqualTo("IND"));
                Assert.That(options.First().Description, Is.EqualTo("UK Individual (our default value)"));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        public void GetTldExtendedAttributesNoAttributes()
        {
            var client = new MockDnsimpleClient(GetTldExtendedAttributesNoAttributesFixture);
            Assert.That(client.Tlds.GetTldExtendedAttributes("boom").Data, Is.Empty);
        }
    }
}
