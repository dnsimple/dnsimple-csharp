using System;
using System.Globalization;
using System.Linq;
using System.Net;
using dnsimple;
using dnsimple.Services;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class ZonesTest
    {
        private JToken _jToken;
        private const string ListZonesFixture = "listZones/success.http";
        private const string GetZoneFixture = "getZone/success.http";
        private const string GetZoneFileFixture = "getZoneFile/success.http";
        private const string CheckZoneDistributionSuccessFixture =
            "checkZoneDistribution/success.http";
        private const string CheckZoneDistributionErrorFixture =
            "checkZoneDistribution/error.http";
        private const string CheckZoneDistributionFailureFixture =
            "checkZoneDistribution/failure.http";

        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2015-04-23T07:40:03Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2015-04-23T07:40:03Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListZonesFixture);
            _jToken = JObject.Parse(loader.ExtractJsonPayload());
        }

        [Test]
        public void ZonesData()
        {
            var zones = new ZonesData(_jToken).Zones;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, zones.First().Id);
                Assert.AreEqual(1010, zones.First().AccountId);
                Assert.AreEqual("example-alpha.com", zones.First().Name);
                Assert.IsFalse(zones.First().Reverse);
                Assert.AreEqual(CreatedAt, zones.First().CreatedAt);
                Assert.AreEqual(UpdatedAt, zones.First().UpdatedAt);
            });
        }

        [Test]
        public void ZonesResponse()
        {
            var response = new ZonesResponse(_jToken);

            Assert.AreEqual(2, response.Data.Zones.Count);
        }

        [Test]
        [TestCase(1010)]
        public void ListZones(long accountId)
        {
            var client = new MockDnsimpleClient(ListZonesFixture);
            var response = client.Zones.ListZones(accountId);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, response.Data.Zones.Count);
                Assert.AreEqual(1, response.Pagination.CurrentPage);
            });
        }

        [Test]
        [TestCase(1010, "example-alpha.com")]
        public void GetZone(long accountId, string zoneName)
        {
            var client = new MockDnsimpleClient(GetZoneFixture);
            var zone = client.Zones.GetZone(accountId, zoneName).Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, zone.Id);
                Assert.AreEqual(accountId, zone.AccountId);
                Assert.AreEqual(zoneName, zone.Name);
                Assert.False(zone.Reverse);
                Assert.AreEqual(CreatedAt, zone.CreatedAt);
                Assert.AreEqual(UpdatedAt, zone.UpdatedAt);
            });
        }

        [Test]
        [TestCase(1010, "example.com",
            "$ORIGIN example.com.\n$TTL 1h\nexample.com. 3600 IN SOA ns1.dnsimple.com. admin.dnsimple.com. 1453132552 86400 7200 604800 300\nexample.com. 3600 IN NS ns1.dnsimple.com.\nexample.com. 3600 IN NS ns2.dnsimple.com.\nexample.com. 3600 IN NS ns3.dnsimple.com.\nexample.com. 3600 IN NS ns4.dnsimple.com.\n")]
        public void GetZoneFile(long accountId, string zoneName, string zoneFile)
        {
            var client = new MockDnsimpleClient(GetZoneFileFixture);
            var file = client.Zones.GetZoneFile(accountId, zoneName).Data;

            Assert.AreEqual(zoneFile, file.Zone);
        }

        [Test]
        [TestCase(1010, "example.com")]
        public void CheckZoneDistributionSuccess(long accountId, string zoneName)
        {
            var client = new MockDnsimpleClient(CheckZoneDistributionSuccessFixture);
            var zone =
                client.Zones.CheckZoneDistribution(accountId, zoneName);
            
            Assert.IsTrue(zone.Data.Distributed);
        }
        
        [Test]
        [TestCase(1010, "example.com")]
        public void CheckZoneDistributionFailure(long accountId, string zoneName)
        {
            var client = new MockDnsimpleClient(CheckZoneDistributionFailureFixture);
            var zone =
                client.Zones.CheckZoneDistribution(accountId, zoneName);
            
            Assert.IsFalse(zone.Data.Distributed);
        }

        [Test]
        [TestCase(1010, "example.com")]
        public void CheckZoneDistributionError(long accountId, string zoneName)
        {
            var client = new MockDnsimpleClient(CheckZoneDistributionErrorFixture);
            client.StatusCode(HttpStatusCode.GatewayTimeout);

            Assert.Throws(Is.TypeOf<DnSimpleException>().And.Message.EqualTo("Could not query zone, connection timed out"),
                delegate
                {
                    client.Zones.CheckZoneDistribution(accountId, zoneName);
                }
            );
        }
    }
}