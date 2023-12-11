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
                new ListResponse<VanityNameServer>(_response);
            var vanityServer = vanityServers.Data.First();

            Assert.Multiple(() =>
            {
                Assert.That(vanityServer.Id, Is.EqualTo(1));
                Assert.That(vanityServer.Name, Is.EqualTo("ns1.example.com"));
                Assert.That(vanityServer.Ipv4, Is.EqualTo("127.0.0.1"));
                Assert.That(vanityServer.Ipv6, Is.EqualTo("::1"));
                Assert.That(vanityServer.CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(vanityServer.UpdatedAt, Is.EqualTo(UpdatedAt));
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
                Assert.That(vanityNameServers.Count, Is.EqualTo(4));

                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.PUT));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
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
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.DELETE));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }
    }
}
