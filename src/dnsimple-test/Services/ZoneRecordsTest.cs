using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using dnsimple;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class ZoneRecordsTest
    {
        private JToken _jToken;
        private RestResponse _response;

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
            _response = new RestResponse();
            _response.Content = loader.ExtractJsonPayload();
            _jToken = JObject.Parse(loader.ExtractJsonPayload());
        }

        [Test]
        public void ZoneRecordData()
        {
            var records = new ZoneRecordsData(_jToken).Records;
            var record = records.First();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, record.Id);
                Assert.AreEqual("example.com", record.ZoneId);
                Assert.IsNull(record.ParentId);
                Assert.AreEqual("", record.Name);
                Assert.AreEqual(
                    "ns1.dnsimple.com admin.dnsimple.com 1458642070 86400 7200 604800 300",
                    record.Content);
                Assert.AreEqual(3600, record.Ttl);
                Assert.IsNull(record.Priority);
                Assert.AreEqual(ZoneRecordType.SOA, record.Type);
                Assert.Contains("global", record.Regions);
                Assert.IsTrue(record.SystemRecord);
                Assert.AreEqual(CreatedAt, record.CreatedAt);
                Assert.AreEqual(UpdatedAt, record.UpdatedAt);
            });
        }

        [Test]
        public void ZoneRecordsResponse()
        {
            var response = new PaginatedDnsimpleResponse<ZoneRecordsData>(_response);

            Assert.AreEqual(5, response.Data.Count);
        }

        [Test]
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/records")]
        public void ListRecords(long account, string zoneId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListZoneRecordsFixture);
            var response = client.Zones.ListRecords(account, zoneId);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(5, response.Data.Count);
                Assert.AreEqual(1, response.PaginationData.CurrentPage);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/records?sort=id:asc%2cname:desc%2ccontent:asc%2ctype:desc&name_like=example&name=boom&type=SOA&per_page=42&page=7")]
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

            client.Zones.ListRecords(account, zoneId, options);

            Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                .CreateRecord(accountId, "example.com", record)
                .Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, created.Id);
                Assert.AreEqual("example.com", created.ZoneId);
                Assert.AreEqual(name, created.Name);
                Assert.AreEqual("127.0.0.1", created.Content);
                Assert.AreEqual(600, created.Ttl);
                Assert.AreEqual(ZoneRecordType.A, created.Type);
                Assert.Contains("global", created.Regions);

                Assert.AreEqual(Method.POST, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "example.com", 5,
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/records/5")]
        public void GetZoneRecord(long accountId, string zoneId, long recordId,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetZoneRecordFixture);
            var record = client.Zones.GetRecord(accountId, zoneId, recordId)
                .Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(recordId, record.Id);
                Assert.AreEqual(zoneId, record.ZoneId);
                Assert.IsNull(record.ParentId);
                Assert.IsEmpty(record.Name);
                Assert.AreEqual("mxa.example.com", record.Content);
                Assert.AreEqual(600, record.Ttl);
                Assert.AreEqual(10, record.Priority);
                Assert.AreEqual(ZoneRecordType.MX, record.Type);
                Assert.Contains("SV1", record.Regions);
                Assert.Contains("IAD", record.Regions);
                Assert.IsFalse(record.SystemRecord);

                Assert.AreEqual(Method.GET, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                client.Zones.UpdateRecord(accountId, zoneId, recordId, data)
                    .Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(recordId, record.Id);
                Assert.AreEqual(zoneId, record.ZoneId);
                Assert.IsNull(record.ParentId);
                Assert.IsEmpty(record.Name);
                Assert.AreEqual("mxb.example.com", record.Content);
                Assert.AreEqual(3600, record.Ttl);
                Assert.AreEqual(20, record.Priority);
                Assert.AreEqual(ZoneRecordType.MX, record.Type);
                Assert.Contains("global", record.Regions);
                Assert.IsFalse(record.SystemRecord);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
                Assert.AreEqual(Method.PATCH, client.HttpMethodUsed());
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
                    client.Zones.DeleteRecord(accountId, zoneId, recordId);
                });
                
                Assert.AreEqual(Method.DELETE, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
        
        [Test]
        [TestCase(1010, "example.com", 5)]
        public void CheckRecordDistributionError(long accountId, string zoneId, long recordId)
        {
            var client = new MockDnsimpleClient(CheckZoneRecordDistributionErrorFixture);
            client.StatusCode(HttpStatusCode.GatewayTimeout);

            Assert.Throws(
                Is.TypeOf<DnSimpleException>().And.Message
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
                Assert.AreEqual(filters, options.UnpackFilters());
                Assert.AreEqual(sorting, options.UnpackSorting());
                Assert.AreEqual(pagination, options.UnpackPagination());
            });
        }
    }
}