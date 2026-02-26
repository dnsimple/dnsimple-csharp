using System.Collections.Generic;
using dnsimple;
using dnsimple.Services;
using NUnit.Framework;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class DomainsResearchTest
    {
        private const string GetDomainResearchStatusFixture = "getDomainsResearchStatus/success-unavailable.http";

        [Test]
        [TestCase("https://api.sandbox.dnsimple.com/v2/1010/domains/research/status?domain=taken.com")]
        public void GetDomainResearchStatus(string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetDomainResearchStatusFixture);
            var response = client.Domains.GetDomainResearchStatus(1010, "taken.com");

            Assert.Multiple(() =>
            {
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(response.Data.RequestId, Is.EqualTo("25dd77cb-2f71-48b9-b6be-1dacd2881418"));
                Assert.That(response.Data.Domain, Is.EqualTo("taken.com"));
                Assert.That(response.Data.Availability, Is.EqualTo("unavailable"));
                Assert.That(response.Data.Errors, Is.Not.Null);
                Assert.That(response.Data.Errors, Is.Empty);
            });
        }
    }
}
