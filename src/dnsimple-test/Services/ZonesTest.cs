using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using dnsimple;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using NUnit.Framework;
using RestSharp;
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
        private const string ActivateDNSFixture = "activateZoneService/success.http";
        private const string DeactivateDNSFixture = "deactivateZoneService/success.http";
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
            var zones = new PaginatedResponse<Zone>(_response).Data;

            Assert.Multiple(() =>
            {
                Assert.That(zones.First().Id, Is.EqualTo(1));
                Assert.That(zones.First().AccountId, Is.EqualTo(1010));
                Assert.That(zones.First().Name, Is.EqualTo("example-alpha.com"));
                Assert.That(zones.First().Reverse, Is.False);
                Assert.That(zones.First().Secondary, Is.False);
                Assert.That(zones.First().LastTransferredAt, Is.Null);
                Assert.That(zones.First().Active, Is.True);
                Assert.That(zones.First().CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(zones.First().UpdatedAt, Is.EqualTo(UpdatedAt));
            });
        }

        [Test]
        public void ZonesResponse()
        {
            var response = new PaginatedResponse<Zone>(_response);

            Assert.That(response.Data.Count, Is.EqualTo(2));
        }

        [Test]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/zones")]
        public void ListZones(long accountId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListZonesFixture);
            var response = client.Zones.ListZones(accountId);

            Assert.Multiple(() =>
            {
                Assert.That(response.Data.Count, Is.EqualTo(2));
                Assert.That(response.Pagination.CurrentPage, Is.EqualTo(1));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "https://api.sandbox.dnsimple.com/v2/1010/zones?sort=id:asc,name:desc&name_like=example.com&per_page=42&page=7")]
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
                Assert.That(response.Data.Count, Is.EqualTo(2));
                Assert.That(response.Pagination.CurrentPage, Is.EqualTo(1));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
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
                Assert.That(zone.Id, Is.EqualTo(1));
                Assert.That(zone.AccountId, Is.EqualTo(accountId));
                Assert.That(zone.Name, Is.EqualTo(zoneName));
                Assert.That(zone.Reverse, Is.False);
                Assert.That(zone.Secondary, Is.False);
                Assert.That(zone.LastTransferredAt, Is.Null);
                Assert.That(zone.Active, Is.True);
                Assert.That(zone.CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(zone.UpdatedAt, Is.EqualTo(UpdatedAt));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
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
                Assert.That(file.Zone, Is.EqualTo(zoneFile));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
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
                Assert.That(zone.Data.Distributed, Is.True);

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
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

            Assert.That(zone.Data.Distributed, Is.False);
        }

        [Test]
        [TestCase(1010, "example.com")]
        public void CheckZoneDistributionError(long accountId, string zoneName)
        {
            var client =
                new MockDnsimpleClient(CheckZoneDistributionErrorFixture);
            client.StatusCode(HttpStatusCode.GatewayTimeout);

            Assert.Throws(
                Is.TypeOf<DnsimpleException>().And.Message
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
                Assert.That(options.UnpackFilters(), Is.EqualTo(filters));
                Assert.That(options.UnpackSorting(), Is.EqualTo(sorting));
                Assert.That(options.UnpackPagination(), Is.EqualTo(pagination));
            });
        }

        [Test]
        [TestCase(1010, "example.com")]
        public void ActivateDns(long accountId, string zoneName)
        {
            var client = new MockDnsimpleClient(ActivateDNSFixture);
            var zone = client.Zones.ActivateDns(accountId, zoneName).Data;
            var expectedUrl = $"https://api.sandbox.dnsimple.com/v2/{accountId}/zones/{zoneName}/activation";

            Assert.Multiple(() =>
            {
                Assert.That(zone.Id, Is.EqualTo(1));
                Assert.That(zone.AccountId, Is.EqualTo(1010));
                Assert.That(zone.Name, Is.EqualTo("example.com"));
                Assert.That(zone.Reverse, Is.False);
                Assert.That(zone.CreatedAt, Is.EqualTo(Convert.ToDateTime("2022-09-28T04:45:24Z")));
                Assert.That(zone.UpdatedAt, Is.EqualTo(Convert.ToDateTime("2023-07-06T11:19:48Z")));

                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.PUT));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "example.com")]
        public void DeactivateDns(long accountId, string zoneName)
        {
            var client = new MockDnsimpleClient(DeactivateDNSFixture);
            var zone = client.Zones.DeactivateDns(accountId, zoneName).Data;
            var expectedUrl = $"https://api.sandbox.dnsimple.com/v2/{accountId}/zones/{zoneName}/activation";

            Assert.Multiple(() =>
            {
                Assert.That(zone.Id, Is.EqualTo(1));
                Assert.That(zone.AccountId, Is.EqualTo(1010));
                Assert.That(zone.Name, Is.EqualTo("example.com"));
                Assert.That(zone.Reverse, Is.False);
                Assert.That(zone.CreatedAt, Is.EqualTo(Convert.ToDateTime("2022-09-28T04:45:24Z")));
                Assert.That(zone.UpdatedAt, Is.EqualTo(Convert.ToDateTime("2023-08-08T04:19:52Z")));

                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.DELETE));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }
    }
}
