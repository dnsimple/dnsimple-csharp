using System;
using System.Globalization;
using System.Net;
using dnsimple;
using NUnit.Framework;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class DomainsDnssecTest
    {
        private DateTime CreatedAt { get; } = DateTime.ParseExact(
                    "2017-03-03T13:49:58Z", "yyyy-MM-ddTHH:mm:ssZ",
                    CultureInfo.CurrentCulture);
        
        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
                    "2017-03-03T13:49:58Z", "yyyy-MM-ddTHH:mm:ssZ",
                    CultureInfo.CurrentCulture);

        [Test]
        [TestCase(1010, "100", "https://api.sandbox.dnsimple.com/v2/1010/domains/100/dnssec")]
        [TestCase(1010, "example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/dnssec")]
        public void EnableDnssec(long accountId, string domainIdentifier, string expectedUrl)
        {
            var client =
                new MockDnsimpleClient("enableDnssec/success.http");
            var response = client.Domains.EnableDnssec(accountId, domainIdentifier);
            
            Assert.Multiple(() =>
            {
                Assert.IsTrue(response.Data.Enabled);
                Assert.AreEqual(CreatedAt, response.Data.CreatedAt);
                Assert.AreEqual(UpdatedAt, response.Data.UpdatedAt);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "100", "https://api.sandbox.dnsimple.com/v2/1010/domains/100/dnssec")]
        [TestCase(1010, "example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/dnssec")]
        public void DisableDnssec(long accountId, string domainIdentifier, string expectedUrl)
        {
            var client = new MockDnsimpleClient("disableDnssec/success.http");
            
            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(delegate
                {
                    client.Domains.DisableDnssec(accountId, domainIdentifier);
                });
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
        
        [Test]
        [TestCase(1010, "100")]
        [TestCase(1010, "example.com")]
        public void DisableDnssecNotEnabled(long accountId, string domainIdentifier)
        {
            var client = new MockDnsimpleClient("disableDnssec/not-enabled.http");
            client.StatusCode(HttpStatusCode.NotImplemented);
            
            Assert.Throws<DnSimpleException>(delegate
            {
                client.Domains.DisableDnssec(accountId, domainIdentifier);
                    
            }, "DNSSEC cannot be disabled because it is not enabled");
        }

        [Test]
        [TestCase(1010, "100", "https://api.sandbox.dnsimple.com/v2/1010/domains/100/dnssec")]
        [TestCase(1010, "example.com", "https://api.sandbox.dnsimple.com/v2/1010/domains/example.com/dnssec")]
        public void GetDnssec(long accountId, string domainIdentifier, string expectedUrl)
        {
            var dateTime = DateTime.ParseExact(
                "2017-02-03T17:43:22Z", "yyyy-MM-ddTHH:mm:ssZ",
                CultureInfo.CurrentCulture);
            
            var client = new MockDnsimpleClient("getDnssec/success.http");
            var response = client.Domains.GetDnssec(accountId, domainIdentifier);
            
            Assert.Multiple(() =>
            {
                Assert.IsTrue(response.Data.Enabled);
                Assert.AreEqual(dateTime, response.Data.CreatedAt);
                Assert.AreEqual(dateTime, response.Data.UpdatedAt);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
    }
}