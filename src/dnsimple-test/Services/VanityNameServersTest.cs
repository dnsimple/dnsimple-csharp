using System;
using System.Globalization;
using System.Linq;
using dnsimple.Services;
using NUnit.Framework;
using RestSharp;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class VanityNameServersTest
    {
        private MockResponse _response;

        private const string EnableVanityNameServersFixture =
            "enableVanityNameServers/success.http";

        private const string DisableVanityNameServersFixture =
            "disableVanityNameServers/success.http";

        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2016-07-14T13:22:17Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2016-07-14T13:22:17Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);
        
        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", EnableVanityNameServersFixture);
            _response = new MockResponse(loader);
        }

        [Test]
        public void VanityNameServersResponse()
        {
            var vanityServers =
                new ListDnsimpleResponse<VanityNameServer>(_response);
            var vanityServer = vanityServers.Data.First();
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, vanityServer.Id);
                Assert.AreEqual("ns1.example.com", vanityServer.Name);
                Assert.AreEqual("127.0.0.1", vanityServer.Ipv4);
                Assert.AreEqual("::1", vanityServer.Ipv6);
                Assert.AreEqual(CreatedAt, vanityServer.CreatedAt);
                Assert.AreEqual(UpdatedAt, vanityServer.UpdatedAt);
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes",
            "https://api.sandbox.dnsimple.com/v2/1010/vanity/ruby.codes")]
        public void EnableVanityNameServers(long accountId, string domain,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(EnableVanityNameServersFixture);
            var vanityNameServers =
                client.VanityNameServers.EnableVanityNameServers(accountId,
                    domain).Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(4, vanityNameServers.Count);

                Assert.AreEqual(Method.PUT, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes",
            "https://api.sandbox.dnsimple.com/v2/1010/vanity/ruby.codes")]
        public void DisableVanityNameServers(long accountId, string domain, string expectedUrl)
        {
            var client = new MockDnsimpleClient(DisableVanityNameServersFixture);
            client.VanityNameServers.DisableVanityNameServers(accountId,
                domain);
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(Method.DELETE, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
    }
}