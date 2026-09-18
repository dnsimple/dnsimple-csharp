using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using dnsimple;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
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

        private const string BatchChangeZoneRecordsFixture =
            "batchChangeZoneRecords/success.http";
        private const string BatchChangeZoneRecordsCreateValidationFailedFixture =
            "batchChangeZoneRecords/error_400_create_validation_failed.http";
        private const string BatchChangeZoneRecordsUpdateValidationFailedFixture =
            "batchChangeZoneRecords/error_400_update_validation_failed.http";
        private const string BatchChangeZoneRecordsDeleteValidationFailedFixture =
            "batchChangeZoneRecords/error_400_delete_validation_failed.http";

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
                #pragma warning disable 618
                Assert.That(record.ParentId, Is.Null);
                #pragma warning restore 618
                Assert.That(record.Name, Is.EqualTo(""));
                Assert.That(
                    record.Content, Is.EqualTo("ns1.dnsimple.com admin.dnsimple.com 1458642070 86400 7200 604800 300"));
                Assert.That(record.Ttl, Is.EqualTo(3600));
                Assert.That(record.Priority, Is.Null);
                Assert.That(record.Type, Is.EqualTo("SOA"));
                Assert.That(record.Regions, Contains.Item("global"));
                Assert.That(record.SystemRecord, Is.True);
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
                .FilterByType("SOA")
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
                Type = "A",
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
                Assert.That(created.Type, Is.EqualTo("A"));
                Assert.That(created.Regions, Contains.Item("global"));

                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.Post));
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
                #pragma warning disable 618
                Assert.That(record.ParentId, Is.Null);
                #pragma warning restore 618
                Assert.That(record.Name, Is.Empty);
                Assert.That(record.Content, Is.EqualTo("mxa.example.com"));
                Assert.That(record.Ttl, Is.EqualTo(600));
                Assert.That(record.Priority, Is.EqualTo(10));
                Assert.That(record.Type, Is.EqualTo("MX"));
                Assert.That(record.Regions, Contains.Item("SV1"));
                Assert.That(record.Regions, Contains.Item("IAD"));
                Assert.That(record.SystemRecord, Is.False);

                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.Get));
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
                #pragma warning disable 618
                Assert.That(record.ParentId, Is.Null);
                #pragma warning restore 618
                Assert.That(record.Name, Is.Empty);
                Assert.That(record.Content, Is.EqualTo("mxb.example.com"));
                Assert.That(record.Ttl, Is.EqualTo(3600));
                Assert.That(record.Priority, Is.EqualTo(20));
                Assert.That(record.Type, Is.EqualTo("MX"));
                Assert.That(record.Regions, Contains.Item("global"));
                Assert.That(record.SystemRecord, Is.False);

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.Patch));
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

                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.Delete));
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
                Assert.That(record.Distributed, Is.True);

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
                Assert.That(record.Distributed, Is.False);

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
        [TestCase(1010, "example.com",
            "https://api.sandbox.dnsimple.com/v2/1010/zones/example.com/batch")]
        public void BatchChangeZoneRecords(long accountId, string zoneId,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(BatchChangeZoneRecordsFixture);
            var input = new BatchChangeZoneRecordsInput
            {
                Creates = new List<BatchCreateZoneRecordInput>
                {
                    new BatchCreateZoneRecordInput { Name = "ab", Type = "A", Content = "3.2.3.4" },
                    new BatchCreateZoneRecordInput { Name = "ab", Type = "A", Content = "4.2.3.4", Ttl = 3600, Regions = new List<string> { "global" } }
                },
                Updates = new List<BatchUpdateZoneRecordInput>
                {
                    new BatchUpdateZoneRecordInput { Id = 67622534, Content = "3.2.3.40" },
                    new BatchUpdateZoneRecordInput { Id = 67622537, Name = "", Priority = 10 }
                },
                Deletes = new List<ZoneRecordId>
                {
                    new ZoneRecordId { Id = 67622509 },
                    new ZoneRecordId { Id = 67622527 }
                }
            };

            var result = client.Zones.BatchChangeZoneRecords(accountId, zoneId, input).Data;

            Assert.Multiple(() =>
            {
                Assert.That(result.Creates.Count, Is.EqualTo(2));
                Assert.That(result.Creates[0].Id, Is.EqualTo(67623409));
                Assert.That(result.Creates[0].ZoneId, Is.EqualTo("example.com"));
                Assert.That(result.Creates[0].Name, Is.EqualTo("ab"));
                Assert.That(result.Creates[0].Content, Is.EqualTo("3.2.3.4"));
                Assert.That(result.Creates[0].Ttl, Is.EqualTo(3600));
                Assert.That(result.Creates[0].Priority, Is.Null);
                Assert.That(result.Creates[0].Type, Is.EqualTo("A"));
                Assert.That(result.Creates[0].Regions, Contains.Item("global"));
                Assert.That(result.Creates[0].SystemRecord, Is.False);
                Assert.That(result.Creates[1].Id, Is.EqualTo(67623410));
                Assert.That(result.Creates[1].Content, Is.EqualTo("4.2.3.4"));

                Assert.That(result.Updates.Count, Is.EqualTo(2));
                Assert.That(result.Updates[0].Id, Is.EqualTo(67622534));
                Assert.That(result.Updates[0].Name, Is.EqualTo("update1-1757049890"));
                Assert.That(result.Updates[0].Content, Is.EqualTo("3.2.3.40"));
                Assert.That(result.Updates[1].Id, Is.EqualTo(67622537));
                Assert.That(result.Updates[1].Name, Is.EqualTo("update2-1757049890"));
                Assert.That(result.Updates[1].Content, Is.EqualTo("5.2.3.40"));

                Assert.That(result.Deletes.Select(record => record.Id), Is.EqualTo(new long[] { 67622509, 67622527 }));

                Assert.That(client.HttpMethodUsed(), Is.EqualTo(Method.Post));
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
                Assert.That(client.PayloadSent(), Is.EqualTo(
                    "{\"creates\":[" +
                    "{\"name\":\"ab\",\"type\":\"A\",\"content\":\"3.2.3.4\"}," +
                    "{\"name\":\"ab\",\"type\":\"A\",\"content\":\"4.2.3.4\",\"ttl\":3600,\"regions\":[\"global\"]}]," +
                    "\"updates\":[" +
                    "{\"id\":67622534,\"content\":\"3.2.3.40\"}," +
                    "{\"id\":67622537,\"name\":\"\",\"priority\":10}]," +
                    "\"deletes\":[{\"id\":67622509},{\"id\":67622527}]}"));
            });
        }

        [Test]
        public void BatchChangeZoneRecordsOmitsAbsentOperations()
        {
            var client = new MockDnsimpleClient(BatchChangeZoneRecordsFixture);
            var input = new BatchChangeZoneRecordsInput
            {
                Deletes = new List<ZoneRecordId>
                {
                    new ZoneRecordId { Id = 67622509 }
                }
            };

            client.Zones.BatchChangeZoneRecords(1010, "example.com", input);

            Assert.That(client.PayloadSent(), Is.EqualTo("{\"deletes\":[{\"id\":67622509}]}"));
        }

        [Test]
        [TestCase(BatchChangeZoneRecordsCreateValidationFailedFixture)]
        [TestCase(BatchChangeZoneRecordsUpdateValidationFailedFixture)]
        [TestCase(BatchChangeZoneRecordsDeleteValidationFailedFixture)]
        public void BatchChangeZoneRecordsValidationFailed(string fixture)
        {
            var client = new MockDnsimpleClient(fixture);
            client.StatusCode(HttpStatusCode.BadRequest);

            Assert.Throws(
                Is.TypeOf<DnsimpleValidationException>().And.Message
                    .EqualTo("Validation failed"),
                delegate
                {
                    client.Zones.BatchChangeZoneRecords(1010, "example.com",
                        new BatchChangeZoneRecordsInput());
                });
        }

        [Test]
        public void BatchChangeZoneRecordsValidationErrors()
        {
            var client = new MockDnsimpleClient(BatchChangeZoneRecordsCreateValidationFailedFixture);
            client.StatusCode(HttpStatusCode.BadRequest);

            var exception = Assert.Throws<DnsimpleValidationException>(delegate
            {
                client.Zones.BatchChangeZoneRecords(1010, "example.com",
                    new BatchChangeZoneRecordsInput());
            });

            Assert.That(exception.GetAttributeErrors()["creates"]?[0]?["errors"]?["record_type"]?[0]?.ToString(), Is.EqualTo("unsupported"));
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
                .FilterByType("SOA")
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
