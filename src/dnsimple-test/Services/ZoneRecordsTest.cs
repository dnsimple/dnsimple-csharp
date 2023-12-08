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
    public class ZoneRecordsTest
    {
        private MockResponse _response;

        private const string ListZoneRecordsFixture =
            "listZoneRecords/success.http";

        private const string CreateZoneRecordFixture =
            "createZoneRecord/created.http";

        private const string CreateApexZoneRecordFixture =
            "createZoneRecord/created-apex.http";

        private const string GetZoneRecordFixture =
            "getZoneRecord/success.http";

        private const string UpdateZoneRecordFixture =
            "updateZoneRecord/success.http";

        private const string DeleteZoneRecordFixture =
            "deleteZoneRecord/success.http";

        private const string CheckZoneRecordDistributionSuccessFixture =
            "checkZoneRecordDistribution/success.http";
        private const string CheckZoneRecordDistributionErrorFixture =
            "checkZoneRecordDistribution/error.http";
        private const string CheckZoneRecordDistributionFailureFixture =
            "checkZoneRecordDistribution/failure.http";

        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2016-03-22T10:20:53Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2016-10-05T09:26:38Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListZoneRecordsFixture);
            _response = new MockResponse(loader);
        }

        [Test]
        public void ZoneRecordData()
        {
            var records = new PaginatedResponse<ZoneRecord>(_response).Data;
            var record = records.First();

            Assert.Multiple(() =>
            {
                Assert.That(record.Id, Is.EqualTo(1));
                Assert.That(record.ZoneId, Is.EqualTo("example.com"));
                Assert.IsNull(record.ParentId);
                Assert.That(record.Name, Is.EqualTo(""));
                Assert.That(
                    record.Content, Is.EqualTo("ns1.dnsimple.com admin.dnsimple.com 1458642070 86400 7200 604800 300"));
                Assert.That(record.Ttl, Is.EqualTo(3600));
                Assert.IsNull(record.Priority);
                Assert.That(record.Type, Is.EqualTo(ZoneRecordType.SOA));
                Assert.Contains("global", record.Regions);
                Assert.IsTrue(record.SystemRecord);
                Assert.That(record.CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(record.UpdatedAt, Is.EqualTo(UpdatedAt));
            });
        }

        [Test]
        public void ZoneRecordsResponse()
        {
            var response = new PaginatedResponse<ZoneRecord>(_response);

            Assert.That(response.Data.Count, Is.EqualTo(5));
        }

        [Test]
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/records")]
        public void ListRecords(long account, string zoneId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListZoneRecordsFixture);
            var response = client.Zones.ListZoneRecords(account, zoneId);

            Assert.Multiple(() =>
            {
                Assert.That(response.Data.Count, Is.EqualTo(5));
                Assert.That(response.Pagination.CurrentPage, Is.EqualTo(1));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/records?sort=id:asc,name:desc,content:asc,type:desc&name_like=example&name=boom&type=SOA&per_page=42&page=7")]
        public void ListRecordsWithOptions(long account, string zoneId,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListZoneRecordsFixture);

            var options = new ZoneRecordsListOptions
            {
                Pagination = new Pagination
                {
                    PerPage = 42,
                    Page = 7
                }
            }.FilterByName("example")
                .FilterByExactName("boom")
                .FilterByType(ZoneRecordType.SOA)
                .SortById(Order.asc)
                .SortByName(Order.desc)
                .SortByContent(Order.asc)
                .SortByType(Order.desc);

            client.Zones.ListZoneRecords(account, zoneId, options);

            Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
        }

        [Test]
        [TestCase(CreateApexZoneRecordFixture, 1010, "",
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/records")]
        [TestCase(CreateZoneRecordFixture, 1010, "www",
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/records")]
        public void CreateZoneRecord(string fixture, long accountId,
            string name, string expectedUrl)
        {
            var client = new MockDnsimpleClient(fixture);

            var record = new ZoneRecord
            {
                Name = name,
                Type = ZoneRecordType.A,
                Content = "127.0.0.1",
                Ttl = 600,
            };

            var created = client.Zones
                .CreateZoneRecord(accountId, "example.com", record)
                .Data;

            Assert.Multiple(() =>
            {
                Assert.That(created.Id, Is.EqualTo(1));
                Assert.That(created.ZoneId, Is.EqualTo("example.com"));
                Assert.That(created.Name, Is.EqualTo(name));
                Assert.That(created.Content, Is.EqualTo("127.0.0.1"));
                Assert.That(created.Ttl, Is.EqualTo(600));
                Assert.That(created.Type, Is.EqualTo(ZoneRecordType.A));
                Assert.Contains("global", created.Regions);

                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.POST));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "example.com", 5,
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/records/5")]
        public void GetZoneRecord(long accountId, string zoneId, long recordId,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetZoneRecordFixture);
            var record = client.Zones.GetZoneRecord(accountId, zoneId, recordId)
                .Data;

            Assert.Multiple(() =>
            {
                Assert.That(record.Id, Is.EqualTo(recordId));
                Assert.That(record.ZoneId, Is.EqualTo(zoneId));
                Assert.IsNull(record.ParentId);
                Assert.IsEmpty(record.Name);
                Assert.That(record.Content, Is.EqualTo("mxa.example.com"));
                Assert.That(record.Ttl, Is.EqualTo(600));
                Assert.That(record.Priority, Is.EqualTo(10));
                Assert.That(record.Type, Is.EqualTo(ZoneRecordType.MX));
                Assert.Contains("SV1", record.Regions);
                Assert.Contains("IAD", record.Regions);
                Assert.IsFalse(record.SystemRecord);

                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.GET));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "example.com", 5,
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/records/5")]
        public void UpdateZoneRecord(long accountId, string zoneId,
            long recordId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(UpdateZoneRecordFixture);
            var data = new ZoneRecord
            {
                Name = "",
                Content = "mxb.example.com",
                Ttl = 3600,
                Priority = 20
            };

            var record =
                client.Zones.UpdateZoneRecord(accountId, zoneId, recordId, data)
                    .Data;

            Assert.Multiple(() =>
            {
                Assert.That(record.Id, Is.EqualTo(recordId));
                Assert.That(record.ZoneId, Is.EqualTo(zoneId));
                Assert.IsNull(record.ParentId);
                Assert.IsEmpty(record.Name);
                Assert.That(record.Content, Is.EqualTo("mxb.example.com"));
                Assert.That(record.Ttl, Is.EqualTo(3600));
                Assert.That(record.Priority, Is.EqualTo(20));
                Assert.That(record.Type, Is.EqualTo(ZoneRecordType.MX));
                Assert.Contains("global", record.Regions);
                Assert.IsFalse(record.SystemRecord);

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.PATCH));
            });
        }

        [Test]
        [TestCase(1010, "example.com", 5,
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/records/5")]
        public void DeleteRecord(long accountId, string zoneId, long recordId,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(DeleteZoneRecordFixture);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(delegate
                {
                    client.Zones.DeleteZoneRecord(accountId, zoneId, recordId);
                });

                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.DELETE));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "example.com", 5,
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/records/5/distribution")]
        public void CheckRecordDistribution(long accountId, string zoneId, long recordId,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(CheckZoneRecordDistributionSuccessFixture);
            var record =
                client.Zones.CheckRecordDistribution(accountId, zoneId,
                    recordId).Data;

            Assert.Multiple(() =>
            {
                Assert.IsTrue(record.Distributed);

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "example.com", 5,
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/records/5/distribution")]
        public void CheckRecordDistributionFailure(long accountId, string zoneId, long recordId,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(CheckZoneRecordDistributionFailureFixture);
            var record =
                client.Zones.CheckRecordDistribution(accountId, zoneId,
                    recordId).Data;

            Assert.Multiple(() =>
            {
                Assert.IsFalse(record.Distributed);

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "example.com", 5)]
        public void CheckRecordDistributionError(long accountId, string zoneId, long recordId)
        {
            var client = new MockDnsimpleClient(CheckZoneRecordDistributionErrorFixture);
            client.StatusCode(HttpStatusCode.GatewayTimeout);

            Assert.Throws(
                Is.TypeOf<DnsimpleException>().And.Message
                    .EqualTo("Could not query zone, connection timed out"),
                delegate
                {
                    client.Zones.CheckRecordDistribution(accountId, zoneId,
                        recordId);
                });
        }

        [Test]
        public void ZoneRecordsListOptions()
        {
            var filters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("name_like", "example"),
                new KeyValuePair<string, string>("name", "boom"),
                new KeyValuePair<string, string>("type", "SOA")
            };
            var sorting = new KeyValuePair<string, string>("sort",
                "id:asc,name:desc,content:asc,type:desc");
            var pagination = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("per_page", "42"),
                new KeyValuePair<string, string>("page", "7")
            };

            var options = new ZoneRecordsListOptions
            {
                Pagination = new Pagination
                {
                    PerPage = 42,
                    Page = 7
                }
            }.FilterByName("example")
                .FilterByExactName("boom")
                .FilterByType(ZoneRecordType.SOA)
                .SortById(Order.asc)
                .SortByName(Order.desc)
                .SortByContent(Order.asc)
                .SortByType(Order.desc);

            Assert.Multiple(() =>
            {
                Assert.That(options.UnpackFilters(), Is.EqualTo(filters));
                Assert.That(options.UnpackSorting(), Is.EqualTo(sorting));
                Assert.That(options.UnpackPagination(), Is.EqualTo(pagination));
            });
        }
    }
}
