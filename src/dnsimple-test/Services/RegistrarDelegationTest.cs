using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using RestSharp;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class RegistrarDelegationTest
    {
        private const string GetDomainDelegationFixture =
            "getDomainDelegation/success.http";

        private const string GetDomainDelegationEmptyFixture =
            "getDomainDelegation/success-empty.http";

        private const string ChangeDomainDelegationFixture =
            "changeDomainDelegation/success.http";

        private const string ChangeDomainDelegationToVanityFixture =
            "changeDomainDelegationToVanity/success.http";

        private const string ChangeDomainDelegationFromVanityFixture =
            "changeDomainDelegationFromVanity/success.http";

        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2016-07-11T09:40:19Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2016-07-11T09:40:19Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        [Test]
        [TestCase(1010, "ruby.codes",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/delegation")]
        [TestCase(1010, "42",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/42/delegation")]
        public void GetDomainDelegation(long accountId, string domain,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetDomainDelegationFixture);

            var domainNameServers =
                client.Registrar.GetDomainDelegation(accountId, domain)
                    .Data;

            var expected = new List<string>
            {
                "ns1.dnsimple.com", "ns2.dnsimple.com", "ns3.dnsimple.com",
                "ns4.dnsimple.com"
            };

            Assert.Multiple(() =>
            {
                Assert.That(domainNameServers, Is.EqualTo(expected));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes")]
        public void GetEmptyDomainDelegation(long accountId, string domain)
        {
            var client =
                new MockDnsimpleClient(GetDomainDelegationEmptyFixture);

            var domainNameServers =
                client.Registrar.GetDomainDelegation(accountId, domain)
                    .Data;

            Assert.That(domainNameServers, Is.EqualTo(new List<string>()));
        }

        [Test]
        [TestCase(1010, "ruby.codes",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/delegation")]
        [TestCase(1010, "42",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/42/delegation")]
        public void ChangeDomainDelegation(long accountId, string domain,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(ChangeDomainDelegationFixture);

            var delegation = new List<string>
            {
                "ns1.dnsimple.com", "ns2.dnsimple.com", "ns3.dnsimple.com",
                "ns4.dnsimple.com"
            };

            var newDelegation = client.Registrar.ChangeDomainDelegation(
                accountId, domain,
                delegation);

            Assert.Multiple(() =>
            {
                Assert.That(newDelegation.Data, Is.EqualTo(delegation));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.PUT));
            });
        }

        [Test]
        [TestCase(1010, "42", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/42/delegation/vanity")]
        [TestCase(1010, "ruby.codes", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/delegation/vanity")]
        public void ChangeDomainDelegationToVanity(long accountId, string domain, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ChangeDomainDelegationToVanityFixture);
            var delegation = new List<string>
            {
                "ns1.dnsimple.com", "ns2.dnsimple.com"
            };

            var vanityDelegation =
                client.Registrar.ChangeDomainDelegationToVanity(accountId,
                    domain, delegation).Data;

            Assert.Multiple(() =>
            {
                Assert.That(vanityDelegation.Count, Is.EqualTo(2));

                Assert.That(vanityDelegation.First().Id, Is.EqualTo(1));
                Assert.That(vanityDelegation.First().Name, Is.EqualTo("ns1.example.com"));
                Assert.That(vanityDelegation.First().Ipv4, Is.EqualTo("127.0.0.1"));
                Assert.That(vanityDelegation.First().Ipv6, Is.EqualTo("::1"));
                Assert.That(vanityDelegation.First().CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(vanityDelegation.First().UpdatedAt, Is.EqualTo(UpdatedAt));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.PUT));
            });
        }

        [Test]
        [TestCase(1010, "42", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/42/delegation/vanity")]
        [TestCase(1010, "ruby.codes", "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/delegation/vanity")]
        public void ChangeDomainDelegationFromVanity(long accountId, string domain, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ChangeDomainDelegationFromVanityFixture);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(delegate
                {
                    client.Registrar.ChangeDomainDelegationFromVanity(
                        accountId, domain);
                });

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.DELETE));
            });
        }
    }
}
