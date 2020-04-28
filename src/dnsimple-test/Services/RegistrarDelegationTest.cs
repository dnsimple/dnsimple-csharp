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
                Assert.AreEqual(expected, domainNameServers);
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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

            Assert.AreEqual(new List<string>(), domainNameServers);
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
                Assert.AreEqual(delegation, newDelegation.Data);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
                Assert.AreEqual(Method.PUT, client.HttpMethodUsed());
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
                Assert.AreEqual(2, vanityDelegation.Count);
                
                Assert.AreEqual(1, vanityDelegation.First().Id);
                Assert.AreEqual("ns1.example.com", vanityDelegation.First().Name);
                Assert.AreEqual("127.0.0.1", vanityDelegation.First().Ipv4);
                Assert.AreEqual("::1", vanityDelegation.First().Ipv6);
                Assert.AreEqual(CreatedAt, vanityDelegation.First().CreatedAt);
                Assert.AreEqual(UpdatedAt, vanityDelegation.First().UpdatedAt);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
                Assert.AreEqual(Method.PUT, client.HttpMethodUsed());
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
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
                Assert.AreEqual(Method.DELETE, client.HttpMethodUsed());
            });
        }
    }
}