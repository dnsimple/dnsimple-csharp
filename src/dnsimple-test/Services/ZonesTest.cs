using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using dnsimple;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using NUnit.Framework;
using Pagination = dnsimple.Services.ListOptions.Pagination;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class ZonesTest
    {
        private MockResponse _response;
        
        private const string ListZonesFixture = "listZones/success.http";
        private const string GetZoneFixture = "getZone/success.http";
        private const string GetZoneNotFoundFixture = "notfound-zone.http";
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
            _response = new MockResponse(loader);
        }

        [Test]
        public void Zones()
        {
            var zones = new PaginatedDnsimpleResponse<Zone>(_response).Data;

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
            var response = new PaginatedDnsimpleResponse<Zone>(_response);

            Assert.AreEqual(2, response.Data.Count);
        }

        [Test]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/zones")]
        public void ListZones(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListZonesFixture);
            var response = client.Zones.ListZones(accountId);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, response.Data.Count);
                Assert.AreEqual(1, response.Pagination.CurrentPage);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
        
        [Test]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/zones?sort=id:asc%2cname:desc&name_like=example.com&per_page=42&page=7")]
        public void ListZonesWithOptions(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListZonesFixture);

            var options = new ZonesListOptions
                {
                    Pagination = new Pagination
                    {
                        PerPage = 42,
                        Page = 7
                    }
                }.FilterByName("example.com")
                .SortById(Order.asc)
                .SortByName(Order.desc);
            
            var response = client.Zones.ListZones(accountId, options);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, response.Data.Count);
                Assert.AreEqual(1, response.Pagination.CurrentPage);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "example-alpha.com", "https://api.sandbox.dnsimple.com/v2/1010/zones/example-alpha.com")]
        public void GetZone(long accountId, string zoneName, string expectedUrl)
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
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "0")]
        public void GetZoneNotFound(long accountId, string zoneName)
        {
            var client = new MockDnsimpleClient(GetZoneNotFoundFixture);
            client.StatusCode(HttpStatusCode.NotFound);

            Assert.Throws<NotFoundException>(
                delegate { client.Zones.GetZone(accountId, zoneName); },
                "Zone `0` not found");
        }

        [Test]
        [TestCase(1010, "example.com",
            "$ORIGIN example.com.\n$TTL 1h\nexample.com. 3600 IN SOA ns1.dnsimple.com. admin.dnsimple.com. 1453132552 86400 7200 604800 300\nexample.com. 3600 IN NS ns1.dnsimple.com.\nexample.com. 3600 IN NS ns2.dnsimple.com.\nexample.com. 3600 IN NS ns3.dnsimple.com.\nexample.com. 3600 IN NS ns4.dnsimple.com.\n",
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/file")]
        public void GetZoneFile(long accountId, string zoneName,
            string zoneFile, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetZoneFileFixture);
            var file = client.Zones.GetZoneFile(accountId, zoneName).Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(zoneFile, file.Zone);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "example.com", 
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/distribution")]
        public void CheckZoneDistributionSuccess(long accountId,
            string zoneName, string expectedUrl)
        {
            var client =
                new MockDnsimpleClient(CheckZoneDistributionSuccessFixture);
            var zone =
                client.Zones.CheckZoneDistribution(accountId, zoneName);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(zone.Data.Distributed);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "example.com")]
        public void CheckZoneDistributionFailure(long accountId,
            string zoneName)
        {
            var client =
                new MockDnsimpleClient(CheckZoneDistributionFailureFixture);
            var zone =
                client.Zones.CheckZoneDistribution(accountId, zoneName);

            Assert.IsFalse(zone.Data.Distributed);
        }

        [Test]
        [TestCase(1010, "example.com")]
        public void CheckZoneDistributionError(long accountId, string zoneName)
        {
            var client =
                new MockDnsimpleClient(CheckZoneDistributionErrorFixture);
            client.StatusCode(HttpStatusCode.GatewayTimeout);

            Assert.Throws(
                Is.TypeOf<DnSimpleException>().And.Message
                    .EqualTo("Could not query zone, connection timed out"),
                delegate
                {
                    client.Zones.CheckZoneDistribution(accountId, zoneName);
                }
            );
        }

        [Test]
        public void ZonesListOptions()
        {
            var filters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("name_like", "example.com")
            };
            var sorting = new KeyValuePair<string, string>("sort", "id:asc,name:desc");
            var pagination = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("per_page", "42"),
                new KeyValuePair<string, string>("page", "7")
            };

            var options = new ZonesListOptions
                {
                    Pagination = new Pagination
                    {
                        PerPage = 42,
                        Page = 7
                    }
                }.FilterByName("example.com")
                .SortById(Order.asc)
                .SortByName(Order.desc);
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(filters, options.UnpackFilters());
                Assert.AreEqual(sorting, options.UnpackSorting());
                Assert.AreEqual(pagination, options.UnpackPagination());
            });
        }
    }
}